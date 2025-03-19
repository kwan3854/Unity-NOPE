namespace NOPE_Examples.Scripts.Error
{
    public enum SampleErrorType
    {
        Fatal = 0, // Terminate the application
        Retryable = 1, // Retry the operation
        Ignorable = 2 // Ignore the error
    }
}