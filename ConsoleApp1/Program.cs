using System;
using System.Numerics;

namespace ConsoleApp1 
{
    public class Program
    {
        static void Main(string[] args)
        {
            RegisterExceptionHandlers();

            var spaceShip = new SpaceShip(new Vector2(-7, 3), 90, 100, 5f, 100f);
            spaceShip.SetPosition(new Vector2(12, 5));
            spaceShip.SetDirection(10);

            var moveCommand = new MoveCommand(spaceShip);
            var rotateCommand = new RotateCommand(spaceShip);
            var moveAndBurnFuelCommand = new MacroCommand(new List<ICommand>()
            {
                new CheckFuelCommand(spaceShip),
                new MoveCommand(spaceShip),
                new BurnFuelCommand(spaceShip),
            });
            var rotateAndChangeVelocityCommand = new MacroCommand(new List<ICommand>()
            {
                new RotateCommand(spaceShip),
                new ChangeVelocityCommand(spaceShip, new Vector2(5, 8)),
            });

            CommandCollection.Add(moveCommand);
            CommandCollection.Add(rotateCommand);
            CommandCollection.Add(moveAndBurnFuelCommand);
            CommandCollection.Add(rotateAndChangeVelocityCommand);

            CommandCollection.LoopUntilNotEmpty();
        }

        static void RegisterExceptionHandlers()
        {
            ExceptionHandler.RegisterHandler(typeof(MoveCommand), typeof(Exception), (c, e) => { return new ConsoleOutCommand(e); });
            ExceptionHandler.RegisterHandler(typeof(RotateCommand), typeof(Exception), (c, e) => { return new ConsoleOutCommand(e); });
        }
    }
}