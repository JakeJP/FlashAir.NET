/*
 * The MIT License (MIT)
 * Copyright (c) 2015 Yokinsoft http://www.yo-ki.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashAirClient
{
    public class Command : CgiHost
    {
        internal Command(String baseUrl)
            : base(baseUrl + "command.cgi")
        {
        }
        protected TResult Get<TResult>(int op)
        {
            var kv = new Dictionary<String, String> {
                { "op", op.ToString() }
            };
            return Get<TResult>(kv);
        }
        protected TResult Get<TResult>(int op, Dictionary<String, String> qs)
        {
            qs["op"] = op.ToString();
            return Get<TResult>(qs);
        }

        public IEnumerable<FileInfo> FileList(String dir)
        {
            var stream = Get<System.IO.Stream>(100, new Dictionary<string, string> { { "DIR", dir } });
            var reader = new System.IO.StreamReader(stream);
            var firstline = reader.ReadLine();
            if (firstline != "WLANSD_FILELIST") throw new InvalidOperationException("First line is not WLANSD_FILELIST.");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var ar = line.Split(",".ToCharArray());
                var _date = int.Parse(ar[4]);
                var _time = int.Parse(ar[5]);
                var f = new FileInfo
                {
                    Directory = ar[0],
                    Name = ar[1],
                    Size = long.Parse(ar[2]),
                    Attributes = (FileAttribute)int.Parse(ar[3]),
                    Time = _date == 0 ? DateTime.MinValue : FileInfo.FTIMEtoDateTime(_date,_time)

                };
                yield return f;
            }
        }
        public int FileCount(String dir)
        {
            return Get<int>(101, new Dictionary<String, String> { { "DIR", dir } });
        }
        public bool IsUpdated { get { return Get<int>(102) == 1; } }
        public String MAC { get { return Get<String>(106); } }
        public String AcceptLanguage { get { return Get<String>(107); } }
        public String FirmwareVersion { get { return Get<String>(108); } }

        // config.cgi entries
        public TimeSpan DeviceTimeout { get { return TimeSpan.FromMilliseconds( Get<long>(111)); } }
        public String NetworkSSID { get { return Get<String>(104); } }
        public String NetworkPassword { get { return Get<String>(105); } }
        public String ControlImagePath { get { return Get<String>(109); } }
        public WiFiOperationMode OperationMode { get { return (WiFiOperationMode)Get<int>(110); } }
        public String AppCustomInfo { get { return Get<String>(117); } }
        public bool UploadEnabled { get { return Get<String>(118) == "1"; } }
        public String CID { get { return Get<String>(120); } }
        public long LastUpdatedTimestamp { get { return Get<long>(121); } }

        public byte[] ReadExtensionRegisterSingleBlockCommand(int addr, int len)
        {
            throw new NotImplementedException();
        }
        public bool WriteExtensionRegisterSingleBlockCommand(int addr, int len, byte[] data)
        {
            throw new NotImplementedException();
        }

        public FreeSectorInfo GetAvailableSectorInfo()
        {
            var line = Get<String>(140).Split("/,".ToCharArray());
            return new FreeSectorInfo { Free = long.Parse(line[0]), Total = long.Parse(line[1]), SectorSize = int.Parse(line[2]) };
        }

        public bool TurnOnPhotoShare(String dir, DateTime date )
        {
            var qs = new Dictionary<string,string> {
                {"DIR", dir },
                {"DATE", FileInfo.DateTimeToFDATE(date).ToString() }
            };
            return Get<String>(200, qs) == "OK";
        }
        public bool TurnOffPhotoShare()
        {
            return Get<String>(201) == "OK";
        }
        public bool IsPhotoShareEnabled()
        {
            return Get<String>(202) == "SHAREMODE";
        }
        public String PhotoShareSSID
        {
            get { return Get<String>(203); }
        }
        public WebDAVStatus WebDAVStatus
        {
            get { return (WebDAVStatus)Get<int>(220); }
        }
        public TimeSpan TimeZone
        {
            get { return TimeSpan.FromMinutes( Get<int>(221) * 15 ); }
        }
    }
}
