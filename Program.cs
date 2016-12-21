using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConstantDelete
{
    class Program
    {
        static StreamWriter sw;

        static void Main(string[] args)
        {
            System.Console.WriteLine("-----------------------------------------------");
            System.Console.WriteLine("ConstantDelete バージョン:1.0.0.0");
            System.Console.WriteLine("This software is licensed under the MIT license.");
            System.Console.WriteLine("Copyright (C) 2016 kousokujin.");
            System.Console.WriteLine("-----------------------------------------------");


            string confFile = "config.xml";

            TimerData td;

            //LOGファイル
            sw = new System.IO.StreamWriter(@"LOG.txt", true,
                    System.Text.Encoding.GetEncoding("Shift_Jis"));
            writeLOG("ログファイルを開きました。");
            //sw.Close();

            //コンフィグファイル読み込み
            if (File.Exists(confFile))
            {
                saveTime st = loadFile(confFile);
                td = new TimerData(st.filePosition, st.interval, st.priventTime);
                writeLOG("コンフィグファイルを読み込みました。");
                writeLOG(string.Format("削除対象は「{0}」です。",td.getfilePosition()));
            }else
            {
                td = new TimerData("ここにファイルの場所を入力してください。");
                saveFile(td,confFile);
                writeLOG("コンフィグファイルを生成しました。");
            }

            long distTime = getNowUNIXTime() - td.getPriventTime();

            writeLOG("ファイルの削除判定を実行します。");
            writeLOG(string.Format("前回の削除:{0}",getLocalTime(td.getPriventTime())));
            if(distTime > (long)td.getInterval())
            {
                //ファイル削除
                writeLOG(string.Format("以下のファイルまたはフォルダを削除します。\r\n{0}", td.getfilePosition()));
                if(File.Exists(td.getfilePosition())){
                    System.IO.File.Delete(td.getfilePosition());
                    writeLOG("ファイルを削除しました。");
                    td.setPriventTime();
                }
                else
                {
                    if (Directory.Exists(td.getfilePosition())) //ディレクトリ削除
                    {
                        string[] folderContent = Directory.GetFiles(td.getfilePosition());
                        foreach(string Content in folderContent)
                        {
                            File.SetAttributes(Content, FileAttributes.Normal);
                            File.Delete(Content);
                        }
                        writeLOG("フォルダの中身を削除しました。");

                        System.IO.Directory.Delete(td.getfilePosition());
                        writeLOG("フォルダを削除しました。");
                        td.setPriventTime();
                    }
                    else
                    {
                        writeLOG("ファイルまたはフォルダが存在しません。");
                    }
                }

            }
            else
            {
                writeLOG(string.Format("前回の削除から{0}秒経過してないのでファイルを削除しません。",td.getInterval()));
                
            }

            //コンフィグファイル更新
            saveFile(td,confFile);
            writeLOG("コンフィグファイルを更新しました。");
            writeLOG("アプリケーションを終了します。");

            sw.Close();
            System.Console.WriteLine();


        }

        static long getNowUNIXTime()
        {
            DateTime nowTime = DateTime.Now;
            return GetUnixTime(nowTime);
        }

        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        static long GetUnixTime(DateTime targetTime)
        {
            // UTC時間に変換
            targetTime = targetTime.ToUniversalTime();

            // UNIXエポックからの経過時間を取得
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;

            // 経過秒数に変換
            return (long)elapsedTime.TotalSeconds;
        }

        static string getLocalTime(long unixTime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dt = dt.AddSeconds(unixTime);
            dt = TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);

            return dt.ToString();
            
        }

        static void saveFile(TimerData td,string confFile) {
            //コンフィグファイル更新
            saveTime save = new saveTime();
            save.set(td);

            System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(saveTime));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
            confFile, false, new System.Text.UTF8Encoding(false));
            serializer.Serialize(sw, save);
            sw.Close();
        }

        static saveTime loadFile(string path)
        {
            //XmlSerializerオブジェクトを作成
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(saveTime));
            //読み込むファイルを開く
            System.IO.StreamReader sr = new System.IO.StreamReader(
                path, new System.Text.UTF8Encoding(false));
            //XMLファイルから読み込み、逆シリアル化する
            saveTime obj = (saveTime)serializer.Deserialize(sr);
            //ファイルを閉じる
            sr.Close();

            return obj;
        }

        static void writeLOG(string text)
        {
            System.Console.WriteLine("[{0}]{1}", DateTime.Now, text);
            sw.WriteLine("[{0}]{1}", DateTime.Now, text);
        }
    }
}
