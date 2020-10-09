using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Oragon.Common.RingBuffer
{
    public class RingBuffer<T>
    {
        private readonly Func<T> itemFactoryFunc;

        protected readonly ConcurrentQueue<T> availableBuffer;

        public RingBuffer(int capacity, Func<T> bufferFactory) : this(capacity, bufferFactory, TimeSpan.FromMilliseconds(50))
        {
        }

        public RingBuffer(int capacity, Func<T> itemFactoryFunc, TimeSpan waitTime)
        {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero");
            this.Capacity = capacity;
            this.itemFactoryFunc = itemFactoryFunc ?? throw new ArgumentNullException(nameof(itemFactoryFunc), "ItemFactoryFunc can't be null");
            this.WaitTime = waitTime;
            this.availableBuffer = new ConcurrentQueue<T>();
            this.Initialize();
        }

        private void Initialize()
        {
            for (var i = 0; i < this.Capacity; i++)
            {
                this.availableBuffer.Enqueue(itemFactoryFunc());
            }
        }

        public int Capacity { get; }

        public TimeSpan WaitTime { get; }

        public int Available => this.availableBuffer.Count;

        public virtual IAccquisitonController<T> Accquire() => new AccquisitonController(this.availableBuffer, this.WaitTime);

        public class AccquisitonController : IAccquisitonController<T>
        {
            private readonly ConcurrentQueue<T> availableBuffer;

            internal AccquisitonController(ConcurrentQueue<T> availableBuffer, TimeSpan waitTime)
            {
                this.availableBuffer = availableBuffer;

                T tmpBufferElement;

                while (this.availableBuffer.TryDequeue(out tmpBufferElement) is false)
                {

#if DEBUG
                    System.Diagnostics.Debug.WriteLine($"RingBuffer | Waiting.. Disponibilidade:{availableBuffer.Count}");
#endif

                    Thread.Sleep(waitTime);
                }

#if DEBUG
                System.Diagnostics.Debug.WriteLine($"RingBuffer | Ok! Disponibilidade: {availableBuffer.Count}");
#endif
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
