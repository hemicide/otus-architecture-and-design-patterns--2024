using commands;
using factory;
using generators;
using scopes;
using SpaceBattle.Commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Tests
{
    [TestClass]
    public class AdapterGeneratorTests
    {
        [TestInitialize]
        public void Initialize()
        {
            new InitCommand().Execute();
            var IoCScope = IoC.Resolve<object>("IoC.Scope.Create");
            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", IoCScope).Execute();
        }

        [TestCleanup]
        public void Dispose()
        {
            IoC.Resolve<ICommand>("IoC.Scope.Current.Clear").Execute();
        }

        [TestMethod]
        public void IoC_Should_Resolve_Registered_Dependency_For_Created_AdapterInstance()
        {
            IUObject uobject = new UObject();

            IoC.Resolve<ICommand>("IoC.Register", "Adapter", (object[] args) => {
                Type intType = (Type)args[0];
                var proxyType = CompileAssembly.CreateProxyType(intType);
                return Activator.CreateInstance(proxyType, args[1]);
            }).Execute();

            Assert.IsInstanceOfType<IMovable>(IoC.Resolve<IMovable>("Adapter", typeof(IMovable), uobject));
        }

        [TestMethod]
        public void IoC_Should_Resolve_Registered_Dependency_Movable_GetPosition()
        {
            var testdata = new
            {
                position = new Vector2(12, 5),
                want = new Vector2(12, 5)
            };

            IUObject uobject = new UObject();
            uobject.SetProperty("location", testdata.position);
            uobject.SetProperty("velocity", 10);
            uobject.SetProperty("angle", 5);

            IoC.Resolve<ICommand>("IoC.Register", "Adapter", (object[] args) => {
                Type intType = (Type)args[0];
                var proxyType = CompileAssembly.CreateProxyType(intType);
                return Activator.CreateInstance(proxyType, args[1]);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "IMovable.GetPosition", (object[] args) => {
                return ((IUObject)args[0]).GetProperty("location");
            }).Execute();

            var adapter = IoC.Resolve<IMovable>("Adapter", typeof(IMovable), uobject);

            Assert.AreEqual(testdata.want, adapter.GetPosition());
        }

        [TestMethod]
        public void IoC_Should_Resolve_Registered_Dependency_Movable_SetPosition()
        {
            var testdata = new
            {
                position = new Vector2(12, 5),
                newPosition = new Vector2(5, 7),
                want = new Vector2(5, 7)
            };

            IUObject uobject = new UObject();
            uobject.SetProperty("location", testdata.position);

            IoC.Resolve<ICommand>("IoC.Register", "Adapter", (object[] args) => {
                Type intType = (Type)args[0];
                var proxyType = CompileAssembly.CreateProxyType(intType);
                return Activator.CreateInstance(proxyType, args[1]);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "IMovable.GetPosition", (object[] args) => {
                return new SetPropertyCommand((UObject)args[0], "location", args[1]);
            }).Execute();

            var adapter = IoC.Resolve<IMovable>("Adapter", typeof(IMovable), uobject);
            adapter.SetPosition(testdata.newPosition);

            Assert.AreEqual(testdata.want, adapter.GetPosition());
        }

        [TestMethod]
        public void IoC_Should_Resolve_Registered_Dependency_Movable_GetVelocity()
        {
            var testdata = new
            {
                velocity = 10,
                angle = 5,
                want = new Vector2(2.8366218f, -9.589243f)
            };

            IUObject uobject = new UObject();
            uobject.SetProperty("velocity", testdata.velocity);
            uobject.SetProperty("angle", testdata.angle);

            IoC.Resolve<ICommand>("IoC.Register", "Adapter", (object[] args) => {
                Type intType = (Type)args[0];
                var proxyType = CompileAssembly.CreateProxyType(intType);
                return Activator.CreateInstance(proxyType, args[1]);
            }).Execute();

            IoC.Resolve<ICommand>("IoC.Register", "IMovable.GetVelocity", (object[] args) => {
                var velocity = (int)((IUObject)args[0]).GetProperty("velocity");
                var angle = (int)((IUObject)args[0]).GetProperty("angle");
                var x = velocity * Math.Cos(angle);
                var y = velocity * Math.Sin(angle);
                return (object)new Vector2((float)x, (float)y);
            }).Execute();

            var adapter = IoC.Resolve<IMovable>("Adapter", typeof(IMovable), uobject);

            Assert.AreEqual(testdata.want, adapter.GetVelocity());
        }
    }
}
