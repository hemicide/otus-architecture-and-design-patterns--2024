using commands;
using SpaceBattle.Commands;
using SpaceBattle.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Tests
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


        [TestMethod]
        public void CheckFuelCommand_CheckTest()
        {
            var testdata = new
            {
                CheckFuelLevel = false
            };

            // Arrange
            var mock = new Mock<IFuelCheckable>();
            mock.Setup(m => m.Check()).Returns(testdata.CheckFuelLevel);

            // Act
            var command = new CheckFuelCommand(mock.Object);

            // Assert
            Assert.ThrowsException<CommandException>(() => command.Execute());
        }

        [TestMethod]
        public void BurnFuelCommand_BurnTest()
        {
            // Arrange
            var mock = new Mock<IFuelBurnable>();
            mock.Setup(m => m.Burn());

            // Act
            var command = new BurnFuelCommand(mock.Object);
            command.Execute();

            // Assert
            mock.Verify(c => c.Burn(), Times.Exactly(1));
        }

        [TestMethod]
        public void MoveAndBurnFuelCommand_ExecuteTest()
        {
            var testdata = new
            {
                setPosition = new Vector2(12, 5),
                setVelocity = new Vector2(-7, 3),
                setFuelTank = 100,                  // Объем бака
                setFuel = 50,                       // Топлива в баке
                setFuelConsumption = 10,            // Топлива сжигается при перемещении
                wantPosition = new Vector2(5, 8),
                wantFuel = 40,
            };

            // Arrange
            var actPosition = Vector2.Zero;
            var actFuel = 0;

            var mockFuelCheckable = new Mock<IFuelCheckable>();
            mockFuelCheckable.Setup(move => move.Check()).Returns(testdata.setFuel >= testdata.setFuelConsumption);

            var mockMovable = new Mock<IMovable>();
            mockMovable.Setup(m => m.GetPosition()).Returns(testdata.setPosition);
            mockMovable.Setup(m => m.GetVelocity()).Returns(testdata.setVelocity);
            mockMovable.Setup(m => m.SetPosition(It.IsAny<Vector2>())).Callback<Vector2>((p) => actPosition = p);

            var mockFuelBurnable = new Mock<IFuelBurnable>();
            mockFuelBurnable.Setup(m => m.Burn()).Callback(() => actFuel = testdata.setFuel - testdata.setFuelConsumption);

            // Act
            //var command = new MoveAndBurnFuelCommand(mockFuelCheckable.Object, mockMovable.Object, mockFuelBurnable.Object);
            List<ICommand> cmds = new List<ICommand>()
            {
                new CheckFuelCommand(mockFuelCheckable.Object),
                new MoveCommand(mockMovable.Object),
                new BurnFuelCommand(mockFuelBurnable.Object),
            };

            var moveAndBurnFuelCommand = new MacroCommand(cmds);
            moveAndBurnFuelCommand.Execute();

            // Assert
            Assert.AreEqual(actPosition, testdata.wantPosition);
            Assert.AreEqual(actFuel, testdata.wantFuel);

        }

        [TestMethod]
        public void ChangeVelocityCommand_ExecuteTest()
        {
            var testdata = new
            {
                setVelocity = new Vector2(5, 8),
                wantVelocity = new Vector2(5, 8)
            };

            var actVelocity = Vector2.Zero;

            // Arrange
            var mock = new Mock<IChangeVelocity>();
            mock.Setup(m => m.SetVelocity(It.IsAny<Vector2>())).Callback<Vector2>((p) => actVelocity = p);

            // Act
            var command = new ChangeVelocityCommand(mock.Object, testdata.setVelocity);
            command.Execute();

            // Assert
            Assert.AreEqual(testdata.wantVelocity, actVelocity);
        }

        [TestMethod]
        public void RotateAndChangeVelocityCommand_ExecuteTest()
        {
            var testdata = new
            {
                setAngularVelocity = 100,
                setDirectionsNumber = 90,
                setDirection = 100,
                setVelocity = new Vector2(5, 8),
                wantDirection = 20,
                wantVelocity = new Vector2(5, 8),
            };

            // Arrange
            var actDirection = 0;
            var actVelocity = Vector2.Zero;

            var mockRotable = new Mock<IRotable>();
            mockRotable.Setup(m => m.GetAngularVelocity()).Returns(testdata.setAngularVelocity);
            mockRotable.Setup(m => m.GetDirectionsNumber()).Returns(testdata.setDirectionsNumber);
            mockRotable.Setup(m => m.GetDirection()).Returns(testdata.setDirection);
            mockRotable.Setup(m => m.SetDirection(It.IsAny<int>())).Callback<int>((v) => actDirection = v);

            var mockChangeVelocity = new Mock<IChangeVelocity>();
            mockChangeVelocity.Setup(m => m.SetVelocity(It.IsAny<Vector2>())).Callback<Vector2>((p) => actVelocity = p);

            // Act
            List<ICommand> cmds = new List<ICommand>()
            {
                new RotateCommand(mockRotable.Object),
                new ChangeVelocityCommand(mockChangeVelocity.Object, testdata.setVelocity),
            };

            var rotateAndChangeVelocityCommand = new MacroCommand(cmds);
            rotateAndChangeVelocityCommand.Execute();

            // Assert
            Assert.AreEqual(actDirection, testdata.wantDirection);
            Assert.AreEqual(actVelocity, testdata.wantVelocity);
        }
    }
}
