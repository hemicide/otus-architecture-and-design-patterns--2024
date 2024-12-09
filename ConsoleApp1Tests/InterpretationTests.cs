using commands;
using factory;
using generators;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using scopes;
using SpaceBattle.Commands;
using SpaceBattle.Interfaces;
using System;
using System.Numerics;
using System.Text.Json;

namespace SpaceBattle.Tests
{
    [TestClass]
    public class InterpretationTests
    {
        [TestInitialize]
        public void Initialize()
        {
            new InitCommand().Execute();
            var uobject = new UObject();
            uobject.SetProperty("id", 548);
            uobject.SetProperty("velocity", 0);
            uobject.SetProperty("angle", (long)15);
            uobject.SetProperty("location", new Vector2(0, 0));

            var IoCScope = IoC.Resolve<object>("IoC.Scope.Create");
            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", IoCScope).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "CreateCommand", (object[] args) => CommandFactory.Invoke(args)).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "Command.StartMove", (object[] args) => new StartMoveCommand(uobject, (string)args[0], args[1])).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "UObject.SetProperty", (object[] args) => {
                return new WrapperCommand(() => ((IUObject)args[0]).SetProperty((string)args[1], args[2]));
            }).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "Adapter", (object[] args) => {
                Type intType = (Type)args[0];
                var proxyType = CompileAssembly.CreateProxyType(intType);
                return Activator.CreateInstance(proxyType, args[1]);
            }).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "IMovable.SetPosition", (object[] args) => {
                return new WrapperCommand(() => ((UObject)args[0]).SetProperty("location", args[1]));
            }).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "Command.GetGameObject", (object[] args) => {
                return uobject;
            }).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "IMovable.GetPosition", (object[] args) => {
                return ((IUObject)args[0]).GetProperty("location");
            }).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "IMovable.GetVelocity", (object[] args) => {
                var velocity = (long)((IUObject)args[0]).GetProperty("velocity");
                var angle = (long)((IUObject)args[0]).GetProperty("angle");
                var x = velocity * Math.Cos(angle);
                var y = velocity * Math.Sin(angle);
                return (object)new Vector2((float)x, (float)y);
            }).Execute();
        }

        [TestCleanup]
        public void Dispose()
        {
            IoC.Resolve<ICommand>("IoC.Scope.Current.Clear").Execute();
        }

        [TestMethod]
        public void Interpretation_StartMoveCommand_Test()
        {
            var testdata = new
            { 
                message_json = "{\"id\": \"548\",\"action\": \"StartMove\",\"properties\": {\"velocity\": 2 }}"
            };

            // Arrange
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(testdata.message_json);
            var uobject = IoC.Resolve<IUObject>("Command.GetGameObject");

            var startPosition = uobject.GetProperty("location");
            var startVelocity = uobject.GetProperty("velocity");

            // Act
            var command = new InterpretationCommand(message);
            command.Execute();

            var endPosition = uobject.GetProperty("location");
            var endVelocity = uobject.GetProperty("velocity");

            // Assert
            Assert.AreNotEqual(startPosition, endPosition);
            Assert.AreNotEqual(startVelocity, endVelocity);
            Assert.AreEqual(endVelocity, (long)2);
        }

        [TestMethod]
        public void Interpretation_UnresolveCommand_Test()
        {
            var testdata = new
            {
                message_json = "{\"id\": \"548\",\"action\": \"StopMove\",\"properties\": {\"velocity\": 2 }}"
            };

            // Arrange
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(testdata.message_json);

            // Act
            var command = new InterpretationCommand(message);

            // Assert
            Assert.ThrowsException<Exception>(command.Execute);
        }

        [TestMethod]
        public void Interpretation_NotFoundGameObject_Test()
        {
            var testdata = new
            {
                message_json = "{\"id\": \"549\",\"action\": \"StartMove\",\"properties\": {\"velocity\": 2 }}"
            };

            // Arrange
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(testdata.message_json);

            // Act
            var command = new InterpretationCommand(message);

            // Assert
            Assert.ThrowsException<Exception>(command.Execute);
        }
    }
}
