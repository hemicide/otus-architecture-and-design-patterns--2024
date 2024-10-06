using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Tests
{
    [TestClass]
    public class ExceptionHandlerTests
    {
        /// <summary>
        /// Реализовать Команду, которая записывает информацию о выброшенном исключении в лог.
        /// </summary>
        [TestMethod]
        public void TestMethod_LogThrownException()
        {
            // Arrange
            var mock = new Mock<ICommand>();
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException"));

            CommandCollection.Add(mock.Object);

            // Register handlers
            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new ConsoleOutCommand(e); });

            // Act
            CommandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(1));
        }

        /// <summary>
        /// Реализовать обработчик исключения, который ставит Команду, пишущую в лог в очередь Команд.
        /// </summary>
        [TestMethod]
        public void TestMethod_RetryWithDelayCommand()
        {
            // Arrange
            var mock = new Mock<ICommand>();
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException"));

            CommandCollection.Add(mock.Object);

            // Register handlers
            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(new ConsoleOutCommand(e)); });

            // Act
            CommandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(1));
        }

        /// <summary>
        /// Реализовать Команду, которая повторяет Команду, выбросившую исключение.
        /// </summary>
        [TestMethod]
        public void TestMethod_RetryWithDelayCommandThrowExceptions()
        {
            // Arrange
            var mock = new Mock<ICommand>();
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException"));

            CommandCollection.Add(mock.Object);

            // Register handlers
            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(c); });

            // Act
            CommandCollection.LoopPerCount(5);

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(2));
        }

        /// <summary>
        /// Реализовать обработчик исключения, который ставит в очередь Команду - повторитель команды, выбросившей исключение.
        /// </summary>
        [TestMethod]
        public void TestMethod_RetryWithDelayNowCommandThrowExceptions()
        {
            // Arrange
            var mock = new Mock<ICommand>();
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException"));

            CommandCollection.Add(mock.Object);

            // Register handlers
            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new RetryWithDelayNowCommand(c); });

            // Act
            CommandCollection.LoopPerCount(10);

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(1));
        }

        /// <summary>
        /// При первом выбросе исключения повторить команду, при повторном выбросе исключения записать информацию в лог.
        /// </summary>
        [TestMethod]
        public void TestMethod_FirstRetryThenThrowExceptions()
        {
            // Arrange
            var mock = new Mock<ICommand>();
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException"));

            CommandCollection.Add(mock.Object);

            // Register handlers
            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(new FirstRetryCommand(c)); });
            ExceptionHandler.RegisterHandler(typeof(FirstRetryCommand), typeof(IOException), (c, e) => { return new ConsoleOutCommand(e); });

            // Act
            CommandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(2));
        }

        /// <summary>
        /// При первом выбросе исключения повторить команду два раза, при следующем выбросе исключения записать информацию в лог.
        /// </summary>
        [TestMethod]
        public void TestMethod_SecondRetryThenThrowExceptions()
        {
            // Arrange
            var mock = new Mock<ICommand>();
            mock.Setup(c => c.Execute()).Callback(() => throw new IOException("IOException"));

            CommandCollection.Add(mock.Object);

            // Register handlers
            ExceptionHandler.RegisterHandler(mock.Object.GetType(), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(new FirstRetryCommand(c)); });
            ExceptionHandler.RegisterHandler(typeof(FirstRetryCommand), typeof(IOException), (c, e) => { return new RetryWithDelayCommand(new SecondRetryCommand(c)); });
            ExceptionHandler.RegisterHandler(typeof(SecondRetryCommand), typeof(IOException), (c, e) => { return new ConsoleOutCommand(e); });

            // Act
            CommandCollection.LoopUntilNotEmpty();

            // Assert
            mock.Verify(c => c.Execute(), Times.Exactly(3));
        }
    }
}
