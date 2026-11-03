using Carbon.MassTransit.AsyncReqResp.Events;
using MassTransit;
using System;
using static Carbon.MassTransit.AsyncReqResp.StaticHelpers;

namespace Carbon.MassTransit.AsyncReqResp
{

    public class RequestResponseState : CarbonStateMachineInstance, ISagaVersion
    {
        public RequestStarterRequest RequestData { get; set; }
        public string Response { get; set; }

        /// <summary>
        /// RequestId of the original request (GetResponseFromReqRespAsync). MassTransit 8 no longer copies the RequestId
        /// to messages sent from a consume context, so it is kept on the saga and set explicitly on the outgoing messages.
        /// </summary>
        public Guid? RequestId { get; set; }
        public ResponseCode ResponseCode { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Func<string, bool> ExecuteActionOnResponse { get; set; }
        #region Data

        public Guid TenantId { get; set; }
        public string ErrorMessage { get; set; }
        public int Version { get; set; }

        #endregion
    }
}
