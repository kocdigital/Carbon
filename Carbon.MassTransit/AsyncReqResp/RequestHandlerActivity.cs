using Automatonymous;
using Carbon.MassTransit.AsyncReqResp.Events;
using MassTransit;
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
            try
            {
                RequestCarrierRequest requestCarrierRequest = new RequestCarrierRequest(message.CorrelationId, message.RequestData.RequestBody);
                var sendEp = await context.GetSendEndpoint(new Uri("queue:" + requestData.DestinationEndpointName));
                await sendEp.Send(requestCarrierRequest);
            }
            catch(Exception ex)
            {
                RequestSentFailed requestSentFailed = new RequestSentFailed(message.CorrelationId);
                requestSentFailed.ErrorMessage = ex.Message;
                requestSentFailed.StackTrace = ex.StackTrace;

                await context.Publish(requestSentFailed)
                                    .ConfigureAwait(false);
            }



            await next.Execute(context).ConfigureAwait(false);
        }
    }
}
