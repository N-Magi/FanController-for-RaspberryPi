using System;
using System.Collections.Generic;
using System.Text;

namespace FanController.Data
{
    class Config
    {
        public int FanRollPin { get; set; } = 25;
        public int PwmPin { get; set; } = 1;
        public List<int> SpeedConfig_Temp { get; set; }
        public List<int> SpeedConfig_Usage { get; set; }
    }
}
