using commands;
using scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using factory;

namespace SpaceBattle.Tests
{
    [TestClass]
    public class FactoryTests
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
        public void IoC_Should_Resolve_Registered_Dependency_In_CurrentScope()
        {
            IoC.Resolve<ICommand>("IoC.Register", "someDependency", (object[] args) => (object)1).Execute();

            Assert.AreEqual(1, IoC.Resolve<int>("someDependency"));
        }

        [TestMethod]
        public void IoC_Should_Throw_Exception_On_Unregistered_Dependency_In_CurrentScope()
        {
            Assert.ThrowsException<Exception>(() => IoC.Resolve<int>("someDependency"));
        }

        [TestMethod]
        public void IoC_Should_Use_Parent_Scope_If_Resolving_Dependency_Is_Not_Defined_In_Current_Scope()
        {
            IoC.Resolve<ICommand>("IoC.Register", "someDependency", (object[] args) => (object)1).Execute();

            var parentIoCScope = IoC.Resolve<object>("IoC.Scope.Current");

            var IoCScope = IoC.Resolve<object>("IoC.Scope.Create");
            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", IoCScope).Execute();

            Assert.AreEqual(IoCScope, IoC.Resolve<object>("IoC.Scope.Current"));
            Assert.AreEqual(1, IoC.Resolve<int>("someDependency"));
        }

        [TestMethod]
        public void Parent_Scope_Can_Be_Set_Manually_For_Creating_Scope()
        {
            var scope1 = IoC.Resolve<object>("IoC.Scope.Create");
            var scope2 = IoC.Resolve<object>("IoC.Scope.Create", scope1);

            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", scope1);
            IoC.Resolve<ICommand>("IoC.Register", "someDependency", (object[] args) => (object)2).Execute();
            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", scope2);

            Assert.AreEqual(2, IoC.Resolve<int>("someDependency"));
        }

        [TestMethod]
        public void IoC_Should_Use_Local_Scope()
        {
            var IoCScope = IoC.Resolve<object>("IoC.Scope.Create");
            IoC.Resolve<ICommand>("IoC.Scope.Current.Set", IoCScope).Execute();
            IoC.Resolve<ICommand>("IoC.Register", "a", (object[] args) => (object)1).Execute();
            Assert.AreEqual(1, IoC.Resolve<int>("a"));
            IoC.Resolve<ICommand>("IoC.Scope.Current.Clear").Execute();
        }
    }
}
