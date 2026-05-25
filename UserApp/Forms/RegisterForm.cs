using System;
using System.Drawing;
using System.Text.RegularExpressions;
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
        private readonly BookingDbContext _context;
        private readonly IAuthService _authService;
        private Panel cardPanel;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnRegister;
        private Button btnBack;

        public RegisterForm()
        {
            _context = new BookingDbContext();
            _authService = new AuthService(_context);
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.BackColor = UIHelper.BodyBg;
            this.ClientSize = new Size(420, 550);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Create Account";
            this.MaximizeBox = false;

            Panel headerBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = UIHelper.Teal
            };
            Label clinicName = new Label
            {
                Text = "✦ Patient Portal",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerBar.Controls.Add(clinicName);

            cardPanel = new Panel
            {
                Size = new Size(340, 370),
                Location = new Point((this.ClientSize.Width - 340) / 2, 110),
                BackColor = UIHelper.CardBg
            };
            UIHelper.MakeRounded(cardPanel, 8);
            UIHelper.ApplyCardStyle(cardPanel);

            Label lblSub = new Label
            {
                Text = "Create Account",
                ForeColor = UIHelper.Charcoal,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(280, 40)
            };
            cardPanel.Controls.Add(lblSub);

            // Username
            Label lblUser = UIHelper.MakeLabel("Username");
            lblUser.Location = new Point(30, 70);
            cardPanel.Controls.Add(lblUser);

            txtUsername = new TextBox
            {
                Location = new Point(30, 90),
                Size = new Size(280, 30),
                PlaceholderText = "Choose a username"
            };
            UIHelper.ApplyInputStyle(txtUsername);
            cardPanel.Controls.Add(txtUsername);

            // Email
            Label lblEmail = UIHelper.MakeLabel("Email");
            lblEmail.Location = new Point(30, 130);
            cardPanel.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(30, 150),
                Size = new Size(280, 30),
                PlaceholderText = "your@email.com"
            };
            UIHelper.ApplyInputStyle(txtEmail);
            cardPanel.Controls.Add(txtEmail);

            // Password
            Label lblPass = UIHelper.MakeLabel("Password (min 6 characters)");
            lblPass.Location = new Point(30, 190);
            cardPanel.Controls.Add(lblPass);

            txtPassword = new TextBox
            {
                Location = new Point(30, 210),
                Size = new Size(280, 30),
                PasswordChar = '*',
                PlaceholderText = "••••••"
            };
            UIHelper.ApplyInputStyle(txtPassword);
            cardPanel.Controls.Add(txtPassword);

            // Register button
            btnRegister = new Button { Text = "CREATE ACCOUNT", Location = new Point(30, 260), Size = new Size(280, 45) };
            UIHelper.ApplyPrimaryBtn(btnRegister);
            btnRegister.Click += btnRegister_Click;
            cardPanel.Controls.Add(btnRegister);

            // Back button
            btnBack = new Button { Text = "←  Back to Login", Location = new Point(30, 320), Size = new Size(280, 30) };
            UIHelper.ApplyOutlineBtn(btnBack);
            btnBack.Click += (s, e) => this.Close();
            cardPanel.Controls.Add(btnBack);

            this.Controls.Add(cardPanel);
            this.Controls.Add(headerBar);
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and password are required.");
                return;
            }

            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.");
                return;
            }

            btnRegister.Enabled = false;
            btnRegister.Text = "CREATING...";

            try
            {
                var user = new User
                {
                    Username = username,
                    Email = email,
                    Role = "User"
                };

                bool success = await _authService.RegisterAsync(user, password);
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
            finally
            {
                btnRegister.Enabled = true;
                btnRegister.Text = "CREATE ACCOUNT";
            }
        }
    }
}
