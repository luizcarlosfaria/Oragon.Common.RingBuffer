using Oragon.Common.RingBuffer.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Oragon.Common.RingBuffer.Tests
{
    public class DisposableRingBufferTests
    {
        readonly int bufferSize = 70;
        readonly int runCount = 1000;
        readonly TimeSpan workTime = TimeSpan.FromMilliseconds(20);
        readonly TimeSpan waitTime = TimeSpan.FromMilliseconds(10);


        [Fact]
        public void TestInline()
        {
            RingBuffer.RingBuffer<DisposableTestableClass> buffer = null;
            Queue<DisposableTestableClass> queue = new Queue<DisposableTestableClass>();
            for (var i = 0; i < bufferSize; i++)
                queue.Enqueue(new DisposableTestableClass());

            using (DisposableRingBuffer<DisposableTestableClass> disposableBuffer = new DisposableRingBuffer<DisposableTestableClass>(
                bufferSize,
                () => queue.Dequeue(),
                waitTime))
            {
                buffer = disposableBuffer;
                var factory = new TaskFactory();
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < runCount; i++)
                {
                    using var x = disposableBuffer.Accquire();
                    System.Threading.Thread.Sleep(workTime);
                }
                Assert.Equal(bufferSize, disposableBuffer.Available);
                Assert.Equal(disposableBuffer.Capacity, disposableBuffer.Available);
            }


            Assert.All(queue, it => Assert.True(it.IsDisposed));
            Assert.Empty(queue);
            Assert.Equal(0, buffer.Available);
            Assert.Equal(bufferSize, buffer.Capacity);

        }


        [Fact]
        public void TestMultiThreadDisposableTest()
        {
            RingBuffer.RingBuffer<DisposableTestableClass> buffer = null;
            Queue<DisposableTestableClass> queue = new Queue<DisposableTestableClass>();
            for (var i = 0; i < bufferSize; i++)
                queue.Enqueue(new DisposableTestableClass());

            using (DisposableRingBuffer<DisposableTestableClass> disposableBuffer = new DisposableRingBuffer<DisposableTestableClass>(
                bufferSize,
                () => queue.Dequeue(),
                waitTime))
            {
                buffer = disposableBuffer;
                List<Thread> threads = new List<Thread>();

                for (int i = 0; i < runCount; i++)
                {
                    Thread thread = new Thread(() => {
                        using var x = disposableBuffer.Accquire();
                        System.Threading.Thread.Sleep(workTime);
                        System.Diagnostics.Debug.WriteLine($"Item {x.Current} - processado!");
                    });
                    thread.Start();
                    threads.Add(thread);
                }

                foreach (Thread thread in threads)
                    thread.Join();

                Assert.Equal(bufferSize, disposableBuffer.Available);
                Assert.Equal(disposableBuffer.Capacity, disposableBuffer.Available);
            }


            Assert.All(queue, it => Assert.True(it.IsDisposed));
            Assert.Empty(queue);
            Assert.Equal(0, buffer.Available);
            Assert.Equal(bufferSize, buffer.Capacity);


           

        }

    }

    public class DisposableTestableClass : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            this.IsDisposed = true;
        }
    }
}
