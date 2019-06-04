using System;
using System.Runtime.Serialization;
using System.Text;

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

        public override string ToString()
        {
            var ressb = new StringBuilder();
         
            if (!string.IsNullOrEmpty(Message))
                ressb.Append($"{Message}");

            if (!string.IsNullOrEmpty(AdditionalErrorInfo))
                ressb.Append($" :{AdditionalErrorInfo}");

            return ressb.ToString();
        }
    }
}
