// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: callcontext_test.proto
#region Designer generated code

using System; 
using System.Threading.Tasks; 
using DotBPE.Rpc; 
using DotBPE.Protocol.Amp; 
using DotBPE.Rpc.Exceptions; 
using Google.Protobuf; 

namespace DotBPE.IntegrationTesting {

//start for class CallContextTestClient
public sealed class CallContextTestClient : AmpInvokeClient 
{
public CallContextTestClient(IRpcClient<AmpMessage> client) : base(client)
{
}
public async Task<CommonRsp> TestAsync(VoidReq request,int timeOut=3000)
{
AmpMessage message = AmpMessage.CreateRequestMessage(50001, 1);
message.Data = request.ToByteArray();
var response = await base.CallInvoker.AsyncCall(message,timeOut);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new CommonRsp();
}
return CommonRsp.Parser.ParseFrom(response.Data);
}

//同步方法
public CommonRsp Test(VoidReq request)
{
AmpMessage message = AmpMessage.CreateRequestMessage(50001, 1);
message.Data = request.ToByteArray();
var response =  base.CallInvoker.BlockingCall(message);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new CommonRsp();
}
return CommonRsp.Parser.ParseFrom(response.Data);
}
}
//end for class CallContextTestClient
}
#endregion
