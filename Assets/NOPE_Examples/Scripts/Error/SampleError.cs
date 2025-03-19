namespace NOPE_Examples.Scripts.Error
{
    public readonly struct SampleError
    {
        public SampleErrorType Type { get; }
        public string Message { get; }

        public SampleError(SampleErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}