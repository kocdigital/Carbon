namespace Carbon.Common
{
    public class ErrorCode
    {
        public ErrorCode(int key, string code)
        {
            Key = key;
            Code = code;
        }
        public int Key { get; private set; }
        public string Code { get; private set; }
        public string GetMessage()
        {
            return Code;
        }
    }
}
