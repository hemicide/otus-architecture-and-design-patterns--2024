using commands;
using factory;
using SpaceBattle.Commands;
using SpaceBattle.Interfaces;
using System;
using System.Numerics;
using generators;
using System.Net.Http.Headers;

namespace SpaceBattle 
{
    public class Program
    {
        static void Main(string[] args)
        {
            var commandCollection = new CommandCollection();

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

            var processingCommandCollection = IoC.Resolve<ICommand>("ProcessingCommand.Run", Array.Empty<object>());
            var softStopCommandCollection = IoC.Resolve<ICommand>("ProcessingCommand.Stop", new object[] { /* force */ false });
            var hardStopCommandCollection = IoC.Resolve<ICommand>("ProcessingCommand.Stop", new object[] { /* force */ true });

            // ...

            commandCollection.Add(moveCommand);
            commandCollection.Add(rotateCommand);
            //CommandCollection.Add(softStopCommandCollection);
            //CommandCollection.Add(hardStopCommandCollection);
            commandCollection.Add(moveAndBurnFuelCommand);
            commandCollection.Add(rotateAndChangeVelocityCommand);

            //CommandCollection.LoopUntilNotEmpty();

            processingCommandCollection.Execute();
            //processingCommandCollection.Execute();

            // Генерация адаптора по интерфейсу

            IUObject uobject = new UObject();
            uobject.SetProperty("velocity", 10);
            uobject.SetProperty("angle", 5);

            var adapter = IoC.Resolve<IMovable>("Adapter", typeof(IMovable), uobject);
            adapter.SetPosition(new Vector2(15, 15));
            var newpos = adapter.GetPosition();
            var velocity = adapter.GetVelocity();
        }

        static void DependencyResolve(SpaceShip spaceShip)
        {
            new scopes.InitCommand().Execute();
            var iocScope = IoC.Resolve<object>("IoC.Scope.Create");
            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", iocScope).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "MacroCommand", (object[] args) => {
                return new MacroCommand(args as IEnumerable<ICommand>);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "ProcessingCommand.Run", (object[] args) => {
                return new StartProcessingCommandCollectionCommand(args[0] as CommandCollection);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "ProcessingCommand.Stop", (object[] args) => {
                return new StopProcessingCommandCollectionCommand(args[0] as CommandCollection, (bool)args[1]);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "ProcessingCommand.StopHard", (object[] args) => {
                return new StopProcessingCommandCollectionCommand(args[0] as CommandCollection, force: (bool)args[1]);
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

            IoC.Resolve<ICommand>("IoC.Register", "IMovable.SetPosition", (object[] args) => {
                return new WrapperCommand(() => ((UObject)args[0]).SetProperty("location", args[1]));
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "IMovable.GetPosition", (object[] args) => {
                return ((IUObject)args[0]).GetProperty("location");
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "IMovable.GetVelocity", (object[] args) => {
                var velocity = (int)((IUObject)args[0]).GetProperty("velocity");
                var angle = (int)((IUObject)args[0]).GetProperty("angle");
                var x = velocity * Math.Cos(angle);
                var y = velocity * Math.Sin(angle);
                return (object)new Vector2((float)x, (float)y);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "Adapter", (object[] args) => {
                Type intType = (Type)args[0];
                var proxyType = CompileAssembly.CreateProxyType(intType);
                var instance = Activator.CreateInstance(proxyType, args[1]);
                
                CompileAssembly.UnloadAssembly();

                return instance; //Activator.CreateInstance(proxyType, args[1]);
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