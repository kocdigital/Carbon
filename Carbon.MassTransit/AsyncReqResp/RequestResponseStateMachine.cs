using Carbon.MassTransit.AsyncReqResp.Events;
using Microsoft.Extensions.Logging;
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
                                _logger.LogInformation($"Request retrieved, now starting! CorrelationId: {ctx.Message.CorrelationId} To: {ctx.Message.DestinationEndpointName} RespAddress: {payload.ResponseAddress}");
                                ctx.Saga.RequestId = ctx.RequestId;
                                ctx.Saga.RequestData = new RequestStarterRequest(ctx.Message.CorrelationId, ctx.Message.RequestBody, ctx.Message.DestinationEndpointName, payload.ResponseAddress);
                            })
                           .TransitionTo(ResponsePendingState)
                           .Activity(x => x.OfInstanceType<RequestHandlerActivity>())
                           .Then(ctx =>
                           {
                               _logger.LogInformation($"Request sent, now pending response! CorrelationId: {ctx.Message.CorrelationId} To: {ctx.Message.DestinationEndpointName}");
                           })
                     );

            During(ResponsePendingState,
                When(ResponseSucceed)
                    .TransitionTo(ResponseSucceedState)
                    .Then(ctx =>
                    {
                        _logger.LogInformation($"Response has been successfully received, finalizing! State : CorrelationId: {ctx.Message.CorrelationId} From: {ctx.Saga.RequestData.DestinationEndpointName}");
                        ctx.Saga.ResponseCode = StaticHelpers.ResponseCode.Ok;
                        ctx.Saga.Response = ctx.Message.ResponseBody;
                    })
                    .TransitionTo(RequestFinalizingState)
                    .Activity(x => x.OfInstanceType<ResponseHandlerActivity>())
                    .Finalize(),
                When(ResponseFailed)
                    .TransitionTo(ResponseFailedState)
                    .Then(ctx =>
                    {
                        _logger.LogInformation($"Response has failed, finalizing! State : CorrelationId: {ctx.Message.CorrelationId} From: {ctx.Saga.RequestData.DestinationEndpointName}");
                        ctx.Saga.ResponseCode = StaticHelpers.ResponseCode.ServerError;
                        ctx.Saga.Response = ctx.Message.ResponseBody;
                    })
                    .TransitionTo(RequestFinalizingState)
                    .Activity(x => x.OfInstanceType<ResponseHandlerActivity>())
                    .Finalize(),
                When(RequestSentFailed)
                    .TransitionTo(ResponseFailedState)
                    .Then(ctx =>
                    {
                        _logger.LogInformation($"Request cannot be sent, no response will be expected, finalizing! State : CorrelationId: {ctx.Message.CorrelationId} From: {ctx.Saga.RequestData.DestinationEndpointName} Exc: {ctx.Message.ErrorMessage}");
                        ctx.Saga.ResponseCode = StaticHelpers.ResponseCode.BadRequest;
                        ctx.Saga.Response = ctx.Message.ErrorMessage;
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
