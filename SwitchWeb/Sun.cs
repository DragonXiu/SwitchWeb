using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using log4net;
using log4net.Layout;
using log4net.Config;
using System.Reflection;

namespace SwitchWeb
{
    public partial class Sun : Form
    {
        #region 参数
        public static ILog log;
        static double longitude = 120.0;
        static double latitude = 36.0;
        static string date = string.Empty;
        SunTimeResult GetSunTime;
        static string ipone = string.Empty;
        static string portone = string.Empty;
        static string iptwo = string.Empty;
        static string porttwo = string.Empty;
        static string ipthree = string.Empty;
        static string portthree = string.Empty;
        static string ipfour = string.Empty;
        static string portfour = string.Empty;
        static string ipfive = string.Empty;
        static string portfive = string.Empty;
        static string ipsix = string.Empty;
        static string portsix = string.Empty;
        static string ipseven = string.Empty;
        static string portseven = string.Empty;
        static string ipeight = string.Empty;
        static string porteight = string.Empty;
        static string result = string.Empty;
        bool on = false;
        bool off = false;
        private static Encoding encode = Encoding.Default;
        #endregion
        public Sun()
        {
            InitializeComponent();
        }

        private void Sun_Load(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();
            log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            var logPattern = "%d{yyyy-MM-dd HH:mm:ss} --%-5p-- %m%n";
            //设置打印日志
            var list_logAppender = new log.ListViewLog()
            {
                listView = this.listView1,
                Layout = new PatternLayout(logPattern)
            };
            BasicConfigurator.Configure(list_logAppender);
            button1.BackColor = Color.Green;
            butone.BackColor = Color.Green;
            buttwo.BackColor = Color.Green;
            butthree.BackColor = Color.Green;
            butseven.BackColor = Color.Green;
            buteight.BackColor = Color.Green;
            butsix.BackColor = Color.Green;
            butfour.BackColor = Color.Green;
            butfive.BackColor = Color.Green;
            log.Info("开机");
            StartOne();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.BackColor == Color.Red)
            {
                button1.BackColor = Color.Green;
                button1.Text = "打开定时";
                this.timer1.Enabled = false;
                longt.Enabled = true;
                lait.Enabled = true;
                times.Text = string.Empty;
                rise.Text = string.Empty;
                set.Text = string.Empty;
                log.Info("关闭定时");
            }
            else
            {
                if (string.IsNullOrEmpty(longt.Text.ToString().Trim()) || string.IsNullOrEmpty(lait.Text.ToString().Trim()))
                {
                    MessageBox.Show("经度或纬度不能为空");
                    return;
                }
                longt.Enabled = false;
                lait.Enabled = false;
                button1.BackColor = Color.Red;
                button1.Text = "关闭定时";
                this.timer1.Enabled = true;
                longitude = double.Parse(longt.Text.ToString().Trim());
                latitude = double.Parse(lait.Text.ToString().Trim());
                times.Text = DateTime.Now.ToString("yyyy-MM-dd");
                GetSunTime = SunTimes.GetSunTime(DateTime.Now, longitude, latitude);
                rise.Text = GetSunTime.SunriseTime.ToString("HH:mm:ss");
                set.Text = GetSunTime.SunsetTime.ToString("HH:mm:ss");
                log.Info("打开定时");
            }
        }
        #region 链接按钮
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static string Receive(Socket socket, int timeout)
        {

            socket.ReceiveTimeout = timeout;
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[1024];
            int length = 0;
            try
            {
                while ((length = socket.Receive(buffer)) > 0)
                {
                    for (int j = 0; j < length; j++)
                    {
                        data.Add(buffer[j]);
                    }
                    if (length < buffer.Length)
                    {
                        break;
                    }
                }
            }
            catch { }
            if (data.Count > 0)
            {
                result = encode.GetString(data.ToArray(), 0, data.Count);
            }
            return result;
        }
        public bool Send(string host, int port, string data)
        {
            try
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.SendTimeout=3000;
                clientSocket.Connect(host, port);
                clientSocket.Send(encode.GetBytes(data));
                Console.WriteLine("Send：" + data);
                result = Receive(clientSocket, 5000 * 2); //5*2 seconds timeout.
                Console.WriteLine("Receive：" + result);
                DestroySocket(clientSocket);
                return true;
            }
            catch (Exception e)
            {
                log.Error("链接设备失败，请查询IP:"+host+"/端口:"+port+"是否正确");
                return false;
            }
            //   MessageBox.Show(result);
        }
        /// <summary>
        /// 销毁Socket对象
        /// </summary>
        /// <param name="socket"></param>
        private static void DestroySocket(Socket socket)
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }
        private void butone_Click(object sender, EventArgs e)
        {
            try
            {
                if (butone.BackColor == Color.Red)
                {
                    butone.BackColor = Color.Green;
                    butone.Text = "链接";
                    ip1.Enabled = true;
                    port1.Enabled = true;
                    lab1.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip1.Text.ToString().Trim()) || string.IsNullOrEmpty(port1.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    ipone = ip1.Text.ToString().Trim();
                    portone = port1.Text.ToString().Trim();
                    // Send(ipone,int.Parse(portone), "state=?");
                    if (!Send(ipone, int.Parse(portone), "state=?"))
                    {
                        return;
                    }
                    lab1.Text = JsonDate();
                    ip1.Enabled = false;
                    port1.Enabled = false;
                    butone.BackColor = Color.Red;
                    butone.Text = "断开";

                }
            }
            catch (Exception)
            {

                return;
            }
        }

        private void buttwo_Click(object sender, EventArgs e)
        {
            try
            {


                if (buttwo.BackColor == Color.Red)
                {
                    buttwo.BackColor = Color.Green;
                    buttwo.Text = "链接";
                    ip2.Enabled = true;
                    port2.Enabled = true;
                    lab2.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip2.Text.ToString().Trim()) || string.IsNullOrEmpty(port2.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    iptwo = ip2.Text.ToString().Trim();
                    porttwo = port2.Text.ToString().Trim();
                    if (!Send(iptwo, int.Parse(porttwo), "state=?"))
                    {
                        return;
                    }

                    lab2.Text = JsonDate();
                    ip2.Enabled = false;
                    port2.Enabled = false;
                    buttwo.BackColor = Color.Red;
                    buttwo.Text = "断开";


                }
            }
            catch (Exception)
            {

                return;
            }
        }

        private void butthree_Click(object sender, EventArgs e)
        {
            try
            {

                if (butthree.BackColor == Color.Red)
                {
                    butthree.BackColor = Color.Green;
                    butthree.Text = "链接";
                    ip3.Enabled = true;
                    port3.Enabled = true;
                    lab3.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip3.Text.ToString().Trim()) || string.IsNullOrEmpty(port3.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    ipthree = ip3.Text.ToString().Trim();
                    portthree = port3.Text.ToString().Trim();
                    if (!Send(ipthree, int.Parse(portthree), "state=?"))
                    {
                        return;
                    }
                    lab3.Text = JsonDate();
                    ip3.Enabled = false;
                    port3.Enabled = false;
                    butthree.BackColor = Color.Red;
                    butthree.Text = "断开";


                }

            }
            catch (Exception)
            {

                return;
            }
        }

        private void butfour_Click(object sender, EventArgs e)
        {
            try
            {


                if (butfour.BackColor == Color.Red)
                {
                    butfour.BackColor = Color.Green;
                    butfour.Text = "链接";
                    ip4.Enabled = true;
                    port4.Enabled = true;
                    lab4.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip4.Text.ToString().Trim()) || string.IsNullOrEmpty(port4.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    ipfour = ip4.Text.ToString().Trim();
                    portfour = port4.Text.ToString().Trim();
                    if (!Send(ipfour, int.Parse(portfour), "state=?"))
                    {
                        return;
                    }
                    lab4.Text = JsonDate();
                    ip4.Enabled = false;
                    port4.Enabled = false;
                    butfour.BackColor = Color.Red;
                    butfour.Text = "断开";


                }
            }
            catch (Exception)
            {

                return;
            }
        }

        private void butfive_Click(object sender, EventArgs e)
        {
            try
            {

                if (butfive.BackColor == Color.Red)
                {
                    butfive.BackColor = Color.Green;
                    butfive.Text = "链接";
                    ip5.Enabled = true;
                    port5.Enabled = true;
                    lab5.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip5.Text.ToString().Trim()) || string.IsNullOrEmpty(port5.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    ipfive = ip5.Text.ToString().Trim();
                    portfive = port5.Text.ToString().Trim();
                    if (!Send(ipfive, int.Parse(portfive), "state=?"))
                    {
                        return;
                    }
                    lab5.Text = JsonDate();
                    ip5.Enabled = false;
                    port5.Enabled = false;
                    butfive.BackColor = Color.Red;
                    butfive.Text = "断开";


                }

            }
            catch (Exception)
            {

                return;
            }
        }

        private void butsix_Click(object sender, EventArgs e)
        {
            try
            {


                if (butsix.BackColor == Color.Red)
                {
                    butsix.BackColor = Color.Green;
                    butsix.Text = "链接";
                    ip6.Enabled = true;
                    port6.Enabled = true;
                    lab6.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip6.Text.ToString().Trim()) || string.IsNullOrEmpty(port6.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    ipsix = ip6.Text.ToString().Trim();
                    portsix = port6.Text.ToString().Trim();
                    if (!Send(ipsix, int.Parse(portsix), "state=?"))
                    {
                        return;
                    }
                    lab6.Text = JsonDate();
                    ip6.Enabled = false;
                    port6.Enabled = false;
                    butsix.BackColor = Color.Red;
                    butsix.Text = "断开";


                }
            }
            catch (Exception)
            {

                return;
            }
        }

        private void butseven_Click(object sender, EventArgs e)
        {
            try
            {


                if (butseven.BackColor == Color.Red)
                {
                    butseven.BackColor = Color.Green;
                    butseven.Text = "链接";
                    ip7.Enabled = true;
                    port7.Enabled = true;
                    lab7.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip7.Text.ToString().Trim()) || string.IsNullOrEmpty(port7.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    ipseven = ip7.Text.ToString().Trim();
                    portseven = port7.Text.ToString().Trim();

                    if (!Send(ipseven, int.Parse(portseven), "state=?"))
                    {
                        return;
                    }
                    lab7.Text = JsonDate();
                    ip7.Enabled = false;
                    port7.Enabled = false;
                    butseven.BackColor = Color.Red;
                    butseven.Text = "断开";


                }
            }
            catch (Exception)
            {

                return;
            }
        }

        private void buteight_Click(object sender, EventArgs e)
        {
            try
            {


                if (buteight.BackColor == Color.Red)
                {
                    buteight.BackColor = Color.Green;
                    buteight.Text = "链接";
                    ip8.Enabled = true;
                    port8.Enabled = true;
                    lab8.Text = string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(ip8.Text.ToString().Trim()) || string.IsNullOrEmpty(port8.Text.ToString().Trim()))
                    {
                        MessageBox.Show("IP或端口不能为空");
                        return;
                    }
                    ipeight = ip8.Text.ToString().Trim();
                    porteight = port8.Text.ToString().Trim();

                    if (!Send(ipeight, int.Parse(porteight), "state=?"))
                    {
                        return;
                    }
                    lab8.Text = JsonDate();
                    ip8.Enabled = false;
                    port8.Enabled = false;
                    buteight.BackColor = Color.Red;
                    buteight.Text = "断开";


                }
            }
            catch (Exception)
            {

                return;
            }
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime time = DateTime.Now;
                if (time.ToString("yyyy-MM-dd") != times.Text.ToString().Trim())
                {
                    log.Info("日期变了,前一天;" + times.Text.ToString() + "后一天:" + time.ToString("yyyy-MM-dd"));
                    times.Text = time.ToString("yyyy-MM-dd");
                }
                GetSunTime = SunTimes.GetSunTime(DateTime.Now, longitude, latitude);
                string risetime = GetSunTime.SunriseTime.ToString("HH:mm:ss");
                string settime = GetSunTime.SunsetTime.ToString("HH:mm:ss");
                if (risetime != rise.Text.ToString().Trim())
                {
                    rise.Text = risetime;
                    set.Text = settime;
                }


                if (time.ToString("HH:mm:ss") == risetime)
                {
                    if (!on)
                    {
                        if (butone.BackColor == Color.Red)
                        {
                            Send(ipone, int.Parse(portone), "setr=0000xxxxxx");
                            lab1.Text = JsonDate();
                            log.Info("IP:" + ipone + ",端口:" + portone + "-关闭" + ",设备状态:" + lab1.Text);
                        }
                        if (buttwo.BackColor == Color.Red)
                        {
                            Send(iptwo, int.Parse(porttwo), "setr=0000xxxxxx");
                            lab2.Text = JsonDate();
                            log.Info("IP:" + iptwo + ",端口:" + porttwo + "-关闭" + ",设备状态:" + lab2.Text);
                        }
                        if (butthree.BackColor == Color.Red)
                        {
                            Send(ipthree, int.Parse(portthree), "setr=0000xxxxxx");
                            lab3.Text = JsonDate();
                            log.Info("IP:" + ipthree + ",端口:" + portthree + "-关闭" + ",设备状态:" + lab3.Text);
                        }
                        if (butfour.BackColor == Color.Red)
                        {
                            Send(ipfour, int.Parse(portfour), "setr=0000xxxxxx");
                            lab4.Text = JsonDate();
                            log.Info("IP:" + ipfour + ",端口:" + portfour + "-关闭" + ",设备状态:" + lab4.Text);
                        }
                        if (butfive.BackColor == Color.Red)
                        {
                            Send(ipfive, int.Parse(portfive), "setr=0000xxxxxx");
                            lab5.Text = JsonDate();
                            log.Info("IP:" + ipfive + ",端口:" + portfive + "-关闭" + ",设备状态:" + lab5.Text);
                        }
                        if (butsix.BackColor == Color.Red)
                        {
                            Send(ipsix, int.Parse(portsix), "setr=0000xxxxxx");
                            lab6.Text = JsonDate();
                            log.Info("IP:" + ipsix + ",端口:" + portsix + "-关闭" + ",设备状态:" + lab6.Text);
                        }
                        if (butseven.BackColor == Color.Red)
                        {
                            Send(ipseven, int.Parse(portseven), "setr=0000xxxxxx");
                            lab7.Text = JsonDate();
                            log.Info("ipseven:" + iptwo + ",端口:" + portseven + "-关闭" + ",设备状态:" + lab7.Text);
                        }
                        if (buteight.BackColor == Color.Red)
                        {
                            Send(ipeight, int.Parse(porteight), "setr=0000xxxxxx");
                            lab8.Text = JsonDate();
                            log.Info("IP:" + ipeight + ",端口:" + porteight + "-关闭" + ",设备状态:" + lab8.Text);
                        }
                        on = true;
                    }

                }
                else
                {
                    on = false;
                }
                if (time.ToString("HH:mm:ss") == settime)
                {
                    if (!off)
                    {
                        if (butone.BackColor == Color.Red)
                        {
                            Send(ipone, int.Parse(portone), "setr=1111xxxxxx");
                            lab1.Text = JsonDate();
                            log.Info("IP:" + ipone + ",端口:" + portone + "-开启" + ",设备状态:" + lab1.Text);
                        }
                        if (buttwo.BackColor == Color.Red)
                        {
                            Send(iptwo, int.Parse(porttwo), "setr=1111xxxxxx");
                            lab2.Text = JsonDate();
                            log.Info("IP:" + iptwo + ",端口:" + porttwo + "-开启" + ",设备状态:" + lab2.Text);
                        }
                        if (butthree.BackColor == Color.Red)
                        {
                            Send(ipthree, int.Parse(portthree), "setr=1111xxxxxx");
                            lab3.Text = JsonDate();
                            log.Info("IP:" + ipthree + ",端口:" + portthree + "-开启" + ",设备状态:" + lab3.Text);
                        }
                        if (butfour.BackColor == Color.Red)
                        {
                            Send(ipfour, int.Parse(portfour), "setr=1111xxxxxx");
                            lab4.Text = JsonDate();
                            log.Info("IP:" + ipfour + ",端口:" + portfour + "-开启" + ",设备状态:" + lab4.Text);
                        }
                        if (butfive.BackColor == Color.Red)
                        {
                            Send(ipfive, int.Parse(portfive), "setr=1111xxxxxx");
                            lab5.Text = JsonDate();
                            log.Info("IP:" + ipfive + ",端口:" + portfive + "-开启" + ",设备状态:" + lab5.Text);
                        }
                        if (butsix.BackColor == Color.Red)
                        {
                            Send(ipsix, int.Parse(portsix), "setr=1111xxxxxx");
                            lab6.Text = JsonDate();
                            log.Info("IP:" + ipsix + ",端口:" + portsix + "-开启" + ",设备状态:" + lab6.Text);
                        }
                        if (butseven.BackColor == Color.Red)
                        {
                            Send(ipseven, int.Parse(portseven), "setr=1111xxxxxx");
                            lab7.Text = JsonDate();
                            log.Info("IP:" + ipseven + ",端口:" + portseven + "-开启" + ",设备状态:" + lab7.Text);
                        }
                        if (buteight.BackColor == Color.Red)
                        {
                            Send(ipeight, int.Parse(porteight), "setr=1111xxxxxx");
                            lab8.Text = JsonDate();
                            log.Info("IP:" + ipeight + ",端口:" + porteight + "-开启" + ",设备状态:" + lab8.Text);
                        }
                        off = true;
                    }

                }
                else
                {
                    off = false;
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        public string JsonDate()
        {
            Root rt = JsonConvert.DeserializeObject<Root>(result);
            return rt.output;
        }
        public void StartOne()
        {
            if (!string.IsNullOrEmpty(ip1.Text.ToString().Trim()) && !string.IsNullOrEmpty(port1.Text.ToString().Trim()))
            {
                ipone = ip1.Text.ToString().Trim();
                portone = port1.Text.ToString().Trim();
                // Send(ipone,int.Parse(portone), "state=?");
                if (Send(ipone, int.Parse(portone), "state=?"))
                {

                    lab1.Text = JsonDate();
                    ip1.Enabled = false;
                    port1.Enabled = false;
                    butone.BackColor = Color.Red;
                    butone.Text = "断开";
                }
            }


            if (!string.IsNullOrEmpty(ip2.Text.ToString().Trim()) && !string.IsNullOrEmpty(port2.Text.ToString().Trim()))
            {
                iptwo = ip2.Text.ToString().Trim();
                porttwo = port2.Text.ToString().Trim();
                if (Send(iptwo, int.Parse(porttwo), "state=?"))
                {

                    lab2.Text = JsonDate();
                    ip2.Enabled = false;
                    port2.Enabled = false;
                    buttwo.BackColor = Color.Red;
                    buttwo.Text = "断开";
                }

            }


            if (!string.IsNullOrEmpty(ip3.Text.ToString().Trim()) && !string.IsNullOrEmpty(port3.Text.ToString().Trim()))
            {
                //   MessageBox.Show("IP或端口不能为空");
                ipthree = ip3.Text.ToString().Trim();
                portthree = port3.Text.ToString().Trim();
                if (Send(ipthree, int.Parse(portthree), "state=?"))
                {

                    lab3.Text = JsonDate();
                    ip3.Enabled = false;
                    port3.Enabled = false;
                    butthree.BackColor = Color.Red;
                    butthree.Text = "断开";
                }
            }



            if (!string.IsNullOrEmpty(ip4.Text.ToString().Trim()) && !string.IsNullOrEmpty(port4.Text.ToString().Trim()))
            {
                ipfour = ip4.Text.ToString().Trim();
                portfour = port4.Text.ToString().Trim();
                if (Send(ipfour, int.Parse(portfour), "state=?"))
                {

                    lab4.Text = JsonDate();
                    ip4.Enabled = false;
                    port4.Enabled = false;
                    butfour.BackColor = Color.Red;
                    butfour.Text = "断开";
                }
            }


            if (!string.IsNullOrEmpty(ip5.Text.ToString().Trim()) && !string.IsNullOrEmpty(port5.Text.ToString().Trim()))
            {
                ipfive = ip5.Text.ToString().Trim();
                portfive = port5.Text.ToString().Trim();
                if (Send(ipfive, int.Parse(portfive), "state=?"))
                {

                    lab5.Text = JsonDate();
                    ip5.Enabled = false;
                    port5.Enabled = false;
                    butfive.BackColor = Color.Red;
                    butfive.Text = "断开";
                }
            }

            if (!string.IsNullOrEmpty(ip6.Text.ToString().Trim()) && !string.IsNullOrEmpty(port6.Text.ToString().Trim()))
            {
                ipsix = ip6.Text.ToString().Trim();
                portsix = port6.Text.ToString().Trim();
                if (Send(ipsix, int.Parse(portsix), "state=?"))
                {
                    lab6.Text = JsonDate();
                    ip6.Enabled = false;
                    port6.Enabled = false;
                    butsix.BackColor = Color.Red;
                    butsix.Text = "断开";
                }
            }



            if (!string.IsNullOrEmpty(ip7.Text.ToString().Trim()) && !string.IsNullOrEmpty(port7.Text.ToString().Trim()))
            {

                ipseven = ip7.Text.ToString().Trim();
                portseven = port7.Text.ToString().Trim();
                if (Send(ipseven, int.Parse(portseven), "state=?"))
                {
                    lab7.Text = JsonDate();
                    ip7.Enabled = false;
                    port7.Enabled = false;
                    butseven.BackColor = Color.Red;
                    butseven.Text = "断开";
                }
            }





            if (!string.IsNullOrEmpty(ip8.Text.ToString().Trim()) && !string.IsNullOrEmpty(port8.Text.ToString().Trim()))
            {
                //   MessageBox.Show("IP或端口不能为空");
                ipeight = ip8.Text.ToString().Trim();
                porteight = port8.Text.ToString().Trim();

                if (Send(ipeight, int.Parse(porteight), "state=?"))
                {
                    lab8.Text = JsonDate();
                    ip8.Enabled = false;
                    port8.Enabled = false;
                    buteight.BackColor = Color.Red;
                    buteight.Text = "断开";
                }

            }


            if (!string.IsNullOrEmpty(longt.Text.ToString().Trim()) && !string.IsNullOrEmpty(lait.Text.ToString().Trim()))
            {
                longt.Enabled = false;
                lait.Enabled = false;
                button1.BackColor = Color.Red;
                button1.Text = "关闭定时";
                this.timer1.Enabled = true;
                longitude = double.Parse(longt.Text.ToString().Trim());
                latitude = double.Parse(lait.Text.ToString().Trim());
                times.Text = DateTime.Now.ToString("yyyy-MM-dd");
                GetSunTime = SunTimes.GetSunTime(DateTime.Now, longitude, latitude);
                rise.Text = GetSunTime.SunriseTime.ToString("HH:mm:ss");
                set.Text = GetSunTime.SunsetTime.ToString("HH:mm:ss");
                log.Info("打开定时");
            }
            else
            {
                MessageBox.Show("经度或纬度不能为空");
            }


        }
    }
}
