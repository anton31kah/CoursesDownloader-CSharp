using System.Collections;
using System.Collections.Generic;

namespace CoursesDownloader.Models.Links.DownloadableLinkImplementations.DownloadAsShortcut.Helpers
{
    public class LimitedQueue<T> : Queue<T>, IEnumerable<T>
    {
        private Queue<T> Queue { get; }

        private int MaxSize { get; }

        public LimitedQueue(int maxSize)
        {
            Queue = new Queue<T>(maxSize);
            MaxSize = maxSize;
        }

        public new void Enqueue(T element)
        {
            Queue.Enqueue(element);

            if (Queue.Count > MaxSize)
            {
                Queue.Dequeue();
            }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return Queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Queue.GetEnumerator();
        }
    }
}