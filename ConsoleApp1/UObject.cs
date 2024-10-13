using SpaceBattle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle
{
    public class UObject : IUObject
    {
        private Dictionary<string, object> _dictionary;

        public UObject()
        {
            _dictionary = new Dictionary<string, object>();
        }    

        public object GetProperty(string key)
        {
            _dictionary.TryGetValue(key, out var value);
            return value ?? new object();
        }

        public void SetProperty(string key, object value)
        {
            _dictionary.TryAdd(key, value); 
        }
    }
}
