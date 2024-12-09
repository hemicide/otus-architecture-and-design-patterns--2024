using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Interfaces
{
    public interface IMessage
    {
        public string GameObjectID { get; }
        public string Action { get; }
        public IDictionary<string, object> Properties { get; }
    }

}
