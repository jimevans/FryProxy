using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace FryProxy.Headers
{
    /// <summary>
    ///     Collection of HTTP headers
    /// </summary>
    public class HttpHeadersCollection
    {
        private const string HeaderValueSeparator = ",";

        private const char HeaderNameSeparator = ':';

        private static readonly char[] HeaderNameSeparatorArray =
        {
            HeaderNameSeparator
        };

        private readonly List<KeyValuePair<string, string>> _headers;

        /// <summary>
        ///     Create new instance of HTTP header collection
        /// </summary>
        public HttpHeadersCollection()
        {
            _headers = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        ///     Create new headers from given strings
        /// </summary>
        /// <param name="headers">
        ///     HTTP message headers as {name} : {value} strings
        /// </param>
        public HttpHeadersCollection(IEnumerable<string> headers) : this()
        {
            Contract.Requires<ArgumentNullException>(headers != null, "headers");

            headers.Where(str => !string.IsNullOrEmpty(str))
                .Select(ParseHeaderLine)
                .ToList()
                .ForEach(Add);
        }

        /// <summary>
        ///     Add or update header to collection
        /// </summary>
        /// <param name="name">header name</param>
        /// <returns>header value</returns>
        public string this[string name]
        {
            get
            {
                return Contains(name)
                    ? string.Join(HeaderValueSeparator, _headers.Where(h => h.Key == name).Select(h => h.Value))
                    : null;
            }

            set
            {
                var newHeader = new KeyValuePair<string, string>(name, value);

                var existingHeaderIndex = _headers.FindIndex(h => h.Key == name);

                if (existingHeaderIndex != -1)
                {
                    _headers[existingHeaderIndex] = newHeader;
                }
                else
                {
                    _headers.Add(newHeader);
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Pairs
        {
            get { return _headers.AsReadOnly(); }
        }

        public IEnumerable<string> Raw
        {
            get { return _headers.Select(FormatHeader); }
        }

        private static KeyValuePair<string, string> ParseHeaderLine(string headerLine)
        {
            Contract.Requires<ArgumentNullException>(headerLine != null, "headerLine");
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(headerLine), "headerLine");

            var header = headerLine.Split(HeaderNameSeparatorArray, 2, StringSplitOptions.None);

            if (header.Length < 2 || string.IsNullOrWhiteSpace(header[0]))
            {
                throw new ArgumentException(string.Format("Invalid header: [{0}]", headerLine), "headerLine");
            }

            return new KeyValuePair<string, string>(header[0].Trim(), header[1].Trim());
        }

        private static string FormatHeader(KeyValuePair<string, string> header)
        {
            return header.Key + HeaderNameSeparator + header.Value;
        }

        public void Add(KeyValuePair<string, string> header)
        {
            _headers.Add(header);
        }

        public void Add(string name, string value)
        {
            Add(new KeyValuePair<string, string>(name, value));
        }

        public bool Remove(string key)
        {
            return _headers.Remove(_headers.Find(h => h.Key == key));
        }

        public int RemoveAll(string name)
        {
            return _headers.RemoveAll(h => h.Key == name);
        }

        public bool Remove(KeyValuePair<string, string> header)
        {
            return _headers.Remove(header);
        }

        public bool Contains(string key)
        {
            return _headers.Any(h => h.Key == key);
        }

        public override string ToString()
        {
            return string.Join("\n", Raw);
        }
    }
}