using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;

namespace SwitchWeb
{
    public partial class Form1 : Form
    {
        public static string onUrl = "http://admin:12345678@192.168.177.161/relay.cgi?relayon1=on";
        public static string offUrl = "http://admin:12345678@192.168.177.161/relay.cgi?relayoff1=off";
        private static Encoding encode = Encoding.Default;
        private TcpClient tcpsz = null;
        #region time
        string onOne = string.Empty;
        string offOne = string.Empty;
        string onTwo = string.Empty;
        string offTwo = string.Empty;
        string onThree = string.Empty;
        string offThree = string.Empty;
        string onFour = string.Empty;
        string offFour = string.Empty;
        #endregion
        string ip = string.Empty;
        string port = string.Empty;
        #region bool
        bool one = false;
        bool two = false;
        bool three = false;
        bool four = false;
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.BackColor = Color.Green;
            button3.BackColor = Color.Green;
            button4.BackColor = Color.Green;
            button5.BackColor = Color.Green;
            button6.BackColor = Color.Green;
            this.Text = textIP.Text.ToString();
            //   button3.BackColor = Color.Green;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Net.HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(onUrl);
            string uid = "admin";
            string pwd = "12345678";
            string domain = "";
            NetworkCredential nc = new NetworkCredential(uid, pwd, domain);
            request.Credentials = nc;
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            StreamReader myreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseText = myreader.ReadToEnd();
            myreader.Close();
           // MessageBox.Show(responseText);
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { // 总是接受
            return true;
        }

        public void socket()
        {
            tcpsz = new TcpClient();
            tcpsz.BeginConnect("192.168.177.161", Convert.ToInt32(1234), new AsyncCallback(ConnectCallback), tcpsz);
        }
        private void ConnectCallback(IAsyncResult ar)

        {

            TcpClient t = (TcpClient)ar.AsyncState;

            try

            {

                if (t.Connected)

                {

                    t.EndConnect(ar);//函数运行到这里就说明连接成功
                    MessageBox.Show("连接成功");

                }

                else

                {

                }

            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public void Send(string host, int port, string data)
        {
            string result = string.Empty;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(host, port);
            clientSocket.Send(encode.GetBytes(data));
            Console.WriteLine("Send：" + data);
            result = Receive(clientSocket, 5000 * 2); //5*2 seconds timeout.
            Console.WriteLine("Receive：" + result);
            DestroySocket(clientSocket);
         //   MessageBox.Show(result);
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static string Receive(Socket socket, int timeout)
        {
            string result = string.Empty;
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
        private void button2_Click(object sender, EventArgs e)
        {
      
            this.Text = textIP.Text.ToString();
            if (button2.BackColor == Color.Red)
            {
                button2.BackColor = Color.Green;
                this.timer1.Enabled = false;
                textIP.Enabled = true;
                textPort.Enabled = true;

            }
            else
            {
                textIP.Enabled = false;
                textPort.Enabled = false;
                button2.BackColor = Color.Red;
                this.timer1.Enabled = true;
                ip = textIP.Text.ToString().Trim();
                port = textPort.Text.ToString().Trim();
                if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port))
                {
                    MessageBox.Show("IP或端口不能为空");
                    return;
                }
                Send(ip, int.Parse(port), "state=?");
            }


            //send("192.168.177.161",1234, "setr=10xxxxxxxxx");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string a = "x";
            string b = "x";
            string c = "x";
            string d = "x";
            try
            {
                if (button3.BackColor == Color.Red)
                {
                    if (time == onOne && !one)
                    {
                        Send(ip, int.Parse(port), "setr=1xxxxxxxxxx");
                        a = "1";
                        one = true;
                    }
                    if (time == offOne && one)
                    {
                        Send(ip, int.Parse(port), "setr=0xxxxxxxxxx");
                        a = "0";
                        one = false;
                    }
                }
                if (button4.BackColor == Color.Red)
                {
                    if (time == onTwo && !two)
                    {
                        Send(ip, int.Parse(port), "setr=x1xxxxxxxxx");
                        b = "1";
                        two = true;
                    }
                    if (time == offTwo && two)
                    {
                        Send(ip, int.Parse(port), "setr=x0xxxxxxxxx");
                        b = "0";
                        two = false;
                    }
                }
                if (button5.BackColor == Color.Red)
                {
                    if (time == onThree && !three)
                    {
                         Send(ip, int.Parse(port), "setr=xx1xxxxxxxx");
                        c = "1";
                        three = true;
                    }
                    if (time == offThree && three)
                    {
                        Send(ip, int.Parse(port), "setr=xx0xxxxxxxx");
                        c = "0";
                        three = false;
                    }
                }
                if (button6.BackColor == Color.Red)
                {
                    if (time == onFour && !four)
                    {
                         Send(ip, int.Parse(port), "setr=xxx1xxxxxxx");
                        d = "1";
                        four = true;
                    }
                    if (time == offFour && four)
                    {
                        Send(ip, int.Parse(port), "setr=xxx0xxxxxxx");
                        d = "0";
                        four = false;
                    }
                }
                //string data = string.Format("setr={0}{1}{2}{3}", a, b, c, d);
               // send(ip, int.Parse(port), data);

            }
            catch (Exception er)
            {
                //MessageBox.Show(er.ToString());
                return;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            SunTimes.GetSunTime(time,120,26);
        }
    }
}
