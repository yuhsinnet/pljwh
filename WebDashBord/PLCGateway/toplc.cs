using System;
using System.Net.Sockets;
using System.Timers;



namespace TOPLC
{
    class Toplc
    {
        public const string PLC_Serial = "B1";
        public string[] R = new string[8071];
        public string[] D = new string[4059];
        public bool[] M = new bool[2001];


        private TcpClient Tcp_c;

        /// <summary>
        /// 輪巡計時器
        /// </summary>
        private Timer Polltimer;


        private Timer ReConnectTimer;

        public bool IsConnected;
        public int Tcp_c_TGPort { get; private set; }
        public string Tcp_c_TGIP { get; private set; }


        enum Reg { R, D, M };




        /// <summary>
        /// 初始化PLC 連線
        /// </summary>
        /// <param name="TGIP">目標IP 或DNS</param>
        /// <param name="TGPort">目標連線埠號</param>
        public Toplc(string TGIP, int TGPort)
        {
            Tcp_c = new TcpClient();
            Tcp_c_TGIP = TGIP;
            Tcp_c_TGPort = TGPort;

            Tcp_c.BeginConnect(Tcp_c_TGIP, Tcp_c_TGPort, new System.AsyncCallback(Connected), Tcp_c);


            Polltimer = new Timer(250);
            Polltimer.Elapsed += Polltimer_Elapsed;

            ReConnectTimer = new Timer(5000);
            ReConnectTimer.Elapsed += ReConnctTimer_Elapsed;
            ReConnectTimer.AutoReset = false;


        }

        private void ReConnctTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Tcp_c.Close();

            Tcp_c = new TcpClient();

            Tcp_c.BeginConnect(Tcp_c_TGIP, Tcp_c_TGPort, new System.AsyncCallback(Connected), Tcp_c);

            DeBug_P("重新連線");


        }

        private void Connected(IAsyncResult ar)
        {
            TcpClient tcpClient = (TcpClient)ar.AsyncState;


            try
            {
                if (tcpClient?.Connected ?? false)
                {
                    Polltimer.Start();
                    IsConnected = true;
                }
                else
                {
                    tcpClient.Close();
                    ConnectError();

                }
            }
            catch (Exception)
            {

                tcpClient.Close();
                ConnectError();
            }






        }

        private void ConnectError()
        {
            Polltimer.Stop();
            IsConnected = false;
            DeBug_P("無法連線");

            ReConnectTimer.Start();

        }

        private void Polltimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Polltimer.Stop();

            try
            {
                ReadPLC(Reg.R, 4151, 1, Tcp_c);
                ReadPLC(Reg.R, 100, 30, Tcp_c);
                //ReadPLC(Reg.D, 110, 1, Tcp_c);
                //ReadPLC(Reg.M, 1000, 1, Tcp_c);
                //ReadPLC(Reg.R, 300, 12, Tcp_c);
                //ReadPLC(Reg.D, 700, 2, Tcp_c);
                //ReadPLC(Reg.D, 110, 1, Tcp_c);
                //ReadPLC(Reg.D, 391, 1, Tcp_c);


            }
            catch (Exception)
            {

                Tcp_c.Close();
                ConnectError();
            }



            Polltimer.Start();
        }

        void ReadPLC(Reg regtyp, int startmem, int lenmem, TcpClient tcp)
        {

            string RX, TX;
            TX = FatekCMDgen(regtyp, startmem, lenmem);
            try
            {
                RX = WriteToPLCV2(TX, tcp);
            }
            catch (Exception)
            {

                throw;
            }

            WriteToMemR(TX, RX);



        }

        string FatekCMDgen(Reg regtyp, int startmem, int lenmem)
        {

            string site = "01";
            string cmdtype;
            string len;
            string str_reg_name;


            switch (regtyp)
            {
                case Reg.R:
                    cmdtype = "46";
                    len = Convert.ToString(lenmem, 16).PadLeft(2, '0').ToUpper();
                    str_reg_name = "R" + startmem.ToString().PadLeft(5, '0');
                    break;
                case Reg.D:
                    cmdtype = "46";
                    len = Convert.ToString(lenmem, 16).PadLeft(2, '0').ToUpper();
                    str_reg_name = "D" + startmem.ToString().PadLeft(5, '0');
                    break;
                case Reg.M:
                    cmdtype = "44";
                    len = Convert.ToString(lenmem, 16).PadLeft(2, '0').ToUpper();
                    str_reg_name = "M" + startmem.ToString().PadLeft(4, '0');
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }



            string cmd = site + cmdtype + len + str_reg_name;

            return cmd + FATEKCheckSum(cmd);

        }


        /// <summary>
        /// 寫入本機D暫存器
        /// </summary>
        /// <param name="mem">暫存器代號名稱</param>
        /// <param name="Start">起始位置</param>
        /// <param name="RX">接收內容</param>
        void WriteToMemD(string TX, string RX)
        {
            int MemStart = Convert.ToInt32(TX.Substring(7, 5));
            int MemLen = HEX2DEC(TX.Substring(4, 2));
            string subRX = RX.Substring(6, MemLen * 4);

            for (int i = 0; i <= MemLen - 1; i++)
            {
                D[MemStart + i] = subRX.Substring(i * 4, 4);
            }

            DeBug_P(String.Format("MemStart: {0}.MemLen: {1}", MemStart, MemLen));
            DeBug_P(subRX);
            DeBug_P(subRX.Length.ToString());
            DeBug_P(D[1008]);

        }

        /// <summary>
        /// 寫入本機R暫存器
        /// </summary>
        /// <param name="mem">暫存器代號名稱</param>
        /// <param name="Start">起始位置</param>
        /// <param name="RX">接收內容</param>
        void WriteToMemR(string TX, string RX)
        {
            int MemStart;
            int MemLen = HEX2DEC(TX.Substring(4, 2));
            string subRX;
            string memType = TX.Substring(6, 1);
            Reg regtype;
            if (memType == "D")
            {
                regtype = Reg.D;
            }
            else if (memType == "R")
            {
                regtype = Reg.R;
            }
            else if (memType == "M")
            {
                regtype = Reg.M;
            }
            else
            {
                throw new Exception();
            }


            switch (regtype)
            {
                case Reg.R:
                    MemStart = Convert.ToInt32(TX.Substring(7, 5));
                    subRX = RX.Substring(6, MemLen * 4);
                    for (int i = 0; i <= MemLen - 1; i++) R[MemStart + i] = subRX.Substring(i * 4, 4);

                    break;
                case Reg.D:
                    MemStart = Convert.ToInt32(TX.Substring(7, 5));
                    subRX = RX.Substring(6, MemLen * 4);
                    for (int i = 0; i <= MemLen - 1; i++) D[MemStart + i] = subRX.Substring(i * 4, 4);
                    break;
                case Reg.M:
                    MemStart = Convert.ToInt32(TX.Substring(7, 4));
                    subRX = RX.Substring(6, MemLen);
                    for (int i = 0; i <= MemLen - 1; i++)
                    {
                        if (subRX.Substring(i, 1) == "1") M[MemStart + i] = true;
                        else M[MemStart + i] = false;
                    }
                    break;
                default:
                    break;
            }



            //DeBug_P(String.Format("MemStart: {0}.MemLen: {1}", MemStart, MemLen));
            //DeBug_P(subRX);
            //DeBug_P(subRX.Length.ToString());
            //DeBug_P(R[4151]);

        }

        void WriteToMemM(string TX, string RX)
        {
            int MemStart = Convert.ToInt32(TX.Substring(7, 4));
            int MemLen = HEX2DEC(TX.Substring(4, 2));
            string subRX = RX.Substring(6, MemLen);

            for (int i = 0; i <= MemLen - 1; i++)
            {

                if (subRX.Substring(i, 1) == "1") M[MemStart + i] = true;
                else M[MemStart + i] = false;



            }

            //DeBug_P(String.Format("MemStart: {0}.MemLen: {1}", MemStart, MemLen));
            //DeBug_P(subRX);
            //DeBug_P(subRX.Length.ToString());
            //DeBug_P(M[1000].ToString());
            //DeBug_P(D[1008]);

        }

        void DeBug_P(string str)
        {



            System.Diagnostics.Debug.Print(str);

        }

        int HEX2DEC(string hex)
        {

            return Convert.ToInt32(hex, 16);


        }


        /// <summary>
        /// 新的PLC讀寫核心，新增無資料重送及CRC 檢驗功能。
        /// </summary>
        /// <param name="cmd">指令，不含控制字元。</param>
        /// <param name="tcp">TCPClient 類別</param>
        /// <returns></returns>
        public string WriteToPLCV2(string cmd, TcpClient tcp)
        {
            byte[] sendBuf = System.Text.Encoding.ASCII.GetBytes((char)2 + cmd + (char)3);
            int trytimes = 0;
        ReTry:
            if (trytimes++ >= 60) throw new Exception("overtrytime");


            DeBug_P(String.Format("TryTimes: {0} 次", trytimes.ToString()));


            bool closed;
            try
            {
                //使用Peek測試連線是否仍存在
                if (tcp.Connected && tcp.Client.Poll(0, SelectMode.SelectRead))
                    closed = tcp.Client.Receive(new byte[1], SocketFlags.Peek) == 0;
            }
            catch (SocketException se)
            {
                closed = true;
            }


            try
            {
                //tcp.Client.Receive(new byte[1], SocketFlags.Peek);
                if (tcp?.Connected ?? false && !closed)
                {


                    tcp.GetStream().Write(sendBuf, 0, sendBuf.Length);

                    int waittimes = 0;
                    while (!tcp.GetStream().DataAvailable)
                    {



                        if (waittimes++ > 30)
                        {
                            waittimes = 0;

                            throw new Exception("overwattime");
                        }
                        System.Threading.Thread.Sleep(5);
                    }
                    byte[] rec_buf = new byte[tcp.Available];
                    tcp.GetStream().Read(rec_buf, 0, rec_buf.Length);
                    string Str = System.Text.Encoding.ASCII.GetString(rec_buf);


                    if (FATEKCRC_check(cmd, Str)) return Str;
                    else throw new Exception("CRCerror");



                }
                else throw new SocketException((int)SocketError.SocketError);
            }
            catch (Exception e)
            {
                if (e.Message == "overwattime" | e.Message == "CRCerror")
                {
                    goto ReTry;
                }


                tcp.Close();

                throw;

            }





        }

        /// <summary>
        /// 檢查LCR 是否正確。
        /// </summary>
        /// <param name="TXcmd">傳送出的資料</param>
        /// <param name="RXcmd">收到的資料</param>
        /// <returns></returns>
        bool FATEKCRC_check(string TXcmd, string RXcmd)
        {
            try
            {
                string PLC_BK_Site = RXcmd.Substring(1, 2);
                string PLC_BK_cmd = RXcmd.Substring(3, 2);
                string PLC_BK_CLR = RXcmd.Substring(RXcmd.Length - 3, 2);

                string PLC_In_Site = TXcmd.Substring(0, 2);
                string PLC_In_cmd = TXcmd.Substring(2, 2);
                string PLC_CLR = FATEKCheckSum(RXcmd.Substring(1, RXcmd.Length - 4));

                bool check = (PLC_BK_Site == PLC_In_Site) & (PLC_BK_cmd == PLC_In_cmd) & (PLC_CLR == PLC_BK_CLR);

                if (check) return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }


        }


        /// <summary>
        /// 永宏PLC LCR
        /// </summary>
        /// <param name="chkstr">傳入參數，不含控制字元。</param>
        /// <returns>檢查碼</returns>
        public string FATEKCheckSum(string chkstr)
        {

            char[] str = chkstr.ToCharArray();
            int sum = 2;

            foreach (char c in str) sum += Convert.ToInt32(c);

            string HEX = Convert.ToString(sum, 16);

            return HEX.Substring(HEX.Length - 2, 2).ToUpper();

        }
        /// <summary>
        /// PLC 資料寫入核心，前後已加控制字元。
        /// </summary>
        /// <param name="cmd">指令，不含控制字元。</param>
        /// <param name="tcpc">TCPClient 類別</param>
        /// <returns></returns>
        public string WriteToPLC(string cmd, TcpClient tcpc) //TCP 讀寫核心
        {
            try
            {

                byte[] sendBuf = System.Text.Encoding.ASCII.GetBytes((char)2 + cmd + (char)3);
                String Str = string.Empty;


                tcpc.GetStream().Write(sendBuf, 0, sendBuf.Length);
                System.Threading.Thread.Sleep(100);
                if (tcpc.GetStream().DataAvailable == true)
                {

                    Byte[] rec_buf = new byte[tcpc.Available];
                    tcpc.GetStream().Read(rec_buf, 0, rec_buf.Length);
                    Str = System.Text.Encoding.ASCII.GetString(rec_buf);
                }

                return Str;
            }
            catch (SocketException)
            {
                return string.Empty;
            }





        }

    }
}
