using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Interfaces;

namespace ConsoleApp1
{
    public interface ICommand
    {
        public void Execute();
    }

    public class MoveCommand : ICommand
    {
        IMovable _movable;
        public MoveCommand(IMovable obj) { _movable = obj; }

        public void Execute() => _movable.SetPosition(Vector2.Add(_movable.GetPosition(), _movable.GetVelocity()));
    }

    public class RotateCommand : ICommand
    {
        IRotable _rotate;
        public RotateCommand(IRotable obj) { _rotate = obj; }

        public void Execute() => _rotate.SetDirection((_rotate.GetDirection() + _rotate.GetAngularVelocity()) % _rotate.GetDirectionsNumber());
    }

    public class ConsoleOutCommand : ICommand
    {
        Exception _ex;
        public ConsoleOutCommand(Exception e) { _ex = e; }
        public void Execute() => Console.WriteLine(_ex.Message);
    }

    public class RetryWithDelayCommand : ICommand
    {
        ICommand _command;
        public RetryWithDelayCommand(ICommand c) { _command = c; }
        public void Execute() => CommandCollection.Add(_command);
    }

    public class RetryNowCommand : ICommand
    {
        ICommand _command;
        public RetryNowCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }

    public class RetryWithDelayNowCommand : ICommand
    {
        ICommand _command;
        public RetryWithDelayNowCommand(ICommand c) { _command = c; }
        public void Execute() => CommandCollection.Add(new RetryWithDelayCommand(_command));
    }

    public class FirstRetryCommand : ICommand
    {
        ICommand _command;
        public FirstRetryCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }

    public class SecondRetryCommand : ICommand
    {
        ICommand _command;
        public SecondRetryCommand(ICommand c) { _command = c; }
        public void Execute() => _command.Execute();
    }

    public class CheckFuelCommand : ICommand
    {
        IFuelCheckable _fuel;
        public CheckFuelCommand(IFuelCheckable obj) { _fuel = obj; }

        public void Execute()
        {
            if (!_fuel.Check())
                throw new CommandException();
        }
    }

    public class BurnFuelCommand : ICommand
    {
        IFuelBurnable _fuel;
        public BurnFuelCommand(IFuelBurnable obj) { _fuel = obj; }

        public void Execute() => _fuel.Burn();
    }

    public class MoveAndBurnFuelCommand : ICommand
    {
        MacroCommand _macrocommand;
        public MoveAndBurnFuelCommand(IFuelCheckable checkable, IMovable movable, IFuelBurnable burnable)
        {
            List<ICommand> cmds = new List<ICommand>()
            {
                new CheckFuelCommand(checkable),
                new MoveCommand(movable),
                new BurnFuelCommand(burnable),
            };

            _macrocommand = new MacroCommand(cmds);
        }

        public void Execute() => _macrocommand.Execute();
    }

    public class ChangeVelocityCommand : ICommand
    {
        IChangeVelocity _changeVelosity;
        Vector2 _velocity = Vector2.Zero;
        public ChangeVelocityCommand(IChangeVelocity obj, Vector2 newV)
        {
            _changeVelosity = obj;
            _velocity = newV;
        }

        public void Execute() => _changeVelosity.SetVelocity(_velocity);
    }

    public class MacroCommand : ICommand
    {
        IEnumerable<ICommand> _cmds;

        public MacroCommand(IEnumerable<ICommand> cmds) { _cmds = cmds; }

        public void Execute()
        {
            foreach (var cmd in _cmds)
            {
                try
                {
                    cmd.Execute();
                }
                catch (Exception)
                {
                    throw new CommandException();
                }
            }
        }
    }
}
