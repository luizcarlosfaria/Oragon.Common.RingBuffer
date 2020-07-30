using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Oragon.Common.RingBuffer.Specialized;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Oragon.Common.RingBuffer.BenchmarkApp
{
    public class ConsumerRoleProgram
    {
        private static ConnectionFactory ConnectionFactory;
        //private static IConnection Connection;
        private static DisposableRingBuffer<IModel> modelRingBuffer;
        private static DisposableRingBuffer<IConnection> connectionRingBuffer;

        static IModel ModelFactory()
        {
            using var connectionWrapper = ConsumerRoleProgram.connectionRingBuffer.Accquire();
            IModel model = connectionWrapper.Current.CreateModel();
            return model;
        }

        static IConnection ConnectionFactoryFunc()
        {
            IConnection connection = ConsumerRoleProgram.ConnectionFactory.CreateConnection();

            return connection;
        }


        private static void Init()
        {
            ConsumerRoleProgram.ConnectionFactory = new ConnectionFactory()
            {
                Port = 5672,
                HostName = "rabbitmq",
                UserName = "logUser",
                Password = "logPwd",
                VirtualHost = "EnterpriseLog",
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromMinutes(1),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                DispatchConsumersAsync = true,
            };

            //ConsumerRoleProgram.Connection = ConsumerRoleProgram.ConnectionFactory.CreateConnection();

            ConsumerRoleProgram.connectionRingBuffer = new DisposableRingBuffer<IConnection>(2, ConnectionFactoryFunc, TimeSpan.FromMilliseconds(5));

            ConsumerRoleProgram.modelRingBuffer = new DisposableRingBuffer<IModel>(4, ModelFactory, TimeSpan.FromMilliseconds(5));
        }


        public static void Start()
        {
            Console.WriteLine("Wait... 1"); Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Wait... 2"); Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Wait... 3"); Thread.Sleep(TimeSpan.FromSeconds(10));

            Console.WriteLine("Initializing...");

            Init();

            
            for (int i = 0; i < ConsumerRoleProgram.modelRingBuffer.Capacity; i++)
            {
                var bufferedItem = ConsumerRoleProgram.modelRingBuffer.Accquire();
                var consumer = new AsyncEventingBasicConsumer(bufferedItem.Current);
                consumer.Received += async (ch, ea) =>
                {
                    //bufferedItem.Current.BasicAck(ea.DeliveryTag, false);
                    await Task.Yield();
                };
                bufferedItem.Current.BasicConsume("log", true, consumer);
            }

            while (true)
                Console.ReadLine();
        }




    }


}
