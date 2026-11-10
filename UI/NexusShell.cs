using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace SleckEc2KdInterface.UI
{
    internal static class NexusShell
    {
        internal static void ApplyLogin(NexusWindow form)
        {
            NexusTheme.ApplyWindow(form);
            NexusTheme.ApplyControlTree(form);

            form.Text = "登录 · 一件代发";
            form.ClientSize = new Size(1080, 680);
            form.MinimumSize = new Size(1080, 680);
            form.MaximumSize = new Size(1080, 680);
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;

            Control group = FindControl(form, "uiGroupBox1");
            if (group == null) return;
            form.Controls.Remove(group);

            Panel root = new Panel { Dock = DockStyle.Fill, BackColor = NexusTheme.Background };
            NexusLoginBrand brand = new NexusLoginBrand { Dock = DockStyle.Left, Width = 570 };
            Panel loginArea = new Panel { Dock = DockStyle.Fill, BackColor = NexusTheme.Background };
            root.Controls.Add(loginArea);
            root.Controls.Add(brand);
            loginArea.Controls.Add(group);

            group.Dock = DockStyle.None;
            group.Size = new Size(420, 474);
            group.Text = string.Empty;
            group.BackColor = NexusTheme.Surface;
            UIGroupBox uiGroup = group as UIGroupBox;
            if (uiGroup != null)
            {
                uiGroup.FillColor = NexusTheme.Surface;
                uiGroup.RectColor = NexusTheme.Border;
                uiGroup.Padding = new Padding(0);
                uiGroup.Radius = 14;
            }

            Label title = new Label
            {
                Text = "欢迎回来",
                Location = new Point(36, 30),
                Size = new Size(340, 48),
                Font = NexusTheme.Font(24F, FontStyle.Bold),
                ForeColor = NexusTheme.Text,
                BackColor = Color.Transparent
            };
            Label subtitle = new Label
            {
                Text = "登录以进入订单业务工作台",
                Location = new Point(38, 82),
                Size = new Size(340, 28),
                Font = NexusTheme.Font(10F),
                ForeColor = NexusTheme.TextSecondary,
                BackColor = Color.Transparent
            };
            group.Controls.Add(title);
            group.Controls.Add(subtitle);

            Control userLabel = FindControl(group, "uiLabel1");
            Control passwordLabel = FindControl(group, "uiLabel2");
            Control userInput = FindControl(group, "uitxtLoginId");
            Control passwordInput = FindControl(group, "uitxtLoginIPwd");
            Control remember = FindControl(group, "uiCheckBox1");
            Control loginButton = FindControl(group, "uiButton1");

            ConfigureLoginLabel(userLabel, "用户名", 134);
            ConfigureLoginLabel(passwordLabel, "密码", 226);
            ConfigureLoginInput(userInput, 162);
            ConfigureLoginInput(passwordInput, 254);

            if (remember != null)
            {
                remember.Location = new Point(38, 315);
                remember.Size = new Size(220, 30);
                remember.Font = NexusTheme.Font(9F);
            }
            if (loginButton != null)
            {
                loginButton.Location = new Point(38, 366);
                loginButton.Size = new Size(344, 46);
                loginButton.Text = "登  录  →";
                loginButton.Font = NexusTheme.Font(11F, FontStyle.Bold);
                form.AcceptButton = loginButton as IButtonControl;
            }

            Label footer = new Label
            {
                Text = "安全登录  ·  订单协同工作台",
                Location = new Point(38, 432),
                Size = new Size(344, 22),
                Font = NexusTheme.Font(8F),
                ForeColor = NexusTheme.Muted,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            group.Controls.Add(footer);

            loginArea.Resize += delegate
            {
                group.Location = new Point(Math.Max(24, (loginArea.ClientSize.Width - group.Width) / 2),
                    Math.Max(24, (loginArea.ClientSize.Height - group.Height) / 2));
            };
            group.Location = new Point(45, 82);

            NexusTitleBar titleBar = new NexusTitleBar(form, "订单协同工作台", false);
            form.Controls.Add(root);
            form.Controls.Add(titleBar);
            StyleMenus(form);
        }

        internal static void ApplyMain(NexusWindow form, UITabControl tabs, bool distributor)
        {
            if (form == null || tabs == null) return;

            NexusTheme.ApplyWindow(form);
            NexusTheme.ApplyControlTree(form);
            form.Text = distributor ? "一件代发订单业务处理 · 分销商" : "一件代发订单业务处理";
            form.ClientSize = new Size(1500, 900);
            form.MinimumSize = new Size(1280, 760);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Controls.Remove(tabs);

            NexusTitleBar titleBar = new NexusTitleBar(form, "订单协同工作台", true);
            Panel workspace = new Panel { Dock = DockStyle.Fill, BackColor = NexusTheme.Background };
            Panel navigation = new Panel { Dock = DockStyle.Left, Width = 224, BackColor = NexusTheme.Navigation };
            Panel content = new Panel { Dock = DockStyle.Fill, BackColor = NexusTheme.Background };
            Panel pageHeader = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = NexusTheme.Background };
            Panel viewport = new Panel { Dock = DockStyle.Fill, BackColor = NexusTheme.Background };

            workspace.Controls.Add(content);
            workspace.Controls.Add(navigation);
            content.Controls.Add(viewport);
            content.Controls.Add(pageHeader);
            form.Controls.Add(workspace);
            form.Controls.Add(titleBar);

            Label pageTitle = new Label
            {
                Location = new Point(22, 10),
                Size = new Size(560, 36),
                Font = NexusTheme.Font(18F, FontStyle.Bold),
                ForeColor = NexusTheme.Text,
                BackColor = Color.Transparent
            };
            Label pageSubtitle = new Label
            {
                Location = new Point(24, 46),
                Size = new Size(720, 24),
                Font = NexusTheme.Font(9F),
                ForeColor = NexusTheme.TextSecondary,
                BackColor = Color.Transparent
            };
            Label workspaceMark = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Size = new Size(220, 24),
                Font = NexusTheme.Font(8.5F),
                ForeColor = NexusTheme.Muted,
                Text = "简单 · 高效 · 协同",
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };
            pageHeader.Controls.Add(pageTitle);
            pageHeader.Controls.Add(pageSubtitle);
            pageHeader.Controls.Add(workspaceMark);
            pageHeader.Resize += delegate
            {
                workspaceMark.Location = new Point(pageHeader.Width - 244, 28);
            };

            Panel account = new Panel { Dock = DockStyle.Bottom, Height = 74, BackColor = NexusTheme.Navigation };
            NexusAvatar avatar = new NexusAvatar { Location = new Point(20, 20) };
            Label userName = new Label
            {
                Location = new Point(66, 13),
                Size = new Size(136, 25),
                Font = NexusTheme.Font(9.5F, FontStyle.Bold),
                ForeColor = NexusTheme.Text,
                Text = "当前用户",
                BackColor = Color.Transparent
            };
            Label userRole = new Label
            {
                Location = new Point(66, 39),
                Size = new Size(136, 20),
                Font = NexusTheme.Font(8F),
                ForeColor = NexusTheme.Muted,
                Text = distributor ? "分销商工作台" : "客户工作台",
                BackColor = Color.Transparent
            };
            account.Controls.Add(avatar);
            account.Controls.Add(userName);
            account.Controls.Add(userRole);

            Panel navigationTop = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = NexusTheme.Navigation };
            Label navigationTitle = new Label
            {
                Location = new Point(20, 0),
                Size = new Size(120, 54),
                Font = NexusTheme.Font(9F, FontStyle.Bold),
                ForeColor = NexusTheme.TextSecondary,
                Text = "工作导航",
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            Button collapse = new Button
            {
                Location = new Point(174, 12),
                Size = new Size(32, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = NexusTheme.Surface,
                ForeColor = NexusTheme.TextSecondary,
                Font = new Font("Segoe UI", 12F),
                Text = "≡",
                TabStop = false
            };
            collapse.FlatAppearance.BorderSize = 0;
            collapse.FlatAppearance.MouseOverBackColor = NexusTheme.SurfaceAlt;
            navigationTop.Controls.Add(navigationTitle);
            navigationTop.Controls.Add(collapse);

            FlowLayoutPanel navigationFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = NexusTheme.Navigation,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(12, 4, 12, 12),
                Margin = new Padding(0)
            };
            navigation.Controls.Add(navigationFlow);
            navigation.Controls.Add(account);
            navigation.Controls.Add(navigationTop);

            List<NexusNavItem> items = new List<NexusNavItem>();
            List<Label> sections = new List<Label>();
            List<NavEntry> entries = BuildEntries(distributor);
            string currentSection = null;
            foreach (NavEntry entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.Section) && entry.Section != currentSection)
                {
                    Label section = CreateSectionLabel(entry.Section);
                    sections.Add(section);
                    navigationFlow.Controls.Add(section);
                    currentSection = entry.Section;
                }

                NexusNavItem item = new NexusNavItem(entry.Title, entry.Number, entry.PageIndex)
                {
                    Width = 188,
                    Margin = new Padding(0, 2, 0, 2)
                };
                item.Click += delegate { tabs.SelectedIndex = entry.PageIndex; };
                items.Add(item);
                navigationFlow.Controls.Add(item);
            }

            Action updatePage = delegate
            {
                NavEntry selected = entries.Find(entry => entry.PageIndex == tabs.SelectedIndex);
                if (selected == null && entries.Count > 0) selected = entries[0];
                if (selected != null)
                {
                    pageTitle.Text = selected.Title;
                    pageSubtitle.Text = selected.Subtitle;
                }
                foreach (NexusNavItem item in items)
                {
                    item.Selected = item.PageIndex == tabs.SelectedIndex;
                }
            };
            tabs.SelectedIndexChanged += delegate { updatePage(); };
            updatePage();

            bool collapsed = false;
            collapse.Click += delegate
            {
                collapsed = !collapsed;
                navigation.Width = collapsed ? 72 : 224;
                navigationTitle.Visible = !collapsed;
                navigationFlow.Padding = collapsed ? new Padding(10, 4, 10, 12) : new Padding(12, 4, 12, 12);
                foreach (NexusNavItem item in items)
                {
                    item.Compact = collapsed;
                    item.Width = collapsed ? 46 : 188;
                }
                foreach (Label section in sections) section.Visible = !collapsed;
                userName.Visible = !collapsed;
                userRole.Visible = !collapsed;
                account.Height = collapsed ? 16 : 74;
                collapse.Location = new Point(collapsed ? 20 : 174, 12);
            };

            viewport.Controls.Add(tabs);
            tabs.Dock = DockStyle.None;
            tabs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabs.BackColor = NexusTheme.Background;
            Action layoutTabs = delegate
            {
                tabs.SetBounds(-3, -43, viewport.ClientSize.Width + 6, viewport.ClientSize.Height + 46);
            };
            viewport.Resize += delegate { layoutTabs(); };
            layoutTabs();

            form.Load += delegate
            {
                string name = wflogin.fcustName;
                userName.Text = string.IsNullOrWhiteSpace(name) ? "当前用户" : name;
                avatar.Text = string.IsNullOrWhiteSpace(name) ? "订" : name.Trim().Substring(0, 1);
                avatar.Invalidate();
                updatePage();
                layoutTabs();
            };
            form.Shown += delegate
            {
                layoutTabs();
                updatePage();
            };
            StyleMenus(form);
        }

        private static List<NavEntry> BuildEntries(bool distributor)
        {
            List<NavEntry> entries = new List<NavEntry>
            {
                new NavEntry("订单中心", "订单导入", "01", 0, "选择 Excel、校验数据并保存订单"),
                new NavEntry(null, "订单管理", "02", 1, "查询、提交并跟踪订单处理状态"),
                new NavEntry(null, "发货查询", "03", 2, "按日期、单号与收件信息查询发货记录"),
                new NavEntry(null, "退货查询", "04", 3, "查询退货明细并导出所需数据"),
                new NavEntry("经营分析", "对账中心", "05", 4, "统一核对订货、发货与退货数据"),
                new NavEntry("数据中心", "基础资料", "06", 5, "查询物流、仓库、店铺与价目资料")
            };

            if (distributor)
            {
                entries.Add(new NavEntry("账户", "修改密码", "07", 6, "维护当前分销商账号的登录密码"));
            }
            else
            {
                entries.InsertRange(4, new[]
                {
                    new NavEntry("仓储协同", "旺店通订单", "05", 6, "查询旺店通订单并处理重量、箱规与拣货单"),
                    new NavEntry(null, "调拨单", "06", 7, "计算调拨量并生成调拨申请单"),
                    new NavEntry(null, "装箱查询", "07", 8, "按物流单、原始单号与货号查询装箱信息")
                });
                entries[7].Number = "08";
                entries[8].Number = "09";
            }
            return entries;
        }

        private static Label CreateSectionLabel(string text)
        {
            return new Label
            {
                Text = text.ToUpperInvariant(),
                Size = new Size(188, 30),
                Margin = new Padding(8, 12, 0, 2),
                Font = NexusTheme.Font(8F, FontStyle.Bold),
                ForeColor = NexusTheme.Muted,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static void ConfigureLoginLabel(dynamic label, string text, int y)
        {
            if (label == null) return;
            label.Text = text;
            label.Location = new Point(38, y);
            label.Size = new Size(330, 24);
            label.Font = NexusTheme.Font(9F, FontStyle.Bold);
            label.ForeColor = NexusTheme.TextSecondary;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static void ConfigureLoginInput(Control input, int y)
        {
            if (input == null) return;
            input.Location = new Point(38, y);
            input.Size = new Size(344, 38);
            input.Font = NexusTheme.Font(10.5F);
        }

        private static Control FindControl(Control root, string name)
        {
            Control[] matches = root.Controls.Find(name, true);
            return matches.Length == 0 ? null : matches[0];
        }

        private static void StyleMenus(Form form)
        {
            FieldInfo[] fields = form.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                ContextMenuStrip menu = field.GetValue(form) as ContextMenuStrip;
                if (menu != null)
                {
                    menu.Renderer = new NexusMenuRenderer();
                    menu.BackColor = NexusTheme.Surface;
                    menu.ForeColor = NexusTheme.Text;
                    menu.Font = NexusTheme.Font(9F);
                }
            }
        }

        private sealed class NavEntry
        {
            internal string Section;
            internal string Title;
            internal string Number;
            internal int PageIndex;
            internal string Subtitle;

            internal NavEntry(string section, string title, string number, int pageIndex, string subtitle)
            {
                Section = section;
                Title = title;
                Number = number;
                PageIndex = pageIndex;
                Subtitle = subtitle;
            }
        }
    }
}
