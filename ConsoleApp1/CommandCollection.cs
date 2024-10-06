using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CommandCollection
    {
        public static BlockingCollection<ICommand>? _collection;
        private static int _countTake = 0;

        public static void Init() => _collection = new BlockingCollection<ICommand>();

        public static void Add(ICommand cmd)
        {
            _collection ??= [];
            _collection!.Add(cmd);
        }

        public static void LoopInfinity() => Loop(() => false);
        public static void LoopPerCount(int count) => Loop(() => _countTake < count);
        public static void LoopUntilNotEmpty() => Loop(() => !CollectionIsEmpty());

        public static void Loop(Func<bool> stop)
        {
            while (stop())
            {
                if (CollectionIsNull())
                    return;

                var cmd = _collection!.Take();
                try
                {
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(cmd, ex).Execute();
                }
                _countTake++;
            }
        }

        private static bool CollectionIsEmpty() => _collection?.Count == 0;
        private static bool CollectionIsNull() => _collection == null;
    }
}
