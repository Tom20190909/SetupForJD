using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SleckEc2KdInterface.UI
{
    public static class NexusMessageBox
    {
        public static DialogResult Show(string text, string title = "提示",
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            return ShowDialog(text, title, buttons, icon);
        }

        public static DialogResult ShowError(string text)
        {
            return Show(text, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult ShowWarning(string text)
        {
            return Show(text, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static DialogResult ShowSuccess(string text)
        {
            return Show(text, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static DialogResult ShowDialog(string text, string title,
            MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Form owner = Form.ActiveForm;
            if (owner != null && owner.InvokeRequired)
            {
                return (DialogResult)owner.Invoke(new Func<string, string, MessageBoxButtons, MessageBoxIcon, DialogResult>(ShowDialog),
                    text, title, buttons, icon);
            }

            using (NexusDialog dialog = new NexusDialog(text, title, buttons, icon))
            {
                if (owner == null || owner.IsDisposed)
                {
                    dialog.StartPosition = FormStartPosition.CenterScreen;
                    return dialog.ShowDialog();
                }

                using (Form overlay = CreateOverlay(owner))
                {
                    overlay.Show(owner);
                    dialog.StartPosition = FormStartPosition.Manual;
                    dialog.Location = new Point(
                        owner.Left + Math.Max(0, (owner.Width - dialog.Width) / 2),
                        owner.Top + Math.Max(0, (owner.Height - dialog.Height) / 2));
                    return dialog.ShowDialog(overlay);
                }
            }
        }

        private static Form CreateOverlay(Form owner)
        {
            return new Form
            {
                BackColor = Color.Black,
                Opacity = 0.16,
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                Bounds = owner.Bounds,
                TopMost = false
            };
        }
    }

    internal sealed class NexusDialog : Form
    {
        private readonly List<DialogButtonSpec> specs;
        private readonly UIButton acceptButton;
        private readonly UIButton cancelButton;

        internal NexusDialog(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            specs = CreateButtonSpecs(buttons);
            Text = title;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = NexusTheme.Surface;
            ForeColor = NexusTheme.Text;
            AutoScaleMode = AutoScaleMode.Dpi;
            KeyPreview = true;
            DoubleBuffered = true;
            Font = NexusTheme.Font(9F);

            int messageHeight = TextRenderer.MeasureText(message ?? string.Empty, NexusTheme.Font(10F),
                new Size(400, 0), TextFormatFlags.WordBreak).Height;
            int dialogHeight = Math.Min(460, Math.Max(224, messageHeight + 158));
            ClientSize = new Size(460, dialogHeight);

            NexusDialogIcon dialogIcon = new NexusDialogIcon(icon)
            {
                Location = new Point(24, 22),
                Size = new Size(32, 32)
            };
            Controls.Add(dialogIcon);

            Label titleLabel = new Label
            {
                Text = title,
                Location = new Point(66, 20),
                Size = new Size(330, 38),
                Font = NexusTheme.Font(14F, FontStyle.Bold),
                ForeColor = NexusTheme.Text,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(titleLabel);

            Button closeButton = new Button
            {
                Text = "×",
                Location = new Point(414, 12),
                Size = new Size(32, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = NexusTheme.Surface,
                ForeColor = NexusTheme.Muted,
                Font = new Font("Segoe UI", 13F),
                TabStop = false
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatAppearance.MouseOverBackColor = NexusTheme.SurfaceAlt;
            closeButton.Click += delegate
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            Controls.Add(closeButton);

            RichTextBox messageBox = new RichTextBox
            {
                Text = message ?? string.Empty,
                Location = new Point(28, 70),
                Size = new Size(404, dialogHeight - 154),
                Font = NexusTheme.Font(10F),
                ForeColor = NexusTheme.Text,
                BackColor = NexusTheme.Surface,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                DetectUrls = false,
                TabStop = false
            };
            Controls.Add(messageBox);

            Panel footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                BackColor = NexusTheme.Surface,
                Padding = new Padding(0)
            };
            footer.Paint += delegate(object sender, PaintEventArgs e)
            {
                using (Pen pen = new Pen(NexusTheme.Border))
                {
                    e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0);
                }
            };
            Controls.Add(footer);

            int totalWidth = specs.Count * 88 + (specs.Count - 1) * 10;
            int left = Width - 24 - totalWidth;
            UIButton primary = null;
            UIButton cancel = null;
            for (int i = 0; i < specs.Count; i++)
            {
                DialogButtonSpec spec = specs[i];
                UIButton button = new UIButton
                {
                    Text = spec.Text,
                    Size = new Size(88, 34),
                    Location = new Point(left + i * 98, 15),
                    Tag = spec.Result
                };
                button.Click += delegate
                {
                    DialogResult = (DialogResult)button.Tag;
                    Close();
                };
                footer.Controls.Add(button);
                if (spec.Primary) primary = button;
                if (spec.Result == DialogResult.Cancel || spec.Result == DialogResult.No) cancel = button;
            }
            acceptButton = primary;
            cancelButton = cancel;

            NexusTheme.ApplyControlTree(this);
            foreach (Control control in footer.Controls)
            {
                UIButton button = control as UIButton;
                if (button == null) continue;
                bool isPrimary = button == primary;
                if (isPrimary)
                {
                    button.FillColor = NexusTheme.Accent;
                    button.FillHoverColor = NexusTheme.AccentHover;
                    button.FillPressColor = NexusTheme.AccentPress;
                    button.RectColor = NexusTheme.Accent;
                    button.RectHoverColor = NexusTheme.AccentHover;
                    button.RectPressColor = NexusTheme.AccentPress;
                    button.ForeColor = Color.White;
                    button.ForeHoverColor = Color.White;
                    button.ForePressColor = Color.White;
                }
            }

            if (acceptButton != null) AcceptButton = acceptButton;
            if (cancelButton != null) CancelButton = cancelButton;
            titleLabel.MouseDown += DragWindow;
            dialogIcon.MouseDown += DragWindow;
            MouseDown += DragWindow;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parameters = base.CreateParams;
                parameters.ClassStyle |= 0x00020000;
                return parameters;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            using (GraphicsPath path = NexusTheme.RoundedPath(ClientRectangle, 10))
            {
                Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (GraphicsPath path = NexusTheme.RoundedPath(new Rectangle(0, 0, Width, Height), 10))
            using (Pen pen = new Pen(NexusTheme.Border))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape && cancelButton == null)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void DragWindow(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x00A1, (IntPtr)2, IntPtr.Zero);
            }
        }

        private static List<DialogButtonSpec> CreateButtonSpecs(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    return new List<DialogButtonSpec>
                    {
                        new DialogButtonSpec("确定", DialogResult.OK, true),
                        new DialogButtonSpec("取消", DialogResult.Cancel, false)
                    };
                case MessageBoxButtons.YesNo:
                    return new List<DialogButtonSpec>
                    {
                        new DialogButtonSpec("是", DialogResult.Yes, true),
                        new DialogButtonSpec("否", DialogResult.No, false)
                    };
                case MessageBoxButtons.YesNoCancel:
                    return new List<DialogButtonSpec>
                    {
                        new DialogButtonSpec("是", DialogResult.Yes, true),
                        new DialogButtonSpec("否", DialogResult.No, false),
                        new DialogButtonSpec("取消", DialogResult.Cancel, false)
                    };
                case MessageBoxButtons.RetryCancel:
                    return new List<DialogButtonSpec>
                    {
                        new DialogButtonSpec("重试", DialogResult.Retry, true),
                        new DialogButtonSpec("取消", DialogResult.Cancel, false)
                    };
                case MessageBoxButtons.AbortRetryIgnore:
                    return new List<DialogButtonSpec>
                    {
                        new DialogButtonSpec("中止", DialogResult.Abort, true),
                        new DialogButtonSpec("重试", DialogResult.Retry, false),
                        new DialogButtonSpec("忽略", DialogResult.Ignore, false)
                    };
                default:
                    return new List<DialogButtonSpec>
                    {
                        new DialogButtonSpec("确定", DialogResult.OK, true)
                    };
            }
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private sealed class DialogButtonSpec
        {
            internal string Text;
            internal DialogResult Result;
            internal bool Primary;

            internal DialogButtonSpec(string text, DialogResult result, bool primary)
            {
                Text = text;
                Result = result;
                Primary = primary;
            }
        }
    }

    internal sealed class NexusDialogIcon : Control
    {
        private readonly MessageBoxIcon icon;

        internal NexusDialogIcon(MessageBoxIcon icon)
        {
            this.icon = icon;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bool error = icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Hand;
            bool warning = icon == MessageBoxIcon.Exclamation || icon == MessageBoxIcon.Warning;
            bool question = icon == MessageBoxIcon.Question;
            Color foreground = error ? NexusTheme.Danger : warning ? NexusTheme.Warning : NexusTheme.Accent;
            Color background = error ? NexusTheme.DangerSurface : warning ? Color.FromArgb(255, 247, 230) : Color.FromArgb(232, 240, 255);
            using (SolidBrush brush = new SolidBrush(background))
            {
                e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }
            using (Font font = new Font("Segoe UI", 15F, FontStyle.Bold))
            {
                TextRenderer.DrawText(e.Graphics, error || warning ? "!" : question ? "?" : "i", font,
                    ClientRectangle, foreground, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
    }
}
