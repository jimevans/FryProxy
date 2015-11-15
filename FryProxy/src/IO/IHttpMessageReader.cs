using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FryProxy.IO
{
    public interface IHttpMessageReader
    {

        HttpResponseMessage ReadResponseMessage(Stream stream);

        HttpRequestMessage ReadRequestMessage(Stream stream);
    }
}
