/*
 * The MIT License (MIT)
 * Copyright (c) 2015 Yokinsoft http://www.yo-ki.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FlashAirClient
{
    public class Thumbnail : CgiHost
    {
        internal Thumbnail(String baseUrl) : base( baseUrl + "thumbnail.cgi")
        {
        }
        public String GetUrl(FileInfo file)
        {
            return GetUrl(file.Directory + "/" + file.Name);
        }
        public String GetUrl(String path)
        {
            return BaseUrl + "?" + path;
        }
        public Stream Get( FileInfo file )
        {
            return Get(file.Directory + "/" + file.Name);
        }
        public Stream Get( String path )
        {
            return Get<Stream>(BaseUrl + "?" + path);
        }
        public Stream Get(String path, out ExifInfo exif)
        {
            var response = GetResponse(BaseUrl + "?" + path);
            if (!response.IsSuccessStatusCode)
            {
                exif = null;
                return null;
            }
            var xWidth = response.Headers.GetValues("X-exif-WIDTH").FirstOrDefault();
            var xHeight = response.Headers.GetValues("X-exif-HEIGHT").FirstOrDefault();
            var xOrientation = response.Headers.GetValues("X-exif-ORIENTATION").FirstOrDefault();
            if( !String.IsNullOrEmpty(xWidth) &&!String.IsNullOrEmpty(xHeight) &&!String.IsNullOrEmpty(xOrientation) )
            {
                exif = new ExifInfo { Height = int.Parse(xHeight), Width = int.Parse(xWidth), Orientation = (ExifOientation)int.Parse(xOrientation) };
            }
            else
            {
                exif = null;
            }
            return response.Content.ReadAsStreamAsync().Result;
        }
    }
}
