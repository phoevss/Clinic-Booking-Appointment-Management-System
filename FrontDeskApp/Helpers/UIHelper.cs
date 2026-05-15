using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FrontDeskApp.Helpers
{
    public static class UIHelper
    {
        public static void SetGradientBackground(Control control, Color startColor, Color endColor)
        {
            control.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(control.ClientRectangle, startColor, endColor, 45F))
                {
                    e.Graphics.FillRectangle(brush, control.ClientRectangle);
                }
            };
        }

        public static void MakeRounded(Control control, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }
    }
}
