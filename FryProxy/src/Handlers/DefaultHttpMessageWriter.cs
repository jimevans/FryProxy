using System;
using System.Diagnostics.Contracts;
using System.IO;
using FryProxy.Messages;

namespace FryProxy.Handlers
{
    internal class DefaultHttpMessageWriter : RequestHandlerSkeleton, IHttpMessageWriter
    {
        public void WriteHttpMessage(HttpMessage httpMessage, Stream outputStream)
        {
            Contract.Requires<ArgumentNullException>(httpMessage != null, "httpMessage");
            Contract.Requires<ArgumentNullException>(outputStream != null, "outputStream");

            try
            {
                httpMessage.WriteTo(outputStream);
            }
            catch (IOException ex)
            {
                if (!HandleSocketException(ex))
                {
                    throw;
                }
            }
        }
    }
}