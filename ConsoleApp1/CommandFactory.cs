using commands;
using factory;
using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle
{
    public class CommandFactory
    {
        public static object Invoke(params object[] args)
        {
            var message = (IMessage)args[0];

            try
            {   var cmd = IoC.Resolve<ICommand>("Command." + message.Action, message.GameObjectID, message.Properties);
                return cmd;
            }
            catch (Exception ex)
            {
                throw new Exception("Unknown IoC dependency key: " + ex.Message);
            }
        }
    }
}
