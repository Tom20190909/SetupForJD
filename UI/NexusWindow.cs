using Sunny.UI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SleckEc2KdInterface.UI
{
    public class NexusWindow : UIForm
    {
        private const int WmNcHitTest = 0x0084;
        private const int WmGetMinMaxInfo = 0x0024;
        private const int HtClient = 1;
        private bool nexusChrome;

        public NexusWindow()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.CenterScreen;
        }

        internal void EnableNexusChrome()
        {
            nexusChrome = true;
            FormBorderStyle = FormBorderStyle.None;
            ShowTitle = false;
            ShowRadius = false;
            ShowShadow = true;
            ShowRect = false;
            SizeGripStyle = SizeGripStyle.Hide;
        }

        protected override void WndProc(ref Message m)
        {
            if (nexusChrome && m.Msg == WmGetMinMaxInfo)
            {
                Screen screen = Screen.FromHandle(Handle);
                MINMAXINFO info = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));
                info.MaxPosition = new POINT(screen.WorkingArea.Left - screen.Bounds.Left,
                    screen.WorkingArea.Top - screen.Bounds.Top);
                info.MaxSize = new POINT(screen.WorkingArea.Width, screen.WorkingArea.Height);
                Marshal.StructureToPtr(info, m.LParam, true);
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);

            bool fixedSize = !MaximumSize.IsEmpty && MaximumSize == MinimumSize;
            if (!nexusChrome || m.Msg != WmNcHitTest || m.Result != (IntPtr)HtClient ||
                WindowState == FormWindowState.Maximized || fixedSize)
            {
                return;
            }

            int x = unchecked((short)((long)m.LParam & 0xFFFF));
            int y = unchecked((short)(((long)m.LParam >> 16) & 0xFFFF));
            Point point = PointToClient(new Point(x, y));
            const int border = 7;
            bool left = point.X <= border;
            bool right = point.X >= ClientSize.Width - border;
            bool top = point.Y <= border;
            bool bottom = point.Y >= ClientSize.Height - border;

            if (left && top) m.Result = (IntPtr)13;
            else if (right && top) m.Result = (IntPtr)14;
            else if (left && bottom) m.Result = (IntPtr)16;
            else if (right && bottom) m.Result = (IntPtr)17;
            else if (left) m.Result = (IntPtr)10;
            else if (right) m.Result = (IntPtr)11;
            else if (top) m.Result = (IntPtr)12;
            else if (bottom) m.Result = (IntPtr)15;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            internal int X;
            internal int Y;

            internal POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            internal POINT Reserved;
            internal POINT MaxSize;
            internal POINT MaxPosition;
            internal POINT MinTrackSize;
            internal POINT MaxTrackSize;
        }
    }

    internal sealed class NexusTitleBar : Control
    {
        private readonly NexusWindow window;
        private readonly bool allowMaximize;
        private readonly Label brandLabel;
        private readonly Label productLabel;
        private readonly Button minimizeButton;
        private readonly Button maximizeButton;
        private readonly Button closeButton;

        internal NexusTitleBar(NexusWindow window, string product, bool allowMaximize)
        {
            this.window = window;
            this.allowMaximize = allowMaximize;
            Dock = DockStyle.Top;
            Height = 42;
            BackColor = NexusTheme.Navigation;
            Font = NexusTheme.Font(9F);
            NexusTheme.DoubleBuffer(this);

            brandLabel = new Label
            {
                AutoSize = false,
                Location = new Point(18, 0),
                Size = new Size(94, 42),
                Font = NexusTheme.Font(11F, FontStyle.Bold),
                ForeColor = NexusTheme.Accent,
                Text = "一件代发",
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            productLabel = new Label
            {
                AutoSize = false,
                Location = new Point(116, 0),
                Size = new Size(390, 42),
                Font = NexusTheme.Font(9F),
                ForeColor = NexusTheme.TextSecondary,
                Text = product,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            Controls.Add(brandLabel);
            Controls.Add(productLabel);

            closeButton = CreateWindowButton("×", 42);
            closeButton.Dock = DockStyle.Right;
            closeButton.Font = new Font("Segoe UI", 16F);
            closeButton.Click += delegate { window.Close(); };
            Controls.Add(closeButton);

            maximizeButton = CreateWindowButton("□", 42);
            maximizeButton.Dock = DockStyle.Right;
            maximizeButton.Visible = allowMaximize;
            maximizeButton.Click += delegate { ToggleMaximize(); };
            Controls.Add(maximizeButton);

            minimizeButton = CreateWindowButton("—", 42);
            minimizeButton.Dock = DockStyle.Right;
            minimizeButton.Click += delegate { window.WindowState = FormWindowState.Minimized; };
            Controls.Add(minimizeButton);

            MouseDown += DragWindow;
            brandLabel.MouseDown += DragWindow;
            productLabel.MouseDown += DragWindow;
            DoubleClick += delegate { if (allowMaximize) ToggleMaximize(); };
            brandLabel.DoubleClick += delegate { if (allowMaximize) ToggleMaximize(); };
            productLabel.DoubleClick += delegate { if (allowMaximize) ToggleMaximize(); };
            Resize += delegate
            {
                productLabel.Width = Math.Max(120, Width - 410);
            };
        }

        private static Button CreateWindowButton(string text, int width)
        {
            return new Button
            {
                Text = text,
                Width = width,
                Dock = DockStyle.None,
                FlatStyle = FlatStyle.Flat,
                BackColor = NexusTheme.Navigation,
                ForeColor = NexusTheme.TextSecondary,
                Font = new Font("Segoe UI", 11F),
                TabStop = false,
                UseVisualStyleBackColor = false,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            foreach (Control control in Controls)
            {
                Button button = control as Button;
                if (button != null)
                {
                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.MouseOverBackColor = button == closeButton
                        ? Color.FromArgb(190, 45, 70)
                        : NexusTheme.SurfaceAlt;
                    button.FlatAppearance.MouseDownBackColor = button == closeButton
                        ? Color.FromArgb(155, 35, 58)
                        : NexusTheme.Border;
                }
            }
        }

        private void ToggleMaximize()
        {
            window.WindowState = window.WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
            maximizeButton.Text = window.WindowState == FormWindowState.Maximized ? "❐" : "□";
        }

        private void DragWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && window.WindowState != FormWindowState.Maximized)
            {
                ReleaseCapture();
                SendMessage(window.Handle, 0x00A1, (IntPtr)2, IntPtr.Zero);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(NexusTheme.Border))
            {
                e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }

    internal sealed class NexusNavItem : Control
    {
        private bool hovered;
        private bool selected;
        private bool compact;

        internal int PageIndex { get; private set; }
        internal string Number { get; private set; }

        internal bool Selected
        {
            get { return selected; }
            set { selected = value; Invalidate(); }
        }

        internal bool Compact
        {
            get { return compact; }
            set { compact = value; Invalidate(); }
        }

        internal NexusNavItem(string text, string number, int pageIndex)
        {
            Text = text;
            Number = number;
            PageIndex = pageIndex;
            Height = 42;
            Cursor = Cursors.Hand;
            Font = NexusTheme.Font(9.5F);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            hovered = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(NexusTheme.Navigation);
            Color background = selected ? Color.FromArgb(232, 240, 255) : hovered ? Color.FromArgb(242, 243, 245) : NexusTheme.Navigation;
            using (GraphicsPath path = NexusTheme.RoundedPath(new Rectangle(1, 2, Width - 2, Height - 4), 6))
            using (SolidBrush brush = new SolidBrush(background))
            {
                e.Graphics.FillPath(brush, path);
            }

            Rectangle iconBounds = compact
                ? new Rectangle((Width - 18) / 2, 12, 18, 18)
                : new Rectangle(18, 12, 18, 18);
            DrawIcon(e.Graphics, iconBounds, selected ? NexusTheme.Accent : NexusTheme.Muted);

            if (!compact)
            {
                Rectangle textRect = new Rectangle(48, 0, Width - 55, Height);
                TextRenderer.DrawText(e.Graphics, Text, Font, textRect,
                    selected ? NexusTheme.Accent : NexusTheme.TextSecondary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private void DrawIcon(Graphics graphics, Rectangle bounds, Color color)
        {
            int x = bounds.X;
            int y = bounds.Y;
            using (Pen pen = new Pen(color, 1.5F))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                string iconKey = Text == "对账中心" ? "08" : Text == "基础资料" ? "09" : Number;
                switch (iconKey)
                {
                    case "01":
                        graphics.DrawRectangle(pen, x + 2, y + 12, 14, 4);
                        graphics.DrawLine(pen, x + 9, y + 1, x + 9, y + 11);
                        graphics.DrawLine(pen, x + 5, y + 5, x + 9, y + 1);
                        graphics.DrawLine(pen, x + 9, y + 1, x + 13, y + 5);
                        break;
                    case "02":
                        graphics.DrawRectangle(pen, x + 4, y + 1, 11, 16);
                        graphics.DrawLine(pen, x + 7, y + 6, x + 12, y + 6);
                        graphics.DrawLine(pen, x + 7, y + 10, x + 12, y + 10);
                        graphics.DrawLine(pen, x + 7, y + 14, x + 11, y + 14);
                        break;
                    case "03":
                        graphics.DrawRectangle(pen, x + 1, y + 4, 10, 8);
                        graphics.DrawLines(pen, new[] { new Point(x + 11, y + 7), new Point(x + 14, y + 7), new Point(x + 17, y + 10), new Point(x + 17, y + 12), new Point(x + 11, y + 12) });
                        graphics.DrawEllipse(pen, x + 4, y + 12, 3, 3);
                        graphics.DrawEllipse(pen, x + 13, y + 12, 3, 3);
                        break;
                    case "04":
                        graphics.DrawArc(pen, x + 3, y + 3, 12, 12, 45, 275);
                        graphics.DrawLine(pen, x + 3, y + 3, x + 3, y + 9);
                        graphics.DrawLine(pen, x + 3, y + 3, x + 9, y + 3);
                        break;
                    case "05":
                        graphics.DrawEllipse(pen, x + 2, y + 2, 14, 5);
                        graphics.DrawArc(pen, x + 2, y + 5, 14, 8, 0, 180);
                        graphics.DrawArc(pen, x + 2, y + 9, 14, 7, 0, 180);
                        graphics.DrawLine(pen, x + 2, y + 4, x + 2, y + 12);
                        graphics.DrawLine(pen, x + 16, y + 4, x + 16, y + 12);
                        break;
                    case "06":
                        graphics.DrawLine(pen, x + 2, y + 6, x + 16, y + 6);
                        graphics.DrawLine(pen, x + 12, y + 2, x + 16, y + 6);
                        graphics.DrawLine(pen, x + 16, y + 6, x + 12, y + 10);
                        graphics.DrawLine(pen, x + 16, y + 13, x + 2, y + 13);
                        graphics.DrawLine(pen, x + 6, y + 9, x + 2, y + 13);
                        graphics.DrawLine(pen, x + 2, y + 13, x + 6, y + 17);
                        break;
                    case "07":
                        if (Text.Contains("密码"))
                        {
                            graphics.DrawRectangle(pen, x + 3, y + 8, 12, 8);
                            graphics.DrawArc(pen, x + 5, y + 1, 8, 11, 180, 180);
                            graphics.DrawLine(pen, x + 9, y + 11, x + 9, y + 13);
                        }
                        else
                        {
                            graphics.DrawLines(pen, new[] { new Point(x + 2, y + 5), new Point(x + 9, y + 1), new Point(x + 16, y + 5), new Point(x + 9, y + 9), new Point(x + 2, y + 5) });
                            graphics.DrawLines(pen, new[] { new Point(x + 2, y + 5), new Point(x + 2, y + 13), new Point(x + 9, y + 17), new Point(x + 16, y + 13), new Point(x + 16, y + 5) });
                            graphics.DrawLine(pen, x + 9, y + 9, x + 9, y + 17);
                        }
                        break;
                    case "08":
                        graphics.DrawLine(pen, x + 2, y + 1, x + 2, y + 16);
                        graphics.DrawLine(pen, x + 2, y + 16, x + 17, y + 16);
                        graphics.DrawRectangle(pen, x + 5, y + 9, 2, 5);
                        graphics.DrawRectangle(pen, x + 9, y + 5, 2, 9);
                        graphics.DrawRectangle(pen, x + 13, y + 2, 2, 12);
                        break;
                    default:
                        graphics.DrawRectangle(pen, x + 2, y + 2, 5, 5);
                        graphics.DrawRectangle(pen, x + 11, y + 2, 5, 5);
                        graphics.DrawRectangle(pen, x + 2, y + 11, 5, 5);
                        graphics.DrawRectangle(pen, x + 11, y + 11, 5, 5);
                        break;
                }
            }
        }
    }

    internal sealed class NexusAvatar : Control
    {
        internal NexusAvatar()
        {
            Size = new Size(34, 34);
            Text = "订";
            Font = NexusTheme.Font(11F, FontStyle.Bold);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(NexusTheme.Navigation);
            using (SolidBrush brush = new SolidBrush(NexusTheme.Accent))
            {
                e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    internal sealed class NexusLoginBrand : Control
    {
        internal NexusLoginBrand()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (LinearGradientBrush background = new LinearGradientBrush(ClientRectangle,
                Color.FromArgb(249, 251, 255), Color.FromArgb(234, 241, 255), 35F))
            {
                e.Graphics.FillRectangle(background, ClientRectangle);
            }

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(24, NexusTheme.Accent)))
            {
                e.Graphics.FillEllipse(brush, Width - 280, -100, 390, 390);
            }
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(18, 124, 136, 255)))
            {
                e.Graphics.FillEllipse(brush, -180, Height - 280, 360, 360);
            }

            using (GraphicsPath path = NexusTheme.RoundedPath(new Rectangle(48, 44, 160, 32), 16))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(232, 240, 255)))
            {
                e.Graphics.FillPath(brush, path);
            }
            using (Font small = NexusTheme.Font(9F, FontStyle.Bold))
            using (SolidBrush accent = new SolidBrush(NexusTheme.Accent))
            {
                e.Graphics.DrawString("订单协同工作台", small, accent, 70, 51);
            }

            using (Font title = NexusTheme.Font(29F, FontStyle.Bold))
            using (SolidBrush text = new SolidBrush(NexusTheme.Text))
            {
                e.Graphics.DrawString("让订单处理", title, text, 46, 116);
                e.Graphics.DrawString("更简单、更高效", title, text, 46, 172);
            }

            using (Font subtitle = NexusTheme.Font(11F))
            using (SolidBrush secondary = new SolidBrush(NexusTheme.TextSecondary))
            {
                e.Graphics.DrawString("订单、仓储与对账，一站式协同处理", subtitle, secondary, 50, 252);
                e.Graphics.DrawString("清晰的工作流，让团队专注于业务本身。", subtitle, secondary, 50, 282);
            }

            DrawFeature(e.Graphics, 50, 340, "01", "订单处理", "导入、校验、提交与状态跟踪");
            DrawFeature(e.Graphics, 50, 424, "02", "仓储协同", "旺店通、调拨、装箱与物流");
            DrawFeature(e.Graphics, 50, 508, "03", "经营对账", "订货、发货、退货统一核算");

            using (Font footer = NexusTheme.Font(9F))
            using (SolidBrush muted = new SolidBrush(NexusTheme.Muted))
            {
                e.Graphics.DrawString("简单  ·  高效  ·  协同", footer, muted, 50, Height - 44);
            }
        }

        private static void DrawFeature(Graphics graphics, int x, int y, string number, string title, string subtitle)
        {
            using (GraphicsPath shadow = NexusTheme.RoundedPath(new Rectangle(x + 2, y + 4, 438, 68), 10))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(12, 31, 35, 41)))
            {
                graphics.FillPath(shadowBrush, shadow);
            }
            using (GraphicsPath card = NexusTheme.RoundedPath(new Rectangle(x, y, 438, 68), 10))
            using (SolidBrush cardBrush = new SolidBrush(Color.White))
            using (Pen border = new Pen(Color.FromArgb(235, 238, 245)))
            {
                graphics.FillPath(cardBrush, card);
                graphics.DrawPath(border, card);
            }
            using (GraphicsPath icon = NexusTheme.RoundedPath(new Rectangle(x + 16, y + 16, 36, 36), 8))
            using (SolidBrush iconBrush = new SolidBrush(Color.FromArgb(232, 240, 255)))
            {
                graphics.FillPath(iconBrush, icon);
            }
            using (Font numberFont = new Font("Segoe UI", 9F, FontStyle.Bold))
            {
                TextRenderer.DrawText(graphics, number, numberFont, new Rectangle(x + 16, y + 16, 36, 36),
                    NexusTheme.Accent, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            using (Font titleFont = NexusTheme.Font(10.5F, FontStyle.Bold))
            using (SolidBrush text = new SolidBrush(NexusTheme.Text))
            {
                graphics.DrawString(title, titleFont, text, x + 68, y + 12);
            }
            using (Font subtitleFont = NexusTheme.Font(8.5F))
            using (SolidBrush secondary = new SolidBrush(NexusTheme.TextSecondary))
            {
                graphics.DrawString(subtitle, subtitleFont, secondary, x + 68, y + 39);
            }
        }
    }
}
