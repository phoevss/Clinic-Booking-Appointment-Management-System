using System;
using System.Windows.Forms;
using UserApp.Forms;

namespace UserApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Seed admin synchronously
                using (var context = new BookingData.BookingDbContext())
                {
                    var auth = new BookingData.Services.AuthService(context);
                    auth.SeedAdminAsync().GetAwaiter().GetResult();
                }

                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("error.log", ex.ToString());
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}