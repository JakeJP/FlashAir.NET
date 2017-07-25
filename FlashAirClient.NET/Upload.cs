/*
 * The MIT License (MIT)
 * Copyright (c) 2015 Yokinsoft http://www.yo-ki.com
 * History:
 *  2017.7.25 updated for W-04 compatibility
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
            string boundary = "----faClient" + DateTime.Now.Ticks.ToString("x");
            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent(boundary);
                // Since FlashAir W-04 does not accept quoted bounary string. This trick is nescessary.
                content.Headers.Remove("Content-Type");
                content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

                // Since FlashAir does not cusume filename without quotation enclosure and utf-8 filename*=...
                // we have to rebuild header line.
                var sc = new StreamContent(fileStream);
#if true 
                // Trick to handle non-ascii file name.
                // If your platform cannot handle multibyte characters correctly, exclude following lines might work.
                var bytes = Encoding.UTF8.GetBytes(filename);
                var filename_safe = new Char[bytes.Length];
                for (int i = 0; i < bytes.Length; i++) filename_safe[i] = Convert.ToChar(bytes[i]);
                filename = new string( filename_safe );
#endif
                // Strange behavior of W-04: Content-Type must come after Content-Disposition!!
                sc.Headers.Add("Content-Disposition", @"form-data; name=""file""; filename=""" + filename + @"""");
                sc.Headers.Add("Content-Type", "application/octed-stream");
                content.Add( sc, "file", filename);
                var response = client.PostAsync(BaseUrl, content).Result.Content.ReadAsStringAsync().Result;
                return response != null && (response.Contains("Success") || response.Contains("SUCCESS"));
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
