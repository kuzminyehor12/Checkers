using Checkers.Client.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TCPClient.Instance.Connect(textBox1.Text, textBox2.Text);

            if (TCPClient.Instance.Client != null)
            {
                CheckersForm game = new CheckersForm();
                game.Show();
                Visible = false;
            }
            else
            {
                label6.Text = "Waiting for connection...";
            }
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {

        }

        private void ConnectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TCPClient.Instance.Client.Close();
            TCPClient.Instance.Client.Dispose();
        }
    }
}
