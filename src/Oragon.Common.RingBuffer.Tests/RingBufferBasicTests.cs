using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;

namespace Oragon.Common.RingBuffer.Tests
{
    public class RingBufferBasicTests
    {
        [Fact]
        public void TestDefaultCreation()
        {
            int qnt = 10;
            int i = 0;
            RingBuffer<int> ringBuffer = new Common.RingBuffer.RingBuffer<int>(qnt, () => i++);
            Assert.Equal(ringBuffer.Capacity, ringBuffer.Available);
            Assert.Equal(ringBuffer.Capacity, qnt);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void TestCreationCapacityZero(int value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var _ = new Common.RingBuffer.RingBuffer<int>(value, () => 1);
            });
        }

        [Fact]
        public void TestCreationNullFactory()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = new Common.RingBuffer.RingBuffer<int>(1, null);
            });
        }

       

    }

}
