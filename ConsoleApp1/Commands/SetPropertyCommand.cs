using commands;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Commands
{

    public class SetPropertyCommand : ICommand
    {
        object _value;
        UObject _obj;
        string _propertyName;
        public SetPropertyCommand(UObject obj, string propertyName, object newValue)
        {
            _obj = obj;
            _value = newValue;
            _propertyName = propertyName; 
        }

        public void Execute() => _obj.SetProperty(_propertyName, _value);
    }
}
