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
        private static bool _runLoop = false;
        private static Func<bool> _predicateRun = () => false;

        public static void Init() => _collection = new BlockingCollection<ICommand>();

        public static void Add(ICommand cmd)
        {
            _collection ??= [];
            _collection!.Add(cmd);
        }

        public static void LoopInfinity()
        {
            _predicateRun = () => true;
            Loop();
        }
        public static void LoopPerCount(int count) 
        {
            _predicateRun = () => _countTake < count;
            Loop();
        }
        public static void LoopUntilNotEmpty()
        {
            _predicateRun = () => !CollectionIsEmpty();
            Loop();
        }

        public static void BackgroundLoop()
        {
            new Thread(() =>
            {
                LoopInfinity();
            }).Start();
        }

        public static void Stop(bool force = false)
        {
            if (force)
                _predicateRun = () => false;
            else
                _predicateRun = () => !CollectionIsEmpty();
        }

        private static void Loop()
        {
            if (CollectionIsNull())
                return;

            while (_predicateRun())
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
        }

        private static bool CollectionIsEmpty() => _collection?.Count == 0;
        private static bool CollectionIsNull() => _collection == null;
    }
}
