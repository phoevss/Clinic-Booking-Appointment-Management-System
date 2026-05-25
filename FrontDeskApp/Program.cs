using System;
using System.Windows.Forms;
using FrontDeskApp.Forms;

namespace FrontDeskApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Seed database
                using (var context = new BookingData.BookingDbContext())
                {
                    var auth = new BookingData.Services.AuthService(context);
                    auth.SeedAdminAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                var logPath = System.IO.Path.Combine(Application.StartupPath, "seed_error.log");
                System.IO.File.WriteAllText(logPath, $"{DateTime.Now}: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Database seed error: {ex.Message}\n\nDetails written to seed_error.log");
            }

            Application.Run(new AdminLoginForm());
        }
    }
}