using commands;
using factory;
using SpaceBattle.Commands;
using SpaceBattle.Interfaces;
using System;
using System.Numerics;

namespace SpaceBattle 
{
    public class Program
    {
        static void Main(string[] args)
        {
            var spaceShip = new SpaceShip(new Vector2(-7, 3), 90, 100, 5f, 100f);
            spaceShip.SetPosition(new Vector2(12, 5));
            spaceShip.SetDirection(10);

            RegisterExceptionHandlers();
            DependencyResolve(spaceShip);

            // ...

            var moveCommand = IoC.Resolve<ICommand>("Commands.Move", spaceShip);
            var rotateCommand = IoC.Resolve<ICommand>("Commands.Rotate", spaceShip);
            var moveAndBurnFuelCommand = IoC.Resolve<ICommand>("Commands.MoveAndBurnFuel", spaceShip);
            var rotateAndChangeVelocityCommand = IoC.Resolve<ICommand>("Commands.RotateAndChangeVelocity", spaceShip);

            var adapter = IoC.Resolve<object>("Adapter", typeof(IMovable), spaceShip);

            CommandCollection.Add(moveCommand);
            CommandCollection.Add(rotateCommand);
            CommandCollection.Add(moveAndBurnFuelCommand);
            CommandCollection.Add(rotateAndChangeVelocityCommand);

            CommandCollection.LoopUntilNotEmpty();
        }

        static void DependencyResolve(SpaceShip spaceShip)
        {
            new scopes.InitCommand().Execute();
            var iocScope = IoC.Resolve<object>("IoC.Scope.Create");
            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", iocScope).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "MacroCommand", (object[] args) => {
                return new MacroCommand(args as IEnumerable<ICommand>);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Commands.Move", (object[] args) => {
                return new MoveCommand(spaceShip);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Commands.Rotate", (object[] args) => {
                return new RotateCommand(spaceShip);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Commands.BurnFuel", (object[] args) => {
                return new BurnFuelCommand(spaceShip);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Commands.ChangeVelocity", (object[] args) => {
                return new ChangeVelocityCommand(spaceShip, new Vector2(5, 8));
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Commands.MoveAndBurnFuel", (object[] args) => {
                return new MacroCommand(new List<ICommand>()
                {
                    new CheckFuelCommand(spaceShip),
                    new MoveCommand(spaceShip),
                    new BurnFuelCommand(spaceShip),
                });
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Descriptions.RotateAndChangeVelocity", (object[] args) => {
                return new string[] { "Commands.Rotate", "Commands.ChangeVelocity" };
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Commands.RotateAndChangeVelocity", (object[] args) => {
                var commandsString = IoC.Resolve<string[]>("Descriptions.RotateAndChangeVelocity");
                var cmds = new ICommand[commandsString.Length];
                for (int i = 0; i < cmds.Length; i++)
                    cmds[i] = IoC.Resolve<ICommand>(commandsString[i], args[0]);

                return IoC.Resolve<ICommand>("MacroCommand", cmds);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Adapter", (object[] args) => {
                Type intType = (Type)args[0];

                var t = AutoGenerate.CreateNewObject(intType);
                var f = t.GetType().GetMethod("GetPosition");
                //var g = f.Invoke(t, []);

                //var adapter = AdapterGenerator.CreateAdapter<IExample>();
                //adapter.MethodA();
                //int result = adapter.MethodB("test");

                var adapter2 = (IMovable)AdapterGenerator.CreateAdapter(intType, args[1]);
                var b = adapter2.GetPosition();
                //int result = adapter.MethodB("test");

                return (object)1;
            }).Execute();
        }

        public interface IExample
        {
            void MethodA();
            int MethodB(string input);
        }

        static void RegisterExceptionHandlers()
        {
            ExceptionHandler.RegisterHandler(typeof(MoveCommand), typeof(Exception), (c, e) => { return new ConsoleOutCommand(e); });
            ExceptionHandler.RegisterHandler(typeof(RotateCommand), typeof(Exception), (c, e) => { return new ConsoleOutCommand(e); });
        }
    }
}