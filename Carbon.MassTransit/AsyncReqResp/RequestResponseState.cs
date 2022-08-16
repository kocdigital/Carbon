using Carbon.MassTransit.AsyncReqResp.Events;
using MassTransit.RedisIntegration;
using System;
using static Carbon.MassTransit.AsyncReqResp.StaticHelpers;

namespace Carbon.MassTransit.AsyncReqResp
{

    public class RequestResponseState : CarbonStateMachineInstance, IVersionedSaga
    {
        public RequestStarterRequest RequestData { get; set; }
        public string Response { get; set; }
        public ResponseCode ResponseCode { get; set; }

        public Func<string, bool> ExecuteActionOnResponse { get; set; }
        #region Data

        public Guid TenantId { get; set; }
        public string ErrorMessage { get; set; }
        public int Version { get; set; }

        #endregion
    }
}
