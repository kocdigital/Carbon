namespace Carbon.Common
{
    public class NullErrorCodes : IErrorCodes
    {
        public string GetMessage(int code)
        {
            return "";
        }
    }
}
