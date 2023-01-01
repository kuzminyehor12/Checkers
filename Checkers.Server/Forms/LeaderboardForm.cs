using Checkers.Forms.Forms;
using Checkers.Server.DataManagement;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Checkers.Client.Forms
{
    public partial class LeaderboardForm : Form
    {
        private readonly IUserService _userService;
        public LeaderboardForm()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void LeaderboardForm_Load(object sender, EventArgs e)
        {
            var users = _userService.GetUsers();
            foreach (var user in users.OrderByDescending(u => u.VictoriesQuantity).ThenByDescending(u => u.Points))
            {
                dataGridView1.Rows.Add(user.Nickname, user.VictoriesQuantity, user.Points);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ConnectionForm connection = new ConnectionForm();
            connection.Show();
            this.Close();
        }
    }
}
