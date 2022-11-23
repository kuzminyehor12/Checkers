using Checkers.Server.DataManagement;
using Checkers.Server.Enums;
using Checkers.Server.Models;
using Checkers.Server.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers.Forms.Forms
{
    public partial class ConnectionForm : Form
    {
        private readonly IUserService _userService;
        public User CurrentUser { get; private set; }
        public ConnectionForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            _userService = new UserService();
            textBox1.Text = GetIP();
        }

        private string GetIP()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Environment.MachineName);

            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.ToString();
                }
            }

            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Nickname is required to play");
                return;
            }

            var users = _userService.GetUsers();
            CurrentUser = users.FirstOrDefault(u => u.Nickname == textBox4.Text);

            if(CurrentUser is null)
            {
                CurrentUser = new User
                {
                    Nickname = textBox4.Text,
                    VictoriesQuantity = 0
                };

                _userService.CreateUser(CurrentUser);
            }
           
            TCPServer.Instance.Start(textBox1.Text, textBox2.Text);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TCPServer.Instance.Client != null && TCPServer.Instance.Client.Connected)
            {
                timer1.Stop();
                CheckersForm game;

                if (radioButton1.Checked)
                {
                    game = new CheckersForm(CurrentUser, GameMode.BO3);
                }
                else if (radioButton2.Checked)
                {
                    game = new CheckersForm(CurrentUser, GameMode.BO5);
                }
                else
                {
                    game = new CheckersForm(CurrentUser, GameMode.BO1);
                }
               
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
