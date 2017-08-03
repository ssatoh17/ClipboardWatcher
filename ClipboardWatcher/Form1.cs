using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipboardWatcher
{
    public partial class Form1 : Form
    {
        
        ClipBoardWatcher cbw;
        DateTime preCalledTime = DateTime.MinValue;
        int DrawClipBoardイベントを無視した回数 = 0;

        string newText = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbw = new ClipBoardWatcher();


            //クリップボードの中身が変わった時に発生するイベント？？
            cbw.DrawClipBoard += (sender2, e2) =>
            {
                //🔴🔴 何故か連続でCallされることがあるので、1.5秒以内のcallなら無視する（１秒以内だと、同じ値で複数イベントが発生してしまうことがあったため、1.5と少し長めにした）
                if ((DateTime.Now - preCalledTime) < TimeSpan.FromSeconds(1.0)) // 1.5だと、png、jpegファイルへの直リンクURLが表示されないことがあるので、やはり１秒に戻した
                {
                    DrawClipBoardイベントを無視した回数++;
                    Console.WriteLine(DrawClipBoardイベントを無視した回数.ToString() + " 回目の処理飛ばし");
                    preCalledTime = DateTime.Now;
                    return;
                }
                else
                {
                    preCalledTime = DateTime.Now;
                    DrawClipBoardイベントを無視した回数 = 0; //初期化
                }



                if (Clipboard.ContainsText()) // 例外：System.Runtime.InteropServices.ExternalException が発生しました。要求されたクリップボード操作に成功しませんでした。のエラーが・・・  
                {
                    string clipboardText = Clipboard.GetText();
                    //this.listBox1.Items.Add(clipboardText);
                    //this.listBox1.Items.Add(DateTime.Now.ToLongTimeString() + ";" + clipboardText);
                    this.listBox1.Items.Add(DateTime.Now.ToString("HH:mm:ss.fff") + ": " + clipboardText);

                    var timer = new System.Diagnostics.Stopwatch(); // 時間計測用のタイマー
                    timer.Start();
                    string url = clipboardText;

                    //SnagitかLightShotでキャプチャした場合は、変数xPathに対象の要素(画像の直リンクが埋め込まれているimg要素)のxPathをSet
                    string xPath = "";
                    if (clipboardText.Contains(@"https://www.screencast.com/t/")){
                        xPath = "//*[@id=\"view-page-container\"]/div/div[3]/div[2]/div[1]/div[1]/div/a/img";
                    }
                    if (clipboardText.Contains(@"http://prntscr.com/")){ //LightShotの場合
                        //エラーになってしまう！！
                        xPath = "//*[@id=\"screenshot-image\"]";
                    }

                    int counter = 0;
                    //スナグイットもしくはLightShotのURLなら、その先のhtmlのコンテンツをスクレイピングする
                    if (xPath != "") 
                    {
                        retry:

                        url = url.Replace("http://", "https://"); //何故か LightShotでキャプチャした場合、httpのままであるため
                        string htmlText = "";
                        htmlText = Webサイトの内容をhtml文字列として取得(url);  // Webページの内容を文字列として取得

                        Console.WriteLine("HTML文字列取得直前: {0:0.000}秒", timer.Elapsed.TotalSeconds);
                        //htmlText = (new HttpClient()).GetStringAsync(url).Result; // HttpClinetを使う場合はこの１行でいけるかも 
                        //何故か、クリップボードには、http:// としてコピーされていた！！
                        //https://prnt.sc/g3ujr3
                        //htmlText = (new HttpClient()).GetStringAsync(url.Replace("http://","https://")).Result; // HttpClinetを使う場合はこの１行でいけるかも
                        //htmlText = (new HttpClient()).GetStringAsync(url).Result; // HttpClinetを使う場合はこの１行でいけるかも

                        //XPath が以下である要素の src属性値を取得
                        //  //*[@id="view-page-container"]/div/div[3]/div[2]/div[1]/div[1]/div/a/img
                        if (htmlText != ""){
                            Console.WriteLine("HTML取得完了: {0:0.000}秒", timer.Elapsed.TotalSeconds);
                        }
                        else{
                            Console.WriteLine("🔴🔴HTML取得できていない！！: {0:0.000}秒", timer.Elapsed.TotalSeconds);
                        }
           retryWhenHttpClient:
                        if (htmlText != null)
                        {
                            if(htmlText == "")
                            {
                                counter++;
                                Console.WriteLine("リトライ " + counter.ToString() + " 回目");
                                if(counter <= 2)
                                {
                                    Thread.Sleep(1000);
                                    goto retry;
                                    //goto retryWhenHttpClient;
                                }
                                else
                                {
                                    return;
                                }

                            }
                            // HtmlDocumentオブジェクトを構築
                            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                            htmlDoc.LoadHtml(htmlText);
                            Console.WriteLine("HtmlDocument構築完了: {0:0.000}秒", timer.Elapsed.TotalSeconds);

                            //目的の <img>要素を(全て)取り出し
                            // そのsrc属性を持つ匿名型オブジェクトのコレクションを作る（LINQ）
                            var imgs = htmlDoc.DocumentNode
                                        .SelectNodes(xPath)
                                        .Select(img => new {
                                            src = img.Attributes["src"].Value.Trim()
                                        });
                        
                            Console.WriteLine("指定したimg要素の取り出し完了: {0:0.000}秒", timer.Elapsed.TotalSeconds);
                            // 表示する
                            Console.WriteLine("src先頭1件（全{0}記事中）", imgs.Count());
                            foreach (var img in imgs.Take(1))
                            {
                                //Console.WriteLine(img.src);
                                //Clipboard.SetText(img.src); //これによって、また変更イベントが発生してしまう → 別スレッドでコピーした方がいいかも
                                newText = img.src;

                                // 例外（ウィンドウのハンドルを作成中にエラーが発生しました）が発生してしまうので、ここでは timer1オブジェクトのEnabledプロパティにアクセスすることはやめた
                                //timer1.Enabled = true;  //例外：System.ComponentModel.Win32Exception が発生しました。ウィンドウのハンドルを作成中にエラーが発生しました。

                                //listBox1.Items.Add("🌟：" + img.src); //これがあると、競合してしまうかもしれない
                                //return; // 念のため
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            };
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cbw.Dispose(); //クリップボード監視オブジェクトの破棄
        }

        //http://www.atmarkit.co.jp/ait/articles/1501/27/news140.html
        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("HttpClientクラスで取得したWebページを解析する（Html Agility Pack）");
            // 時間計測用のタイマー
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            // .NET TIPSの日付順インデックスのURL（シフトJISのページ）
            string url = @"http://www.atmarkit.co.jp/ait/subtop/features/dotnet/index_date.html";
            Uri webUri = new Uri(url);

            string htmlText = Webサイトの内容をhtml文字列として取得(url);  // Webページの内容を文字列として取得
            Console.WriteLine("HTML取得完了: {0:0.000}秒", timer.Elapsed.TotalSeconds);

            if (htmlText != null)
            {
                // HtmlDocumentオブジェクトを構築する
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlText);
                Console.WriteLine("HtmlDocument構築完了: {0:0.000}秒", timer.Elapsed.TotalSeconds);

                // 目的の<a>要素を全て取り出して（XPath）、
                // そのhref属性とInnerTextを持つ匿名型オブジェクトのコレクションを作る（LINQ）
                // ※冒頭に「using System.Linq;」の追加が必要
                var articles
                  = htmlDoc.DocumentNode
                    .SelectNodes(@"//div[@class=""da-tips-index-target""]/div[not(@class)]/a")
                    .Select(a => new
                    {
                        Url = a.Attributes["href"].Value.Trim(),
                        Title = a.InnerText.Trim(),
                    });
                Console.WriteLine("タイトル取り出し完了: {0:0.000}秒", timer.Elapsed.TotalSeconds);
                Console.WriteLine();

                // 先頭10件を表示する
                Console.WriteLine("記事タイトル先頭10件（全{0}記事中）", articles.Count());
                foreach (var a in articles.Take(10))
                {
                    Console.WriteLine(a.Title);
                    Console.WriteLine(" - {0}", a.Url);
                }
            }
#if DEBUG
            Console.ReadKey();
#endif
        }

        // http://www.katch.ne.jp/~h-inoue/tips/cs/0001.html からパクリ
        private string Webサイトの内容をhtml文字列として取得(string url)
        {
            string htmlStr = "";
            try
            {
                // WebClientを作成
                WebClient wc = new WebClient();

                // WebClientからStreamとStreamReaderを作成
                // args[0]にはURLが入っているものとする
                //wc.Headers.Add
                Stream st = wc.OpenRead(url);           //  LightShotで取得した場合、ここで例外が発生してしまう！（403エラー）（←サーバ側で弾いている可能性がある）
                StreamReader sr = new StreamReader(st);

                //リソースからすべて読み取る
                htmlStr = sr.ReadToEnd();

                // StreamとStreamReaderを閉じる
                sr.Close();
                st.Close();

            }
            catch (Exception e)
            {
                // URLのファイルが見つからない等のエラーが発生
                Console.WriteLine("エラーが発生しました\r\n\r\n" + e.ToString());

            }
            return htmlStr;
        }


        //グローバル変数 newText を監視するタイマー
        private void timer1_Tick(object sender, EventArgs e)
        {
            //timer1.Enabled = false;
            if (newText != "")
            {
                Clipboard.SetText(newText); //これによって、また変更イベントが発生してしまう → 別スレッドでコピーした方がいいかも
                pictureBox1.ImageLocation = newText; //URLの画像を表示 🔴もしかしたらメインスレッドと競合するかもしれない
                newText = ""; //初期化
            }

        }

        //GUIをクリックした時の挙動
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //画像がSetされていたら、それをクリップボードにコピーする
            Clipboard.SetImage(pictureBox1.Image);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //クリックしたアイテムをクリップボードにコピー
            string pasteStr = listBox1.SelectedItem.ToString();
            pasteStr = pasteStr.Substring(14, pasteStr.Length - 14);
            Clipboard.SetText(pasteStr);   
        }
    }

}
