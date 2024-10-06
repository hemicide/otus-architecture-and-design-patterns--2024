using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
}
