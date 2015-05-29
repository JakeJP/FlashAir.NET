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
    public class Config : CgiHost
    {
        internal Config(String baseUrl)
            : base(baseUrl + "config.cgi")
        { }

        /// <summary>
        /// Configオブジェクトに対して設定した情報を FlashAir にまとめて送信します。
        /// </summary>
        /// <param name="masterCode"></param>
        /// <returns></returns>
        public bool Submit(String masterCode)
        {
            CONFIG["MASTERCODE"] = masterCode;
            return Get<String>(CONFIG) == "SUCCESS";
        }
        /// <summary>
        /// ローカルで設定したCONFIG情報をすべてクリアします。
        /// </summary>
        public void Clear()
        {
            CONFIG.Clear();
        }
        // CONFIG entries
        Dictionary<String, String> CONFIG = new Dictionary<string, string>();
        TResult GetCONFIG<TResult>(String key) { return (TResult)Convert.ChangeType(CONFIG[key], typeof(TResult), null); }
        /// <summary>
        /// 無線LAN機能の自動タイムアウト時間を設定します。単位はミリ秒です。 設定可能な値は、60000から4294967294で、デフォルト値は300000(5分)です。
        /// 0を指定すると自動停止しない設定になります。ただし、ホスト機器のスリープでカードへの電源供給が遮断された場合などに、停止することがあります。
        /// </summary>
        public TimeSpan DeviceTimeout
        {
            get { return TimeSpan.FromMilliseconds( GetCONFIG<long>("APPAUTOTIME") ); }
            set
            {
                long ms = (long)value.TotalMilliseconds;
                if (ms < 60000 || ms > 4294967294) throw new ArgumentOutOfRangeException("value must be between 60000 and 4294967294.");
                CONFIG["APPAUTOTIME"] = ms.ToString();
            }
        }
        public String AppCustomInfo { get { return CONFIG["APPINFO"]; } set { CONFIG["APPINFO"] = value; } }
        public WiFiOperationMode OperationMode { get { return (WiFiOperationMode)GetCONFIG<int>("APPMODE"); } set { CONFIG["APPMODE"] = ((int)value).ToString(); } }
        public String NetworkPassword { get { return CONFIG["APPNETWORKKEY"]; } set { CONFIG["APPNETWORKKEY"] = value; } }
        public String NetworkSSID { get { return CONFIG["APPSSID"]; } set { CONFIG["APPSSID"] = value; } }
        public String BridgeNetworkPassword { get { return CONFIG["BRGNETWORKKEY"]; } set { CONFIG["BRGNETWORKKEY"] = value; } }
        public String BridgeNetworkSSID { get { return CONFIG["BRGSSID"]; } set { CONFIG["BRGSSID"] = value; } }
        public String ControlImagePath { get { return CONFIG["CIPATH"]; } set { CONFIG["CIPATH"] = value; } }
        public bool ClearMasterCode { get { return CONFIG["CLEARCODE"] == "1"; } set { CONFIG["CLEARCODE"] = value ? "1" : ""; } }
        //public String MasterCode { get { return CONFIG["APPINFO"]; } set { CONFIG["APPINFO"] = value; } }

        public TimeSpan TimeZone { get { return TimeSpan.FromMinutes( GetCONFIG<int>("TIMEZONE") * 15); } set { CONFIG["TIMEZONE"] = ((int)( value.TotalMinutes / 15 )).ToString(); } }
        public WebDAVStatus WebDAVStatus { get { return (WebDAVStatus)GetCONFIG<int>("WEBDAV"); } set { CONFIG["WEBDAV"] = ((int)value).ToString(); } }
    }
}
