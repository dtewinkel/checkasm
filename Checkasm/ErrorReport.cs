using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace CheckAsm
{
    class ErrorReport
    {
        string message = string.Empty;
        string email = string.Empty;

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="email"></param>
        public ErrorReport(string message, string email)
        {
            this.message = message;
            this.email = email;
        }

        /// <summary>
        /// Sends the error report via POST data to web
        /// </summary>
        public void SubmitOnWeb()
        {
            bool succeeded = false;
            while (!succeeded)
            {
                try
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    string postData = "errInfo=" + EncodeTo64(PrepareMessage());
                    byte[] data = encoding.GetBytes(postData);

                    // Prepare web request...
                    HttpWebRequest myRequest =
                      (HttpWebRequest)WebRequest.Create("http://localhost:63454/bug.aspx");
                    myRequest.Method = "POST";
                    myRequest.ContentType = "application/x-www-form-urlencoded";
                    myRequest.ContentLength = data.Length;
                    Stream newStream = myRequest.GetRequestStream();
                    // Send the data.
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                    succeeded = true;
                }
                catch (System.Exception ex)
                {
                    //ignore all unknown errors, error in error dialog would make our customer really angry :)
                    Debug.Assert(false, ex.Message);
                }
            }
        }

        private string PrepareMessage()
        {
            StringBuilder errorReport = new StringBuilder();
            errorReport.AppendLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            errorReport.AppendLine(email);
            errorReport.Append(message);

            string encoded = errorReport.ToString();
            return encoded;
        }

        private static string EncodeTo64(string message)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(message);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }


    }
}
