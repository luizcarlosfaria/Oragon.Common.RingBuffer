using System;
using System.Collections.Generic;
using System.Text;

namespace Oragon.Common.RingBuffer.Specialized
{
    public class DisposableRingBuffer<T> : RingBuffer<T>, IDisposable where T : IDisposable
    {
        public DisposableRingBuffer(int capacity, Func<T> bufferFactory) : base(capacity, bufferFactory)
        {
        }

        public DisposableRingBuffer(int capacity, Func<T> bufferFactory, TimeSpan waitTime) : base(capacity, bufferFactory, waitTime)
        {
        }

        public void Dispose()
        {
            T[] buffer = this.availableBuffer.ToArray();

            this.availableBuffer.Clear();

            foreach (T item in buffer)
            {
                item.Dispose();   
            }
            
            GC.SuppressFinalize(this);
        }

           
    }
}
