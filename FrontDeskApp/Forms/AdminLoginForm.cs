using System;
using System.Drawing;
using System.Windows.Forms;
using BookingData;
using BookingData.Services;
using BookingCore.Interfaces;
using FrontDeskApp.Helpers;

namespace FrontDeskApp.Forms
{
    public partial class AdminLoginForm : Form
    {
        private readonly IAuthService _authService;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;

        public AdminLoginForm()
        {
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
            
            this.SuspendLayout();
            
            // txtUsername
            this.txtUsername.Location = new Point(50, 80);
            this.txtUsername.Size = new Size(200, 25);
            this.txtUsername.PlaceholderText = "Admin Username";
            
            // txtPassword
            this.txtPassword.Location = new Point(50, 120);
            this.txtPassword.Size = new Size(200, 25);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.PlaceholderText = "Password";
            
            // btnLogin
            this.btnLogin.Location = new Point(50, 170);
            this.btnLogin.Size = new Size(200, 40);
            this.btnLogin.Text = "ADMIN LOGIN";
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // AdminLoginForm
            this.ClientSize = new Size(300, 300);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "FrontDesk - Admin Login";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ApplyStyles()
        {
            this.BackColor = Color.FromArgb(45, 45, 48);
            UIHelper.SetGradientBackground(this, Color.FromArgb(50, 50, 50), Color.FromArgb(30, 30, 30));
            
            btnLogin.BackColor = Color.FromArgb(0, 122, 204);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            
            txtUsername.BackColor = Color.FromArgb(60, 60, 60);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            
            txtPassword.BackColor = Color.FromArgb(60, 60, 60);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text;
            string pass = txtPassword.Text;

            var loggedInUser = await _authService.LoginAsync(user, pass);
            if (loggedInUser != null && (loggedInUser.Role == "Admin" || loggedInUser.Role == "SuperAdmin"))
            {
                this.Hide();
                var dashboard = new AdminDashboardForm(loggedInUser);
                dashboard.Show();
            }
            else
            {
                MessageBox.Show("Access Denied: Invalid credentials or insufficient permissions.");
            }
        }
    }
}
