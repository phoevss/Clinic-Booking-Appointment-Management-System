using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FrontDeskApp.Helpers
{
    public static class UIHelper
    {
        public static readonly Color Teal = Color.FromArgb(0, 105, 92);
        public static readonly Color TealDark = Color.FromArgb(0, 77, 64);
        public static readonly Color TealLight = Color.FromArgb(178, 223, 219);
        public static readonly Color Amber = Color.FromArgb(255, 179, 0);
        public static readonly Color Charcoal = Color.FromArgb(38, 50, 56);
        public static readonly Color TextGray = Color.FromArgb(120, 144, 156);
        public static readonly Color CardBg = Color.White;
        public static readonly Color BodyBg = Color.FromArgb(245, 245, 245);
        public static readonly Color Green = Color.FromArgb(67, 160, 71);
        public static readonly Color Red = Color.FromArgb(229, 57, 53);

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

        public static void ApplyCardStyle(Panel panel)
        {
            panel.BackColor = CardBg;
            panel.BorderStyle = BorderStyle.None;
            panel.Paint += (s, e) =>
            {
                Control p = (Control)s;
                Rectangle r = p.ClientRectangle;
                using (Pen pen = new Pen(Color.FromArgb(224, 224, 224)))
                    e.Graphics.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
                    e.Graphics.FillRectangle(brush, r.X, r.Y, r.Width - 1, 3);
            };
        }

        public static void ApplyPrimaryBtn(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Teal;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = TealDark;
            btn.MouseLeave += (s, e) => btn.BackColor = Teal;
        }

        public static void ApplyDangerBtn(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Red;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(198, 40, 40);
            btn.MouseLeave += (s, e) => btn.BackColor = Red;
        }

        public static void ApplySuccessBtn(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Green;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        public static void ApplyOutlineBtn(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Teal;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            btn.FlatAppearance.BorderColor = Teal;
            btn.Cursor = Cursors.Hand;
        }

        public static void ApplySidebarBtn(Button btn, string text)
        {
            btn.Text = text;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = Color.FromArgb(200, 200, 200);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(20, 0, 0, 0);
            btn.Font = new Font("Segoe UI", 10.5f);
            btn.Height = 50;
            btn.BackColor = Color.Transparent;
            btn.Cursor = Cursors.Hand;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(0, 90, 80);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 90, 80);
        }

        public static void ApplyGridStyle(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.ForeColor = Charcoal;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Teal;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.DefaultCellStyle.Padding = new Padding(5);
            dgv.RowTemplate.Height = 35;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(224, 224, 224);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        public static void ApplyInputStyle(TextBoxBase tb)
        {
            tb.BackColor = Color.FromArgb(250, 250, 250);
            tb.ForeColor = Charcoal;
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Font = new Font("Segoe UI", 10);
        }

        public static void ApplyComboStyle(ComboBox cb)
        {
            cb.BackColor = Color.FromArgb(250, 250, 250);
            cb.ForeColor = Charcoal;
            cb.Font = new Font("Segoe UI", 10);
            cb.FlatStyle = FlatStyle.Flat;
        }

        public static Label MakeTitle(string text)
        {
            return new Label
            {
                Text = text,
                ForeColor = Charcoal,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(0, 10, 0, 5)
            };
        }

        public static Label MakeLabel(string text, Color? color = null)
        {
            return new Label
            {
                Text = text,
                ForeColor = color ?? TextGray,
                Font = new Font("Segoe UI", 9),
                Height = 20
            };
        }
    }
}
