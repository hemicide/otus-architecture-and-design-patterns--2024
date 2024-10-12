using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Interfaces
{
    public interface IMovable
    {
        Vector2 GetPosition();
        Vector2 GetVelocity();
        void SetPosition(Vector2 newV);
    }

    public interface IMovable2
    {
        Vector2 GetPosition();
        Vector2 GetVelocity();
    }
}
