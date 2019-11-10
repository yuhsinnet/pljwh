using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            .WithUrl("https://localhost:44307/ChatHub")
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

        private void AutoSend(object state)
        {
            #region snippet_ErrorHandling
            try
            {
                i++;
                #region snippet_InvokeAsync
                connection.InvokeAsync("PushJson", i.ToString());


                textBox1.InvokeIfRequired(() => { textBox1.AppendText(i.ToString() + "\r\n"); });

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
