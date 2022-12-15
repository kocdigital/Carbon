using Automatonymous;
using Carbon.MassTransit.AsyncReqResp.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Carbon.MassTransit.AsyncReqResp
{
    public class ResponseHandlerActivity : CarbonSagaActivity<RequestResponseState>
    {
        private readonly ILogger<RequestResponseState> _logger;
        public ResponseHandlerActivity(ILogger<RequestResponseState> logger, ConsumeContext context) : base(logger, context)
        {
            _logger = logger;
        }

        public async override Task Execute(BehaviorContext<RequestResponseState> context, Behavior<RequestResponseState> next)
        {
            var apiname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            _logger.LogInformation($"Response is now passing to the ResponseHandler To: {apiname}-Req.Resp.Async-RespHandler");
            var message = context.Instance;
            var requestData = message.RequestData;

            ResponseCarrier responseCarrier = new ResponseCarrier();
            responseCarrier.CorrelationId = message.CorrelationId;
            responseCarrier.ResponseBody = message.Response;
            responseCarrier.ResponseCode = message.ResponseCode;
            responseCarrier.ResponseAddress = message.RequestData.ResponseAddress;


            var sendEp = await context.GetSendEndpoint(new Uri("exchange:" + apiname + "-Req.Resp.Async-RespHandler"));
            await sendEp.Send(responseCarrier);

            await next.Execute(context).ConfigureAwait(false);
        }
    }
}
