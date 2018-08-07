using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FanController.configs
{
    static public class Configuration
    {
        static public IConfiguration GetConfiguration()
        {
            string path = $@"/etc/FanController";

            var configBuilder = new ConfigurationBuilder();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!File.Exists($@"{path}/config.json")) File.Copy($@"{Environment.CurrentDirectory}/config.json",$@"{path}/config.json",false);
            configBuilder.SetBasePath(path);
            //Console.WriteLine(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile(@"config.json");

            return configBuilder.Build();
        }
    }
}
