using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace FryProxy.Utils
{
    static public class SocketUtils
    {
        public static bool IsSocketException(this Exception exception, params SocketError[] errorCodes) {
            ContractUtils.Requires<ArgumentNullException>(exception != null, "exception");

            var errorCodeList = errorCodes == null 
                ? new List<SocketError>() 
                : errorCodes.ToList();

            for (;exception != null; exception = exception.InnerException)
            {
                var socketException = exception as SocketException;

                if (socketException == null)
                {
                    continue;
                }

                return errorCodeList.Contains(socketException.SocketErrorCode);
            }

            return false;
        }

        public static SocketException AsSocketException(this Exception exception)
        {
            for (; exception != null; exception = exception.InnerException)
            {
                var socketException = exception as SocketException;

                if (socketException == null)
                {
                    continue;
                }

                return socketException;
            }

            return null;
        }
    }
}