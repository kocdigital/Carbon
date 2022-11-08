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

        /// <summary>
        /// Use this method anywhere where you want to respond to request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="correlationId"></param>
        /// <param name="responseBody"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task SendResponseToReqRespAsync<T>(this ConsumeContext<T> context, string responseBody, ResponseCode responseCode = ResponseCode.Ok) 
            where T:class
        {
            if (responseCode == ResponseCode.Ok)
            {
                ResponseSucceed responseSucceed = new ResponseSucceed(context.CorrelationId.Value);
                responseSucceed.ResponseBody = responseBody;
                await context.Publish(responseSucceed);
            }
            else if (responseCode == ResponseCode.ServerError)
            {
                ResponseFailed responseFailed = new ResponseFailed(context.CorrelationId.Value);
                responseFailed.ResponseBody = responseBody;
                await context.Publish(responseFailed);
            }
        }

        /// <summary>
        /// Use this method anywhere where you want to respond to request. Keep the correlationId with you. Otherwise, you won't be able to use this extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="correlationId"></param>
        /// <param name="responseBody"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task SendResponseToReqRespAsync(this IReqRespResponderBus context, Guid correlationId, string responseBody, ResponseCode responseCode = ResponseCode.Ok)
        {
            if(correlationId == Guid.Empty)
            {
                throw new Exception("CorrelationId cannot be Guid.Empty");
            }

            if (responseCode == ResponseCode.Ok)
            {
                ResponseSucceed responseSucceed = new ResponseSucceed(correlationId);
                responseSucceed.ResponseBody = responseBody;
                await context.Publish(responseSucceed);
            }
            else if (responseCode == ResponseCode.ServerError)
            {
                ResponseFailed responseFailed = new ResponseFailed(correlationId);
                responseFailed.ResponseBody = responseBody;
                await context.Publish(responseFailed);
            }
        }

        /// <summary>
        /// Use this method directly from your response consumer. Your context is aware of correlationId.
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="responseBody"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
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

        public static async Task<IResponder> GetResponseFromReqRespAsync(this IReqRespRequestorBus reqRespRequestorBus, string requestBody, string responseDestinationPath, RequestTimeout requestTimeout = default)
        {
            var reqClient = reqRespRequestorBus.CreateRequestClient<IRequestStarterRequest>(requestTimeout);

            IRequestStarterRequest requestStarterRequest = new RequestStarterRequest(Guid.NewGuid(), requestBody, "Req.Resp.Async-" + responseDestinationPath);
            var responseTaken = await reqClient.GetResponse<IResponder>(requestStarterRequest);
            return responseTaken.Message;
        }

        public static async Task RespondToReqRespAsync(this ConsumeContext<IResponseCarrier> context, string responseBody, ResponseCode responseCode = ResponseCode.Ok, Guid? requestId = default)
        {
            if(context.Message.ResponseAddress == null)
            {
                throw new Exception(nameof(context.Message.ResponseAddress) + " Not Found!");
            }

            IResponder responder = new Responder(context.CorrelationId.Value);
            responder.ResponseBody = responseBody;
            responder.ResponseCode = responseCode;
            var respEp = await context.GetResponseEndpoint<IResponder>(context.Message.ResponseAddress, requestId ?? context.RequestId);
            await respEp.Send(responder);
        }

        public static async Task SendRequestToReqRespAsync(this IReqRespRequestorBus reqRespRequestorBus, string requestBody, string responseDestinationPath)
        {
            if (String.IsNullOrEmpty(responseDestinationPath))
            {
                throw new Exception("responseDestinationPath cannot be null or empty");
            }

            RequestStarterRequest requestStarterRequest = new RequestStarterRequest(Guid.NewGuid(), requestBody, "Req.Resp.Async-" + responseDestinationPath);
            await reqRespRequestorBus.Publish(requestStarterRequest);
        }
    }
}
