using commands;
using Moq;
using SpaceBattle;
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
    public class ChainOfResponsibilityTests
    {

        [TestMethod]
        public void ChainOfResponsibility_TryToRemoveGameObject_CollisionException()
        {
            // Попытка переместить обьект заканчивается ошибкой CollisionException
            var testdata = new
            {
                gameObjectsBySector = new List<IMovable>() { 
                    new SpaceShip(new Vector2(5, 10), new Vector2(7, 3), 15, 90, 100, 5f, 100f),
                    new SpaceShip(new Vector2(5, 10), new Vector2(4, 5), 10, 55, 120, 15f, 90f),
                },
                sectorsOfPosition = new List<ISector>() { 
                    new Sector("Sector1", 5, 10),
                },
            };

            // Arrange
            var playField = new Mock<IPlayfield>();
            playField.Setup(obj => obj.GetSectorsOfPosition(It.IsAny<Vector2>())).Returns(testdata.sectorsOfPosition);
            playField.Setup(obj => obj.GetGameObjectsBySector(It.IsAny<ISector>())).Returns(testdata.gameObjectsBySector);

            
            var gameObject = new Mock<IMovable>();
            gameObject.Setup(obj => obj.GetPosition()).Returns(new Vector2(5, 10));

            // Act
            var command = new MoveMacroCommand(gameObject.Object, playField.Object);

            // Assert
            Assert.ThrowsException<CollisionException>(command.Execute);
        }

        [TestMethod]
        public void ChainOfResponsibility_TryToRemoveGameObject_Ok()
        {
            // Попытка переместить обьект заканчивается успехом
            var testdata = new
            {
                gameObjectsBySector = new List<IMovable>() {
                    new SpaceShip(new Vector2(6, 11), new Vector2(7, 3), 15, 90, 100, 5f, 100f),
                    new SpaceShip(new Vector2(3, 7), new Vector2(4, 5), 10, 55, 120, 15f, 90f),
                },
                sectorsOfPosition = new List<ISector>() {
                    new Sector("Sector1", 5, 10),
                },
            };

            // Arrange
            var playField = new Mock<IPlayfield>();
            playField.Setup(obj => obj.GetSectorsOfPosition(It.IsAny<Vector2>())).Returns(testdata.sectorsOfPosition);
            playField.Setup(obj => obj.GetGameObjectsBySector(It.IsAny<ISector>())).Returns(testdata.gameObjectsBySector);


            var gameObject = new Mock<IMovable>();
            gameObject.Setup(obj => obj.GetPosition()).Returns(new Vector2(5, 10));

            // Act
            var command = new MoveMacroCommand(gameObject.Object, playField.Object);
            command.Execute();

            // Assert
            playField.Verify(c => c.UpdateSectorsForGameObject(It.IsAny<IMovable>()), Times.Exactly(1));
        }
    }
}
