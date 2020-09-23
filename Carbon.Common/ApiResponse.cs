using System.Collections.Generic;

namespace Carbon.Common
{
    public class ApiResponse<T> : IApiResponse
    {
        public ApiResponse()
        {

        }
        public ApiResponse(string identifier, ApiStatusCode statusCode)
        {
            StatusCode = statusCode;
            Identifier = identifier;
        }
        public ApiResponse(string identifier, ApiStatusCode statusCode, int errorCode) : this(identifier, statusCode)
        {
            ErrorCode = errorCode;
        }
        public ApiResponse(string identifier, ApiStatusCode statusCode, IList<string> messages) : this(identifier, statusCode)
        {
            Messages = messages;
        }

        public ApiResponse(string identifier, ApiStatusCode statusCode, int errorCode, IList<string> messages) : this(identifier, statusCode, errorCode)
        {
            Messages = messages;
        }
        public T Data { get; set; }
        public IList<string> Messages { get; set; }
        public int? ErrorCode { get; set; }
        public ApiStatusCode StatusCode { get; set; }
        public bool IsSuccess
        {
            get
            {
                return ErrorCode == null && StatusCode == ApiStatusCode.OK;
            }
        }
        public string Identifier { get; set; }

        public void SetData(T data)
        {
            Data = data;
        }

        public void AddMessage(string message)
        {
            if (Messages == null)
                Messages = new List<string>();

            Messages.Add(message);
        }

        public void AddMessages(params string[] messages)
        {
            if (Messages == null)
                Messages = new List<string>();

            if (messages != null)
            {
                foreach (var msg in messages)
                {
                    Messages.Add(msg);
                }
            }

        }

        public void SetErrorCode(int errorCode)
        {
            ErrorCode = errorCode;
        }

    }


}
