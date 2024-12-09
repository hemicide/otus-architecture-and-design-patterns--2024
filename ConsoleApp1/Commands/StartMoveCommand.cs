using commands;
using factory;
using Newtonsoft.Json.Linq;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{
    public class StartMoveCommand : ICommand
    {
        private IUObject _obj;
        private string _idObj;
        private IDictionary<string, object> _properties;

        public StartMoveCommand(IUObject obj, string id, object properties)
        {
            _obj = obj;
            _idObj = id;
            _properties = properties as IDictionary<string, object> ?? new Dictionary<string, object>();
        }   

        public void Execute()
        {
            if (_obj.GetProperty("id").ToString() != _idObj)
                throw new Exception("Game object not found");

            foreach (var property in _properties)
                IoC.Resolve<ICommand>("UObject.SetProperty", _obj, property.Key, property.Value).Execute();

            var adapter = IoC.Resolve<IMovable>("Adapter", typeof(IMovable), _obj);
            var cmd = new MoveCommand(adapter);
            cmd.Execute();
        }
    }
}
