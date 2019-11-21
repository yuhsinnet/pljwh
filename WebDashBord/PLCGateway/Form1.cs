using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Text.Json;

using TOPLC;

namespace PLCGateway
{
    public partial class Form1 : Form
    {
        HubConnection connection;
        Toplc PLC;
        int i;
        public Form1()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
            .WithUrl("https://whw.cjee.tw/ChatHub")         
            .Build();


            PLC = new Toplc("192.168.0.108", 1501);


            #region snippet_ClosedRestart
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                connection.StartAsync();
                textBox1.AppendText("Connection started\r\n");
                //connectButton.IsEnabled = false;
                //sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                textBox1.AppendText(ex.Message);
            }


            System.Threading.Timer ttmm = new System.Threading.Timer(new TimerCallback(AutoSend));
            ttmm.Change(1000, 1000);
        }

        double RandGen(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }
        /// <summary>
        /// 十六進制轉十進制轉換
        /// </summary>
        /// <param name="hex">十六進制字串</param>
        /// <param name="dot">相乘倍率</param>
        /// <returns>數字字串</returns>
        string HEX2DEC(string hex, double dot)
        {

            string Dec = (dot * Convert.ToInt32(hex, 16)).ToString();
            //string DecDot = Dec.Insert(Dec.Length - dot, ".");
         //double Rounded =  Math.Round(Dec, 1);
            return Dec;


        }
        private void AutoSend(object state)
        {
            #region jsonGen

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,

                

                // 使Json 可以顯示中文
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            WH plj = new WH
            {

                Areas = new List<Area>
                {
                    new Area
                    {
                        AreaName = "A1區",

                     Sensors =new List<Sensor>
                     {
                       new Sensor { Name = "空氣濕度", Value = HEX2DEC(PLC.R[101],0.1), unit = "%"},
                       new Sensor { Name = "空氣溫度", Value = HEX2DEC(PLC.R[100],0.1), unit = "℃"},
                       new Sensor { Name = "葉面濕度", Value = HEX2DEC(PLC.R[102],0.1), unit = "%"}
                     }
                    },
                    new Area
                    {
                        AreaName = "A2區",
                        Sensors = new List<Sensor>
                        {

                          new Sensor { Name = "空氣濕度", Value = HEX2DEC(PLC.R[107],0.1), unit = "%"},
                          new Sensor { Name = "空氣溫度", Value = HEX2DEC(PLC.R[106],0.1), unit = "℃"},
                          new Sensor { Name = "葉面濕度", Value = HEX2DEC(PLC.R[104],0.1), unit = "%"}
                        }
                    },
                    new Area
                    {
                        AreaName = "B區",
                        Sensors = new List<Sensor>
                        {
                          new Sensor { Name = "空氣濕度", Value = HEX2DEC(PLC.R[108],0.1), unit = "%"},
                          new Sensor { Name = "空氣溫度", Value = HEX2DEC(PLC.R[109],0.1), unit = "℃"},
                          new Sensor { Name = "葉面濕度", Value = HEX2DEC(PLC.R[117],0.1), unit = "%"}
                        }
                    },
                    new Area
                    {
                        AreaName = "C區",
                        Sensors = new List<Sensor>
                        {
                          new Sensor { Name = "空氣濕度", Value = HEX2DEC(PLC.R[120],0.1), unit = "%"},
                          new Sensor { Name = "空氣溫度", Value = HEX2DEC(PLC.R[119],0.1), unit = "℃"},
                          new Sensor { Name = "葉面濕度", Value = HEX2DEC(PLC.R[123],0.1), unit = "%"}
                        }
                    }
                }
            };


            WHV2 hV2 = new WHV2
            {
                sensorV2s = new List<SensorV2>
                {
                    new SensorV2{AreaName = "A1區",Name = "空氣濕度", Value = HEX2DEC(PLC.R[101],0.1), unit = "%"},
                    new SensorV2{AreaName = "A1區", Name = "空氣溫度", Value = HEX2DEC(PLC.R[100],0.1), unit = "℃"},
                    new SensorV2{AreaName = "A1區", Name = "葉面濕度", Value = HEX2DEC(PLC.R[102],0.1), unit = "%"},
                    new SensorV2{AreaName = "A2區", Name = "空氣濕度", Value = HEX2DEC(PLC.R[107],0.1), unit = "%"},
                    new SensorV2{AreaName = "A2區", Name = "空氣溫度", Value = HEX2DEC(PLC.R[106],0.1), unit = "℃"},
                    new SensorV2{AreaName = "A2區", Name = "葉面濕度", Value = HEX2DEC(PLC.R[104],0.1), unit = "%"},
                    new SensorV2{AreaName = "B區", Name = "空氣濕度", Value = HEX2DEC(PLC.R[108],0.1), unit = "%"},
                    new SensorV2{AreaName = "B區", Name = "空氣溫度", Value = HEX2DEC(PLC.R[109],0.1), unit = "℃"},
                    new SensorV2{AreaName = "B區", Name = "葉面濕度", Value = HEX2DEC(PLC.R[117],0.1), unit = "%"},
                    new SensorV2{AreaName = "C區", Name = "空氣濕度", Value = HEX2DEC(PLC.R[120],0.1), unit = "%"},
                    new SensorV2{AreaName = "C區", Name = "空氣溫度", Value = HEX2DEC(PLC.R[119],0.1), unit = "℃"},
                    new SensorV2{AreaName = "C區", Name = "葉面濕度", Value = HEX2DEC(PLC.R[123],0.1), unit = "%"},
                    new SensorV2{AreaName = "D區", Name = "空氣濕度", Value = HEX2DEC(PLC.R[114],0.1), unit = "%"},
                    new SensorV2{AreaName = "D區", Name = "空氣溫度", Value = HEX2DEC(PLC.R[113],0.1), unit = "℃"},
                    new SensorV2{AreaName = "D區", Name = "葉面濕度", Value = HEX2DEC(PLC.R[115],0.1), unit = "%"},
                    new SensorV2{AreaName = "E區", Name = "空氣濕度", Value = HEX2DEC(PLC.R[122],0.1), unit = "%"},
                    new SensorV2{AreaName = "E區", Name = "空氣溫度", Value = HEX2DEC(PLC.R[121],0.1), unit = "℃"},
                    new SensorV2{AreaName = "E區", Name = "葉面濕度", Value = HEX2DEC(PLC.R[125],0.1), unit = "%"},
            }
            };


            string WHJson = JsonSerializer.Serialize(hV2.sensorV2s, options);

            #endregion

            #region snippet_ErrorHandling
            try
            {
                i++;
                #region snippet_InvokeAsync
                connection.InvokeAsync("PushJson", WHJson);


                textBox1.InvokeIfRequired(() => { textBox1.Text = i.ToString() + Environment.NewLine + WHJson; });

                //textBox1.AppendText(i.ToString() +"\r\n");
                #endregion
            }
            catch (Exception ex)
            {
                textBox1.InvokeIfRequired(() => { textBox1.AppendText(ex + "\r\n"); });
            }
            #endregion
        }
    }

    class SensorV2
    {
        public string AreaName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string unit { get; set; }
    }

    class WHV2
    {
       public List<SensorV2> sensorV2s { get; set; }
    }




    #region CSS
    class Sensor
    {
        public Sensor()
        {

        }
        public Sensor(string Name, string Value, string unit)
        {
            this.Name = Name;
            this.Value = Value;
            this.unit = unit;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string unit { get; set; }

    }
    class Area
    {
        public string AreaName { get; set; }
        public List<Sensor> Sensors { get; set; }

    }

    class WH
    {
        public List<Area> Areas { get; set; }
    }
    #endregion

    //擴充方法
    public static class Extension
    {
        //非同步委派更新UI
        public static void InvokeIfRequired(
            this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)//在非當前執行緒內 使用委派
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
