/*
 * The MIT License (MIT)
 * Copyright (c) 2015 Yokinsoft http://www.yo-ki.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.IO;

namespace FlashAirClient
{
    public class Upload : CgiHost
    {
        internal Upload(String baseUrl)
            : base(baseUrl + "upload.cgi"){}

        public bool UploadFile( Stream fileStream, String filename )
        {
            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add( new StreamContent(fileStream), "file", filename );
                return client.PostAsync(BaseUrl, content).Result.Content.ReadAsStringAsync().Result == "SUCCESS";
            }
        }
        public bool DeleteFile( String path )
        {
            return Get<String>(new Dictionary<String, String> { { "DEL", path } }) == "SUCCESS";
        }
        public bool SetUploadDirectory( String dir )
        {
            return Get<String>(new Dictionary<String, String> { { "UPDIR", dir } }) == "SUCCESS";
        }
        public bool SetSystemTime( DateTime time )
        {
            return Get<String>(new Dictionary<String, String> { { "FTIME", "0x" + ((FileInfo.DateTimeToFDATE(time) << 16) + FileInfo.DateTimeToFTIME(time)).ToString("X") } }) == "SUCCESS";
        }
        public bool SetWriteProtect()
        {
            return Get<String>(new Dictionary<String, String> { { "WRITEPROTECT", "ON" } }) == "SUCCESS";
        }
    }
}
