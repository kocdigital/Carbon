using Automatonymous;
using Carbon.MassTransit.AsyncReqResp.Events;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core.Contexts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Carbon.MassTransit.AsyncReqResp
{
    public class RequestHandlerActivity : CarbonSagaActivity<RequestResponseState>
    {
        public RequestHandlerActivity(ILogger<RequestResponseState> logger, ConsumeContext context) : base(logger, context)
        {

        }

        public async override Task Execute(BehaviorContext<RequestResponseState> context, Behavior<RequestResponseState> next)
        {
            var message = context.Instance;
            var requestData = message.RequestData;
            var srcAddress = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + "-request-starter-state";
            try
            {
                RequestCarrierRequest requestCarrierRequest = new RequestCarrierRequest(message.CorrelationId, message.RequestData.RequestBody, srcAddress);
                var sendEp = await context.GetSendEndpoint(new Uri(StaticHelpers.GetSendEndpointPrefix() + requestData.DestinationEndpointName));
                await sendEp.Send(requestCarrierRequest);
            }
            catch (Exception ex)
            {
                var sendEp = await context.GetSendEndpoint(new Uri(StaticHelpers.GetSendEndpointPrefix() + srcAddress));
                RequestSentFailed requestSentFailed = new RequestSentFailed(message.CorrelationId);
                requestSentFailed.ErrorMessage = ex.Message;
                requestSentFailed.StackTrace = ex.StackTrace;

                await sendEp.Send(requestSentFailed)
                                    .ConfigureAwait(false);
            }



            await next.Execute(context).ConfigureAwait(false);
        }
    }
}
