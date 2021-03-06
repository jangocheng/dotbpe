﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotBPE.Rpc.Codes;
using DotBPE.Rpc.Logging;
using DotBPE.Rpc.Utils;

namespace DotBPE.Rpc.DefaultImpls
{
    public class DefaultTransportFactory<TMessage>:ITransportFactory<TMessage> where TMessage:InvokeMessage
    {
        static readonly ILogger Logger = Environment.Logger.ForType<DefaultTransportFactory<TMessage>>();
        private readonly IClientBootstrap<TMessage> _bootstrap;

        private readonly Dictionary<string, Lazy<ITransport<TMessage>>> _clients
            = new Dictionary<string, Lazy<ITransport<TMessage>>>();

        private static readonly object lockObj = new object();
        public DefaultTransportFactory(IClientBootstrap<TMessage> bootstrap)
        {
            this._bootstrap = bootstrap;
            //this._bootstrap.DisConnected += Bootstrap_Disconnected;
        }

        private bool RemoveTransport(EndPoint key,out Lazy<ITransport<TMessage>> value){
            string addressKey = ParseUtils.ParseEndPointToString(key);
            bool s = false;
            value = null;
            lock(lockObj){
                if(this._clients.ContainsKey(addressKey)){
                    value = this._clients[addressKey];
                    this._clients.Remove(addressKey);
                    s = true;
                }
                else{
                    Logger.Warning("no transprot address{0}",addressKey);
                }
            }
            return s;
        }
        private Lazy<ITransport<TMessage>> GetOrAdd(EndPoint key,Func<EndPoint,Lazy<ITransport<TMessage>>> createAction){
            Lazy<ITransport<TMessage>> value = null;
            string addressKey = ParseUtils.ParseEndPointToString(key);
            lock(lockObj){
                if(this._clients.ContainsKey(addressKey)){
                    value = this._clients[addressKey];
                }
            }
            if(value!=null){
                return value;
            }
            lock(lockObj){
                if(!this._clients.ContainsKey(addressKey)){
                    value = createAction.Invoke(key);
                    this._clients.Add(addressKey,value);
                }
            }
            if(value!=null){
                return value;
            }
            return GetOrAdd(key,createAction);
        }
        private void Bootstrap_Disconnected(object sender, DisConnectedArgs args)
        {
            var removed = RemoveTransport(args.EndPoint, out var _);
        }

        public ITransport<TMessage> CreateTransport(EndPoint endpoint)
        {
            try
            {
                return GetOrAdd(endpoint
                    , k => new Lazy<ITransport<TMessage>>(() =>
                        {
                            var context = _bootstrap.ConnectAsync(k).Result;
                            var transportans = new DefaultTransport<TMessage>(context);
                            return transportans;
                        }
                    )).Value;
            }
            catch
            {
                RemoveTransport(endpoint, out var _);
                throw;
            }
        }

        public async Task CloseTransportAsync(EndPoint serverAddress)
        {
            try
            {
                bool success = RemoveTransport(serverAddress, out var lazyTransport);
                if (success && lazyTransport !=null && lazyTransport.IsValueCreated)
                {
                    await lazyTransport.Value.CloseAsync();
                }
            }
            catch(Exception ex)
            {
                Logger.Error($"close connection {serverAddress} ,Exception:" + ex.ToString());
            }
        }
    }


}
