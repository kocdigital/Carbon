using System.Collections.Generic;

namespace Carbon.Common
{
    public class ApiResponse<T> : IApiResponse
    {
        public ApiResponse()
        {
            Messages = new List<string>();
        }
        public ApiResponse(string identifier)
        {
            Identifier = identifier;
            Messages = new List<string>();
        }
        public T Data { get; set; }
        public List<string> Messages { get; set; }
        public int? ErrorCode { get; set; }
        public bool IsSuccess
        {
            get
            {
                return ErrorCode == null;
            }
        }
        public string Identifier { get; set; }


    }


}
