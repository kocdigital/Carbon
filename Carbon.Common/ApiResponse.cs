using System.Collections.Generic;

namespace Carbon.Common
{
    public class ApiResponse<T> : IApiResponse
    {
        public ApiResponse()
        {

        }
        public ApiResponse(string identifier)
        {
            Identifier = identifier;
        }
        public ApiResponse(string identifier, int errorCode) : this(identifier)
        {
            ErrorCode = errorCode;
        }
        public ApiResponse(string identifier, IList<string> messages) : this(identifier)
        {
            Messages = messages;
        }
        public ApiResponse(string identifier, int errorCode, IList<string> messages) : this(identifier, errorCode)
        {
            Messages = messages; 
        }
        public T Data { get; set; }
        public IList<string> Messages { get; private set; }
        public int? ErrorCode { get; private set; }
        public bool IsSuccess
        {
            get
            {
                return ErrorCode == null;
            }
        }
        public string Identifier { get; private set; }

        public void AddMessage(string message)
        {
            if (Messages == null)
                Messages = new List<string>();

            Messages.Add(message);
        }

        public void SetErrorCode(int errorCode)
        {
            ErrorCode = errorCode;
        }

    }


}
