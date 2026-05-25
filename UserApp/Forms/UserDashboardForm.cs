using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using BookingCore.Entities;
using BookingCore.Interfaces;
using BookingData;
using BookingData.Services;
using Microsoft.EntityFrameworkCore;
using UserApp.Helpers;

namespace UserApp.Forms
{
    internal class ScheduleSlot
    {
        public Schedule Value { get; set; }
        public ScheduleSlot(Schedule schedule) { Value = schedule; }
        public override string ToString() => DateTime.Today.Add(Value.StartTime).ToString("hh:mm tt");
    }

    public partial class UserDashboardForm : Form
    {
        private readonly User _currentUser;
        private readonly BookingDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly IScheduleService _scheduleService;
        private readonly IServiceManager _serviceManager;

        private static readonly string _logPath = System.IO.Path.Combine(Application.StartupPath, "userapp_debug.log");
        private bool _isShowingBooking;
        private System.Windows.Forms.Timer _refreshTimer;
        private DataGridView _historyGrid;

        private Panel sidebar;
        private Panel contentArea;
        private Label lblWelcome;
        private Button btnNewBooking;
        private Button btnMyBookings;
        private Button btnLogout;

        public UserDashboardForm(User user)
        {
            _currentUser = user;
            _context = new BookingDbContext();
            _appointmentService = new AppointmentService(_context);
            _scheduleService = new ScheduleService(_context);
            _serviceManager = new ServiceManager(_context);

            InitializeComponent();
            ShowHome();

            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 5000;
            _refreshTimer.Tick += async (s, e) => await AutoRefreshHistory();
            _refreshTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
                _refreshTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.sidebar = new Panel();
            this.contentArea = new Panel();
            this.lblWelcome = new Label();
            this.btnNewBooking = new Button();
            this.btnMyBookings = new Button();
            this.btnLogout = new Button();

            this.SuspendLayout();

            // sidebar
            this.sidebar.Dock = DockStyle.Left;
            this.sidebar.Width = 230;
            this.sidebar.BackColor = UIHelper.TealDark;

            // lblWelcome
            this.lblWelcome.Text = $"👤  {_currentUser.Username}";
            this.lblWelcome.ForeColor = Color.White;
            this.lblWelcome.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.lblWelcome.Location = new Point(0, 15);
            this.lblWelcome.Size = new Size(230, 50);
            this.lblWelcome.TextAlign = ContentAlignment.MiddleCenter;
            this.sidebar.Controls.Add(this.lblWelcome);

            Label sep = new Label
            {
                Location = new Point(20, 65),
                Size = new Size(190, 1),
                BackColor = Color.FromArgb(0, 130, 115)
            };
            this.sidebar.Controls.Add(sep);

            // btnNewBooking
            UIHelper.ApplySidebarBtn(this.btnNewBooking, "  📅  New Booking");
            this.btnNewBooking.Location = new Point(0, 80);
            this.btnNewBooking.Width = 230;
            this.btnNewBooking.Click += async (s, e) => { try { await ShowBookingPanel(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            this.sidebar.Controls.Add(this.btnNewBooking);

            // btnMyBookings
            UIHelper.ApplySidebarBtn(this.btnMyBookings, "  📋  My History");
            this.btnMyBookings.Location = new Point(0, 130);
            this.btnMyBookings.Width = 230;
            this.btnMyBookings.Click += async (s, e) => { try { await ShowHistoryPanel(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            this.sidebar.Controls.Add(this.btnMyBookings);

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
            this.btnLogout.Click += (s, e) => { this.Close(); Application.OpenForms["LoginForm"]?.Show(); };
            this.sidebar.Controls.Add(this.btnLogout);

            // contentArea
            this.contentArea.Dock = DockStyle.Fill;
            this.contentArea.BackColor = UIHelper.BodyBg;
            this.contentArea.Padding = new Padding(25);

            // UserDashboardForm
            this.ClientSize = new Size(1050, 650);
            this.Controls.Add(this.contentArea);
            this.Controls.Add(this.sidebar);
            this.Text = "Patient Portal - Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        private Panel CreateContentPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                BackColor = UIHelper.CardBg
            };
        }

        private void ShowHome()
        {
            contentArea.Controls.Clear();
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140,
                BackColor = UIHelper.Teal
            };
            Label welcome = new Label
            {
                Text = $"Welcome, {_currentUser.Username}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                Location = new Point(25, 30),
                Size = new Size(600, 50)
            };
            header.Controls.Add(welcome);
            Label subtext = new Label
            {
                Text = "Book appointments and manage your visits",
                ForeColor = Color.FromArgb(200, 230, 225),
                Font = new Font("Segoe UI", 11),
                Location = new Point(25, 80),
                Size = new Size(600, 30)
            };
            header.Controls.Add(subtext);
            contentArea.Controls.Add(header);

            Label prompt = new Label
            {
                Text = "Select an option from the sidebar to begin",
                ForeColor = UIHelper.TextGray,
                Font = new Font("Segoe UI", 13),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            contentArea.Controls.Add(prompt);
        }

        private async Task ShowBookingPanel()
        {
            if (_isShowingBooking) return;
            _isShowingBooking = true;
            try
            {
                contentArea.Controls.Clear();
                Panel p = CreateContentPanel();
                p.Controls.Add(UIHelper.MakeTitle("New Reservation"));

                // Main form area
                Panel formArea = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(0, 10, 0, 0),
                    BackColor = UIHelper.BodyBg
                };

                // Service selection
                Label lblService = UIHelper.MakeLabel("Select Service", UIHelper.Charcoal);
                lblService.Location = new Point(15, 15);
                formArea.Controls.Add(lblService);

                ComboBox cbServices = new ComboBox
                {
                    Location = new Point(15, 35),
                    Width = 350,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                UIHelper.ApplyComboStyle(cbServices);
                var services = await _serviceManager.GetAllServicesAsync();
                cbServices.DisplayMember = "Name";
                foreach (var s in services) cbServices.Items.Add(s);
                if (cbServices.Items.Count > 0) cbServices.SelectedIndex = 0;
                formArea.Controls.Add(cbServices);

                // Date selection
                Label lblDate = UIHelper.MakeLabel("Select Date", UIHelper.Charcoal);
                lblDate.Location = new Point(15, 70);
                formArea.Controls.Add(lblDate);

                DateTimePicker dtp = new DateTimePicker
                {
                    Location = new Point(15, 90),
                    Width = 350,
                    Font = new Font("Segoe UI", 10)
                };
                formArea.Controls.Add(dtp);

                // Check availability button
                Button btnCheckSlots = new Button
                {
                    Text = "🔍  CHECK AVAILABILITY",
                    Location = new Point(15, 135),
                    Width = 350,
                    Height = 40
                };
                UIHelper.ApplyPrimaryBtn(btnCheckSlots);
                formArea.Controls.Add(btnCheckSlots);

                // Slots list
                Label lblSlots = new Label
                {
                    Text = "Available Time Slots",
                    ForeColor = UIHelper.Charcoal,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(15, 190),
                    Width = 350,
                    Height = 25
                };
                formArea.Controls.Add(lblSlots);

                ListBox lbSlots = new ListBox
                {
                    Location = new Point(15, 215),
                    Width = 350,
                    Height = 180,
                    BackColor = Color.White,
                    ForeColor = UIHelper.Charcoal,
                    Font = new Font("Segoe UI", 11),
                    BorderStyle = BorderStyle.FixedSingle
                };
                formArea.Controls.Add(lbSlots);

                // Confirm button
                Button btnConfirm = new Button
                {
                    Text = "✅  CONFIRM BOOKING",
                    Location = new Point(15, 410),
                    Width = 350,
                    Height = 45
                };
                UIHelper.ApplySuccessBtn(btnConfirm);
                formArea.Controls.Add(btnConfirm);

                p.Controls.Add(formArea);
                formArea.BringToFront();
                contentArea.Controls.Add(p);

                // Event handlers
                btnCheckSlots.Click += async (s, e) =>
                {
                    try
                    {
                        if (cbServices.SelectedItem is Service service)
                        {
                            lbSlots.Items.Clear();
                            var slots = await _scheduleService.GetAvailableSchedulesAsync(service.Id, dtp.Value);
                            if (slots.Any())
                            {
                                foreach (var slot in slots.OrderBy(s => s.StartTime))
                                    lbSlots.Items.Add(new ScheduleSlot(slot));
                                lblSlots.Text = $"Available Time Slots ({lbSlots.Items.Count} found)";
                            }
                            else
                            {
                                MessageBox.Show("No available slots for this date. Please try another day or ask Admin to add slots.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error checking availability: {ex.Message}");
                    }
                };

                btnConfirm.Click += async (s, e) =>
                {
                    if (lbSlots.SelectedItem is ScheduleSlot selectedSlot)
                    {
                        var appointment = new Appointment
                        {
                            UserId = _currentUser.Id,
                            ServiceId = selectedSlot.Value.ServiceId,
                            ScheduleId = selectedSlot.Value.Id,
                            Status = "Pending",
                            Notes = "Booked via Patient Portal"
                        };
                        bool success = await _appointmentService.BookAppointmentAsync(appointment);
                        if (success)
                        {
                            MessageBox.Show("Booking submitted! Awaiting admin approval.");
                            await ShowHistoryPanel();
                        }
                        else
                        {
                            MessageBox.Show("Booking failed. Slot might have been taken.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a time slot first.");
                    }
                };
            }
            finally
            {
                _isShowingBooking = false;
            }
        }

        private async Task ShowHistoryPanel()
        {
            contentArea.Controls.Clear();
            Panel p = CreateContentPanel();
            p.Controls.Add(UIHelper.MakeTitle("My Appointment History"));

            DataGridView dgv = new DataGridView();
            UIHelper.ApplyGridStyle(dgv);
            dgv.Dock = DockStyle.Fill;
            dgv.CellFormatting += (s, e) =>
            {
                if (dgv.Columns[e.ColumnIndex]?.Name == "Status" && e.Value != null)
                {
                    string status = e.Value.ToString();
                    e.CellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
                    e.CellStyle.ForeColor = status switch
                    {
                        "Approved" => UIHelper.Green,
                        "Rejected" => UIHelper.Red,
                        "Pending" => UIHelper.Amber,
                        _ => UIHelper.Charcoal
                    };
                }
            };

            var appointments = await _appointmentService.GetUserAppointmentsAsync(_currentUser.Id);
            var displayData = appointments.Select(a => new {
                a.Id,
                Service = a.Service?.Name,
                Date = a.Schedule?.Date.ToShortDateString(),
                Time = DateTime.Today.Add(a.Schedule?.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt"),
                Status = a.Status
            }).ToList();
            dgv.DataSource = displayData;
            if (dgv.Columns["Id"] != null)
                dgv.Columns["Id"].Visible = false;
            _historyGrid = dgv;
            p.Controls.Add(dgv);

            Panel actionBar = new Panel { Dock = DockStyle.Bottom, Height = 55, Padding = new Padding(5), BackColor = UIHelper.BodyBg };
            Button btnRefresh = new Button { Text = "🔄  REFRESH", Width = 120, Dock = DockStyle.Left, Height = 40 };
            UIHelper.ApplyPrimaryBtn(btnRefresh);
            btnRefresh.Click += async (s, e) => await ShowHistoryPanel();

            Button btnCancel = new Button { Text = "🗑  CANCEL SELECTED", Width = 160, Dock = DockStyle.Left, Height = 40, Margin = new Padding(10, 0, 0, 0) };
            UIHelper.ApplyDangerBtn(btnCancel);
            btnCancel.Click += async (s, e) =>
            {
                if (dgv.CurrentRow != null && dgv.Columns["Id"] != null)
                {
                    int appId = (int)dgv.CurrentRow.Cells["Id"].Value;
                    string status = dgv.CurrentRow.Cells["Status"].Value?.ToString();
                    if (status == "Pending")
                    {
                        var result = MessageBox.Show("Cancel this booking?", "Confirm", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            await _appointmentService.CancelAppointmentAsync(appId);
                            await ShowHistoryPanel();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Only pending bookings can be cancelled.");
                    }
                }
            };
            actionBar.Controls.Add(btnCancel);
            actionBar.Controls.Add(btnRefresh);
            p.Controls.Add(actionBar);
            dgv.BringToFront();
            contentArea.Controls.Add(p);
        }

        private async Task AutoRefreshHistory()
        {
            if (_historyGrid == null || _historyGrid.IsDisposed) return;
            if (!_historyGrid.Visible) return;
            try
            {
                var appointments = await _appointmentService.GetUserAppointmentsAsync(_currentUser.Id);
                var displayData = appointments.Select(a => new {
                    a.Id,
                    Service = a.Service?.Name,
                    Date = a.Schedule?.Date.ToShortDateString(),
                    Time = DateTime.Today.Add(a.Schedule?.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt"),
                    Status = a.Status
                }).ToList();
                _historyGrid.DataSource = displayData;
            }
            catch { }
        }
    }
}
