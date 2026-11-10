using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SleckEc2KdInterface.UI
{
    internal static class NexusPageLayouts
    {
        internal static void Apply(Form form, bool distributor)
        {
            LayoutOrderQuery(form, distributor);
            LayoutSimpleFilters(form);
            LayoutImport(form);
            LayoutToolbars(form, distributor);
            LayoutPasswordPage(form, distributor);
            LayoutSyncPanel(form);
            SetLayoutPadding(form);
        }

        private static void LayoutOrderQuery(Form form, bool distributor)
        {
            List<FieldDefinition> fields = new List<FieldDefinition>
            {
                FieldDefinition.DateRange("uiLabel26", "sdt", "uiLabel25", "edt", 270),
                new FieldDefinition("uiLabel3", "uiTextBox11", 170),
                new FieldDefinition("uiLabel32", "uiTextBox17", 170),
                new FieldDefinition("uiLabel23", "uitxtSrcOrderNo", 180),
                new FieldDefinition("uiLabel24", "uitxtgoodno", 150),
                new FieldDefinition("uiLabel22", "uitxtAddressee", 180),
                new FieldDefinition("uiLabel5", "uitxtlogisticsno", 170)
            };
            string[] buttons =
            {
                "uiButton15", "uiButton11", "uiButton16", "uiButton14", "uiButton26", "uiButton20",
                "uiButton31", "uiButton37", "uiButton1", "uiButton10", "uiButton13"
            };
            LayoutFilterGroup(form, "uiGroupBox5", "订单筛选与操作", fields, buttons, 226);
        }

        private static void LayoutSimpleFilters(Form form)
        {
            List<FieldDefinition> deliveryFields = new List<FieldDefinition>
            {
                FieldDefinition.DateRange("uiLabel15", "ssdt", "uiLabel14", "sedt", 270),
                new FieldDefinition("uiLabel13", "uiTextBox4", 180),
                new FieldDefinition("uiLabel9", "uiTextBox3", 190),
                new FieldDefinition("uiLabel8", "uiTextBox2", 160),
                new FieldDefinition("uiLabel7", "uiTextBox1", 170)
            };
            LayoutFilterGroup(form, "uiGroupBox3", "发货查询条件", deliveryFields,
                new[] { "uiButton5", "uiButton29" }, 190);

            List<FieldDefinition> returnFields = new List<FieldDefinition>
            {
                FieldDefinition.DateRange("uiLabel21", "rsdt", "uiLabel20", "redt", 270),
                new FieldDefinition("uiLabel19", "uiTextBox8", 180),
                new FieldDefinition("uiLabel18", "uiTextBox7", 190),
                new FieldDefinition("uiLabel17", "uiTextBox6", 160),
                new FieldDefinition("uiLabel16", "uiTextBox5", 170)
            };
            LayoutFilterGroup(form, "uiGroupBox4", "退货查询条件", returnFields,
                new[] { "uiButton3", "uiButton28" }, 190);

            List<FieldDefinition> reconcileFields = new List<FieldDefinition>
            {
                FieldDefinition.DateRange("uiLabel12", "dzsdt", "uiLabel11", "dzedt", 270),
                new FieldDefinition("uiLabel10", "uitxtdzCustName", 210)
            };
            LayoutFilterGroup(form, "uiGroupBox2", "对账统计条件", reconcileFields,
                new[] { "uiButton4", "uiButton6", "uiButton23", "uiButton25" }, 164);
        }

        private static void LayoutImport(Form form)
        {
            Control group = Find(form, "uiGroupBox1");
            if (group == null) return;

            List<Control> original = group.Controls.Cast<Control>().ToList();
            List<UIButton> buttons = original.OfType<UIButton>().ToList();
            Control fileLabel = original.FirstOrDefault(control => control.Name == "uiLabel1");
            Control fileInput = original.FirstOrDefault(control => control.Name == "uitxtfilepath");
            Control writeBack = original.FirstOrDefault(control => control.Name == "uiCheckBox1");
            group.Controls.Clear();
            group.Text = "订单导入流程";
            group.Height = 184;
            group.Dock = DockStyle.Top;

            Label steps = new Label
            {
                Dock = DockStyle.Top,
                Height = 34,
                Padding = new Padding(16, 0, 0, 0),
                Text = "01  选择文件      02  数据校验      03  保存订单",
                Font = NexusTheme.Font(9F, FontStyle.Bold),
                ForeColor = NexusTheme.Cyan,
                BackColor = NexusTheme.Surface,
                TextAlign = ContentAlignment.MiddleLeft
            };
            FlowLayoutPanel actions = CreateActionPanel(46, true);
            FlowLayoutPanel fileArea = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = NexusTheme.Surface,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(12, 0, 12, 0),
                Margin = new Padding(0)
            };
            Panel fileField = CreateField(fileLabel, fileInput, null, null, 520);
            fileArea.Controls.Add(fileField);
            if (writeBack != null)
            {
                writeBack.Size = new Size(236, 28);
                writeBack.Margin = new Padding(16, 22, 0, 0);
                fileArea.Controls.Add(writeBack);
            }

            AddButtons(actions, buttons, new[] { "uiButton17", "uiButton2", "uiButton7", "uiButton27", "uiButton8" });
            group.Controls.Add(fileArea);
            group.Controls.Add(actions);
            group.Controls.Add(steps);
            NexusTheme.ApplyControlTree(group);
        }

        private static void LayoutFilterGroup(Form form, string groupName, string title,
            List<FieldDefinition> definitions, string[] buttonOrder, int height)
        {
            Control group = Find(form, groupName);
            if (group == null) return;

            List<Control> original = group.Controls.Cast<Control>().ToList();
            List<UIButton> buttons = original.OfType<UIButton>().ToList();
            List<Panel> fieldPanels = new List<Panel>();
            foreach (FieldDefinition definition in definitions)
            {
                Control label = original.FirstOrDefault(control => control.Name == definition.LabelName);
                Control input = original.FirstOrDefault(control => control.Name == definition.InputName);
                Control secondLabel = string.IsNullOrEmpty(definition.SecondLabelName)
                    ? null : original.FirstOrDefault(control => control.Name == definition.SecondLabelName);
                Control secondInput = string.IsNullOrEmpty(definition.SecondInputName)
                    ? null : original.FirstOrDefault(control => control.Name == definition.SecondInputName);
                if (label == null && input == null) continue;
                fieldPanels.Add(CreateField(label, input, secondLabel, secondInput, definition.Width));
            }
            if (fieldPanels.Count == 0) return;

            group.Controls.Clear();
            group.Text = title;
            group.Height = height;
            group.Dock = DockStyle.Top;

            FlowLayoutPanel filters = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = NexusTheme.Surface,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                Padding = new Padding(10, 30, 10, 0),
                Margin = new Padding(0)
            };
            foreach (Panel field in fieldPanels) filters.Controls.Add(field);

            FlowLayoutPanel actions = CreateActionPanel(height >= 200 ? 78 : 48, true);
            AddButtons(actions, buttons, buttonOrder);
            group.Controls.Add(filters);
            group.Controls.Add(actions);
            NexusTheme.ApplyControlTree(group);
        }

        private static void LayoutToolbars(Form form, bool distributor)
        {
            List<FieldDefinition> wdtFields = new List<FieldDefinition>
            {
                new FieldDefinition("uiLabel27", "uiTextBox12", 140),
                new FieldDefinition("uiLabel34", "uiTextBox18", 250)
            };
            LayoutToolbar(form, "panelWdtToolbar", wdtFields,
                new[] { "uiButton30", "uiButton32", "uiButton33", "uiButton46", "uiButton34", "uiButton35", "uiButton41", "uiButton43" },
                new[] { "progressBar1" }, 108);
            Control redundantLabel = Find(form, "uiLabel33");
            if (redundantLabel != null) redundantLabel.Visible = false;
            Control wdtLabel = Find(form, "uiLabel34");
            if (wdtLabel != null) wdtLabel.Text = "不参与混装货品（逗号分隔）";
            SetToolbarHeight(form, "tableLayoutPanelWdtPage", 108);

            Label comboLabel = new Label { Text = "组合装筛选" };
            LayoutToolbar(form, "panelTransferToolbar",
                new List<FieldDefinition> { new FieldDefinition(comboLabel, Find(form, "textBox1"), 180) },
                new[] { "uiButton36", "uiButton38", "uiButton39", "uiButton40", "uiButton42" },
                new[] { "progressBar2" }, 108);
            SetToolbarHeight(form, "tableLayoutPanelTransferPage", 108);

            List<FieldDefinition> packingFields = new List<FieldDefinition>
            {
                new FieldDefinition("uiLabel28", "uiTextBox13", 190),
                new FieldDefinition("uiLabel29", "uiTextBox14", 190),
                new FieldDefinition("uiLabel30", "uiTextBox15", 170),
                new FieldDefinition("uiLabel31", "uiTextBox16", 160)
            };
            LayoutToolbar(form, "panelPackingToolbar", packingFields,
                new[] { "uiButton44", "uiButton45" }, new string[0], 108);
            SetToolbarHeight(form, "tableLayoutPanelPackingQuery", 108);

            List<FieldDefinition> baseFields = new List<FieldDefinition>
            {
                new FieldDefinition("uiLabel4", "uiTxtName", 190),
                new FieldDefinition("uiLabel6", "uiDatePicker1", 190)
            };
            LayoutToolbar(form, "panelBaseDataToolbar", baseFields,
                new[] { "uiButton12", "uiButton18", "uiButton19", "uiButton24", "uiButton9" },
                new[] { "uiRichTextBox2" }, 108);
            SetToolbarHeight(form, "tableLayoutPanelBaseData", 108);
        }

        private static void LayoutToolbar(Form form, string panelName, List<FieldDefinition> definitions,
            string[] buttonOrder, string[] miscNames, int height)
        {
            Control panel = Find(form, panelName);
            if (panel == null) return;

            List<Control> original = panel.Controls.Cast<Control>().ToList();
            List<UIButton> buttons = original.OfType<UIButton>().ToList();
            List<Control> fields = new List<Control>();
            foreach (FieldDefinition definition in definitions)
            {
                Control label = definition.LabelInstance ?? original.FirstOrDefault(control => control.Name == definition.LabelName);
                Control input = definition.InputInstance ?? original.FirstOrDefault(control => control.Name == definition.InputName);
                if (label == null && input == null) continue;
                fields.Add(CreateField(label, input, null, null, definition.Width));
            }

            List<Control> misc = new List<Control>();
            foreach (string name in miscNames)
            {
                Control control = original.FirstOrDefault(item => item.Name == name);
                if (control != null) misc.Add(control);
            }

            panel.Controls.Clear();
            panel.Height = height;
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = NexusTheme.Surface,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                Padding = new Padding(10, 6, 10, 4),
                Margin = new Padding(0)
            };
            foreach (Control field in fields) flow.Controls.Add(field);
            AddButtons(flow, buttons, buttonOrder);
            foreach (UIButton button in buttons)
            {
                if (button.Parent == flow)
                {
                    button.Margin = new Padding(4, 23, 4, 2);
                }
            }
            foreach (Control control in misc)
            {
                if (control is RichTextBox || control is UIRichTextBox)
                {
                    control.Size = new Size(260, 72);
                    control.Margin = new Padding(8, 0, 4, 2);
                }
                else
                {
                    control.Size = new Size(160, 24);
                    control.Margin = new Padding(8, 16, 4, 2);
                }
                flow.Controls.Add(control);
            }
            panel.Controls.Add(flow);
            NexusTheme.ApplyControlTree(panel);
        }

        private static void LayoutPasswordPage(Form form, bool distributor)
        {
            if (!distributor) return;
            Control page = Find(form, "tabPage10");
            Control oldPassword = Find(form, "uiTxtoldpwd");
            if (page == null || oldPassword == null) return;

            List<Control> controls = page.Controls.Cast<Control>().ToList();
            Panel card = new Panel { Size = new Size(500, 416), BackColor = NexusTheme.Surface };
            foreach (Control control in controls) card.Controls.Add(control);

            Label title = new Label
            {
                Text = "账号安全",
                Location = new Point(38, 28),
                Size = new Size(400, 42),
                Font = NexusTheme.Font(20F, FontStyle.Bold),
                ForeColor = NexusTheme.Text,
                BackColor = Color.Transparent
            };
            Label subtitle = new Label
            {
                Text = "定期更新密码，保护订单与业务数据安全",
                Location = new Point(40, 72),
                Size = new Size(410, 28),
                Font = NexusTheme.Font(9F),
                ForeColor = NexusTheme.TextSecondary,
                BackColor = Color.Transparent
            };
            card.Controls.Add(title);
            card.Controls.Add(subtitle);

            PositionPasswordRow(card, "uiLabel6", "uiTxtoldpwd", 126);
            PositionPasswordRow(card, "uiLabel5", "uiTxtnewpwd", 192);
            PositionPasswordRow(card, "uiLabel4", "uiTxtcirmpwd", 258);
            Control update = Find(card, "uiButton9");
            if (update != null)
            {
                update.Location = new Point(166, 334);
                update.Size = new Size(270, 42);
            }

            page.Controls.Clear();
            page.Controls.Add(card);
            page.Resize += delegate
            {
                card.Location = new Point(Math.Max(20, (page.ClientSize.Width - card.Width) / 2),
                    Math.Max(20, (page.ClientSize.Height - card.Height) / 2));
            };
            NexusTheme.ApplyControlTree(card);
            title.Font = NexusTheme.Font(20F, FontStyle.Bold);
            title.ForeColor = NexusTheme.Text;
            subtitle.Font = NexusTheme.Font(9F);
            subtitle.ForeColor = NexusTheme.TextSecondary;
        }

        private static void LayoutSyncPanel(Form form)
        {
            Control panel = Find(form, "panel1");
            if (panel == null || panel.Parent == null) return;
            Control parent = panel.Parent;
            panel.Size = new Size(420, 180);
            panel.BackColor = NexusTheme.SurfaceAlt;
            Control label = Find(panel, "uiLabel2");
            Control input = Find(panel, "uiTextBox9");
            Control confirm = Find(panel, "uiButton21");
            Control cancel = Find(panel, "uiButton22");
            if (label != null) { label.Location = new Point(30, 28); label.Size = new Size(120, 28); }
            if (input != null) { input.Location = new Point(142, 24); input.Size = new Size(240, 34); }
            if (confirm != null) { confirm.Location = new Point(142, 90); confirm.Size = new Size(110, 36); }
            if (cancel != null) { cancel.Location = new Point(270, 90); cancel.Size = new Size(110, 36); }
            parent.Resize += delegate
            {
                panel.Location = new Point(Math.Max(0, (parent.ClientSize.Width - panel.Width) / 2),
                    Math.Max(0, (parent.ClientSize.Height - panel.Height) / 2));
            };
        }

        private static void SetLayoutPadding(Form form)
        {
            string[] names =
            {
                "tableLayoutPanelImport", "tableLayoutPanelReconcileSummary", "tableLayoutPanelBaseData",
                "tableLayoutPanelWdtPage", "tableLayoutPanelWdtOrders", "tableLayoutPanelTransferPage",
                "tableLayoutPanelTransferOrders", "tableLayoutPanelPackingQuery"
            };
            foreach (string name in names)
            {
                Control control = Find(form, name);
                if (control != null) control.Padding = new Padding(5);
            }
        }

        private static void PositionPasswordRow(Control card, string labelName, string inputName, int y)
        {
            dynamic label = Find(card, labelName);
            Control input = Find(card, inputName);
            if (label != null)
            {
                label.Location = new Point(38, y + 5);
                label.Size = new Size(112, 32);
                label.TextAlign = ContentAlignment.MiddleRight;
            }
            if (input != null)
            {
                input.Location = new Point(166, y);
                input.Size = new Size(270, 38);
            }
        }

        private static FlowLayoutPanel CreateActionPanel(int height, bool dockBottom)
        {
            return new FlowLayoutPanel
            {
                Dock = dockBottom ? DockStyle.Bottom : DockStyle.None,
                Height = height,
                BackColor = NexusTheme.Surface,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                Padding = new Padding(10, 5, 8, 3),
                Margin = new Padding(0)
            };
        }

        private static void AddButtons(FlowLayoutPanel panel, List<UIButton> buttons, string[] order)
        {
            int index = 0;
            foreach (string name in order)
            {
                UIButton button = buttons.FirstOrDefault(item => item.Name == name);
                if (button == null) continue;
                button.AutoSize = false;
                button.Height = 30;
                button.Width = ButtonWidth(button);
                button.Margin = new Padding(4, 2, 4, 2);
                button.TabIndex = index++;
                panel.Controls.Add(button);
            }
        }

        private static int ButtonWidth(Control button)
        {
            int width = TextRenderer.MeasureText(button.Text ?? string.Empty, NexusTheme.Font(9F)).Width + 30;
            return Math.Max(80, Math.Min(260, width));
        }

        private static Panel CreateField(dynamic label, Control input, dynamic separator, Control secondInput, int width)
        {
            Panel field = new Panel
            {
                Size = new Size(width, 50),
                Margin = new Padding(4, 1, 4, 1),
                BackColor = NexusTheme.Surface
            };
            if (label != null)
            {
                label.Dock = DockStyle.Top;
                label.Height = 19;
                label.TextAlign = ContentAlignment.MiddleLeft;
                field.Controls.Add(label);
            }
            if (secondInput != null)
            {
                if (input != null)
                {
                    input.Dock = DockStyle.None;
                    input.Location = new Point(0, 22);
                    input.Size = new Size((width - 20) / 2, 28);
                    field.Controls.Add(input);
                }
                if (separator != null)
                {
                    separator.Dock = DockStyle.None;
                    separator.Location = new Point((width - 20) / 2 + 2, 22);
                    separator.Size = new Size(16, 28);
                    separator.TextAlign = ContentAlignment.MiddleCenter;
                    field.Controls.Add(separator);
                }
                secondInput.Dock = DockStyle.None;
                secondInput.Location = new Point((width - 20) / 2 + 20, 22);
                secondInput.Size = new Size((width - 20) / 2, 28);
                field.Controls.Add(secondInput);
            }
            else if (input != null)
            {
                input.Dock = DockStyle.Bottom;
                input.Height = 28;
                field.Controls.Add(input);
            }
            return field;
        }

        private static void SetToolbarHeight(Form form, string layoutName, int height)
        {
            TableLayoutPanel layout = Find(form, layoutName) as TableLayoutPanel;
            if (layout != null && layout.RowStyles.Count > 0)
            {
                layout.RowStyles[0].SizeType = SizeType.Absolute;
                layout.RowStyles[0].Height = height;
            }
        }

        private static Control Find(Control root, string name)
        {
            Control[] matches = root.Controls.Find(name, true);
            return matches.Length == 0 ? null : matches[0];
        }

        private sealed class FieldDefinition
        {
            internal string LabelName;
            internal string InputName;
            internal string SecondLabelName;
            internal string SecondInputName;
            internal int Width;
            internal Label LabelInstance;
            internal Control InputInstance;

            internal FieldDefinition(string labelName, string inputName, int width)
            {
                LabelName = labelName;
                InputName = inputName;
                Width = width;
            }

            internal FieldDefinition(Label label, Control input, int width)
            {
                LabelInstance = label;
                InputInstance = input;
                Width = width;
            }

            internal static FieldDefinition DateRange(string labelName, string inputName,
                string secondLabelName, string secondInputName, int width)
            {
                return new FieldDefinition(labelName, inputName, width)
                {
                    SecondLabelName = secondLabelName,
                    SecondInputName = secondInputName
                };
            }
        }
    }
}
