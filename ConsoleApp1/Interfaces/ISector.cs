using System.Drawing;

namespace SpaceBattle.Interfaces
{
    public interface ISector
    {

        public List<IMovable> GetGameObjects();

        public void AddGameObject(IMovable gameObject);

        public void RemoveGameObject(IMovable gameObject);
    }
}