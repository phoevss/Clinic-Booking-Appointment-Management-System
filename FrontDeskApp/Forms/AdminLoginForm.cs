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
        private readonly BookingDbContext _context;
        private readonly IAuthService _authService;
        private Panel cardPanel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;

        public AdminLoginForm()
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
            this.ClientSize = new Size(420, 520);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Clinic Management - Admin Login";
            this.MaximizeBox = false;

            // Clinic branding header area
            Panel headerBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = UIHelper.Teal
            };
            Label clinicName = new Label
            {
                Text = "✦ Clinic Management",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerBar.Controls.Add(clinicName);

            // Card panel for login form
            cardPanel = new Panel
            {
                Size = new Size(340, 320),
                Location = new Point((this.ClientSize.Width - 340) / 2, 110),
                BackColor = UIHelper.CardBg,
                BorderStyle = BorderStyle.None
            };
            UIHelper.MakeRounded(cardPanel, 8);
            UIHelper.ApplyCardStyle(cardPanel);

            // Subtitle
            Label lblSub = new Label
            {
                Text = "Admin Sign In",
                ForeColor = UIHelper.Charcoal,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(280, 40)
            };
            cardPanel.Controls.Add(lblSub);

            Label lblHint = new Label
            {
                Text = "Enter your credentials to access the admin panel",
                ForeColor = UIHelper.TextGray,
                Font = new Font("Segoe UI", 9),
                Location = new Point(30, 55),
                Size = new Size(280, 20)
            };
            cardPanel.Controls.Add(lblHint);

            // Username
            Label lblUser = UIHelper.MakeLabel("Username");
            lblUser.Location = new Point(30, 90);
            cardPanel.Controls.Add(lblUser);

            txtUsername = new TextBox
            {
                Location = new Point(30, 110),
                Size = new Size(280, 30),
                PlaceholderText = "admin"
            };
            UIHelper.ApplyInputStyle(txtUsername);
            cardPanel.Controls.Add(txtUsername);

            // Password
            Label lblPass = UIHelper.MakeLabel("Password");
            lblPass.Location = new Point(30, 155);
            cardPanel.Controls.Add(lblPass);

            txtPassword = new TextBox
            {
                Location = new Point(30, 175),
                Size = new Size(280, 30),
                PasswordChar = '*',
                PlaceholderText = "••••••"
            };
            UIHelper.ApplyInputStyle(txtPassword);
            cardPanel.Controls.Add(txtPassword);

            // Login button
            btnLogin = new Button
            {
                Text = "SIGN IN",
                Location = new Point(30, 230),
                Size = new Size(280, 45)
            };
            UIHelper.ApplyPrimaryBtn(btnLogin);
            btnLogin.Click += btnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            this.Controls.Add(cardPanel);
            this.Controls.Add(headerBar);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "SIGNING IN...";

                string user = txtUsername.Text;
                string pass = txtPassword.Text;

                var loggedInUser = await _authService.LoginAsync(user, pass);
                if (loggedInUser != null && loggedInUser.Role == "Admin")
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
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "SIGN IN";
            }
        }
    }
}
