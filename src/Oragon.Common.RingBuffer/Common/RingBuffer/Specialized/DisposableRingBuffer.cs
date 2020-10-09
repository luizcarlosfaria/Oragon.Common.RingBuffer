using System;

namespace Oragon.Common.RingBuffer.Specialized
{
    public class DisposableRingBuffer<T> : RingBuffer<T>, IDisposable where T : IDisposable
    {
        private bool disposedValue;

        public DisposableRingBuffer(int capacity, Func<T> bufferFactory) : base(capacity, bufferFactory)
        {
        }

        public DisposableRingBuffer(int capacity, Func<T> bufferFactory, TimeSpan waitTime) : base(capacity, bufferFactory, waitTime)
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    T[] buffer = this.availableBuffer.ToArray();

                    this.availableBuffer.Clear();

                    foreach (T item in buffer)
                    {
                        item.Dispose();
                    }
                }
                disposedValue = true;
            }
        }
      
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
