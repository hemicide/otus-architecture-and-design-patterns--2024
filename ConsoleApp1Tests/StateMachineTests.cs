using commands;
using Moq;
using SpaceBattle.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Tests
{
    [TestClass]
    public class StateMachineTests
    {
        [TestMethod]
        public void ArterHardStopCommandExecuted_StopProcessing_Test()
        {
            // Arrange
            var commandCollection = new CommandCollection(); // init with default state ProcessingCommandCollection()

            // MoveToCommand

            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);


            var c1 = new Mock<ICommand>();
            var hardStopCommand = new Mock<ICommand>();
            var c3 = new Mock<ICommand>();

            // Act

            c1.Setup(m => m.Execute()).Callback(() => Thread.Sleep(250));
            hardStopCommand.Setup(m => m.Execute()).Callback(() => {
                commandCollection.ChangeState(new CommandCollection.StopStateCommandCollection(commandCollection));
                mre.Set();
            });
            c3.Setup(m => m.Execute()).Callback(() => Thread.Sleep(210));

            commandCollection.Add(c1.Object);
            commandCollection.Add(hardStopCommand.Object);
            commandCollection.Add(c3.Object);
            commandCollection.BackgroundLoop();

            // Assert

            mre.WaitOne();
            c1.Verify(c => c.Execute(), Times.Once);
            c3.Verify(c => c.Execute(), Times.Never);
        }

        [TestMethod]
        public void MoveToCommandExecuted_MovingCommands_Test()
        {
            // Arrange
            var commandCollection = new CommandCollection(); // init with default state ProcessingCommandCollection()
            var newCommandCollection = new CommandCollection();
            var moveToState = new CommandCollection.MoveToStateCommandCollection(commandCollection, newCommandCollection);
            
            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            // Обертка над коннкретной реализацией состояния
            var moveTo = new Mock<IStateCommandCollection>();

            var c1 = new Mock<ICommand>();
            var moveToCommand = new Mock<ICommand>();
            var c3 = new Mock<ICommand>();
            var c4 = new Mock<ICommand>();
            var c5 = new Mock<ICommand>();

            // Act

            // Вызывается конкретная реализация Next() состояния MoveToStateCommandCollection
            moveTo.Setup(m => m.Next()).Returns(moveToState.Next());


            c1.Setup(m => m.Execute()).Callback(() => Thread.Sleep(250));
            moveToCommand.Setup(m => m.Execute()).Callback(() => { 
                commandCollection.ChangeState(moveTo.Object);
                mre.Set();
            });
            c3.Setup(m => m.Execute()).Callback(() => Thread.Sleep(210));
            c4.Setup(m => m.Execute()).Callback(() => Thread.Sleep(240));
            c5.Setup(m => m.Execute()).Callback(() => Thread.Sleep(220));


            commandCollection.Add(c1.Object);
            commandCollection.Add(moveToCommand.Object);
            commandCollection.Add(c3.Object);
            commandCollection.Add(c4.Object);
            commandCollection.Add(c5.Object);
            commandCollection.BackgroundLoop();

            // Assert

            mre.WaitOne();

            // Проверка что метод Next() вызывался у состояния MoveToStateCommandCollection
            moveTo.Verify(c => c.Next(), Times.Exactly(1));
        }

        [TestMethod]
        public void BeginProcessingCommandExecuted_RunCommands_Test()
        {
            // Arrange
            var commandCollection = new CommandCollection(new CommandCollection.StopStateCommandCollection(null)); // init with set state StopStateCommandCollection()
            var runState = new CommandCollection.ProcessingStateCommandCollection(commandCollection);


            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            // Обертка над коннкретной реализацией состояния
            var run = new Mock<IStateCommandCollection>();

            var c1 = new Mock<ICommand>();
            var runCommand = new Mock<ICommand>();
            var c3 = new Mock<ICommand>();
            var c4 = new Mock<ICommand>();
            var c5 = new Mock<ICommand>();

            // Act
            // Вызывается конкретная реализация Next() состояния ProcessingStateCommandCollection
            run.Setup(m => m.Next()).Returns(runState.Next);


            c1.Setup(m => m.Execute()).Callback(() => Thread.Sleep(250));
            c3.Setup(m => m.Execute()).Callback(() => Thread.Sleep(210));
            c4.Setup(m => m.Execute()).Callback(() => Thread.Sleep(240));
            c5.Setup(m => m.Execute()).Callback(() => mre.Set());

            commandCollection.Add(c1.Object);
            commandCollection.Add(c3.Object);
            commandCollection.Add(c4.Object);
            commandCollection.Add(c5.Object);

            // Вызываем команду что обновить состояние и запустить выполнение очереди
            runCommand.Setup(m => m.Execute()).Callback(() => commandCollection.ChangeState(run.Object));
            runCommand.Object.Execute();

            commandCollection.BackgroundLoop();


            // Assert

            mre.WaitOne();
            // Проверка что метод Next() вызывался у состояния ProcessingStateCommandCollection
            run.Verify(c => c.Next(), Times.AtLeastOnce);
        }
    }
}
