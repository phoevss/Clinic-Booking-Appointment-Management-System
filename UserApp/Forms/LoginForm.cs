using System;
using System.Drawing;
using System.Windows.Forms;
using BookingData;
using BookingData.Services;
using BookingCore.Interfaces;
using UserApp.Helpers;

namespace UserApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly IAuthService _authService;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private LinkLabel lnkRegister;

        public LoginForm()
        {
            // Initialize dependencies (In a real app, use DI)
            var context = new BookingDbContext();
            _authService = new AuthService(context);
            
            InitializeComponent();
            ApplyStyles();
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.lnkRegister = new LinkLabel();
            
            this.SuspendLayout();
            
            // txtUsername
            this.txtUsername.Location = new Point(50, 80);
            this.txtUsername.Size = new Size(200, 25);
            this.txtUsername.PlaceholderText = "Username";
            
            // txtPassword
            this.txtPassword.Location = new Point(50, 120);
            this.txtPassword.Size = new Size(200, 25);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.PlaceholderText = "Password";
            
            // btnLogin
            this.btnLogin.Location = new Point(50, 160);
            this.btnLogin.Size = new Size(200, 40);
            this.btnLogin.Text = "LOGIN";
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            
            // lnkRegister
            this.lnkRegister.Location = new Point(50, 210);
            this.lnkRegister.Size = new Size(200, 20);
            this.lnkRegister.Text = "Don't have an account? Register";
            this.lnkRegister.TextAlign = ContentAlignment.MiddleCenter;
            this.lnkRegister.Click += new EventHandler(this.lnkRegister_Click);

            // LoginForm
            this.ClientSize = new Size(300, 300);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lnkRegister);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Appointment System - Login";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ApplyStyles()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            UIHelper.SetGradientBackground(this, Color.FromArgb(45, 45, 45), Color.FromArgb(20, 20, 20));
            
            btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            
            txtUsername.BackColor = Color.FromArgb(50, 50, 50);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            
            txtPassword.BackColor = Color.FromArgb(50, 50, 50);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            
            lnkRegister.LinkColor = Color.FromArgb(0, 190, 255);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter both username and password.");
                return;
            }

            var loggedInUser = await _authService.LoginAsync(user, pass);
            if (loggedInUser != null)
            {
                this.Hide();
                if (loggedInUser.Role == "Admin" || loggedInUser.Role == "SuperAdmin")
                {
                    // Admin UI is in a different project usually, 
                    // but for now let's assume we can launch it or show a message
                    MessageBox.Show("Welcome Admin! Please use the FrontDeskApp.");
                    // In a multi-project setup, you'd usually have one app or different executables.
                }
                else
                {
                    var dashboard = new UserDashboardForm(loggedInUser);
                    dashboard.Show();
                }
            }
            else
            {
                MessageBox.Show("Invalid credentials.");
            }
        }

        private void lnkRegister_Click(object sender, EventArgs e)
        {
            var regForm = new RegisterForm();
            regForm.ShowDialog();
        }
    }
}
