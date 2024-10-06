using commands;
using factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scopes.Commands
{
    public class RegisterDependencyCommand : ICommand
    {
        string _dependency;
        Func<object[], object> _dependencyResolverStratgey;

        public RegisterDependencyCommand(string dependency, Func<object[], object> depednecyResolverStrategy)
        {
            _dependency = dependency;
            _dependencyResolverStratgey = depednecyResolverStrategy;
        }

        public void Execute()
        {
            var currentScope = IoC.Resolve<IDictionary<string, Func<object[], object>>>("IoC.Scope.Current");
            currentScope.Add(_dependency, _dependencyResolverStratgey);
        }
    }
}
