using System.Collections.Generic;

namespace Carbon.Common
{
    public interface IApiResponse
    {
        List<string> Messages { get; set; }
        int? ErrorCode { get; set; }
        bool IsSuccess { get; }
        string Identifier { get; set; }
    }


}
