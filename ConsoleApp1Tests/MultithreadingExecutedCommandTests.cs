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

            CommandCollection.Init();

            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            var c1 = new Mock<ICommand>();
            var processingCommandCollectionCommand = new StartProcessingCommandCollectionCommand();
            processingCommandCollectionCommand.Execute();

            // Act

            c1.Setup(m => m.Execute()).Callback(() => mre.Set());

            CommandCollection.Add(c1.Object);

            // Assert

            mre.WaitOne();
            c1.Verify(c => c.Execute(), Times.Once);
        }

        [TestMethod]
        public void MultithreadingExecutedCommand_SoftStop()
        {
            // Arrange

            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            var processingCommandCollectionCommand = new StartProcessingCommandCollectionCommand();
            var c1 = new Mock<ICommand>();
            var softStopCommandCollection = new StopProcessingCommandCollectionCommand(force: false); // soft stop
            var c3 = new Mock<ICommand>();
            var c4 = new Mock<ICommand>();

            CommandCollection.Init();
            processingCommandCollectionCommand.Execute();

            // Act

            c1.Setup(m => m.Execute()).Callback(() => Thread.Sleep(190));
            c3.Setup(m => m.Execute()).Callback(() => Thread.Sleep(190));
            c4.Setup(m => m.Execute()).Callback(() => mre.Set());

            CommandCollection.Add(c1.Object);
            CommandCollection.Add(softStopCommandCollection);
            CommandCollection.Add(c3.Object);
            CommandCollection.Add(c4.Object);

            // Assert

            mre.WaitOne();
            c4.Verify(c => c.Execute(), Times.Once);
        }



        [TestMethod]
        public void MultithreadingExecutedCommand_HardStop()
        {
            // Arrange

            // Для условного события синхронизации
            var mre = new ManualResetEvent(false);

            var processingCommandCollectionCommand = new StartProcessingCommandCollectionCommand();
            var hardStopCommandCollection = new WrapperCommand(() =>
            {
                new StopProcessingCommandCollectionCommand(force: true).Execute(); // hard stop
                mre.Set();
            });
            var c1 = new Mock<ICommand>();
            var c2 = new Mock<ICommand>();
            var c3 = new Mock<ICommand>();



            CommandCollection.Init();
            processingCommandCollectionCommand.Execute();

            // Act

            c1.Setup(m => m.Execute()).Callback(() => Thread.Sleep(250));
            c2.Setup(m => m.Execute()).Callback(() => Thread.Sleep(190));

            CommandCollection.Add(c1.Object);
            CommandCollection.Add(hardStopCommandCollection);
            CommandCollection.Add(c2.Object);
            CommandCollection.Add(c3.Object);

            // Assert

            mre.WaitOne();
            c1.Verify(c => c.Execute(), Times.Once);
            c2.Verify(c => c.Execute(), Times.Never);
        }
    }
}
