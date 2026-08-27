using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace NoticeApp
{
    public class LocalNoticeModel
    {
        public string Title { get; set; } = "시스템 공지사항";
        public string Version { get; set; } = "v1.0.0";
        public string Content { get; set; } = "공룡 알 테마 업데이트가 완료되었습니다.\r\n\r\n- UI/UX 파스텔 톤 적용\r\n- 모듈 통합 연동 완료\r\n- 창 크기 조절 기능 유지\r\n- 커스텀 초록색 스크롤바 손잡이 적용 완료\r\n\r\n스크롤 테스트를 위한 추가 텍스트입니다.\r\n창을 줄이거나 내용을 늘리면 초록색 스크롤바를 조작할 수 있습니다.\r\n감사합니다.";
    }

    // 초록색 테마 커스텀 스크롤바 클래스
    public class GreenScrollBar : Control
    {
        private int value = 0;
        private int maximum = 100;
        private int largeChange = 10;
        private bool isDragging = false;
        private int dragStartY;
        private int dragStartValue;

        public event EventHandler? ValueChanged;

        public int Value
        {
            get => value;
            set
            {
                int maxVal = Math.Max(0, maximum - largeChange);
                int newVal = Math.Max(0, Math.Min(value, maxVal));
                int clamped = Math.Max(0, Math.Min(value, maxVal));
                if (this.value != clamped)
                {
                    this.value = clamped;
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public int Maximum
        {
            get => maximum;
            set { maximum = value; Invalidate(); }
        }

        public int LargeChange
        {
            get => largeChange;
            set { largeChange = value; Invalidate(); }
        }

        public GreenScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Width = 12;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(245, 240, 215)); // 스크롤 트랙 (배경과 자연스러운 연노랑)

            int maxVal = Math.Max(1, maximum - largeChange);
            float ratio = (float)Value / maxVal;

            int thumbHeight = Math.Max(30, (int)((float)largeChange / maximum * Height));
            int thumbY = (int)(ratio * (Height - thumbHeight));

            // 스크롤바 손잡이 (공룡 알 진한 초록색)
            using (SolidBrush thumbBrush = new SolidBrush(Color.FromArgb(120, 198, 85)))
            {
                g.FillRectangle(thumbBrush, 1, thumbY, Width - 2, thumbHeight);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartY = e.Y;
                dragStartValue = Value;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                int deltaY = e.Y - dragStartY;
                int maxVal = Math.Max(1, maximum - largeChange);
                int thumbHeight = Math.Max(30, (int)((float)largeChange / maximum * Height));
                int availableHeight = Height - thumbHeight;

                if (availableHeight > 0)
                {
                    int change = (int)((float)deltaY / availableHeight * maxVal);
                    Value = dragStartValue + change;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
        }
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            LocalNoticeModel notice = new LocalNoticeModel();

            Font roundTitleFont = new Font("나눔스퀘어라운드 Bold", 13f, FontStyle.Bold);
            if (roundTitleFont.Name != "나눔스퀘어라운드 Bold")
                roundTitleFont = new Font("맑은 고딕", 12f, FontStyle.Bold);

            Font roundContentFont = new Font("나눔스퀘어라운드", 10.5f, FontStyle.Bold);
            if (roundContentFont.Name != "나눔스퀘어라운드")
                roundContentFont = new Font("맑은 고딕", 10.5f, FontStyle.Bold);

            Color bgColor = Color.FromArgb(254, 250, 224); // 원래 파스텔 노랑/크림 복원

            Form form = new Form
            {
                Text = $"{notice.Title} ({notice.Version})",
                Width = 480,
                Height = 380,
                MinimumSize = new Size(380, 280),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.Sizable,
                MaximizeBox = true,
                MinimizeBox = true,
                BackColor = bgColor
            };

            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath))
                {
                    form.Icon = Icon.ExtractAssociatedIcon(exePath);
                }
            }
            catch { }

            // 1. 상단 헤더
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(30, 81, 40)
            };

            Label lblHeader = new Label
            {
                Text = $"🥚 {notice.Title}",
                ForeColor = Color.FromArgb(255, 222, 89),
                Font = roundTitleFont,
                Location = new Point(15, 18),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblHeader);

            // 2. 하단 버튼 영역
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = bgColor
            };

            Button btnClose = new Button
            {
                Text = "확인",
                Font = new Font(roundTitleFont.FontFamily, 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 81, 40),
                BackColor = Color.FromArgb(120, 198, 85),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 38),
                Location = new Point((bottomPanel.Width - 110) / 2, 10),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => form.Close();

            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(255, 210, 63);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(120, 198, 85);

            bottomPanel.Resize += (s, e) =>
            {
                btnClose.Location = new Point((bottomPanel.Width - btnClose.Width) / 2, (bottomPanel.Height - btnClose.Height) / 2);
            };
            bottomPanel.Controls.Add(btnClose);

            // 3. 본문 컨테이너 (원래 노랑 크림색 복원)
            Panel contentContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = bgColor,
                Padding = new Padding(20, 15, 5, 15)
            };

            Label lblContent = new Label
            {
                Text = notice.Content,
                Font = roundContentFont,
                ForeColor = Color.FromArgb(20, 50, 25),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            // 커스텀 초록 스크롤바 추가
            GreenScrollBar customScrollBar = new GreenScrollBar
            {
                Dock = DockStyle.Right,
                Width = 14
            };

            // 스크롤바와 본문 연동
            void UpdateScrollLayout()
            {
                int availableWidth = contentContainer.Width - customScrollBar.Width - 35;
                lblContent.MaximumSize = new Size(Math.Max(100, availableWidth), 0);

                int textHeight = lblContent.Height;
                int viewHeight = contentContainer.Height;

                if (textHeight > viewHeight)
                {
                    customScrollBar.Visible = true;
                    customScrollBar.Maximum = textHeight;
                    customScrollBar.LargeChange = viewHeight;
                }
                else
                {
                    customScrollBar.Visible = false;
                    lblContent.Location = new Point(20, 15);
                }
            }

            customScrollBar.ValueChanged += (s, e) =>
            {
                lblContent.Location = new Point(20, 15 - customScrollBar.Value);
            };

            // 마우스 휠 스크롤 지원
            contentContainer.MouseWheel += (s, e) =>
            {
                if (customScrollBar.Visible)
                {
                    customScrollBar.Value -= e.Delta / 2;
                }
            };

            contentContainer.SizeChanged += (s, e) => UpdateScrollLayout();

            contentContainer.Controls.Add(lblContent);
            contentContainer.Controls.Add(customScrollBar);

            form.Controls.Add(contentContainer);
            form.Controls.Add(bottomPanel);
            form.Controls.Add(headerPanel);

            form.Shown += (s, e) =>
            {
                UpdateScrollLayout();
                btnClose.Focus();
            };

            Application.Run(form);
        }
    }
}