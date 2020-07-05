using System;
using System.Collections.Generic;
using System.Text;

namespace Oragon.Common.RingBuffer
{
    public interface IAccquisitonController<T>: IDisposable
    {
        T Current { get; }
    }
}
