using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using BookingCore.Entities;
using BookingCore.Interfaces;
using BookingData;
using BookingData.Services;
using UserApp.Helpers;

namespace UserApp.Forms
{
    public partial class UserDashboardForm : Form
    {
        private readonly User _currentUser;
        private readonly IAppointmentService _appointmentService;
        private readonly IScheduleService _scheduleService;
        private readonly IServiceManager _serviceManager;
        
        private Panel sidebar;
        private Panel contentArea;
        private Label lblWelcome;
        private Button btnNewBooking;
        private Button btnMyBookings;
        private Button btnLogout;

        public UserDashboardForm(User user)
        {
            _currentUser = user;
            var context = new BookingDbContext();
            _appointmentService = new AppointmentService(context);
            _scheduleService = new ScheduleService(context);
            _serviceManager = new ServiceManager(context);

            InitializeComponent();
            ApplyStyles();
            ShowHome();
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
            this.sidebar.Width = 200;
            this.sidebar.BackColor = Color.FromArgb(40, 40, 40);
            
            // lblWelcome
            this.lblWelcome.Text = $"Welcome, {_currentUser.Username}";
            this.lblWelcome.ForeColor = Color.White;
            this.lblWelcome.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.lblWelcome.Location = new Point(10, 20);
            this.lblWelcome.Size = new Size(180, 40);
            this.sidebar.Controls.Add(this.lblWelcome);
            
            // btnNewBooking
            this.btnNewBooking.Text = "New Booking";
            this.btnNewBooking.Location = new Point(0, 80);
            this.btnNewBooking.Size = new Size(200, 45);
            this.btnNewBooking.Click += (s, e) => ShowBookingPanel();
            this.sidebar.Controls.Add(this.btnNewBooking);
            
            // btnMyBookings
            this.btnMyBookings.Text = "My History";
            this.btnMyBookings.Location = new Point(0, 130);
            this.btnMyBookings.Size = new Size(200, 45);
            this.btnMyBookings.Click += (s, e) => ShowHistoryPanel();
            this.sidebar.Controls.Add(this.btnMyBookings);
            
            // btnLogout
            this.btnLogout.Text = "Logout";
            this.btnLogout.Dock = DockStyle.Bottom;
            this.btnLogout.Height = 45;
            this.btnLogout.Click += (s, e) => { this.Close(); Application.OpenForms["LoginForm"]?.Show(); };
            this.sidebar.Controls.Add(this.btnLogout);
            
            // contentArea
            this.contentArea.Dock = DockStyle.Fill;
            this.contentArea.BackColor = Color.FromArgb(30, 30, 30);
            
            // UserDashboardForm
            this.ClientSize = new Size(1000, 600);
            this.Controls.Add(this.contentArea);
            this.Controls.Add(this.sidebar);
            this.Text = "User Dashboard - Appointment System";
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
                    btn.ForeColor = Color.LightGray;
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.Padding = new Padding(20, 0, 0, 0);
                    btn.Font = new Font("Segoe UI", 10);
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(60, 60, 60);
                    btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
                }
            }
        }

        private void ShowHome()
        {
            contentArea.Controls.Clear();
            Label lbl = new Label { 
                Text = "Please select an option from the sidebar to begin.", 
                ForeColor = Color.Gray, 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14)
            };
            contentArea.Controls.Add(lbl);
        }

        private async void ShowBookingPanel()
        {
            contentArea.Controls.Clear();
            // Simple booking panel
            Panel p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            // Logic to populate and book
            Label title = new Label { Text = "New Reservation", ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), Dock = DockStyle.Top, Height = 50 };
            p.Controls.Add(title);

            // Add controls in reverse order of how they should appear (due to DockStyle.Top)
            
            Button btnConfirm = new Button { Text = "CONFIRM BOOKING", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            p.Controls.Add(btnConfirm);

            ListBox lbSlots = new ListBox { Dock = DockStyle.Fill, BackColor = Color.White, ForeColor = Color.Black, Font = new Font("Segoe UI", 11) };
            p.Controls.Add(lbSlots);

            Label lblSlots = new Label { Text = "Available Time Slots:", ForeColor = Color.Gold, Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.BottomLeft };
            p.Controls.Add(lblSlots);

            Button btnCheckSlots = new Button { Text = "CHECK AVAILABILITY", Dock = DockStyle.Top, Height = 40, Margin = new Padding(0, 10, 0, 10), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            p.Controls.Add(btnCheckSlots);

            DateTimePicker dtp = new DateTimePicker { Dock = DockStyle.Top, Height = 30 };
            p.Controls.Add(dtp);
            p.Controls.Add(new Label { Text = "Select Date:", ForeColor = Color.LightGray, Dock = DockStyle.Top, Height = 25 });

            // Fetch services
            var services = await _serviceManager.GetAllServicesAsync();
            ComboBox cbServices = new ComboBox { Dock = DockStyle.Top, Height = 30, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var s in services) cbServices.Items.Add(s);
            cbServices.DisplayMember = "Name";
            p.Controls.Add(cbServices);
            p.Controls.Add(new Label { Text = "Select Service:", ForeColor = Color.LightGray, Dock = DockStyle.Top, Height = 25 });

            btnCheckSlots.Click += async (s, e) => {
                if (cbServices.SelectedItem is Service service)
                {
                    lbSlots.Items.Clear();
                    var slots = await _scheduleService.GetAvailableSchedulesAsync(service.Id, dtp.Value);
                    if (slots.Any()) {
                        // Sort slots by time so they appear chronologically (e.g. 8am, 9am...)
                        var sortedSlots = slots.OrderBy(s => s.StartTime);
                        foreach (var slot in sortedSlots) {
                            string timeDisplay = DateTime.Today.Add(slot.StartTime).ToString("hh:mm tt");
                            lbSlots.Items.Add(new { Display = timeDisplay, Value = slot });
                        }
                        lbSlots.DisplayMember = "Display";
                        lbSlots.ValueMember = "Value";
                    } else {
                        MessageBox.Show("No available slots for this date. Please try another day or ask Admin to add slots.");
                    }
                }
            };

            btnConfirm.Click += async (s, e) => {
                // Get the actual Schedule object from our formatted item
                var selectedItem = lbSlots.SelectedItem;
                if (selectedItem != null)
                {
                    var prop = selectedItem.GetType().GetProperty("Value");
                    var selectedSlot = (Schedule)prop.GetValue(selectedItem);

                    var appointment = new Appointment {
                        UserId = _currentUser.Id,
                        ServiceId = selectedSlot.ServiceId,
                        ScheduleId = selectedSlot.Id,
                        Status = "Pending",
                        Notes = "Booked via User App"
                    };
                    bool success = await _appointmentService.BookAppointmentAsync(appointment);
                    if (success) {
                        MessageBox.Show("Booking submitted! Awaiting admin approval.");
                        ShowHistoryPanel();
                    } else {
                        MessageBox.Show("Booking failed. Slot might have been taken.");
                    }
                }
                else
                {
                    MessageBox.Show("Please select a time slot first.");
                }
            };

            contentArea.Controls.Add(p);
        }

        private async void ShowHistoryPanel()
        {
            contentArea.Controls.Clear();
            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                ForeColor = Color.Black,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            var appointments = await _appointmentService.GetUserAppointmentsAsync(_currentUser.Id);
            dgv.DataSource = appointments.Select(a => new {
                Service = a.Service?.Name,
                Date = a.Schedule?.Date.ToShortDateString(),
                Time = DateTime.Today.Add(a.Schedule?.StartTime ?? TimeSpan.Zero).ToString("hh:mm tt"),
                Status = a.Status
            }).ToList();

            contentArea.Controls.Add(dgv);
        }
    }
}
