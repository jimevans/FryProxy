using System;
using System.IO;
using System.Net.Sockets;

namespace FryProxy.IO
{
    /// <summary>
    ///     Responsible for creating streams over sockets
    /// </summary>
    public interface IStreamFactory
    {
        /// <summary>
        ///     Create stream over given socket
        /// </summary>
        /// <param name="socket">Open socket</param>
        /// <returns>Stream wrapping socket</returns>
        Stream CreateStream(Socket socket);

        /// <summary>
        ///     Create stream wrapping socket and set read and write timeouts
        /// </summary>
        /// <param name="socket">Open socket</param>
        /// <param name="readTimeout">Stream read timeout</param>
        /// <param name="writeTimeout">Stream write timeout</param>
        /// <returns>Stream wrapping socket</returns>
        Stream CreateStream(Socket socket, TimeSpan readTimeout, TimeSpan writeTimeout);
    }
}