using BFC.BackgroundWorker.Domain.Abstractions;
using BFC.BackgroundWorker.Exceptions;
using System.Collections.Concurrent;

namespace BFC.BackgroundWorker.Domain
{
    public class BackgroundQueue<T> : IBackgroundQueue<T> where T : class
    {
        private readonly ConcurrentQueue<T> _items = new ConcurrentQueue<T>();

        public T Dequeue()
        {
            var success = _items.TryDequeue(out var item);

            return success ? item : null;
        }

        public void Enqueue(T item)
        {
            if (item is null) throw new BackgroundQueueException($"Item {nameof(item)} not found");

            _items.Enqueue(item);
        }
    }
}
