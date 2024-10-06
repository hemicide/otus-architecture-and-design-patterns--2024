using System;
using System.Numerics;

namespace ConsoleApp1 
{
    public class Program
    {
        static void Main(string[] args)
        {
            var spaceShip = new SpaceShip(new Vector2(-7, 3), 90, 100);
            spaceShip.SetPosition(new Vector2(12, 5));
            spaceShip.SetDirection(10);

            var moveCommand = new MoveCommand(spaceShip);
            moveCommand.Execute();

            var rotateCommand = new RotateCommand(spaceShip);
            rotateCommand.Execute();
        }
    }
}