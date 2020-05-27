using System;
using System.Collections.Generic;
using System.Linq;

namespace Carbon.Common
{

    public class ErrorCodes : IErrorCodes
    {
        private readonly IList<ErrorCode> Errors;
        private readonly string GeneralErrorMessage = "AnUnspecifiedErrorOccured";

        public ErrorCodes(IList<ErrorCode> errorCodes)
        {
            var errorsGroupByKey = errorCodes.GroupBy(x => x.Key);

            var anyDublicateKey = errorsGroupByKey.Any(x => x.Count() > 1);

            if (anyDublicateKey)
                throw new ArgumentException("Dublicate key found! Please check keys!");

            Errors = errorCodes;
        }

        public string GetMessage(int key)
        {
            var relatedErrorCode = Errors.FirstOrDefault(x => x.Key == key);

            if (relatedErrorCode == null)
                return GeneralErrorMessage;

            return relatedErrorCode.GetMessage();
        }
    }
}
