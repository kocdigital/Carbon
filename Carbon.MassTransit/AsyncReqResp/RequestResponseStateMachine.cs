using Automatonymous;
using Carbon.MassTransit.AsyncReqResp.Events;
using Microsoft.Extensions.Logging;
using GreenPipes;
using MassTransit;
namespace Carbon.MassTransit.AsyncReqResp
{
    public class RequestResponseStateMachine : MassTransitStateMachine<RequestResponseState>
    {
        #region States
        public State RequestStartedState { get; private set; }
        public State RequestSucceedState { get; private set; }
        public State RequestFailedState { get; private set; }
        public State ResponsePendingState { get; private set; }
        public State ResponseSucceedState { get; private set; }
        public State ResponseFailedState { get; private set; }
        public State RequestFinalizingState { get; private set; }
        #endregion

        private readonly ILogger<RequestResponseStateMachine> _logger;

        public RequestResponseStateMachine(ILogger<RequestResponseStateMachine> logger)
        {
            _logger = logger;

            InstanceState(x => x.CurrentState);

            Initially(
                        When(RequestStarterRequest)
                            .TransitionTo(RequestStartedState)
                            .Then(ctx =>
                            {
                                var payload = ctx.GetPayload<ConsumeContext<IRequestStarterRequest>>();
                                _logger.LogInformation($"Request retrieved, now starting! CorrelationId: {ctx.Data.CorrelationId} To: {ctx.Data.DestinationEndpointName} RespAddress: {payload.ResponseAddress}");
                                ctx.Instance.RequestData = new RequestStarterRequest(ctx.Data.CorrelationId, ctx.Data.RequestBody, ctx.Data.DestinationEndpointName, payload.ResponseAddress);
                            })
                           .TransitionTo(ResponsePendingState)
                           .Activity(x => x.OfInstanceType<RequestHandlerActivity>())
                           .Then(ctx =>
                           {
                               _logger.LogInformation($"Request sent, now pending response! CorrelationId: {ctx.Data.CorrelationId} To: {ctx.Data.DestinationEndpointName}");
                           })
                     );

            During(ResponsePendingState,
                When(ResponseSucceed)
                    .TransitionTo(ResponseSucceedState)
                    .Then(ctx =>
                    {
                        _logger.LogInformation($"Response has been successfully received, finalizing! State : CorrelationId: {ctx.Data.CorrelationId} From: {ctx.Instance.RequestData.DestinationEndpointName}");
                        ctx.Instance.ResponseCode = StaticHelpers.ResponseCode.Ok;
                        ctx.Instance.Response = ctx.Data.ResponseBody;
                    })
                    .TransitionTo(RequestFinalizingState)
                    .Activity(x => x.OfInstanceType<ResponseHandlerActivity>())
                    .Finalize(),
                When(ResponseFailed)
                    .TransitionTo(ResponseFailedState)
                    .Then(ctx =>
                    {
                        _logger.LogInformation($"Response has failed, finalizing! State : CorrelationId: {ctx.Data.CorrelationId} From: {ctx.Instance.RequestData.DestinationEndpointName}");
                        ctx.Instance.ResponseCode = StaticHelpers.ResponseCode.ServerError;
                        ctx.Instance.Response = ctx.Data.ResponseBody;
                    })
                    .TransitionTo(RequestFinalizingState)
                    .Activity(x => x.OfInstanceType<ResponseHandlerActivity>())
                    .Finalize(),
                When(RequestSentFailed)
                    .TransitionTo(ResponseFailedState)
                    .Then(ctx =>
                    {
                        _logger.LogInformation($"Request cannot be sent, no response will be expected, finalizing! State : CorrelationId: {ctx.Data.CorrelationId} From: {ctx.Instance.RequestData.DestinationEndpointName} Exc: {ctx.Data.ErrorMessage}");
                        ctx.Instance.ResponseCode = StaticHelpers.ResponseCode.BadRequest;
                        ctx.Instance.Response = ctx.Data.ErrorMessage;
                    })
                    .TransitionTo(RequestFinalizingState)
                    .Activity(x => x.OfInstanceType<ResponseHandlerActivity>())
                    .Finalize()
                );

            SetCompletedWhenFinalized();
        }


        public Event<IRequestStarterRequest> RequestStarterRequest { get; private set; }

        public Event<IRequestSentSucceed> RequestSentSucceed { get; private set; }
        public Event<IRequestSentFailed> RequestSentFailed { get; private set; }
        public Event<IResponseSucceed> ResponseSucceed { get; private set; }
        public Event<IResponseFailed> ResponseFailed { get; private set; }
        
    }
}
