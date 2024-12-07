using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SpaceBattle.Interfaces
{
    public interface IPlayfield
    {
        public List<ISector> GetSectorsOfPosition(Vector2 position);
        public List<ISector> GetSectorsOfGameObject(IMovable obj);

        public void UpdateSectorsForGameObject(IMovable obj);

        public List<IMovable> GetGameObjectsBySector(ISector sector);
    }
}
