using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FanController.Model
{
    public class Logger
    {
        private string path = "";
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
            }

        }
        public string DirPath { get; set; }

        public Logger(string dirPah, string fileName)
        {
            Path = $@"{dirPah}/{fileName}";
            DirPath = dirPah;

            //if (!Directory.Exists(DirPath)) Directory.CreateDirectory(DirPath);
            //var dir = new DirectoryInfo(dirPah);
            //dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
            //Console.WriteLine(dirPah);
            //Console.WriteLine(path);
            //if (!File.Exists(Path)) File.Create(path);
            //Console.WriteLine(path);
        }

        public void Print(string msg)
        {
            if (Path == null) return;

			Console.WriteLine($@"[{DateTime.Now.ToString("yyyy/MM/dd")} - {DateTime.Now.ToString("HH:mm:ss")}] {msg}");
            //using (StreamWriter sw = new StreamWriter(Path, true))
            //{
            //    DateTime time = DateTime.Now;
            //    string message = $@"[{time.ToString("yyyy/MM/dd")} - {time.ToString("HH:mm:ss")}] {msg}";
            //    sw.WriteLine(message);
            //    sw.Flush();
            //}
        }

        //public void PrintCSV(string data)
        //{
        //    if (Path == null) return;

        //    using (StreamWriter sw = new StreamWriter(Path, true))
        //    {
        //        DateTime time = DateTime.Now;
        //        string message = $@"{time.ToString("yyyy/MM/dd")} - {time.ToString("HH:mm:ss")}   ,{data}";
        //        sw.WriteLine(message);
        //        sw.Flush();
        //    }
        //}
    }
}
