using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Oragon.Common.RingBuffer
{
    public class RingBuffer<T>
    {
        private readonly Func<T> bufferFactory;

        protected readonly ConcurrentQueue<T> availableBuffer;

        public RingBuffer(int capacity, Func<T> bufferFactory) : this(capacity, bufferFactory, TimeSpan.FromMilliseconds(50))
        {
        }

        public RingBuffer(int capacity, Func<T> bufferFactory, TimeSpan waitTime)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero");
            this.Capacity = capacity;
            this.bufferFactory = bufferFactory ?? throw new ArgumentNullException(nameof(bufferFactory), "BufferFactory can't be null");
            this.WaitTime = waitTime;
            this.availableBuffer = new ConcurrentQueue<T>();
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            for (var i = 0; i < this.Capacity; i++)
            {
                this.availableBuffer.Enqueue(bufferFactory());
            }
        }

        public int Capacity { get; }

        public TimeSpan WaitTime { get; }

        public int Available => this.availableBuffer.Count;

        public virtual AccquisitonController Accquire() => new AccquisitonController(this.availableBuffer, this.WaitTime);

        public class AccquisitonController : IDisposable
        {
            private readonly ConcurrentQueue<T> availableBuffer;

            internal AccquisitonController(ConcurrentQueue<T> availableBuffer, TimeSpan waitTime)
            {
                this.availableBuffer = availableBuffer;

                T tmpBufferElement;

                while (this.availableBuffer.TryDequeue(out tmpBufferElement) == false)
                {
                    System.Diagnostics.Debug.WriteLine($"RingBuffer | Waiting.. Disponibilidade:{availableBuffer.Count}");
                    Thread.Sleep(waitTime);
                }
                System.Diagnostics.Debug.WriteLine($"RingBuffer | Ok! Disponibilidade: {availableBuffer.Count}");
                this.Current = tmpBufferElement;

            }

            public T Current { get; }

            public void Dispose()
            {
                this.availableBuffer.Enqueue(this.Current);

                GC.SuppressFinalize(this);
            }
        }






    }
}
