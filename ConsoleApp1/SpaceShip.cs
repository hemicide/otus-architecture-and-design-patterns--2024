using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SpaceBattle.Interfaces;

namespace SpaceBattle
{
    public class SpaceShip : IMovable, IRotable, IFuelBurnable, IFuelCheckable, IChangeVelocity
    {
        private Vector2 _position = Vector2.Zero;
        private Vector2 _velocity = Vector2.Zero;
        private int _direction = 0;
        private int _directionNumber = 0;
        private int _angularVelocity = 0;
        private float _fuelTank = 0;
        private float _fuelСombustion = 0;

        public SpaceShip(Vector2 velocity, int directionNumber, int angularVelocity, float fuelСombustion, float fuelTank)
        {
            _velocity = velocity;
            _directionNumber = directionNumber;
            _angularVelocity = angularVelocity;
            _fuelСombustion = fuelСombustion;
            _fuelTank = fuelTank;
        }

        public SpaceShip(Vector2 position, Vector2 velocity, int direction, int directionNumber, int angularVelocity, float fuelСombustion, float fuelTank)
        {
            _position = position;
            _velocity = velocity;
            _direction = direction;
            _directionNumber = directionNumber;
            _angularVelocity = angularVelocity;
            _fuelСombustion = fuelСombustion;
            _fuelTank = fuelTank;
        }

        public void Burn()
        {
            if (!Check())
                throw new Exception("Fuel");
            else
                _fuelTank -= _fuelСombustion;   
        }

        public bool Check()
        {
            return _fuelTank >= _fuelСombustion;
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

        public void SetVelocity(Vector2 newV)
        {
            _velocity = newV;
        }
    }
}
