/*
 * Test console application for
 *      FlashAir .NET client library 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlashAirClient;

namespace TestConsole
{
    class Program
    {
        const String FlashAirUrl = "http://flashair/";
        const String MasterCode = "2c56dc7699bb";
        int depth = 0;
        FlashAir fa;
        void Run()
        {
            fa = new FlashAir(FlashAirUrl);
            foreach (var pi in typeof(Command).GetProperties())
            {

                Console.WriteLine(String.Format("{0}:{2}{1}", pi.Name, pi.GetValue(fa.Command, null), String.Join("", Enumerable.Repeat(" ", (24 - pi.Name.Length)))));
            }

            ListDirectory("/");
            //
            if (Wait("タイムアウトを設定(Y/n)")) { 
                //
                fa.Config.DeviceTimeout = TimeSpan.FromMinutes(3);
                if (fa.Config.Submit(MasterCode))
                {
                    Console.WriteLine("config.cgi 更新成功");
                }
                else
                {
                    Console.WriteLine("config.cgi 更新失敗");
                }
            }

            var dt = DateTime.Now;
            Console.Write($"upload.cgi FTIME 設定({dt})..");
            {
                if (fa.Upload.SetSystemTime(dt))
                {
                    Console.WriteLine("...成功");
                }
            }
            Console.Write("setting WRITEPROTECT on...");
            if( fa.Upload.SetWriteProtect())
            {
                Console.WriteLine("...成功");
            }
            Console.Write("setting UPDIR=/...");
            if( fa.Upload.SetUploadDirectory("/"))
            {
                Console.WriteLine("...成功");
            }
            if (Wait("/3db5a8f5.jpg を削除(Y/n)"))
            {
                if (fa.Upload.DeleteFile("/3db5a8f5.jpg"))
                {
                    Console.WriteLine("...成功");
                }
                else
                {
                    Console.WriteLine("...失敗");
                }
            }
            if (Wait("/flashairLogo_official_small.png をアップロード(Y/n)"))
            {
                var img = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"flashairLogo_official_small.png");
                using (var fs = System.IO.File.OpenRead(img))
                {
                    if (fa.Upload.UploadFile(fs, "flashairLogo_official_small.png"))
                    {
                        Console.WriteLine("...成功");
                    }
                    else
                    {
                        Console.WriteLine("...失敗");
                    }
                }
            }
            Wait();
        }
        bool Wait(String prompt = "キーを押してください...")
        {
            Console.WriteLine(prompt);
            var keyInfo = Console.ReadKey(true);
            if( keyInfo.Key == ConsoleKey.Escape)
                throw new OperationCanceledException();
            return keyInfo.Key != ConsoleKey.N;
        }
        void ListDirectory( String dir = "/")
        {
            Console.WriteLine(String.Format("Directory {0} contains {1} files.", dir, fa.Command.FileCount(dir)));
            foreach (var file in fa.Command.FileList(dir))
            {
                Console.Write( String.Join( "", Enumerable.Repeat(" ", depth)));
                Console.WriteLine(
                    String.Format("{0} {1} {2} {3} {4}", file.Directory, file.Name, file.Time, file.Size, file.Attributes));
                if ( (file.Attributes & FileAttribute.Directory) == FileAttribute.Directory)
                {
                    depth++;
                    ListDirectory(System.IO.Path.Combine(file.Directory, file.Name));
                    depth--;
                }
                else
                {
                    ExifInfo ex;
                    var st = fa.Thumbnail.Get(System.IO.Path.Combine(file.Directory, file.Name), out ex);
                    if (ex != null)
                    {
                        Console.WriteLine(String.Format("{0} x {1} {2}", ex.Width, ex.Height, ex.Orientation));
                        Console.WriteLine(fa.GetFileUrl(file));
                        try
                        {
                            if (ex.Width > 0 && ex.Height > 0 && ex.Orientation != 0)
                            {
                                using (var fileStream = fa.GetFile(file))
                                {
                                    using (var reader = new System.IO.StreamReader(fileStream))
                                    {
                                        reader.ReadToEnd();
                                    }
                                }
                            }
                        }
                        catch ( Exception ex1)
                        {
                            Console.WriteLine("Exception: " + ex1.Message);
                        }
                    }
                    if (st != null)
                    {
                        st.Dispose();
                    }
                }
            }

        }
        static void Main(string[] args)
        {
            try
            {
                new Program().Run();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation Cancelled.");
            }
        }
    }
}
