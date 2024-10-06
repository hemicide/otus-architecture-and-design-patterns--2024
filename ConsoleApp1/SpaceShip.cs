using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SpaceShip : IMovable, IRotable
    {
        private Vector2 _position = Vector2.Zero;
        private Vector2 _velocity = Vector2.Zero;
        private int _direction = 0;
        private int _directionNumber = 0;
        private int _angularVelocity = 0;

        public SpaceShip(Vector2 velocity, int directionNumber, int angularVelocity)
        {
            _velocity = velocity;
            _directionNumber = directionNumber;
            _angularVelocity = angularVelocity;
        }

        public SpaceShip(Vector2 position, Vector2 velocity, int direction, int directionNumber, int angularVelocity)
        {
            _position = position;
            _velocity = velocity;
            _direction = direction;
            _directionNumber = directionNumber;
            _angularVelocity = angularVelocity;
        }

        public int GetAngularVelocity()
        {
            return _angularVelocity;
        }

        public int GetDirection()
        {
            return _direction;
        }

        public int GetDirectionsNumber()
        {
            return _directionNumber;
        }

        public Vector2 GetPosition()
        {
            return _position;
        }

        public Vector2 GetVelocity()
        {
            return _velocity;
        }

        public void SetDirection(int newV)
        {
            _direction = newV;
        }

        public void SetPosition(Vector2 newV)
        {
            _position = newV;
        }
    }
}
