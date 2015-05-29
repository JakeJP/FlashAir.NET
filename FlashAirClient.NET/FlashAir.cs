/*
 * FlashAir(TM) client library for .NET
 * 
 * The MIT License (MIT)
 * Copyright (c) 2015 Yokinsoft http://www.yo-ki.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;

namespace FlashAirClient
{
    /// <summary>
    /// FlashAirへのアクセスのスタートポイントとなります。コンストラクタの引数にFlashAirへのURLを指定します。
    /// 
    /// </summary>
    /// <remarks>
    /// URLの最後は / で終わっている必要があります。
    /// </remarks>
    /// <example>
    /// var fa = new FlashAir("http://flashair/");
    /// </example>
    public class FlashAir
    {
        public String BaseUrl {　get;　set;}
        Command _command = null;
        /// <summary>
        /// command.cgi のオブジェクト表現
        /// </summary>
        public Command Command {
            get 
            {
                if( _command == null ){
                    _command = new Command(BaseUrl);
                }
                return _command;
            }
        }
        Thumbnail _thumbnail = null;
        /// <summary>
        /// thumbnail.cgi のオブジェクト表現
        /// </summary>
        public Thumbnail Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    _thumbnail = new Thumbnail(BaseUrl);
                }
                return _thumbnail;
            }
        }
        Config _config = null;
        /// <summary>
        /// config.cgi のオブジェクト表現
        /// </summary>
        public Config Config
        {
            get 
            {
                if (_config == null)
                {
                    _config = new Config(BaseUrl);
                }
                return _config;
            }
        }
        Upload _upload = null;
        /// <summary>
        /// upload.cgi のオブジェクト表現
        /// </summary>
        public Upload Upload
        {
            get
            {
                if (_upload == null)
                {
                    _upload = new Upload(BaseUrl);
                }
                return _upload;
            }
        }
        /// <summary>
        /// ファイルを Stream としてダウンロードします。
        /// </summary>
        /// <param name="path">ファイルのフルパスを指定します。</param>
        /// <returns></returns>
        public Stream GetFile(String path)
        {
            using (var client = new HttpClient())
            {
                var url = new Uri(new Uri(BaseUrl), path);
                var stream = client.GetStreamAsync( url ).Result;
                return stream;
            }
        }
        public Stream GetFile( FileInfo file)
        {
            return GetFile( file.Directory + file.Name );
        }
        public String GetFileUrl(String path)
        {
            path = path.Replace("\\", "/");
            return BaseUrl + path;
        }
        public String GetFileUrl(FileInfo file)
        {
            return GetFileUrl( file.Directory + "\\" + file.Name );
        }

        public FlashAir() : this("http://flashair/")
        {
        }
        public FlashAir(String urlBase)
        {
            this.BaseUrl = urlBase;
        }
    }
}
