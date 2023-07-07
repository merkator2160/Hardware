using System.Runtime.Serialization;

namespace Common.Contracts.Exceptions.Application
{
    public class ApiRequestExecutionException : ApplicationException
    {
        public ApiRequestExecutionException()
        {

        }
        public ApiRequestExecutionException(String message) : base(message)
        {

        }
        public ApiRequestExecutionException(String message, Exception ex) : base(message)
        {

        }
        protected ApiRequestExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}