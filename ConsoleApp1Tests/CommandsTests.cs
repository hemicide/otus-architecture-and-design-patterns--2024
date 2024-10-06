using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Tests
{
    [TestClass()]
    public class CommandsTests
    {
        [TestMethod]
        public void MovementTest()
        {
            // Для объекта, находящегося в точке (12, 5) и движущегося со скоростью (-7, 3) движение меняет положение объекта на (5, 8)

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            Vector2 act = default;

            // Arrange
            var mock = new Mock<IMovable>();
            mock.Setup(move => move.GetPosition()).Returns(testdata.position);
            mock.Setup(move => move.GetVelocity()).Returns(testdata.velocity);
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>())).Callback<Vector2>((v) => act = v);

            // Act
            MoveCommand command = new MoveCommand(mock.Object);
            command.Execute();

            // Assert
            Assert.AreEqual(act, testdata.want);
        }

        [TestMethod]
        public void MovementTest_WithoutPosition()
        {
            // Попытка сдвинуть объект, у которого невозможно прочитать положение в пространстве, приводит к ошибке

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            // Arrange
            var mock = new Mock<IMovable>();
            mock.Setup(move => move.GetPosition()).Throws(new NotSupportedException());
            mock.Setup(move => move.GetVelocity()).Returns(testdata.velocity);
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>()));

            // Act
            MoveCommand command = new MoveCommand(mock.Object);

            // Assert
            Assert.ThrowsException<NotSupportedException>(command.Execute);
        }

        [TestMethod]
        public void MovementTest_WithoutVelocity()
        {
            // Попытка сдвинуть объект, у которого невозможно прочитать значение мгновенной скорости, приводит к ошибке

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            // Arrange
            var mock = new Mock<IMovable>();
            mock.Setup(move => move.GetPosition()).Returns(testdata.position);
            mock.Setup(move => move.GetVelocity()).Throws(new NotSupportedException());
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>()));

            // Act
            MoveCommand command = new MoveCommand(mock.Object);

            // Assert
            Assert.ThrowsException<NotSupportedException>(command.Execute);
        }


        [TestMethod]
        public void MovementTest_NoChangePosition()
        {
            // Попытка сдвинуть объект, у которого невозможно изменить положение в пространстве, приводит к ошибке

            var testdata = new
            {
                position = new Vector2(12, 5),
                velocity = new Vector2(-7, 3),
                want = new Vector2(5, 8)
            };

            // Arrange
            var mock = new Mock<IMovable>();
            mock.Setup(move => move.GetPosition()).Returns(testdata.position);
            mock.Setup(move => move.GetVelocity()).Returns(testdata.velocity);
            mock.Setup(move => move.SetPosition(It.IsAny<Vector2>())).Throws(new Exception());

            // Act
            MoveCommand command = new MoveCommand(mock.Object);

            // Assert
            Assert.ThrowsException<Exception>(command.Execute);
        }

        [TestMethod]
        public void RotationTest()
        {

            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotable>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);
            command.Execute();

            // Assert
            Assert.AreEqual(act, testdata.want);
        }

        [TestMethod]
        public void RotationTest_WithoutAngularVelocity()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotable>();
            mock.Setup(move => move.GetAngularVelocity()).Throws(new NotSupportedException());
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.ThrowsException<NotSupportedException>(command.Execute);
        }

        [TestMethod]
        public void RotationTest_DirectionsNumber()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotable>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Throws(new NotSupportedException());
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.ThrowsException<NotSupportedException>(command.Execute);
        }

        [TestMethod]
        public void RotationTest_GetDirection()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            int act = default;

            // Arrange
            var mock = new Mock<IRotable>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Throws(new NotSupportedException());
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Callback<int>((v) => act = v);

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.ThrowsException<NotSupportedException>(command.Execute);
        }

        [TestMethod]
        public void RotationTest_NoChangeDirection()
        {
            var testdata = new
            {
                DirectionsNumber = 90,
                AngularVelocity = 100,
                Direction = 10,
                want = 20
            };

            // Arrange
            var mock = new Mock<IRotable>();
            mock.Setup(move => move.GetAngularVelocity()).Returns(testdata.AngularVelocity);
            mock.Setup(move => move.GetDirectionsNumber()).Returns(testdata.DirectionsNumber);
            mock.Setup(move => move.GetDirection()).Returns(testdata.Direction);
            mock.Setup(move => move.SetDirection(It.IsAny<int>())).Throws(new Exception());

            // Act
            RotateCommand command = new RotateCommand(mock.Object);

            // Assert
            Assert.ThrowsException<Exception>(command.Execute);
        }
    }
}
