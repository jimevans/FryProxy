using System;
using System.IO;
using System.Net.Sockets;

namespace FryProxy.IO
{
    public class DefaultStreamFactory : IStreamFactory
    {
        public Stream CreateStream(Socket socket)
        {
            return new NetworkStream(socket, true);
        }

        public Stream CreateStream(Socket socket, TimeSpan readTimeout, TimeSpan writeTimeout)
        {
            var stream = CreateStream(socket);
            stream.ReadTimeout = (int) readTimeout.TotalMilliseconds;
            stream.WriteTimeout = (int) writeTimeout.TotalMilliseconds;

            return stream;
        }
    }
}