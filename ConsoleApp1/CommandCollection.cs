using commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceBattle
{

    public class CommandCollection
    {
        public BlockingCollection<ICommand>? _collection;
        private int _countTake = 0;
        private bool _stop = false;
        private Func<bool> _predicate = () => true;

        public CommandCollection()
        {
            _collection = new BlockingCollection<ICommand>();
        }

        public void Clear()
        {
            if (_collection != null)
                while (_collection.TryTake(out _)) { }
        }

        public void Add(ICommand cmd)
        {
            _collection ??= [];
            _collection!.Add(cmd);
        }

        public void BackgroundLoop()
        {
            new Thread(() =>
            {
                Loop();
            }).Start();
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
                _stop = true;
            else
                _predicate = () => _collection!.Count > 0;
        }


        public void Loop()
        {
            while (!_stop)
                if (_predicate())
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
                else
                {
                    _stop = true;
                }
        }

        private bool CollectionIsEmpty() => _collection?.Count == 0;
        private bool CollectionIsNull() => _collection == null;
    }
}
