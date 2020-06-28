using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Oragon.Common.RingBuffer.Tests
{
    public class RingBufferThreadTests
    {
        readonly int runCount = 1000;
        readonly int bufferSize = 70;
        readonly TimeSpan workTime = TimeSpan.FromMilliseconds(20);
        readonly TimeSpan waitTime = TimeSpan.FromMilliseconds(10);
        int index = 0;

        [Fact]
        public void TestMultiThreadTest()
        {
            RingBuffer<int> ringBuffer = new Common.RingBuffer.RingBuffer<int>(bufferSize, () => index++, waitTime);

            var factory = new TaskFactory();

            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < runCount; i++)
            {
                Thread thread = new Thread(() => {
                    using var bufferedItem = ringBuffer.Accquire();
                    System.Threading.Thread.Sleep(workTime);
                    System.Diagnostics.Debug.WriteLine($"Item {bufferedItem.Current} - processado!");
                });
                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread thread in threads)
                thread.Join();

        }

    }
}

