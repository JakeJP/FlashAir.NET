/*
 * The MIT License (MIT)
 * Copyright (c) 2015 Yokinsoft http://www.yo-ki.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace FlashAirClient
{
    /// <summary>
    /// base class for CGI methods
    /// </summary>
    public abstract class CgiHost
    {
        public String BaseUrl;
        internal CgiHost(String path)
        {
            BaseUrl = path;
        }
        protected HttpResponseMessage GetResponse(String url)
        {
            var client = new System.Net.Http.HttpClient();
            var result = client.GetAsync(url).Result;
            return result;
        }
        protected TResult Get<TResult>( String url )
        {
            var result = GetResponse(url);
            if (typeof(TResult) == typeof(System.IO.Stream))
            {
                return (TResult)(object)result.Content.ReadAsStreamAsync().Result;
            }
            else
                return (TResult)Convert.ChangeType(result.Content.ReadAsStringAsync().Result, typeof(TResult), null);
        }
        protected TResult Get<TResult>(IEnumerable<KeyValuePair<string, string>> qs)
        {
            var url = BaseUrl; var connector = "?";
            foreach (KeyValuePair<String, String> kv in qs)
            {
                url += connector + Uri.EscapeUriString(kv.Key) + "=" + Uri.EscapeUriString(kv.Value);
                connector = "&";
            }
            return Get<TResult>(url);
        }
    }
}
