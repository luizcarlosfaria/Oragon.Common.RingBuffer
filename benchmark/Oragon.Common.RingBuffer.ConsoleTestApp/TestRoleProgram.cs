using Oragon.Common.RingBuffer.Specialized;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oragon.Common.RingBuffer.ConsoleTestApp
{
    public static class TestRoleProgram
    {
        static DisposableRingBuffer<IConnection> connectionRingBuffer;
        static DisposableRingBuffer<IModel> modelRingBuffer;
        static ConnectionFactory connectionFactory;

        internal static void Start(string[] args)
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));

            connectionFactory = new ConnectionFactory()
            {
                Port = 5672,
                HostName = "rabbitmq",
                UserName = "logUser",
                Password = "logPwd",
                VirtualHost = "EnterpriseLog",
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromMinutes(1)
            };

            Test2();
            Test1();

            Test1();
            Test2();

            Test2();
            Test1();

            Test1();
            Test2();

            Test2();
            Test1();

            Test1();
            Test2();
        }

        public static void Test1()
        {
            var message = new ReadOnlyMemory<byte>(System.Text.Encoding.UTF8.GetBytes("0"));
            Func<IConnection> getConnection = () => connectionFactory.CreateConnection();
            Func<IModel> getModel = () =>
            {
                using var connection = connectionRingBuffer.Accquire();
                return connection.Current.CreateModel();
            };

            connectionRingBuffer = new DisposableRingBuffer<IConnection>(10, getConnection, TimeSpan.FromMilliseconds(10));
            modelRingBuffer = new DisposableRingBuffer<IModel>(15, getModel, TimeSpan.FromMilliseconds(10));

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 30; j++)
                    using (var data = modelRingBuffer.Accquire())
                    {
                        var prop = data.Current.CreateBasicProperties();
                        prop.DeliveryMode = 1;
                        data.Current.BasicPublish("amq.fanout", "none", prop, message);
                    }

            stopwatch.Stop();

            modelRingBuffer.Dispose();
            connectionRingBuffer.Dispose();
            Console.WriteLine($"Test 1 | com RingBuffer | {stopwatch.Elapsed:G}");
        }

        public static void Test2()
        {
            var message = new ReadOnlyMemory<byte>(System.Text.Encoding.UTF8.GetBytes("0"));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 10; i++)
                using (var connection = connectionFactory.CreateConnection())
                {
                    for (var j = 0; j < 30; j++)
                        using (var model = connection.CreateModel())
                        {
                            var prop = model.CreateBasicProperties();
                            prop.DeliveryMode = 1;
                            model.BasicPublish("amq.fanout", "none", prop, message);
                        }
                }
            stopwatch.Stop();

            Console.WriteLine($"Test 2 | sem RingBuffer | {stopwatch.Elapsed:G}");
        }
    }
}
