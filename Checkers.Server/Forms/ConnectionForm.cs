using Checkers.Server.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers.Forms.Forms
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            textBox1.Text = GetIP();
        }

        private string GetIP()
        {
            IPAddress[] localIp = Dns.GetHostAddresses(Dns.GetHostName());

            string ipAddress = "";

            foreach (var address in localIp)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = address.ToString();
                }
            }
            return ipAddress;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TCPServer.Instance.Start(textBox1.Text, textBox2.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TCPServer.Instance.Client != null && TCPServer.Instance.Client.Connected)
            {
                timer1.Stop();
                CheckersForm game = new CheckersForm();
                game.Show();
                Visible = false;
            }
            else if (TCPServer.Instance.Listener != null)
            {
                label4.Text = "The server has been started. Waiting for players...";
            }
            else
            {
                label4.Text = "Waiting for starting...";
            }
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void ConnectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TCPServer.Instance.Client.Close();
            TCPServer.Instance.Client.Dispose();

            TCPServer.Instance.Listener.Stop();
        }
    }
}
