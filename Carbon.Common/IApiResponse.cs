using System.Collections.Generic;

namespace Carbon.Common
{
    public interface IApiResponse
    {
        IList<string> Messages { get; }
        int? ErrorCode { get; }
        bool IsSuccess { get; }
        string Identifier { get; }
        void AddMessage(string message);
    }


}
