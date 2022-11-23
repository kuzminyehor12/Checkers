using Checkers.Client.Models;
using Checkers.Client.Networking;
using Checkers.Server.DataManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
            _userService = new UserService();
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

            if (CurrentUser is null)
            {
                CurrentUser = new User
                {
                    Nickname = textBox4.Text,
                    VictoriesQuantity = 0
                };

                _userService.CreateUser(CurrentUser);
            }

            TCPClient.Instance.Connect(textBox1.Text, textBox2.Text);

            if (TCPClient.Instance.Client != null)
            {
                CheckersForm game = new CheckersForm(CurrentUser);
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
