using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckAsm.Licensing
{
    class LicenseValidationException:Exception
    {
        public string UserMessage { get; set; }

        public LicenseValidationException()
        {
        }

        public LicenseValidationException(string message):base(message)
        {
            UserMessage = message;
        }

        public LicenseValidationException(string message, string userMessage)
            : base(message)
        {
            UserMessage = userMessage;
        }
    }
}
