namespace MyCv.UI.Dtos
{
    internal sealed class ResponseResult
    {
        internal const string SuccessCode = "SUCCESS";
        internal const string InvalidCommandCode = "INVALID_COMMAND";
        internal const string InvalidDomainCode = "INVALID_DOMAIN";

        public string Code { get; }

        private ResponseResult(string code)
        {
            Code = code;
        }

        internal static ResponseResult Success()
        {
            return new ResponseResult(SuccessCode);
        }

        internal static ResponseResult InvalidCommand()
        {
            return new ResponseResult(InvalidCommandCode);
        }

        internal static ResponseResult InvalidDomain()
        {
            return new ResponseResult(InvalidDomainCode);
        }
    }
}
