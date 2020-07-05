using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Oragon.Common.RingBuffer.Specialized;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oragon.Common.RingBuffer.BenchmarkApp
{
    [SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.NetCoreApp50, targetCount: 5)]
    //[MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class BenchmarkProgram
    {
        DisposableRingBuffer<IConnection> connectionRingBuffer;
        DisposableRingBuffer<IModel> modelRingBuffer;
        ConnectionFactory connectionFactory;

        public BenchmarkProgram()
        {

            //System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));

            
          
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            connectionFactory = new ConnectionFactory()
            {
                Port = 5672,
                HostName = "localhost",
                UserName = "logUser",
                Password = "logPwd",
                VirtualHost = "EnterpriseLog",
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = TimeSpan.FromMinutes(1)
            };

            Func<IConnection> getConnection = () => connectionFactory.CreateConnection();
            Func<IModel> getModel = () =>
            {
                using var connection = connectionRingBuffer.Accquire();
                return connection.Current.CreateModel();
            };

            connectionRingBuffer = new DisposableRingBuffer<IConnection>(10, getConnection, TimeSpan.FromMilliseconds(10));
            modelRingBuffer = new DisposableRingBuffer<IModel>(30, getModel, TimeSpan.FromMilliseconds(10));

        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            modelRingBuffer.Dispose();
            connectionRingBuffer.Dispose();
        }


        [Benchmark]
        public int WithRingBuffer()
        {
            var message = new ReadOnlyMemory<byte>(System.Text.Encoding.UTF8.GetBytes("0"));
            
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            for (var i = 0; i < 100; i++)
                for (var j = 0; j < 30; j++)
                    using (var data = modelRingBuffer.Accquire())
                    {
                        var prop = data.Current.CreateBasicProperties();
                        prop.DeliveryMode = 1;
                        data.Current.BasicPublish("amq.fanout", "none", prop, message);
                    }

            stopwatch.Stop();

          
            Console.WriteLine($"Test 1 | com RingBuffer | {stopwatch.Elapsed:G}");
            return 0;
        }

        [Benchmark]
        public int WithoutRingBuffer()
        {
            var message = new ReadOnlyMemory<byte>(System.Text.Encoding.UTF8.GetBytes("0"));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 100; i++)
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
            return 0;
        }
    }
}
