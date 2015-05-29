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
    [Flags]
    public enum FileAttribute
    {
        ReadOnly =1,
        Hidden=2,
        System=4,
        Volume = 8,
        Directory = 16,
        Archive = 32
    }

    public class FileInfo {
        public String Directory {get;set;}
        public String Name {get;set;}
        public long Size { get; set; }
        public DateTime Time { get; set; }
        public FileAttribute Attributes { get; set; }
        public static DateTime FTIMEtoDateTime(int _date, int _time)
        {
            return new DateTime(1980 + ((_date >> 9) & 0x7F), (_date >> 5) & 0xF, _date & 0x1F, (_time >> 11) & 0x1F, (_time >> 5) & 0x3F, ( _time << 1) & 0x3F );
        }
        public static int DateTimeToFDATE( DateTime date )
        {
            return (((date.Year - 1980) << 9) + (date.Month << 5) + date.Day);
        }
        public static int DateTimeToFTIME(DateTime date)
        {
            return (date.Hour << 11) + (date.Minute << 5) + (date.Second >> 1 );
        }
    }
        
    public class FreeSectorInfo 
    {
        public long Free {get;set;}
        public long Total {get;set;}
        public int SectorSize {get;set;}
    }

    public enum WebDAVStatus
    {
        Disabled = 0,
        ReadOnly = 1,
        ReadWrite = 2
    }

    public class ExifInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ExifOientation Orientation { get; set; }
    }

    public enum ExifOientation
    {
        Horizontal = 1, //=> 'Horizontal (normal)',
        MirrorHorizontal = 2, // => 'Mirror horizontal',
        Rotate180 = 3, // => 'Rotate 180',
        MirrorVertical = 4,// => 'Mirror vertical',
        MirrorHhorizontalAndRotate270CW = 5,// => 'Mirror horizontal and rotate 270 CW',
        Rotate90CW = 6, // => '',
        MirrorHorizontalAndRotate90CW = 7, // => 'Mirror horizontal and rotate 90 CW',
        Rotate270CW = 8 // => 'Rotate 270 CW',
    }

    public enum WiFiOperationMode
    {
        /// <summary>
        /// 「無線起動画面」のライトプロテクト解除操作で無線LAN機能を起動します。無線LANモードはAPモードです。
        /// </summary>
        AccessPointDeferred = 0,
        /// <summary>
        /// 「無線起動画面」のライトプロテクト解除操作で無線LAN機能を起動します。無線LANモードはSTAモードです。
        /// </summary>
        StationDeferred = 2,
        /// <summary>
        /// 「無線起動画面」のライトプロテクト解除操作で無線LAN機能を起動します。無線LANモードはインターネット同時接続モードです。 (ファームウェア 2.00.02以上)
        /// </summary>
        BridgeDeferred = 3,
        /// <summary>
        /// //	カード電源投入時に無線LAN機能を起動します。無線LANモードはAPモードです。
        /// </summary>
        AccessPoint = 4,
        /// <summary>
        /// カード電源投入時に無線LAN機能を起動します。無線LANモードはSTAモードです。
        /// </summary>
        Station = 5,
        /// <summary>
        /// カード電源投入時に無線LAN機能を起動します。無線LANモードはインターネット同時接続モードです。 (ファームウェア 2.00.02以上)
        /// </summary>
        Bridge = 6
    }
}
