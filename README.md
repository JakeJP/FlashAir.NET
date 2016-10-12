FlashAir client library for .NET Portable
===================

WiFi対応のSDメモリカードである [TOSHIBA FlashAir(TM)](http://www.toshiba.co.jp/p-media/flashair/) のAPIに接続するための.NET版クラスライブラリです。

FlashAirのAPIの詳細はTOSHIBAの[開発者サイト](https://flashair-developers.com/ja/)で公開されています。ネットワークの通信（HTTP)は完全にカプセル化されています。また各APIの仕様を尊重しつつ、.NET的にデータを取扱い安いようにメソッドの名前、パラメーターなどを再構成しています。

.NET Portable ライブラリの形式でまとめてあるので、一般の.NETアプリケーションをはじめ、Windows Storeアプリ、Windows Phone、ASP.NETウェブアプリなど幅広い用途に使用できます。

ライブラリの設計ポリシーとして、メソッドやプロパティの名称は、オリジナルのAPIを尊重しつつ英語としてわかりやすいものに置き換える。時刻情報やフラグは DateTime、 Enum などに、バイナリ（ファイル）の転送には System.IO.Streamを使うなど、.NETネイティブなオブジェクト表現に置き換えしています。

###Target
- .NET Framework 4
- Silverlight 5
- Windows 8
- Windows Phone 8.1
- Windows Phone Silverlight 8

###Requirement
- .NET 4.0以降
- 依存パッケージ Microsoft.Net.Http (Microsoft.Bcl/Microsoft.Bcl.Build)

----------

簡単なサンプル
------------
http://flashair/ へ接続するとした場合の、ファイルリストを取得、ファイルをStreamとして読み込み、ディレクトリを再帰的にリストするサンプルです。
```cs
using FlashAirClient;
...
FlashAir fa;
int depth = 0;
void Run()
{
	fa = new FlashAir("http://flashair/");
	ListDirectory();
}
void ListDirectory( String dir = "/")
{
    Console.WriteLine(String.Format("Directory {0} contains {1} files.",
	     dir, fa.Command.FileCount(dir)));
    foreach (var file in fa.Command.FileList(dir))
    {
        Console.Write( String.Join( "", Enumerable.Repeat(" ", depth)));
        Console.WriteLine(
            String.Format("{0} {1} {2} {3} {4}", 
	            file.Directory, file.Name, file.Time, file.Size, file.Attributes));
        if ((file.Attributes & FileAttribute.Directory) == FileAttribute.Directory)
        {
            depth++;
            ListDirectory(
	            System.IO.Path.Combine(file.Directory, file.Name));
            depth--;
        }
        else
        {
            ExifInfo ex;
            var st = fa.Thumbnail.Get(
	            System.IO.Path.Combine(file.Directory, file.Name), out ex);
            if (ex != null)
            {
                Console.WriteLine(
	                String.Format("{0} x {1} {2}", 
		                ex.Width, ex.Height, ex.Orientation));
                using (var fileStream = fa.GetFile(file))
                {
                    using ( var reader = new System.IO.StreamReader(fileStream))
                    {
                        reader.ReadToEnd(); // do nothing
                    }
                }
            }
            if (st != null)
            {
                st.Dispose();
            }
        }
    }

}
```
クラスリファレンス
---------------------
`namespace FlashAirClient;`

###class FlashAir
FlashAirアクセスの出発点になるクラスです。

メンバ名                          | 内容
--------------------------------|----------------
FlashAir(String baseUrl)	| コンストラクタ
Command                         | command.cgi への呼び出し
Thumbnail                       | thumbnail.cgi への呼び出し
Config                          | config.cgi への呼び出し
Upload                          | upload.cgi への呼び出し
GetFile( FileInfo file )        | ファイルを System.IO.Stream としてダウンロード
GetFile( String path )          | ファイルを System.IO.Stream としてダウンロード


### class Command

型                          | メンバ名 | 内容
--------------------------------|----------------|----
IEnumerable`<FileInfo`> 		| FileList( String dir ) | dir（ディレクトリ）内のファイルの一覧
int						| FileCount( String dir) | dir(ディレクトリ）内のファイル数取得
その他 | 設定情報などを取得するメソッドまたはプロパティ

###class Thumbnail

型                          | メンバ名 | 内容
--------------------------------|----------------|----
Stream | Get( String path ) | path で指定した画像のサムネイル画像ファイルを Stream として取得
Stream | Get( FileInfo file ) | FileListで取得したFileInfoを引数にして画像のサムネイル画像ファイルを Stream として取得
String | GetUrl( path ) | path で指定した画像のサムネイル画像のURLを取得
String | GetUrl( FileInfo file ) | FileListで取得したFileInfoを引数にして画像のサムネイル画像のURLを取得
###class Config

複数の設定を同時に送信できる仕様にのっとり、Configクラスのメンバ（プロパティ）にセットした内容を Submit( String mastercode ) で送信します。

###class Upload

型                          | メンバ名 | 内容
--------------------------------|----------------|----
bool | UploadFile() | Streamとfilenameからファイルをアップロード
bool | DeleteFile() | ファイルの削除
bool | SetUploadDirectory() | アップロードディレクトリの指定
bool | SetSystemTime() | システムの時刻設定。
bool | SetWriteProtect()| 
###class FileInfo
`Command.FileList()`　でファイルを列挙した際のファイル情報を格納します。

型		| 名前	| 内容
----------------|-------|------------
String		| Directory | ディレクトリ
String		| Name	| ファイル名またはディレクトリ名
long		| Size	| サイズ
DateTime	| Time	| ファイルの日付
FileAttribute	| Attributes	| ファイルの属性情報のフラグ

改定履歴
-------------
-  v3.0.0.1 (2016.10.12)
   upload.cgi ファイルアップロード時のファイル名の扱いを修正
-  v3.0.0.0 リリース (2015.5.28)

ライセンス
-------------
MITライセンス
(c) Yokinsoft http://www.yo-ki.com
