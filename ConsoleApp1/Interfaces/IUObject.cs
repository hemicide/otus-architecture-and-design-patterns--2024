using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle.Interfaces
{
    public interface IUObject
    {
        object GetProperty(string key);
        void SetProperty(string key, object value);
    }
}
