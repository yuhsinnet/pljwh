using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Text.Json;

namespace PLCGateway
{
    public partial class Form1 : Form
    {
        HubConnection connection;
        int i;
        public Form1()
        {
            InitializeComponent();
            connection = new HubConnectionBuilder()
            .WithUrl("http://whw.cjee.tw/ChatHub")
            .Build();

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
                        AreaName = "A區",

                     Sensors =new List<Sensor>
                     {
                       new Sensor { Name = "濕度", Value = RandGen(60,90), unit = "%" },
                       new Sensor { Name = "溫度", Value = RandGen(25,32), unit = "℃" }
                     }
                    },
                    new Area
                    {
                        AreaName = "B區",
                        Sensors = new List<Sensor>
                        {
                            new Sensor { Name = "光度", Value =  RandGen(0,100000), unit = "Lux" },
                            new Sensor { Name = "葉面濕度度", Value = RandGen(80,90), unit = "%" },
                            new Sensor { Name = "葉面濕度度2", Value = RandGen(80,90), unit = "%" }
                        }
                    }


                }
            };
            string WHJson = JsonSerializer.Serialize(plj, options);

            #endregion

            #region snippet_ErrorHandling
            try
            {
                i++;
                #region snippet_InvokeAsync
                connection.InvokeAsync("PushJson", WHJson);


                textBox1.InvokeIfRequired(() => { textBox1.Text = i.ToString() ; });

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
    #region CSS
    class Sensor
    {
        public Sensor()
        {

        }
        public Sensor(string Name, double Value, string unit)
        {
            this.Name = Name;
            this.Value = Value;
            this.unit = unit;
        }

        public string Name { get; set; }
        public double Value { get; set; }
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
