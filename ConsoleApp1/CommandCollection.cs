using commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace SpaceBattle
{
    public interface IStateCommandCollection
    {
        public bool Next();
    }

    public class CommandCollection
    {
        private BlockingCollection<ICommand>? _collection;
        private int _countTake = 0;
        private bool _stop = false;

        private Func<bool> _predicate = () => true;
        private Action? _action = () => { };
        private IStateCommandCollection _state;

        public CommandCollection() : this(null) {
            _state = new ProcessingStateCommandCollection(this);
        }

        public CommandCollection(IStateCommandCollection state)
        {
            _collection = new BlockingCollection<ICommand>();
            _action = DefaultHandler;
            _state = state;
        }

        public IStateCommandCollection ChangeState(IStateCommandCollection newState) => _state = newState;

        public void Add(ICommand cmd) => _collection!.Add(cmd);

        public void Loop() { while (_state.Next()) ; }

        public void BackgroundLoop() => new Thread(() => { while (_state.Next()) { }; }).Start();

        private void DefaultHandler()
        {
            var cmd = _collection!.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(cmd, ex).Execute();
            }

            Interlocked.Increment(ref _countTake);
        }
        
        public void Clear()
        {
            if (_collection != null)
                while (_collection.TryTake(out _)) { }
        }

        public ICommand? Take()
        {
            if (_collection!.TryTake(out var cmd))
                return cmd;

            return null;
        }

        public void LoopPerCount(int count)
        {
            _predicate = () => _countTake < count;
            Loop();
        }

        public void LoopUntilNotEmpty()
        {
            _predicate = () => _collection!.Count > 0;
            Loop();
        }

        public void Stop(bool force = false)
        {
            if (force)
                _action = null;
            else
                _predicate = () => _collection!.Count > 0;
        }

        public class ProcessingStateCommandCollection(CommandCollection parent) : IStateCommandCollection
        {
            public bool Next()
            {
                if (parent == null || parent._action == null)
                    return false;

                if (!parent._predicate())
                    return false;

                parent._action();
                return true;
            }
        }

        public class StopStateCommandCollection(CommandCollection parent) : IStateCommandCollection
        {
            public bool Next() => parent != null && (parent._action = null) != null;
        }

        public class MoveToStateCommandCollection(CommandCollection parent, CommandCollection newCollection) : IStateCommandCollection
        {
            public bool Next()
            {
                ICommand? cmd = default;

                if (parent != null && !parent._collection!.TryTake(out cmd))
                    return false;

                newCollection.Add(cmd!);
                Interlocked.Increment(ref parent!._countTake);

                return true;
            }
        }
    }
}
