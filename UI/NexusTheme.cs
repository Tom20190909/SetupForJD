using Sunny.UI;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace SleckEc2KdInterface.UI
{
    internal static class NexusTheme
    {
        internal static readonly Color Background = Color.FromArgb(245, 246, 247);
        internal static readonly Color Navigation = Color.White;
        internal static readonly Color Surface = Color.White;
        internal static readonly Color SurfaceAlt = Color.FromArgb(245, 246, 247);
        internal static readonly Color Input = Color.White;
        internal static readonly Color Border = Color.FromArgb(229, 230, 235);
        internal static readonly Color Accent = Color.FromArgb(51, 112, 255);
        internal static readonly Color AccentHover = Color.FromArgb(78, 131, 253);
        internal static readonly Color AccentPress = Color.FromArgb(41, 82, 204);
        internal static readonly Color Cyan = Color.FromArgb(51, 112, 255);
        internal static readonly Color Text = Color.FromArgb(31, 35, 41);
        internal static readonly Color TextSecondary = Color.FromArgb(100, 106, 115);
        internal static readonly Color Muted = Color.FromArgb(143, 149, 158);
        internal static readonly Color Success = Color.FromArgb(52, 199, 36);
        internal static readonly Color Warning = Color.FromArgb(255, 136, 0);
        internal static readonly Color Danger = Color.FromArgb(245, 74, 69);
        internal static readonly Color DangerSurface = Color.FromArgb(254, 240, 240);

        internal static Font Font(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font("Microsoft YaHei UI", size, style, GraphicsUnit.Point);
        }

        internal static GraphicsPath RoundedPath(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = Math.Max(2, radius * 2);
            rectangle.Width = Math.Max(1, rectangle.Width - 1);
            rectangle.Height = Math.Max(1, rectangle.Height - 1);
            diameter = Math.Min(diameter, Math.Min(rectangle.Width, rectangle.Height));
            path.AddArc(rectangle.X, rectangle.Y, diameter, diameter, 180, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Y, diameter, diameter, 270, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rectangle.X, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        internal static void DoubleBuffer(Control control)
        {
            PropertyInfo property = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                property.SetValue(control, true, null);
            }
        }

        internal static void ApplyWindow(NexusWindow window)
        {
            window.BackColor = Background;
            window.ForeColor = Text;
            window.AutoScaleMode = AutoScaleMode.Dpi;
            window.PerformAutoScale();
            window.Font = Font(9F);
            window.Style = UIStyle.Custom;
            window.StyleCustomMode = true;
            window.ShowTitle = false;
            window.ShowRadius = false;
            window.ShowShadow = true;
            window.ShowRect = false;
            window.EnableNexusChrome();
        }

        internal static void ApplyControlTree(Control root)
        {
            ApplyControl(root);
            foreach (Control child in root.Controls)
            {
                ApplyControlTree(child);
            }
        }

        private static void ApplyControl(Control control)
        {
            if (control is NexusTitleBar || control is NexusNavItem || control is NexusLoginBrand)
            {
                return;
            }

            if (control is TabPage)
            {
                TabPage page = (TabPage)control;
                page.BackColor = Background;
                page.ForeColor = Text;
                page.UseVisualStyleBackColor = false;
                DoubleBuffer(page);
                return;
            }

            if (control is DataGridView)
            {
                StyleGrid((DataGridView)control);
                return;
            }

            if (control is UIButton)
            {
                StyleButton((UIButton)control);
                return;
            }

            if (control is UITextBox)
            {
                UITextBox textBox = (UITextBox)control;
                textBox.Style = UIStyle.Custom;
                textBox.StyleCustomMode = true;
                textBox.FillColor = Input;
                textBox.FillDisableColor = SurfaceAlt;
                textBox.FillReadOnlyColor = SurfaceAlt;
                textBox.RectColor = Border;
                textBox.RectReadOnlyColor = Border;
                textBox.RectDisableColor = Border;
                textBox.ForeColor = Text;
                textBox.ForeDisableColor = Muted;
                textBox.ForeReadOnlyColor = TextSecondary;
                textBox.WatermarkColor = Muted;
                textBox.WatermarkActiveColor = TextSecondary;
                textBox.ScrollBarColor = Accent;
                textBox.ScrollBarBackColor = SurfaceAlt;
                textBox.SymbolColor = Cyan;
                textBox.TextBox.BorderStyle = BorderStyle.None;
                textBox.Radius = 6;
                textBox.RectSize = 1;
                textBox.Font = Font(9.5F);
                return;
            }

            if (control is UIDatePicker)
            {
                UIDatePicker picker = (UIDatePicker)control;
                picker.Style = UIStyle.Custom;
                picker.StyleCustomMode = true;
                picker.FillColor = Input;
                picker.FillDisableColor = SurfaceAlt;
                picker.RectColor = Border;
                picker.RectDisableColor = Border;
                picker.ForeColor = Text;
                picker.ForeDisableColor = Muted;
                picker.WatermarkColor = Muted;
                picker.WatermarkActiveColor = TextSecondary;
                picker.TextBox.BorderStyle = BorderStyle.None;
                picker.Radius = 6;
                picker.RectSize = 1;
                picker.Font = Font(9.5F);
                return;
            }

            if (control is UIRichTextBox)
            {
                UIRichTextBox richTextBox = (UIRichTextBox)control;
                richTextBox.Style = UIStyle.Custom;
                richTextBox.StyleCustomMode = true;
                richTextBox.FillColor = Input;
                richTextBox.FillDisableColor = SurfaceAlt;
                richTextBox.RectColor = Border;
                richTextBox.RectDisableColor = Border;
                richTextBox.ForeColor = TextSecondary;
                richTextBox.ForeDisableColor = Muted;
                richTextBox.ScrollBarColor = Accent;
                richTextBox.ScrollBarBackColor = SurfaceAlt;
                richTextBox.SelectionColor = Text;
                richTextBox.SelectionBackColor = Color.FromArgb(185, 208, 255);
                richTextBox.RichTextBox.BorderStyle = BorderStyle.None;
                richTextBox.Radius = 6;
                richTextBox.RectSize = 1;
                richTextBox.Font = Font(9F);
                return;
            }

            if (control is UIGroupBox)
            {
                UIGroupBox groupBox = (UIGroupBox)control;
                groupBox.Style = UIStyle.Custom;
                groupBox.StyleCustomMode = true;
                groupBox.FillColor = Surface;
                groupBox.FillDisableColor = Surface;
                groupBox.RectColor = Border;
                groupBox.RectDisableColor = Border;
                groupBox.ForeColor = Text;
                groupBox.ForeDisableColor = Muted;
                groupBox.Radius = 10;
                groupBox.RectSize = 1;
                groupBox.Font = Font(9.5F, FontStyle.Bold);
                DoubleBuffer(groupBox);
                return;
            }

            if (control is UICheckBox)
            {
                UICheckBox checkBox = (UICheckBox)control;
                checkBox.Style = UIStyle.Custom;
                checkBox.StyleCustomMode = true;
                checkBox.ForeColor = TextSecondary;
                checkBox.CheckBoxColor = Accent;
                checkBox.Font = Font(9F);
                return;
            }

            if (control is UILabel)
            {
                UILabel label = (UILabel)control;
                label.Style = UIStyle.Custom;
                label.StyleCustomMode = true;
                label.ForeColor = TextSecondary;
                label.BackColor = Color.Transparent;
                label.Font = Font(9F);
                return;
            }

            if (control is UITabControl)
            {
                UITabControl tabControl = (UITabControl)control;
                tabControl.Style = UIStyle.Custom;
                tabControl.StyleCustomMode = true;
                tabControl.FillColor = Background;
                tabControl.TabBackColor = Surface;
                tabControl.TabSelectedColor = Accent;
                tabControl.TabSelectedForeColor = Color.White;
                tabControl.TabUnSelectedForeColor = TextSecondary;
                tabControl.Font = Font(9.5F);
                tabControl.ItemSize = new Size(132, 36);
                return;
            }

            if (control is RichTextBox)
            {
                RichTextBox richTextBox = (RichTextBox)control;
                richTextBox.BackColor = Input;
                richTextBox.ForeColor = TextSecondary;
                richTextBox.BorderStyle = BorderStyle.None;
                richTextBox.Font = Font(9F);
                return;
            }

            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.BackColor = Input;
                textBox.ForeColor = Text;
                textBox.BorderStyle = BorderStyle.None;
                textBox.Font = Font(9.5F);
                return;
            }

            if (control is Label)
            {
                Label label = (Label)control;
                label.ForeColor = TextSecondary;
                label.BackColor = Color.Transparent;
                label.Font = Font(9F);
                return;
            }

            if (control is TableLayoutPanel)
            {
                control.BackColor = Color.Transparent;
                DoubleBuffer(control);
                return;
            }

            if (control is Panel)
            {
                control.BackColor = Surface;
                control.ForeColor = Text;
                DoubleBuffer(control);
                return;
            }

            if (control is ProgressBar)
            {
                control.BackColor = SurfaceAlt;
                control.ForeColor = Accent;
                return;
            }

            if (control is ToolStrip)
            {
                ToolStrip toolStrip = (ToolStrip)control;
                toolStrip.Renderer = new NexusMenuRenderer();
                toolStrip.BackColor = Surface;
                toolStrip.ForeColor = Text;
                toolStrip.Font = Font(9F);
            }
        }

        private static void StyleButton(UIButton button)
        {
            string text = button.Text ?? string.Empty;
            bool danger = text.Contains("删除") || text.Contains("取消单据");
            bool primary = text.Contains("查询") || text.Contains("保存") || text == "导入" ||
                           text == "提交" || text == "统计" || text.Contains("同步") ||
                           text.Contains("生成") || text.Contains("计算") || text == "登陆" ||
                           text == "登录" || text.Contains("更新密码");

            Color fill = danger ? DangerSurface : primary ? Accent : Color.White;
            Color hover = danger ? Color.FromArgb(253, 226, 226) : primary ? AccentHover : SurfaceAlt;
            Color press = danger ? Color.FromArgb(250, 205, 205) : primary ? AccentPress : Border;
            Color border = danger ? Color.FromArgb(245, 183, 181) : primary ? Accent : Border;

            button.Style = UIStyle.Custom;
            button.StyleCustomMode = true;
            button.FillColor = fill;
            button.FillColor2 = fill;
            button.FillColorGradient = false;
            button.FillHoverColor = hover;
            button.FillPressColor = press;
            button.FillSelectedColor = fill;
            button.FillDisableColor = SurfaceAlt;
            button.RectColor = border;
            button.RectHoverColor = primary ? AccentHover : danger ? Danger : Color.FromArgb(187, 191, 196);
            button.RectPressColor = primary ? AccentPress : border;
            button.RectDisableColor = Border;
            button.ForeColor = danger ? Danger : primary ? Color.White : Text;
            button.ForeHoverColor = danger ? Danger : primary ? Color.White : Text;
            button.ForePressColor = danger ? Danger : primary ? Color.White : Text;
            button.ForeDisableColor = Muted;
            button.Radius = 6;
            button.RectSize = 1;
            button.ShowFocusColor = false;
            button.Font = Font(9F, primary ? FontStyle.Bold : FontStyle.Regular);
            button.Cursor = Cursors.Hand;
        }

        private static void StyleGrid(DataGridView grid)
        {
            UIDataGridView uiGrid = grid as UIDataGridView;
            if (uiGrid != null)
            {
                uiGrid.Style = UIStyle.Custom;
                uiGrid.StyleCustomMode = true;
            }

            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Border;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = SurfaceAlt;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextSecondary;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceAlt;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Text;
            grid.ColumnHeadersDefaultCellStyle.Font = Font(9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            grid.ColumnHeadersHeight = 38;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 255);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.DefaultCellStyle.Font = Font(9F);
            grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            grid.DefaultCellStyle.NullValue = string.Empty;
            grid.RowsDefaultCellStyle.BackColor = Surface;
            grid.RowsDefaultCellStyle.ForeColor = Text;
            grid.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 255);
            grid.RowsDefaultCellStyle.SelectionForeColor = Text;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 252);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Text;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 255);
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Text;
            grid.RowHeadersDefaultCellStyle.BackColor = SurfaceAlt;
            grid.RowHeadersDefaultCellStyle.ForeColor = Muted;
            grid.RowHeadersDefaultCellStyle.SelectionBackColor = Accent;
            grid.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            grid.RowTemplate.Height = 34;
            grid.AllowUserToResizeRows = false;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            grid.Font = Font(9F);
            DoubleBuffer(grid);
        }
    }

    internal sealed class NexusMenuRenderer : ToolStripProfessionalRenderer
    {
        internal NexusMenuRenderer()
            : base(new NexusColorTable())
        {
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Selected ? NexusTheme.Text : NexusTheme.TextSecondary;
            base.OnRenderItemText(e);
        }
    }

    internal sealed class NexusColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground { get { return NexusTheme.Surface; } }
        public override Color MenuBorder { get { return NexusTheme.Border; } }
        public override Color MenuItemBorder { get { return NexusTheme.Accent; } }
        public override Color MenuItemSelected { get { return NexusTheme.SurfaceAlt; } }
        public override Color MenuItemSelectedGradientBegin { get { return NexusTheme.SurfaceAlt; } }
        public override Color MenuItemSelectedGradientEnd { get { return NexusTheme.SurfaceAlt; } }
        public override Color MenuItemPressedGradientBegin { get { return NexusTheme.AccentPress; } }
        public override Color MenuItemPressedGradientEnd { get { return NexusTheme.AccentPress; } }
        public override Color ImageMarginGradientBegin { get { return NexusTheme.Surface; } }
        public override Color ImageMarginGradientMiddle { get { return NexusTheme.Surface; } }
        public override Color ImageMarginGradientEnd { get { return NexusTheme.Surface; } }
        public override Color SeparatorDark { get { return NexusTheme.Border; } }
        public override Color SeparatorLight { get { return NexusTheme.Border; } }
    }
}
