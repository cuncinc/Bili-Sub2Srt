/**
 * @Author: chunson
 * @Created: 2022年11月1日 21点59分
 * 
 * @Indroduction: 批量将bilibili的sub字幕转换为标准srt字幕
 * @Usage: biliDirPath设置为目录文件夹，开始运行即可
 */

using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sub2Srt
{
    class Program
    {
        static string biliDirPath = @"E:\CC\Desktop\test";
        static async Task Main(string[] args)
        {
            DirectoryInfo outDir = new DirectoryInfo(biliDirPath);
            foreach (var numDir in outDir.GetDirectories())
            {
                string subFileBaseName = null;
                string mp4FilePath = numDir.FullName + @"\";

                foreach (var inNumFile in numDir.GetFiles())
                {
                    if (string.Equals(inNumFile.Extension, ".mp4"))
                    {
                        var subFile = inNumFile;
                        subFileBaseName = subFile.Name.Substring(0, subFile.Name.Length-4);
                    }
                }

                Console.WriteLine(subFileBaseName);
                Console.WriteLine(mp4FilePath);

                foreach (var subDir in numDir.GetDirectories())
                {
                    foreach (var file in subDir.GetFiles())
                    {
                        if (string.Equals(file.Extension, ".sub"))
                        {
                            string discription = string.Equals(file.Name, "en.sub") ? "_en" : "_zh";
                            string srt = ToSrt(file);
                            string path = mp4FilePath + subFileBaseName + discription + ".srt";
                            await File.WriteAllTextAsync(path, srt);
                        }
                        //file.Delete();    //如果要删除sub文件夹，则把这2个注释关闭
                    }
                    //subDir.Delete();
                }
            }
            Console.WriteLine("Done!\n");
        }

        static string ToSrt(FileInfo file)
        {
            FileStream fs = file.OpenRead();
            StreamReader sr = new StreamReader(fs);
            var json = sr.ReadToEnd();
            fs.Close();
            sr.Close();

            dynamic subtitle = JsonConvert.DeserializeObject(json);
            dynamic body = subtitle.body;
            string text = "";
            int cnt = 1;
            foreach(dynamic item in body)
            {
                string str = cnt++ + "\n" +
                             FormatTime(item.from.ToString()) + " --> " + FormatTime(item.to.ToString()) + "\n" +
                             item.content + "\n\n";
                text += str;
            }
            return text;
        }
    
        static string FormatTime(string time)
        {
            int a = (int)(double.Parse(time)*100);
            int second = a / 100;
            int mill = a % 100;
            TimeSpan timespan = new TimeSpan(second/3600, second/60%60, second%60);

            string result = string.Format("{0},{1}", timespan, mill.ToString("D3"));
            //Console.WriteLine(result);

            return result;
        }
    }
}
