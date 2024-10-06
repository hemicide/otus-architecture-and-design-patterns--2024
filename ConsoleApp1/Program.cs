using System;
using System.Numerics;

namespace ConsoleApp1 
{
    public class Program
    {
        static void Main(string[] args)
        {
            RegisterExceptionHandlers();

            var spaceShip = new SpaceShip(new Vector2(-7, 3), 90, 100);
            spaceShip.SetPosition(new Vector2(12, 5));
            spaceShip.SetDirection(10);

            var moveCommand = new MoveCommand(spaceShip);
            //moveCommand.Execute();

            var rotateCommand = new RotateCommand(spaceShip);
            //rotateCommand.Execute();

            CommandCollection.Add(moveCommand);
            CommandCollection.Add(rotateCommand);
            CommandCollection.LoopUntilNotEmpty();
        }

        static void RegisterExceptionHandlers()
        {
            ExceptionHandler.RegisterHandler(typeof(MoveCommand), typeof(Exception), (c, e) => { return new ConsoleOutCommand(e); });
            ExceptionHandler.RegisterHandler(typeof(RotateCommand), typeof(Exception), (c, e) => { return new ConsoleOutCommand(e); });
        }
    }
}