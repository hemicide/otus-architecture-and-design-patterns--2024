using commands;

namespace factory.Impl
{
    internal class UpdateIocResolveDependencyStrategyCommand : ICommand
    {
        Func<Func<string, object[], object>, Func<string, object[], object>> _updateIoCStrategy;

        public UpdateIocResolveDependencyStrategyCommand(
            Func<Func<string, object[], object>, Func<string, object[], object>> updater
        )
        {
            _updateIoCStrategy = updater;
        }

        public void Execute()
        {
            IoC._strategy = _updateIoCStrategy(IoC._strategy);
        }
    }
}
