using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.IdentityForms;
using PawnShop.Forms.Validation;
using PawnShop.Oracle.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CustomAuth.Services;

namespace PawnShop.Forms.Forms.BaseForms
{
    public partial class AuthorizationForm : Form
    {
        private string ConnectionString { get; }
        private readonly IdentityService _identityService;
        public AuthorizationForm()
        {
            InitializeComponent();
            ConnectionString = ConfigurationManager.ConnectionStrings["CustomIdentity"].ConnectionString;
            _identityService = new IdentityService(ConnectionString);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.Show();
            Visible = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if(!TryCheckValid())
            {
                return;
            }

            try
            {
                var user = await _identityService.FindByEmailAsync(textBox1.Text);

                if (PasswordSecurity.Verify(textBox2.Text, user.Password))
                {
                    //if(user.Role == RoleName.Dealer.ToString())
                    //{
                    //    var dealerForm = new DealerMainForm();
                    //    dealerForm.Show();
                    //    Visible = false;
                    //}
                    //else
                    //{
                    //    var packerForm = new PackerMainForm();
                    //    packerForm.Show();
                    //    Visible = false;
                    //}
                    MessageBox.Show("Authorization success!");
                    Visible = false;
                    return;
                }

                MessageBox.Show("Login or password has been entered incorrectly.");
            }
            catch (Exception)
            {
                MessageBox.Show("There are no user with those credentials. Try to register!");
            }
           
        }

        private bool TryCheckValid()
        {
            try
            {
                CheckValid();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void CheckValid()
        {
            foreach (var l in GetStateLabels())
            {
                if (l.Text == "Invalid")
                {
                    throw new FormValidationException("Data was input invalid!");
                }
            }
        }
        private Label[] GetStateLabels()
        {
            return new[]
            {
                label2, label3
            };
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                label2.SetInvalid();
            }
            else
            {
                label2.SetValid();
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                label3.SetInvalid();
            }
            else
            {
                label3.SetValid();
            }
        }
    }
}
