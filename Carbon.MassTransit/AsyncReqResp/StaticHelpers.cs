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
        private static MassTransitBusType MassTransitBusType { get; set; }
        public enum ResponseCode
        {
            Ok = 200,
            BadRequest = 400,
            NotFound = 404,
            ServerError = 500
        }

        public static void SetStaticHelperBusType(MassTransitBusType massTransitBusType)
        {
            MassTransitBusType = massTransitBusType;
        }

        public static string GetSendEndpointPrefix()
        {
            if (MassTransitBusType == MassTransitBusType.RabbitMQ)
                return "exchange:";
            else if(MassTransitBusType == MassTransitBusType.AzureServiceBus)
                return "queue:";
            else
                return "exchange:";
        }

        /// <summary>
        /// Use this method anywhere where you want to send a response to request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="correlationId"></param>
        /// <param name="responseBody"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task SendResponseToReqRespAsync<T>(this ConsumeContext<T> context, string responseBody, ResponseCode responseCode = ResponseCode.Ok, Guid? correlationId = default, string sourceAddress = default) 
            where T:class
        {
            if (sourceAddress == default)
            {
                if (responseCode == ResponseCode.Ok)
                {
                    ResponseSucceed responseSucceed = new ResponseSucceed(correlationId ?? context.CorrelationId.Value);
                    responseSucceed.ResponseBody = responseBody;
                    await context.Publish(responseSucceed);
                }
                else
                {
                    ResponseFailed responseFailed = new ResponseFailed(correlationId ?? context.CorrelationId.Value, responseCode);
                    responseFailed.ResponseBody = responseBody;
                    await context.Publish(responseFailed);
                }
            }
            else
            {
                var sendEp = await context.GetSendEndpoint(new Uri(GetSendEndpointPrefix() + sourceAddress));
                if (responseCode == ResponseCode.Ok)
                {
                    ResponseSucceed responseSucceed = new ResponseSucceed(correlationId ?? context.CorrelationId.Value);
                    responseSucceed.ResponseBody = responseBody;
                    await sendEp.Send(responseSucceed);
                }
                else
                {
                    ResponseFailed responseFailed = new ResponseFailed(correlationId ?? context.CorrelationId.Value, responseCode);
                    responseFailed.ResponseBody = responseBody;
                    await sendEp.Send(responseFailed);
                }
            }
        }

        /// <summary>
        /// Use this method anywhere where you want to send a response to request. Keep the correlationId with you. Otherwise, you won't be able to use this extension.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="correlationId"></param>
        /// <param name="responseBody"></param>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task SendResponseToReqRespAsync(this IReqRespResponderBus context, Guid correlationId, string responseBody, ResponseCode responseCode = ResponseCode.Ok, string sourceAddress = default)
        {
            if (correlationId == Guid.Empty)
            {
                throw new Exception("CorrelationId cannot be Guid.Empty");
            }

            if (sourceAddress == default)
            {
                if (responseCode == ResponseCode.Ok)
                {
                    ResponseSucceed responseSucceed = new ResponseSucceed(correlationId);
                    responseSucceed.ResponseBody = responseBody;
                    await context.Publish(responseSucceed);
                }
                else
                {
                    ResponseFailed responseFailed = new ResponseFailed(correlationId, responseCode);
                    responseFailed.ResponseBody = responseBody;
                    await context.Publish(responseFailed);
                }
            }
            else
            {
                var sendEp = await context.GetSendEndpoint(new Uri(GetSendEndpointPrefix() + sourceAddress));
                if (responseCode == ResponseCode.Ok)
                {
                    ResponseSucceed responseSucceed = new ResponseSucceed(correlationId);
                    responseSucceed.ResponseBody = responseBody;
                    await sendEp.Send(responseSucceed);
                }
                else
                {
                    ResponseFailed responseFailed = new ResponseFailed(correlationId, responseCode);
                    responseFailed.ResponseBody = responseBody;
                    await sendEp.Send(responseFailed);
                }
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
            string srcAddress = consumeContext.Message.SourceAddress;
            var sendEp = await consumeContext.GetSendEndpoint(new Uri(GetSendEndpointPrefix() + srcAddress));

            if (responseCode == ResponseCode.Ok)
            {
                ResponseSucceed responseSucceed = new ResponseSucceed(consumeContext.Message.CorrelationId);
                responseSucceed.ResponseBody = responseBody;
                await sendEp.Send(responseSucceed);
            }
            else
            {
                ResponseFailed responseFailed = new ResponseFailed(consumeContext.Message.CorrelationId, responseCode);
                responseFailed.ResponseBody = responseBody;
                await sendEp.Send(responseFailed);
            }
        }

        public static async Task<IResponder> GetResponseFromReqRespAsync(this IReqRespRequestorBus reqRespRequestorBus, string requestBody, string responseDestinationPath, RequestTimeout requestTimeout = default)
        {
            var apiname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var reqClient = reqRespRequestorBus.CreateRequestClient<IRequestStarterRequest>(new Uri(GetSendEndpointPrefix() + apiname + "-request-starter-state"), requestTimeout);

            IRequestStarterRequest requestStarterRequest = new RequestStarterRequest(Guid.NewGuid(), requestBody, "Req.Resp.Async-" + responseDestinationPath);
            var responseTaken = await reqClient.GetResponse<IResponder>(requestStarterRequest);
            return responseTaken.Message;
        }

        public static bool IsRespondable(this ConsumeContext<IResponseCarrier> context)
        {
            return context.Message.ResponseAddress != null;
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

            var apiname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            var sendEp = await reqRespRequestorBus.GetSendEndpoint(new Uri(GetSendEndpointPrefix() + apiname + "-request-starter-state"));
            await sendEp.Send(requestStarterRequest);
        }
    }
}
