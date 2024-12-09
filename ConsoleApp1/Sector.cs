using SpaceBattle.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpaceBattle
{
    public class Sector : ISector
    {
        // Список игровых обьектов в секции
        private List<IMovable> _gameObjects;
        private Point _address;
        private string _name;

        public Sector(string name, int x, int y)
        {
            _name = name;
            _address = new Point(x, y);
            _gameObjects = new List<IMovable>();
        }

        public List<IMovable> GetGameObjects() => _gameObjects.ToList();

        public void AddGameObject(IMovable gameObject) => _gameObjects.Add(gameObject);

        public void RemoveGameObject(IMovable gameObject) => _gameObjects.Remove(gameObject);
        // Адрес секции в виде точки {5,5}
        public Point Address => _address;
        public string Name => _name;
    }
}
