using System;
using System.Runtime.Serialization;

namespace Examinator.other
{
    public class TestException : Exception
    {
        public string AdditionalErrorInfo { get; set; }

        public TestException(string message, string additionalInfo) : base(message)
        {
            AdditionalErrorInfo = additionalInfo;
        }

        public TestException(string message) : base(message)
        {
        }

        public TestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
