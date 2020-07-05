using System;
using System.Collections.Generic;
using System.Threading;
using Oragon.Common.RingBuffer.Specialized;
using RabbitMQ.Client;

namespace Oragon.Common.RingBuffer.BenchmarkApp
{
    public class PublisherRoleProgram
    {
        private static ConnectionFactory ConnectionFactory;
        //private static IConnection Connection;
        private static DisposableRingBuffer<IModel> modelRingBuffer;
        private static DisposableRingBuffer<IConnection> connectionRingBuffer;

        static IModel ModelFactory()
        {
            using var connectionWrapper = PublisherRoleProgram.connectionRingBuffer.Accquire();
            IModel model = connectionWrapper.Current.CreateModel();
            model.QueueDeclare("log", false, false, false);
            return model;
        }


        private static void Init()
        {
            PublisherRoleProgram.ConnectionFactory = new ConnectionFactory()
            {
                Port = 5672,
                HostName = "rabbitmq",
                UserName = "logUser",
                Password = "logPwd",
                VirtualHost = "EnterpriseLog",
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromMinutes(1)
            };

            //Program.Connection = Program.ConnectionFactory.CreateConnection();

            PublisherRoleProgram.connectionRingBuffer = new DisposableRingBuffer<IConnection>(5, PublisherRoleProgram.ConnectionFactory.CreateConnection, TimeSpan.FromMilliseconds(5));

            PublisherRoleProgram.modelRingBuffer = new DisposableRingBuffer<IModel>(20, ModelFactory, TimeSpan.FromMilliseconds(5));
        }


        public static void Start()
        {


            Console.WriteLine("Wait... 1"); Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Wait... 2"); Thread.Sleep(TimeSpan.FromSeconds(10));
            //Console.WriteLine("Wait... 3"); Thread.Sleep(TimeSpan.FromSeconds(10));

            Console.WriteLine("Initializing...");

            Init();


            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(".");

            int threadCount = 20;

            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(() =>
                {
                    while (true)
                    {
                        using (var bufferedItem = PublisherRoleProgram.modelRingBuffer.Accquire())
                        {
                            var body = new ReadOnlyMemory<byte>(messageBodyBytes);

                            IBasicProperties props = bufferedItem.Current.CreateBasicProperties();
                            props.ContentType = "text/plain";
                            props.DeliveryMode = 1;

                            bufferedItem.Current.BasicPublish("", "log", false, props, body);
                        }
                    }
                });
                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread thread in threads)
                thread.Join();

            Console.WriteLine("Hello World!");
        }




    }


}
