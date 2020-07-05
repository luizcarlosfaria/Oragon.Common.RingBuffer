using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading;
using BenchmarkDotNet.Running;
using Oragon.Common.RingBuffer.Specialized;
using RabbitMQ.Client;

namespace Oragon.Common.RingBuffer.BenchmarkApp
{
    class Program
    {
        static int Main(string[] args)
        {
            var role = RunAsCommand(args);

            switch (role)
            {
                case "benchmark":
                    var summary = BenchmarkRunner.Run<BenchmarkProgram>();
                    BenchmarkProgram.Analyse(summary);
                break;
                case "publisher": PublisherRoleProgram.Start(); break;
                case "consumer": ConsumerRoleProgram.Start(); break;
                default:
                    Console.WriteLine($"role {role} not found");
                    return -1;
            }

            return 0;
        }


        static string RunAsCommand(string[] args)
        {
            var root = new RootCommand("RingBuffer BenchmarkApp") {
                new Option<string>("--role", "--role"){
                }
            };
            var parseResult = new CommandLineBuilder(root)
            .Build()
            .Parse(args);

            if (parseResult.Errors.Count > 0)
            {
                foreach (var erro in parseResult.Errors)
                    Console.WriteLine(erro.Message);

                throw new InvalidOperationException();
            }
            Console.WriteLine(parseResult.Diagram());


            var role = parseResult.ValueForOption<string>("role");

            return role;
        }

    }
}
