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
        public static BlockingCollection<ICommand>? _collection;
        private static int _countTake = 0;
        private static bool _stop = false;
        private static Func<bool> _predicate = () => true;

        public static void Init()
        {
            _collection = new BlockingCollection<ICommand>();
        }

        public static void Add(ICommand cmd)
        {
            _collection ??= [];
            _collection!.Add(cmd);
        }

        public static void BackgroundLoop()
        {
            new Thread(() =>
            {
                Loop();
            }).Start();
        }

        public static void LoopPerCount(int count)
        {
            _predicate = () => _countTake < count;
            Loop();
        }

        public static void LoopUntilNotEmpty()
        {
            _predicate = () => _collection!.Count > 0;
            Loop();
        }

        public static void Stop(bool force = false)
        {
            if (force)
                _stop = true;
            else
                _predicate = () => _collection!.Count > 0;
        }


        public static void Loop()
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

        private static bool CollectionIsEmpty() => _collection?.Count == 0;
        private static bool CollectionIsNull() => _collection == null;
    }
}
