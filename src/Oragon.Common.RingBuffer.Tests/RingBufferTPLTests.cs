using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;

namespace Oragon.Common.RingBuffer.Tests
{
    public class RingBufferTPLTests
    {
        readonly int runCount = 1000;
        readonly int bufferSize = 70;
        readonly TimeSpan workTime = TimeSpan.FromMilliseconds(20);
        readonly TimeSpan waitTime = TimeSpan.FromMilliseconds(10);
        int index = 0;

        [Fact]
        public void TestMultiTasks()
        {
            RingBuffer<int> ringBuffer = new Common.RingBuffer.RingBuffer<int>(bufferSize, () => index++, waitTime);

            var factory = new TaskFactory();

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < runCount; i++)
            {
                var task = factory.StartNew(() =>
                {
                    using var accquisiton = ringBuffer.Accquire();
                    System.Threading.Thread.Sleep(workTime);                    
                    //System.Diagnostics.Debug.WriteLine($"Item {x.Current} - processado!");

                });
                tasks.Add(task);
            }

            Task.WhenAll(tasks.ToArray()).GetAwaiter().GetResult();

        }
        [Fact]
        public void TestInline()
        {
            RingBuffer<int> ringBuffer = new Common.RingBuffer.RingBuffer<int>(bufferSize, () => index++, waitTime);

            var factory = new TaskFactory();

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < runCount; i++)
            {
                using var accquisiton = ringBuffer.Accquire();
                System.Threading.Thread.Sleep(workTime);
            }
        }
    }
}
