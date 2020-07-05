using System;
using System.Collections.Generic;
using System.Threading;
using Oragon.Common.RingBuffer.Specialized;
using RabbitMQ.Client;

namespace Oragon.Common.RingBuffer.ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestRoleProgram.Start(args);
            return;

            if (args[0] == "role" && args[1] == "publisher")
            {
                PublisherRoleProgram.Start(args);
                //TestRoleProgram.Start(args);
            }
            else if (args[0] == "role" && args[1] == "consumer")
            {
                ConsumerRoleProgram.Start(args);
            }
        }

    }
}
