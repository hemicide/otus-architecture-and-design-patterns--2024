using commands;
using SpaceBattle.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle
{
    public class ExceptionHandler
    {
        private static Dictionary<Type, Func<ICommand, Exception, ICommand>> _defaultStore = [];
        private static readonly Dictionary<Type, IDictionary<Type, Func<ICommand, Exception, ICommand>>> _store = [];
        public ExceptionHandler()
        {
            _defaultStore.Add(typeof(Exception), (c, e) => { return new ConsoleOutCommand(e); });
        }
        public static ICommand Handle(ICommand c, Exception e)
        {
            Type ct = c.GetType();
            Type et = e.GetType();

            var commandStore = _store.GetValueOrDefault(ct, _defaultStore);

            return commandStore[et](c, e);
        }

        public static void RegisterHandler(Type ct, Type et, Func<ICommand, Exception, ICommand> h)
        {
            if (_store.ContainsKey(ct))
                _store[ct][et] = h;
            else
            {
                Dictionary<Type, Func<ICommand, Exception, ICommand>> _dict = [];
                _dict.Add(et, h);
                _store.Add(ct, _dict);
            }
        }
    }
}
