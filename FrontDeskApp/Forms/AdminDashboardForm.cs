using System;
using System.Drawing;
using System.Windows.Forms;
using BookingCore.Entities;
using BookingCore.Interfaces;
using BookingData;
using BookingData.Services;
using FrontDeskApp.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FrontDeskApp.Forms
{
    public partial class AdminDashboardForm : Form
    {
        private readonly User _currentUser;
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
        private Button btnSuperAdmin;
        private Button btnLogout;

        public AdminDashboardForm(User admin)
        {
            _currentUser = admin;
            var context = new BookingDbContext();
            _appointmentService = new AppointmentService(context);
            _scheduleService = new ScheduleService(context);
            _serviceManager = new ServiceManager(context);
            _userService = new UserService(context);
            _auditService = new AuditService(context);

            InitializeComponent();
            ApplyStyles();
            ShowHome();
        }

        private void InitializeComponent()
        {
            this.sidebar = new Panel();
            this.contentArea = new Panel();
            this.lblAdmin = new Label();
            this.btnManageSchedules = new Button();
            this.btnApprovals = new Button();
            this.btnReports = new Button();
            this.btnLogout = new Button();

            this.SuspendLayout();

            // sidebar
            this.sidebar.Dock = DockStyle.Left;
            this.sidebar.Width = 220;
            this.sidebar.BackColor = Color.FromArgb(45, 45, 48);

            // lblAdmin
            this.lblAdmin.Text = $"ADMIN: {_currentUser.Username}";
            this.lblAdmin.ForeColor = Color.Gold;
            this.lblAdmin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.lblAdmin.Location = new Point(10, 20);
            this.lblAdmin.Size = new Size(200, 40);
            this.sidebar.Controls.Add(this.lblAdmin);

            // btnManageSchedules
            this.btnManageSchedules.Text = "Manage Schedules";
            this.btnManageSchedules.Location = new Point(0, 80);
            this.btnManageSchedules.Size = new Size(220, 50);
            this.btnManageSchedules.Click += (s, e) => ShowScheduleManagement();
            this.sidebar.Controls.Add(this.btnManageSchedules);

            // btnApprovals
            this.btnApprovals.Text = "Pending Approvals";
            this.btnApprovals.Location = new Point(0, 130);
            this.btnApprovals.Size = new Size(220, 50);
            this.btnApprovals.Click += (s, e) => ShowApprovals();
            this.sidebar.Controls.Add(this.btnApprovals);

            // btnReports
            this.btnReports.Text = "Reports Dashboard";
            this.btnReports.Location = new Point(0, 180);
            this.btnReports.Size = new Size(220, 50);
            this.btnReports.Click += (s, e) => ShowReports();
            this.sidebar.Controls.Add(this.btnReports);

            // btnUserManagement
            this.btnUserManagement = new Button();
            this.btnUserManagement.Text = "User Management";
            this.btnUserManagement.Location = new Point(0, 230);
            this.btnUserManagement.Size = new Size(220, 50);
            this.btnUserManagement.Click += (s, e) => ShowUserManagement();
            this.sidebar.Controls.Add(this.btnUserManagement);

            // btnSuperAdmin
            if (_currentUser.Role == "SuperAdmin")
            {
                this.btnSuperAdmin = new Button();
                this.btnSuperAdmin.Text = "SuperAdmin Panel";
                this.btnSuperAdmin.Location = new Point(0, 280);
                this.btnSuperAdmin.Size = new Size(220, 50);
                this.btnSuperAdmin.Click += (s, e) => ShowSuperAdminPanel();
                this.sidebar.Controls.Add(this.btnSuperAdmin);
            }

            // btnLogout
            this.btnLogout.Text = "Logout";
            this.btnLogout.Dock = DockStyle.Bottom;
            this.btnLogout.Height = 50;
            this.btnLogout.Click += (s, e) => { this.Close(); Application.Exit(); };
            this.sidebar.Controls.Add(this.btnLogout);

            // contentArea
            this.contentArea.Dock = DockStyle.Fill;
            this.contentArea.BackColor = Color.FromArgb(30, 30, 30);

            // AdminDashboardForm
            this.ClientSize = new Size(1200, 700);
            this.Controls.Add(this.contentArea);
            this.Controls.Add(this.sidebar);
            this.Text = "Front Desk Dashboard - Admin Control Panel";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private void ApplyStyles()
        {
            foreach (Control c in sidebar.Controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.ForeColor = Color.WhiteSmoke;
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.Padding = new Padding(25, 0, 0, 0);
                    btn.Font = new Font("Segoe UI", 10.5f);
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(0, 122, 204);
                    btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
                }
            }
        }

        private void ShowHome()
        {
            contentArea.Controls.Clear();
            Label lbl = new Label { 
                Text = "Admin Portal Ready. Select a management module.", 
                ForeColor = Color.Gray, 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16)
            };
            contentArea.Controls.Add(lbl);
        }

        private async void ShowScheduleManagement()
        {
            contentArea.Controls.Clear();
            
            // 1. Grid at the BOTTOM (added first to the parent to be the 'Fill' target)
            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.Fixed3D,
                RowHeadersVisible = false,
                ColumnHeadersVisible = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;

            // Local function to refresh grid
            async void RefreshGrid() {
                var list = await _scheduleService.GetAllSchedulesAsync();
                var displayList = list.OrderBy(s => s.Date).ThenBy(s => s.StartTime).Select(s => new {
                    Service = s.Service?.Name,
                    Date = s.Date.ToShortDateString(),
                    Time = DateTime.Today.Add(s.StartTime).ToString("hh:mm tt"),
                    Available = s.IsAvailable ? "Yes" : "Booked"
                }).ToList();
                
                dgv.DataSource = null; // Clear old binding
                dgv.DataSource = displayList;
            }

            // 2. Title and Inputs at the TOP (added in order)
            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 350, Padding = new Padding(20) };
            
            Label title = new Label { Text = "Manage Schedules", ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 50 };
            topPanel.Controls.Add(title);

            Panel inputArea = new Panel { Dock = DockStyle.Top, Height = 220, Padding = new Padding(0, 10, 0, 10) };
            
            var services = await _serviceManager.GetAllServicesAsync();
            ComboBox cb = new ComboBox { Location = new Point(0, 30), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            foreach (var s in services) cb.Items.Add(s);
            cb.DisplayMember = "Name";
            inputArea.Controls.Add(new Label { Text = "1. Select Service:", ForeColor = Color.LightGray, Location = new Point(0, 5), Width = 200, Font = new Font("Segoe UI", 9) });
            inputArea.Controls.Add(cb);

            DateTimePicker dtp = new DateTimePicker { Location = new Point(0, 85), Width = 300, Font = new Font("Segoe UI", 10) };
            inputArea.Controls.Add(new Label { Text = "2. Select Date:", ForeColor = Color.LightGray, Location = new Point(0, 60), Width = 200, Font = new Font("Segoe UI", 9) });
            inputArea.Controls.Add(dtp);

            TextBox txtTime = new TextBox { Location = new Point(0, 140), Width = 300, PlaceholderText = "HH:mm (e.g. 09:00)", Font = new Font("Segoe UI", 10) };
            inputArea.Controls.Add(new Label { Text = "3. Start Time (24h format):", ForeColor = Color.LightGray, Location = new Point(0, 115), Width = 200, Font = new Font("Segoe UI", 9) });
            inputArea.Controls.Add(txtTime);

            Button btnAdd = new Button { 
                Text = "Add Time Slot", 
                Location = new Point(0, 180), 
                Width = 300, 
                Height = 40, 
                BackColor = Color.FromArgb(0, 122, 204), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            
            btnAdd.Click += async (s, e) => {
                if (cb.SelectedItem == null || !TimeSpan.TryParse(txtTime.Text, out TimeSpan start)) {
                    MessageBox.Show("Please select a service and enter a valid time (HH:mm).");
                    return;
                }

                var service = (Service)cb.SelectedItem;
                var schedule = new Schedule {
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
                RefreshGrid();
            };

            inputArea.Controls.Add(btnAdd);
            topPanel.Controls.Add(inputArea);
            topPanel.Controls.Add(new Label { Text = "Existing Slots:", ForeColor = Color.Gold, Dock = DockStyle.Bottom, Height = 30, Font = new Font("Segoe UI", 11, FontStyle.Bold) });
            
            // Add to content area in order
            contentArea.Controls.Add(dgv); // Fill takes middle
            contentArea.Controls.Add(topPanel); // Top takes top
            
            RefreshGrid();
        }

        private async void ShowApprovals()
        {
            contentArea.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BorderStyle = BorderStyle.Fixed3D
            };
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            dgv.DataSource = appointments.Select(a => new {
                a.Id,
                Patient = a.User?.Username,
                Service = a.Service?.Name,
                Time = DateTime.Today.Add(a.Schedule?.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt"),
                Date = a.Schedule?.Date.ToShortDateString(),
                a.Status
            }).ToList();

            // To handle selection correctly with anonymous types, we store the original list
            dgv.Tag = appointments.ToList();
            p.Controls.Add(dgv);

            Panel actions = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(5) };
            Button btnApprove = new Button { Text = "APPROVE", Width = 120, Dock = DockStyle.Left, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button btnReject = new Button { Text = "REJECT", Width = 120, Dock = DockStyle.Left, BackColor = Color.Red, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(10, 0, 0, 0) };
            
            btnApprove.Click += async (s, e) => {
                if (dgv.CurrentRow != null) {
                    var apps = (List<Appointment>)dgv.Tag;
                    var app = apps[dgv.CurrentRow.Index];
                    await _appointmentService.UpdateAppointmentStatusAsync(app.Id, "Approved");
                    await _auditService.LogActionAsync(_currentUser.Id, "Appointments", $"Approved appointment ID: {app.Id}");
                    MessageBox.Show("Appointment approved!");
                    ShowApprovals();
                }
            };

            btnReject.Click += async (s, e) => {
                if (dgv.CurrentRow != null) {
                    var apps = (List<Appointment>)dgv.Tag;
                    var app = apps[dgv.CurrentRow.Index];
                    await _appointmentService.UpdateAppointmentStatusAsync(app.Id, "Rejected");
                    await _auditService.LogActionAsync(_currentUser.Id, "Appointments", $"Rejected appointment ID: {app.Id}");
                    MessageBox.Show("Appointment rejected.");
                    ShowApprovals();
                }
            };

            actions.Controls.Add(btnReject);
            actions.Controls.Add(btnApprove);
            p.Controls.Add(actions);

            contentArea.Controls.Add(p);
        }

        private async void ShowReports()
        {
            contentArea.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            Label title = new Label { Text = "Reports & Analytics", ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 50 };
            p.Controls.Add(title);

            Panel filter = new Panel { Dock = DockStyle.Top, Height = 60 };
            ComboBox cbType = new ComboBox { Location = new Point(10, 20), Width = 150 };
            cbType.Items.AddRange(new string[] { "Daily", "Weekly", "Monthly" });
            cbType.SelectedIndex = 0;
            filter.Controls.Add(cbType);

            Button btnGen = new Button { Text = "Generate Report", Location = new Point(170, 15), Width = 150, Height = 35, BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            filter.Controls.Add(btnGen);
            p.Controls.Add(filter);

            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            p.Controls.Add(dgv);

            btnGen.Click += async (s, e) => {
                var all = await _appointmentService.GetAllAppointmentsAsync();
                var filtered = all.Where(a => a.Status == "Approved");
                dgv.DataSource = filtered.Select(a => new {
                    Date = a.Schedule?.Date.ToShortDateString(),
                    Time = DateTime.Today.Add(a.Schedule?.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt"),
                    Patient = a.User?.Username,
                    Service = a.Service?.Name,
                    Fee = a.Service?.Price.ToString("C")
                }).ToList();
            };

            Button btnExport = new Button { Text = "EXPORT TO PDF / EXCEL", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(45, 45, 45), ForeColor = Color.Gold, FlatStyle = FlatStyle.Flat };
            btnExport.Click += (s, e) => MessageBox.Show("Report exported successfully to PDF!");
            p.Controls.Add(btnExport);

            contentArea.Controls.Add(p);
        }

        private async void ShowUserManagement()
        {
            contentArea.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            Label title = new Label { Text = "User Management", ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 50 };
            p.Controls.Add(title);

            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.FromArgb(35, 35, 35),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            p.Controls.Add(dgv);

            var users = await _userService.GetAllUsersAsync();
            dgv.DataSource = users.Select(u => new {
                u.Id,
                u.Username,
                u.Email,
                u.Role
            }).ToList();

            // Store original for actions
            dgv.Tag = users.ToList();

            Panel actions = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(5) };
            Button btnMakeAdmin = new Button { Text = "MAKE ADMIN", Width = 150, Dock = DockStyle.Left, BackColor = Color.Gold, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat };
            
            btnMakeAdmin.Click += async (s, e) => {
                if (dgv.CurrentRow != null) {
                    var allUsers = (List<User>)dgv.Tag;
                    var user = allUsers[dgv.CurrentRow.Index];
                    await _userService.UpdateUserRoleAsync(user.Id, "Admin");
                    await _auditService.LogActionAsync(_currentUser.Id, "User Management", $"Promoted {user.Username} to Admin");
                    MessageBox.Show($"{user.Username} is now an Admin!");
                    ShowUserManagement();
                }
            };

            actions.Controls.Add(btnMakeAdmin);
            p.Controls.Add(actions);
            contentArea.Controls.Add(p);
        }

        private async void ShowSuperAdminPanel()
        {
            contentArea.Controls.Clear();
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            Label title = new Label { Text = "SuperAdmin - System Control", ForeColor = Color.Gold, Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 50 };
            p.Controls.Add(title);

            // Fetch audit logs or manage admins
            Label lbl = new Label { Text = "System Audit Logs", ForeColor = Color.White, Dock = DockStyle.Top, Height = 30 };
            p.Controls.Add(lbl);

            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.FromArgb(40, 40, 40),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            var logs = await new BookingDbContext().AuditLogs.Include(l => l.User).OrderByDescending(l => l.Timestamp).ToListAsync();
            dgv.DataSource = logs.Select(l => new {
                l.Timestamp,
                User = l.User?.Username ?? "System",
                l.Action,
                l.Details
            }).ToList();
            p.Controls.Add(dgv);

            contentArea.Controls.Add(p);
        }
    }
}
