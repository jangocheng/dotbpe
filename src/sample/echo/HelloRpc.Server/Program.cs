﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Text;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Netty;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Extensions;
using DotBPE.Rpc.Hosting;
using Microsoft.Extensions.Logging;
using HelloRpc.Common;

namespace HelloRpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;


            var host = new RpcHostBuilder()
                .AddRpcCore<AmpMessage>()
                .UseNettyServer<AmpMessage>()
                .UseAmp()
                .AddServiceActor(new GreeterImpl())
                .Build();
          
            Task.Factory.StartNew(async () =>
            {
                //启动主机
                await host.StartAsync();
                Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            });
            Console.ReadLine();

        }
    }

    public class GreeterImpl : GreeterBase
    {
        public override Task<HelloReply> SayHelloAsnyc(HelloRequest request)
        {
            var reply = new HelloReply() { Message = "Hello " + request.Name };
            return Task.FromResult(reply);
        }
    }

}