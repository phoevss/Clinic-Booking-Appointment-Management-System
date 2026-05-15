using System;
using System.Drawing;
using System.Windows.Forms;
using BookingData;
using BookingData.Services;
using BookingCore.Interfaces;
using BookingCore.Entities;
using UserApp.Helpers;

namespace UserApp.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly IAuthService _authService;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnRegister;
        private Button btnBack;

        public RegisterForm()
        {
            var context = new BookingDbContext();
            _authService = new AuthService(context);
            InitializeComponent();
            ApplyStyles();
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPassword = new TextBox();
            this.btnRegister = new Button();
            this.btnBack = new Button();
            
            this.SuspendLayout();
            
            // txtUsername
            this.txtUsername.Location = new Point(50, 60);
            this.txtUsername.Size = new Size(200, 25);
            this.txtUsername.PlaceholderText = "Username";
            
            // txtEmail
            this.txtEmail.Location = new Point(50, 100);
            this.txtEmail.Size = new Size(200, 25);
            this.txtEmail.PlaceholderText = "Email";
            
            // txtPassword
            this.txtPassword.Location = new Point(50, 140);
            this.txtPassword.Size = new Size(200, 25);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.PlaceholderText = "Password";
            
            // btnRegister
            this.btnRegister.Location = new Point(50, 190);
            this.btnRegister.Size = new Size(200, 40);
            this.btnRegister.Text = "REGISTER";
            this.btnRegister.Click += new EventHandler(this.btnRegister_Click);
            
            // btnBack
            this.btnBack.Location = new Point(50, 240);
            this.btnBack.Size = new Size(200, 30);
            this.btnBack.Text = "Back to Login";
            this.btnBack.Click += (s, e) => this.Close();

            // RegisterForm
            this.ClientSize = new Size(300, 350);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnBack);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Register Account";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ApplyStyles()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            UIHelper.SetGradientBackground(this, Color.FromArgb(45, 45, 45), Color.FromArgb(20, 20, 20));
            
            btnRegister.BackColor = Color.FromArgb(0, 120, 215);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.ForeColor = Color.Gray;
            btnBack.FlatAppearance.BorderSize = 0;
            
            txtUsername.BackColor = Color.FromArgb(50, 50, 50);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            
            txtEmail.BackColor = Color.FromArgb(50, 50, 50);
            txtEmail.ForeColor = Color.White;
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            
            txtPassword.BackColor = Color.FromArgb(50, 50, 50);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("All fields are required.");
                return;
            }

            var user = new User
            {
                Username = txtUsername.Text,
                Email = txtEmail.Text,
                Role = "User"
            };

            bool success = await _authService.RegisterAsync(user, txtPassword.Text);
            if (success)
            {
                MessageBox.Show("Registration successful!");
                this.Close();
            }
            else
            {
                MessageBox.Show("User already exists or registration failed.");
            }
        }
    }
}
