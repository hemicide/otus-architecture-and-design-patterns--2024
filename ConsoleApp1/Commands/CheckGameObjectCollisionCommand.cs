using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class CheckGameObjectCollisionCommand : ICommand
    {
        IMovable _movable;
        IPlayfield _playfield;

        public CheckGameObjectCollisionCommand(IMovable obj, IPlayfield field) { _movable = obj; _playfield = field; }

        public void Execute()
        {
            // получает координаты объекта
            var position = _movable.GetPosition();

            // определяет новые окрестности, в которой присутствует игровой объект
            var newSectors = _playfield.GetSectorsOfPosition(position);

            var cmds = new List<ICommand>();

            // для каждого объекта новой окрестности и текущего движущегося объекта создает команду проверки коллизии этих двух объектов.
            // Все эти команды помещает в макрокоманду и эту макрокоманду записывает на место аналогичной макрокоманды для предыдущей окрестности.
            foreach (var sector in newSectors) {
                var gameObjectsBySector = _playfield.GetGameObjectsBySector(sector);
                foreach (var gameObject in gameObjectsBySector)
                    cmds.Add(new СheckСollisionsCommand(gameObject, _movable));
            }

            // вызов цепочки проверки коллизий
            new MacroCommand(cmds).Execute();

            // если объект попал в новую окрестность, то удаляет его из списка объектов старой окрестности
            // и добавляет список объектов новой окрестности.
            _playfield.UpdateSectorsForGameObject(_movable);
        }
    }
}
