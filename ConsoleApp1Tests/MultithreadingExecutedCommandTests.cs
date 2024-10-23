using commands;
using factory;
using Moq;
using scopes;
using SpaceBattle.Commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SpaceBattle.Tests
{
    [TestClass]
    public class MultithreadingExecutedCommandTests
    {

        [TestMethod]
        public void MultithreadingExecutedCommand_StartProcessing()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            var c1 = new Mock<ICommand>();
            var processingCommandCollectionCommand = new StartProcessingCommandCollectionCommand(commandCollection);
            processingCommandCollectionCommand.Execute();

            // Act

            c1.Setup(m => m.Execute()).Callback(() => mre.Set());

            commandCollection.Add(c1.Object);

            // Assert

            mre.WaitOne();
            c1.Verify(c => c.Execute(), Times.Once);
        }

        [TestMethod]
        public void MultithreadingExecutedCommand_SoftStop()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            var processingCommandCollectionCommand = new StartProcessingCommandCollectionCommand(commandCollection);
            var c1 = new Mock<ICommand>();
            var softStopCommandCollection = new StopProcessingCommandCollectionCommand(commandCollection, force: false); // soft stop
            var c3 = new Mock<ICommand>();
            var c4 = new Mock<ICommand>();

            //CommandCollection.Init();
            processingCommandCollectionCommand.Execute();

            // Act

            c1.Setup(m => m.Execute()).Callback(() => Thread.Sleep(190));
            c3.Setup(m => m.Execute()).Callback(() => Thread.Sleep(190));
            c4.Setup(m => m.Execute()).Callback(() => mre.Set());

            commandCollection.Add(c1.Object);
            commandCollection.Add(softStopCommandCollection);
            commandCollection.Add(c3.Object);
            commandCollection.Add(c4.Object);

            // Assert

            mre.WaitOne();
            c4.Verify(c => c.Execute(), Times.Once);
        }



        [TestMethod]
        public void MultithreadingExecutedCommand_HardStop()
        {
            // Arrange
            var commandCollection = new CommandCollection();
            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            var processingCommandCollectionCommand = new StartProcessingCommandCollectionCommand(commandCollection);
            var hardStopCommandCollection = new WrapperCommand(() =>
            {
                new StopProcessingCommandCollectionCommand(commandCollection, force: true).Execute(); // hard stop
                mre.Set();
            });
            var c1 = new Mock<ICommand>();
            var c2 = new Mock<ICommand>();



            //CommandCollection.Init();
            processingCommandCollectionCommand.Execute();

            // Act

            c1.Setup(m => m.Execute()).Callback(() => Thread.Sleep(250));
            c2.Setup(m => m.Execute()).Callback(() => Thread.Sleep(190));

            commandCollection.Add(c1.Object);
            commandCollection.Add(hardStopCommandCollection);
            commandCollection.Add(c2.Object);

            // Assert

            mre.WaitOne();
            c1.Verify(c => c.Execute(), Times.Once);
            c2.Verify(c => c.Execute(), Times.Never);
        }
    }
}
