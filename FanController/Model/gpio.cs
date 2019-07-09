using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FanController.Model
{
    static class GPIO
    {
        /// <summary>
        /// ピンの出力設定をします。
        /// </summary>
        /// <param name="BCMpin">GPIOのピン番号</param>
        /// <param name="mode">入力か出力か</param>
        /// <param name="enable">出力を行うかどうか</param>
        static public void PinMode(int BCMpin, pin_mode mode, bool enable = true)
        {
            string gpioPin_d  = $@"/sys/class/gpio/gpio{BCMpin}";
            CheckPinAvailable(BCMpin, pin_type.GPIO, true);
            //ピンのステータスを設定する。
            SetState($@"{gpioPin_d}/direction", Enum.GetName(typeof(pin_mode), mode).ToLower());
            //ピンの出力を設定する。
            if (enable == false) return;
            SetAttributes(BCMpin, 1, pin_type.GPIO);
            return;
        }
        /// <summary>
        /// ピンの出力設定をします。
        /// "/boot/config"内に"dtoverlay=pwm-2chan"を行ってリブートしてください。起動しません。
        /// </summary>
        /// <param name="BCMpin">PWMの番号[pwm0/pwm1]</param>
        /// <param name="pwm">pwmの詳細設定</param>
        /// <param name="enable">出力を行うかどうか</param>
        static public void PinMode(int BCMpin, PWM pwm, bool enable = true)
        {
            //string pwm_d = $@"/sys/class/pwm/pwm{BCMpin}";
            CheckPinAvailable(BCMpin, pin_type.PWM, true);
            SetAttributes(BCMpin, pwm.Period, pin_type.PWM, $@"period");
            SetAttributes(BCMpin, pwm.Duty_Cycle, pin_type.PWM, $@"duty_cycle");
            if (enable == false) return;
            SetAttributes(BCMpin, 1, pin_type.PWM, $@"enable");
            return;
        }

        /// <summary>
        /// ピンの出力をするかを設定します。
        /// **先に出力設定されている必要があります。**
        /// </summary>
        /// <param name="BCMpin">ピン番号[GPIO/PWM0,PWM1]</param>
        /// <param name="type">GPIO/PWM</param>
        /// <param name="state">出力の設定</param>
        static public void SetPinStatus(int BCMpin, pin_type type, pin_state state)
        {
            SetAttributes(BCMpin, (int)state, type, "enable");
        }

        /// <summary>
        /// ピンのvalue値を設定する
        /// **先に出力が設定されている必要があります**
        /// </summary>
        /// <param name="BCMpin">ピン番号</param>
        /// <param name="value">書き込む値</param>
        static private void SetAttributes(int BCMpin, int value, pin_type type, string file = "value")
        {

            string pin_d = $@"/sys/class/";
            //書き込むのがPWMかGPIOか判定を行う
            pin_d = type == pin_type.GPIO ? $@"{pin_d}/gpio/gpio{BCMpin}" : $@"{pin_d}/pwm/pwmchip0/pwm{BCMpin}";
            //string gpioPin_d = $@"/sys/class/gpio/gpio{BCMpin}";
            SetState($@"{pin_d}/{file}", value.ToString());

        }

        /// <summary>
        /// ピンが有効化されているか否かを確認
        /// </summary>
        /// <param name="BCMpin">ピン番号[GPIO/PWM]</param>
        /// <param name="type">GPIO/PWM</param>
        /// <param name="create">自動的に有効化</param>
        /// <returns></returns>
        static private bool CheckPinAvailable(int BCMpin, pin_type type, bool create = false)
        {
            if (type == pin_type.GPIO)
            {
                string gpio_d = $@"/sys/class/gpio/gpio{BCMpin}";
                bool isActivated = false;
                isActivated = Directory.Exists(gpio_d);

                //GPIOが有効かされていなかったら有効化
                if (isActivated == false)
                {
                    if (create == false) return isActivated;
                    ActivatePin(BCMpin, type);
                    isActivated = true;
                }
                return isActivated;
            }

            //PWMのピンが有効かされているかを確認する。
            string pwm_d = $@"/sys/class/pwm/pwmchip0/pwm{BCMpin}";
            bool isAvctivated = false;
            isAvctivated = Directory.Exists(pwm_d);
            //PWMが有効化されていなかったら有効化
            if (isAvctivated == false)
            {
                if (create == false) return isAvctivated;
                ActivatePin(BCMpin, type);
                isAvctivated = true;
            }
            return isAvctivated;
        }

        /// <summary>
        /// ピンを有効化する
        /// </summary>
        /// <param name="BCMpin">ピン番号/PWM</param>
        /// <param name="type">PWM/GPIO</param>
        static public void ActivatePin(int BCMpin, pin_type type)
        {

            if (type == pin_type.GPIO)
            {
                string gpio_d = $@"/sys/class/gpio";
                SetState($@"{gpio_d}/export", BCMpin.ToString());
                return;
            }
            string pwm_d = $@"/sys/class/pwm";
            SetState($@"{pwm_d}/pwmchip0/export", BCMpin.ToString());
            return;
        }

        static public void InActivatePin(int BCMpin, pin_type type)
        {
            if (type == pin_type.GPIO)
            {
                string gpio_d = $@"/sys/class/gpio";
                SetState($@"{gpio_d}/unexport", BCMpin.ToString());
                return;
            }
            string pwm_d = $@"/sys/class/pwm";
            SetState($@"{pwm_d}/pwmchip0/unexport", BCMpin.ToString());
            return;
        }

        /// <summary>
        /// ハードウェアGPIOに対して書き込み
        /// </summary>
        /// <param name="path">書きこむパス</param>
        /// <param name="msg">書き込む値</param>
        static private void SetState(string path, string msg)
        {

            File.WriteAllText(path, msg);

        }

    }

    public class PWM
    {
        /// <summary>
        /// PWMの設定
        /// </summary>
        /// <param name="frequency">周波数</param>
        /// <param name="duty">Duty比</param>
        public PWM(int frequency, int duty)
        {
            Frequency = frequency;
            Duty = duty;
        }
        private int frequency;
        /// <summary>
        ///周波数を設定
        /// </summary>
        public int Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
                period = (1000000000 / value);
                //Console.WriteLine(period);
                //duty_cycle = (Period / 100) * duty;
            }
        }

        //raspberryPiのperiodはnSオーダー
        private int period;
        /// <summary>
        /// 一周期の時間[nS]
        /// </summary>
        public int Period
        {
            get
            {

                return period;
            }

        }
        private int duty_cycle;
        /// <summary>
        /// Duty比の実時間　
        /// </summary>
        public int Duty_Cycle
        {
            get
            {

                return duty_cycle;
            }
        }

        private int duty;
        /// <summary>
        /// Duty比　frequencyを先に設定してください.
        /// </summary>
        public int Duty
        {
            get
            {
                return duty;
            }
            set
            {
                duty = value;
                //Duty比での入力をスイッチング時間に置き換える
                duty_cycle = (Period / 100) * value;
                //Console.WriteLine(duty_cycle);
            }
        }
    }
    public enum pin_mode
    {
        IN, OUT
    }
    public enum pin_type
    {
        PWM, GPIO
    }
    public enum pin_state
    {
        Enable = 1, Disable = 0
    }
}
