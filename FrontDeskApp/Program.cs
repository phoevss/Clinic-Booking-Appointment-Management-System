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

            // Seed database
            using (var context = new BookingData.BookingDbContext())
            {
                var auth = new BookingData.Services.AuthService(context);
                auth.SeedAdminAsync().GetAwaiter().GetResult();
            }

            Application.Run(new AdminLoginForm());
        }
    }
}