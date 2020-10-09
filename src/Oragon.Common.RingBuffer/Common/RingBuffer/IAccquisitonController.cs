using System;

namespace Oragon.Common.RingBuffer
{
    public interface IAccquisitonController<out T>: IDisposable
    {
        T Current { get; }
    }
}
