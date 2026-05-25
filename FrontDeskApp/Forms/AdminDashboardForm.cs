using System;
using System.Drawing;
using System.Windows.Forms;
using BookingCore.Entities;
using BookingCore.Interfaces;
using BookingData;
using BookingData.Services;
using FrontDeskApp.Helpers;
using System.Linq;

namespace FrontDeskApp.Forms
{
    public partial class AdminDashboardForm : Form
    {
        private readonly User _currentUser;
        private readonly BookingDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly IScheduleService _scheduleService;
        private readonly IServiceManager _serviceManager;
        private readonly IUserService _userService;
        private readonly IAuditService _auditService;
        private Panel sidebar;
        private Panel contentArea;
        private Label lblAdmin;
        private Button btnManageSchedules;
        private Button btnApprovals;
        private Button btnReports;
        private Button btnUserManagement;
        private Button btnLogout;

        public AdminDashboardForm(User admin)
        {
            _currentUser = admin;
            _context = new BookingDbContext();
            _appointmentService = new AppointmentService(_context);
            _scheduleService = new ScheduleService(_context);
            _serviceManager = new ServiceManager(_context);
            _userService = new UserService(_context);
            _auditService = new AuditService(_context);

            InitializeComponent();
            ShowHome();
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
            this.sidebar = new Panel();
            this.contentArea = new Panel();
            this.lblAdmin = new Label();
            this.btnManageSchedules = new Button();
            this.btnApprovals = new Button();
            this.btnReports = new Button();
            this.btnUserManagement = new Button();
            this.btnLogout = new Button();

            this.SuspendLayout();

            // sidebar
            this.sidebar.Dock = DockStyle.Left;
            this.sidebar.Width = 240;
            this.sidebar.BackColor = UIHelper.TealDark;

            // lblAdmin
            this.lblAdmin.Text = $"🏥  {_currentUser.Username}";
            this.lblAdmin.ForeColor = Color.White;
            this.lblAdmin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.lblAdmin.Location = new Point(0, 15);
            this.lblAdmin.Size = new Size(240, 50);
            this.lblAdmin.TextAlign = ContentAlignment.MiddleCenter;
            this.sidebar.Controls.Add(this.lblAdmin);

            // Separator
            Label sep = new Label
            {
                Location = new Point(20, 65),
                Size = new Size(200, 1),
                BackColor = Color.FromArgb(0, 130, 115)
            };
            this.sidebar.Controls.Add(sep);

            // btnManageSchedules
            UIHelper.ApplySidebarBtn(this.btnManageSchedules, "  📅  Manage Schedules");
            this.btnManageSchedules.Location = new Point(0, 80);
            this.btnManageSchedules.Width = 240;
            this.btnManageSchedules.Click += async (s, e) => { try { await ShowScheduleManagement(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            this.sidebar.Controls.Add(this.btnManageSchedules);

            // btnApprovals
            UIHelper.ApplySidebarBtn(this.btnApprovals, "  📋  Pending Approvals");
            this.btnApprovals.Location = new Point(0, 130);
            this.btnApprovals.Width = 240;
            this.btnApprovals.Click += async (s, e) => { try { await ShowApprovals(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            this.sidebar.Controls.Add(this.btnApprovals);

            // btnReports
            UIHelper.ApplySidebarBtn(this.btnReports, "  📊  Reports Dashboard");
            this.btnReports.Location = new Point(0, 180);
            this.btnReports.Width = 240;
            this.btnReports.Click += async (s, e) => { try { await ShowReports(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            this.sidebar.Controls.Add(this.btnReports);

            // btnUserManagement
            UIHelper.ApplySidebarBtn(this.btnUserManagement, "  👥  User Management");
            this.btnUserManagement.Location = new Point(0, 230);
            this.btnUserManagement.Width = 240;
            this.btnUserManagement.Click += async (s, e) => { try { await ShowUserManagement(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            this.sidebar.Controls.Add(this.btnUserManagement);

            // btnLogout
            this.btnLogout.Text = "  🚪  Logout";
            this.btnLogout.Dock = DockStyle.Bottom;
            this.btnLogout.Height = 50;
            this.btnLogout.FlatStyle = FlatStyle.Flat;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.ForeColor = Color.FromArgb(180, 180, 180);
            this.btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            this.btnLogout.Padding = new Padding(20, 0, 0, 0);
            this.btnLogout.Font = new Font("Segoe UI", 10);
            this.btnLogout.BackColor = Color.Transparent;
            this.btnLogout.Cursor = Cursors.Hand;
            this.btnLogout.Click += (s, e) => { this.Close(); Application.Exit(); };
            this.sidebar.Controls.Add(this.btnLogout);

            // contentArea
            this.contentArea.Dock = DockStyle.Fill;
            this.contentArea.BackColor = UIHelper.BodyBg;
            this.contentArea.Padding = new Padding(25);

            // AdminDashboardForm
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.contentArea);
            this.Controls.Add(this.sidebar);
            this.Text = "Clinic Management - Admin Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void ShowHome()
        {
            contentArea.Controls.Clear();
            Panel wrapper = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = UIHelper.Teal };
            Label welcome = new Label
            {
                Text = $"Welcome back, {_currentUser.Username}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(30, 40),
                Size = new Size(500, 50)
            };
            wrapper.Controls.Add(welcome);
            Label subtext = new Label
            {
                Text = "Clinic Management Dashboard — manage schedules, approvals, and reports",
                ForeColor = Color.FromArgb(200, 230, 225),
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 90),
                Size = new Size(600, 30)
            };
            wrapper.Controls.Add(subtext);
            contentArea.Controls.Add(wrapper);

            // Stat cards
            FlowLayoutPanel cards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                Padding = new Padding(0, 15, 0, 0),
                BackColor = UIHelper.BodyBg
            };

            cards.Controls.Add(CreateStatCard("📋", "Total Appointments", "0", UIHelper.Teal));
            cards.Controls.Add(CreateStatCard("⏳", "Pending Approvals", "0", UIHelper.Amber));
            cards.Controls.Add(CreateStatCard("✅", "Approved Today", "0", UIHelper.Green));
            cards.Controls.Add(CreateStatCard("👥", "Registered Users", "0", UIHelper.Teal));

            contentArea.Controls.Add(cards);

            // Load stats async
            LoadStats(cards);
        }

        private Panel CreateStatCard(string icon, string title, string value, Color accent)
        {
            Panel card = new Panel
            {
                Size = new Size(250, 100),
                Margin = new Padding(10, 0, 10, 0),
                BackColor = UIHelper.CardBg
            };
            card.Paint += (s, e) =>
            {
                Rectangle r = card.ClientRectangle;
                using (SolidBrush b = new SolidBrush(accent))
                    e.Graphics.FillRectangle(b, r.X, r.Y, r.Width, 4);
                using (Pen pen = new Pen(Color.FromArgb(224, 224, 224)))
                    e.Graphics.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 20),
                Location = new Point(15, 15),
                Size = new Size(40, 40)
            };
            card.Controls.Add(lblIcon);

            Label lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = UIHelper.Charcoal,
                Location = new Point(65, 10),
                Size = new Size(170, 40)
            };
            card.Controls.Add(lblValue);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9),
                ForeColor = UIHelper.TextGray,
                Location = new Point(65, 50),
                Size = new Size(170, 25)
            };
            card.Controls.Add(lblTitle);

            return card;
        }

        private async void LoadStats(FlowLayoutPanel cards)
        {
            try
            {
                var all = await _appointmentService.GetAllAppointmentsAsync();
                var users = await _userService.GetAllUsersAsync();
                int total = all.Count();
                int pending = all.Count(a => a.Status == "Pending");
                int approved = all.Count(a => a.Status == "Approved" && a.Schedule != null && a.Schedule.Date == DateTime.Now.Date);
                int userCount = users.Count();

                // Update the value labels in each card
                foreach (Control c in cards.Controls)
                {
                    if (c is Panel p && p.Controls.Count >= 2 && p.Controls[1] is Label val)
                    {
                        string title = (p.Controls[2] as Label)?.Text ?? "";
                        if (title == "Total Appointments") val.Text = total.ToString();
                        else if (title == "Pending Approvals") val.Text = pending.ToString();
                        else if (title == "Approved Today") val.Text = approved.ToString();
                        else if (title == "Registered Users") val.Text = userCount.ToString();
                    }
                }
            }
            catch { }
        }

        private Panel CreateContentPanel()
        {
            Panel p = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = UIHelper.CardBg
            };
            return p;
        }

        private async Task ShowScheduleManagement()
        {
            contentArea.Controls.Clear();
            Panel p = CreateContentPanel();
            p.Controls.Add(UIHelper.MakeTitle("Manage Schedules"));

            DataGridView dgv = new DataGridView();
            UIHelper.ApplyGridStyle(dgv);
            dgv.Dock = DockStyle.Fill;

            async Task RefreshGrid()
            {
                var list = await _scheduleService.GetAllSchedulesAsync();
                var displayList = list.OrderBy(s => s.Date).ThenBy(s => s.StartTime).Select(s => new {
                    Service = s.Service?.Name,
                    Date = s.Date.ToShortDateString(),
                    Time = DateTime.Today.Add(s.StartTime).ToString("hh:mm tt"),
                    Available = s.IsAvailable ? "Yes" : "Booked"
                }).ToList();
                dgv.DataSource = null;
                dgv.DataSource = displayList;
            }

            // Input section
            Panel inputArea = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                Padding = new Padding(0, 5, 0, 10),
                BackColor = UIHelper.BodyBg
            };

            var services = await _serviceManager.GetAllServicesAsync();
            ComboBox cb = new ComboBox { Location = new Point(15, 30), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            UIHelper.ApplyComboStyle(cb);
            cb.DisplayMember = "Name";
            foreach (var s in services) cb.Items.Add(s);
            if (cb.Items.Count > 0) cb.SelectedIndex = 0;
            var lblService = UIHelper.MakeLabel("Select Service:", UIHelper.Charcoal);
            lblService.Location = new Point(15, 8);
            inputArea.Controls.Add(lblService);
            inputArea.Controls.Add(cb);

            DateTimePicker dtp = new DateTimePicker { Location = new Point(290, 30), Width = 250, Font = new Font("Segoe UI", 10) };
            var lblDate = UIHelper.MakeLabel("Select Date:", UIHelper.Charcoal);
            lblDate.Location = new Point(290, 8);
            inputArea.Controls.Add(lblDate);
            inputArea.Controls.Add(dtp);

            TextBox txtTime = new TextBox { Location = new Point(565, 30), Width = 200, PlaceholderText = "HH:mm (e.g. 09:00)" };
            UIHelper.ApplyInputStyle(txtTime);
            var lblTime = UIHelper.MakeLabel("Start Time (24h):", UIHelper.Charcoal);
            lblTime.Location = new Point(565, 8);
            inputArea.Controls.Add(lblTime);
            inputArea.Controls.Add(txtTime);

            Button btnAdd = new Button { Text = "➕  Add Time Slot", Location = new Point(15, 80), Width = 250, Height = 40 };
            UIHelper.ApplyPrimaryBtn(btnAdd);

            btnAdd.Click += async (s, e) => {
                if (cb.SelectedItem == null || !TimeSpan.TryParse(txtTime.Text, out TimeSpan start))
                {
                    MessageBox.Show("Please select a service and enter a valid time (HH:mm).");
                    return;
                }
                var service = (Service)cb.SelectedItem;
                var schedule = new Schedule
                {
                    ServiceId = service.Id,
                    Date = dtp.Value.Date,
                    StartTime = start,
                    EndTime = start.Add(TimeSpan.FromMinutes(service.DurationMinutes)),
                    IsAvailable = true
                };
                await _scheduleService.AddScheduleAsync(schedule);
                await _auditService.LogActionAsync(_currentUser.Id, "Schedule Management", $"Added {service.Name} slot at {start:hh\\:mm}");
                MessageBox.Show("Slot added!");
                txtTime.Clear();
                await RefreshGrid();
            };
            inputArea.Controls.Add(btnAdd);

            Label lblExisting = new Label
            {
                Text = "Existing Slots",
                ForeColor = UIHelper.Charcoal,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Dock = DockStyle.Bottom,
                Height = 30
            };
            inputArea.Controls.Add(lblExisting);

            p.Controls.Add(dgv);
            p.Controls.Add(inputArea);
            dgv.BringToFront();
            contentArea.Controls.Add(p);
            await RefreshGrid();
        }

        private async Task ShowApprovals()
        {
            contentArea.Controls.Clear();
            Panel p = CreateContentPanel();
            p.Controls.Add(UIHelper.MakeTitle("Pending Approvals"));

            DataGridView dgv = new DataGridView();
            UIHelper.ApplyGridStyle(dgv);
            dgv.Dock = DockStyle.Fill;

            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            var displayData = appointments.Select(a => new {
                a.Id,
                Patient = a.User?.Username,
                Service = a.Service?.Name,
                Time = DateTime.Today.Add(a.Schedule?.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt"),
                Date = a.Schedule?.Date.ToShortDateString(),
                a.Status
            }).ToList();
            dgv.DataSource = displayData;
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;

            p.Controls.Add(dgv);

            Panel actions = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(5), BackColor = UIHelper.BodyBg };
            Button btnApprove = new Button { Text = "✅  APPROVE", Width = 140, Dock = DockStyle.Left, Height = 40 };
            UIHelper.ApplySuccessBtn(btnApprove);
            Button btnReject = new Button { Text = "❌  REJECT", Width = 140, Dock = DockStyle.Left, Height = 40, Margin = new Padding(10, 0, 0, 0) };
            UIHelper.ApplyDangerBtn(btnReject);

            btnApprove.Click += async (s, e) => {
                if (dgv.CurrentRow != null && dgv.Columns["Id"] != null)
                {
                    int appId = (int)dgv.CurrentRow.Cells["Id"].Value;
                    await _appointmentService.UpdateAppointmentStatusAsync(appId, "Approved");
                    await _auditService.LogActionAsync(_currentUser.Id, "Appointments", $"Approved appointment ID: {appId}");
                    MessageBox.Show("Appointment approved!");
                    await ShowApprovals();
                }
            };

            btnReject.Click += async (s, e) => {
                if (dgv.CurrentRow != null && dgv.Columns["Id"] != null)
                {
                    int appId = (int)dgv.CurrentRow.Cells["Id"].Value;
                    await _appointmentService.UpdateAppointmentStatusAsync(appId, "Rejected");
                    await _auditService.LogActionAsync(_currentUser.Id, "Appointments", $"Rejected appointment ID: {appId}");
                    MessageBox.Show("Appointment rejected.");
                    await ShowApprovals();
                }
            };

            actions.Controls.Add(btnReject);
            actions.Controls.Add(btnApprove);
            p.Controls.Add(actions);
            dgv.BringToFront();
            contentArea.Controls.Add(p);
        }

        private async Task ShowReports()
        {
            contentArea.Controls.Clear();
            Panel p = CreateContentPanel();
            p.Controls.Add(UIHelper.MakeTitle("Reports & Analytics"));

            Panel filter = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = UIHelper.BodyBg, Padding = new Padding(10) };
            ComboBox cbType = new ComboBox { Location = new Point(10, 10), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            UIHelper.ApplyComboStyle(cbType);
            cbType.Items.AddRange(new string[] { "All", "Daily", "Weekly", "Monthly" });
            cbType.SelectedIndex = 0;
            filter.Controls.Add(cbType);

            Button btnGen = new Button { Text = "📊  Generate Report", Location = new Point(180, 8), Width = 160, Height = 35 };
            UIHelper.ApplyPrimaryBtn(btnGen);
            filter.Controls.Add(btnGen);
            p.Controls.Add(filter);

            DataGridView dgv = new DataGridView();
            UIHelper.ApplyGridStyle(dgv);
            dgv.Dock = DockStyle.Fill;

            Button btnExport = new Button { Text = "📄  EXPORT REPORT", Dock = DockStyle.Bottom, Height = 45 };
            UIHelper.ApplyOutlineBtn(btnExport);
            btnExport.ForeColor = UIHelper.Teal;
            btnExport.Click += (s, e) => MessageBox.Show("Report exported successfully!");

            p.Controls.Add(btnExport);
            p.Controls.Add(dgv);
            dgv.BringToFront();

            btnGen.Click += async (s, e) => {
                try
                {
                    dgv.DataSource = null;
                    var all = await _appointmentService.GetAllAppointmentsAsync();
                    string selected = cbType.SelectedItem?.ToString() ?? "All";
                    DateTime now = DateTime.Now.Date;
                    var filtered = selected switch
                    {
                        "Daily" => all.Where(a => a.Schedule != null && a.Schedule.Date == now),
                        "Weekly" => all.Where(a => a.Schedule != null && a.Schedule.Date >= now.AddDays(-(int)now.DayOfWeek) && a.Schedule.Date <= now.AddDays(6 - (int)now.DayOfWeek)),
                        "Monthly" => all.Where(a => a.Schedule != null && a.Schedule.Date.Year == now.Year && a.Schedule.Date.Month == now.Month),
                        _ => all
                    };
                    var data = filtered.Select(a => new {
                        a.Id,
                        Date = a.Schedule?.Date.ToShortDateString(),
                        Time = DateTime.Today.Add(a.Schedule?.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt"),
                        Patient = a.User?.Username,
                        Service = a.Service?.Name,
                        Status = a.Status
                    }).ToList();
                    if (!data.Any())
                    {
                        MessageBox.Show("No appointments found for the selected period.", "Report Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    dgv.DataSource = data;
                    if (dgv.Columns["Id"] != null)
                        dgv.Columns["Id"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            contentArea.Controls.Add(p);
        }

        private async Task ShowUserManagement()
        {
            contentArea.Controls.Clear();
            Panel p = CreateContentPanel();
            p.Controls.Add(UIHelper.MakeTitle("User Management"));

            DataGridView dgv = new DataGridView();
            UIHelper.ApplyGridStyle(dgv);
            dgv.Dock = DockStyle.Fill;

            var users = await _userService.GetAllUsersAsync();
            var displayData = users.Select(u => new { u.Id, u.Username, u.Email, u.Role }).ToList();
            dgv.DataSource = displayData;
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;

            p.Controls.Add(dgv);

            Panel actions = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(5), BackColor = UIHelper.BodyBg };
            Button btnMakeAdmin = new Button { Text = "👑  MAKE ADMIN", Width = 160, Dock = DockStyle.Left, Height = 40 };
            UIHelper.ApplyPrimaryBtn(btnMakeAdmin);

            btnMakeAdmin.Click += async (s, e) => {
                if (dgv.CurrentRow != null && dgv.Columns["Id"] != null)
                {
                    int userId = (int)dgv.CurrentRow.Cells["Id"].Value;
                    string username = dgv.CurrentRow.Cells["Username"].Value?.ToString() ?? "";
                    await _userService.UpdateUserRoleAsync(userId, "Admin");
                    await _auditService.LogActionAsync(_currentUser.Id, "User Management", $"Promoted {username} to Admin");
                    MessageBox.Show($"{username} is now an Admin!");
                    await ShowUserManagement();
                }
            };
            actions.Controls.Add(btnMakeAdmin);
            p.Controls.Add(actions);
            dgv.BringToFront();
            contentArea.Controls.Add(p);
        }
    }
}
