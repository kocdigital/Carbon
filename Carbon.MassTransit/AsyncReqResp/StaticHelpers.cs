using Carbon.MassTransit.AsyncReqResp.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.MassTransit.AsyncReqResp
{

    public static class StaticHelpers
    {
        public enum ResponseCode
        {
            Ok = 200,
            BadRequest = 400,
            ServerError = 500
        }

        public static async Task SendResponseToReqRespAsync(this ConsumeContext<IRequestCarrierRequest> consumeContext, string responseBody, ResponseCode responseCode = ResponseCode.Ok)
        {
            if (responseCode == ResponseCode.Ok)
            {
                ResponseSucceed responseSucceed = new ResponseSucceed(consumeContext.Message.CorrelationId);
                responseSucceed.ResponseBody = responseBody;
                await consumeContext.Publish(responseSucceed);
            }
            else if (responseCode == ResponseCode.ServerError)
            {
                ResponseFailed responseFailed = new ResponseFailed(consumeContext.Message.CorrelationId);
                responseFailed.ResponseBody = responseBody;
                await consumeContext.Publish(responseFailed);
            }
        }

        public static async Task SendRequestToReqRespAsync(this IReqRespRequestorBus reqRespRequestorBus, string responseBody, string responseDestinationPath)
        {
            if(String.IsNullOrEmpty(responseDestinationPath))
            {
                throw new Exception("responseDestinationPath cannot be null or empty");
            }

            RequestStarterRequest requestStarterRequest = new RequestStarterRequest(Guid.NewGuid(), responseBody, "Req.Resp.Async-" + responseDestinationPath);
            await reqRespRequestorBus.Publish(requestStarterRequest);
        }
    }
}
