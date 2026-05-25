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
        private readonly BookingDbContext _context;
        private readonly IAuthService _authService;
        private Panel cardPanel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private LinkLabel lnkRegister;

        public LoginForm()
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
            this.Text = "Clinic Portal - Patient Login";
            this.MaximizeBox = false;

            // Header bar
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

            // Card panel
            cardPanel = new Panel
            {
                Size = new Size(340, 340),
                Location = new Point((this.ClientSize.Width - 340) / 2, 110),
                BackColor = UIHelper.CardBg
            };
            UIHelper.MakeRounded(cardPanel, 8);
            UIHelper.ApplyCardStyle(cardPanel);

            Label lblSub = new Label
            {
                Text = "Patient Sign In",
                ForeColor = UIHelper.Charcoal,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(30, 20),
                Size = new Size(280, 40)
            };
            cardPanel.Controls.Add(lblSub);

            Label lblHint = new Label
            {
                Text = "Enter your credentials to book appointments",
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
                PlaceholderText = "Enter your username"
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
            btnLogin = new Button { Text = "SIGN IN", Location = new Point(30, 225), Size = new Size(280, 45) };
            UIHelper.ApplyPrimaryBtn(btnLogin);
            btnLogin.Click += btnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            // Register link
            lnkRegister = new LinkLabel
            {
                Location = new Point(30, 285),
                Size = new Size(280, 25),
                Text = "Don't have an account? Register",
                TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = UIHelper.Teal,
                ActiveLinkColor = UIHelper.TealDark,
                Font = new Font("Segoe UI", 9)
            };
            lnkRegister.Click += lnkRegister_Click;
            cardPanel.Controls.Add(lnkRegister);

            this.Controls.Add(cardPanel);
            this.Controls.Add(headerBar);
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

            btnLogin.Enabled = false;
            btnLogin.Text = "SIGNING IN...";

            try
            {
                var loggedInUser = await _authService.LoginAsync(user, pass);
                if (loggedInUser != null)
                {
                    this.Hide();
                    if (loggedInUser.Role == "Admin")
                    {
                        MessageBox.Show("Please use the FrontDeskApp for admin access.");
                        this.Close();
                        return;
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
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "SIGN IN";
            }
        }

        private void lnkRegister_Click(object sender, EventArgs e)
        {
            var regForm = new RegisterForm();
            regForm.ShowDialog();
        }
    }
}
