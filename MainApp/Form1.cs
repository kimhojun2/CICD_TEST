using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MainApp
{
    public partial class Form1 : Form
    {
        private Panel cardPanel;
        private Label lblTitle;
        private Label lblSubTitle;
        private Button btnOpenNotice;
        private Label lblStatus;

        public Form1()
        {
            InitializeComponent();
            SetupDinoEggThemeUI();
        }

        private void SetupDinoEggThemeUI()
        {
            // 시스템 기본 둥근 폰트 설정 (기본 맑은 고딕도 둥근 스타일 지원)
            Font roundTitleFont = new Font("나눔스퀘어라운드 Bold", 16f, FontStyle.Bold);
            if (roundTitleFont.Name != "나눔스퀘어라운드 Bold")
                roundTitleFont = new Font("맑은 고딕", 15f, FontStyle.Bold);

            Font roundSubFont = new Font("나눔스퀘어라운드", 9.5f, FontStyle.Regular);
            if (roundSubFont.Name != "나눔스퀘어라운드")
                roundSubFont = new Font("맑은 고딕", 9.5f, FontStyle.Regular);

            // 1. 메인 폼 설정 (부드러운 연노랑/아이보리 배경)
            this.Text = "Main Control Center";
            this.Size = new Size(480, 360); // 폼 크기 여유있게 확장
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(254, 250, 224); // 파스텔 노랑/크림

            // 실행 파일 아이콘 추출
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath))
                {
                    this.Icon = Icon.ExtractAssociatedIcon(exePath);
                }
            }
            catch { }

            // 2. 중앙 메인 카드 패널 (딥 다크 그린 포인트)
            cardPanel = new Panel
            {
                Size = new Size(420, 210),
                Location = new Point(22, 20),
                BackColor = Color.FromArgb(30, 81, 40) // 깊은 초록색
            };

            lblTitle = new Label
            {
                Text = "메인 컨트롤 센터",
                ForeColor = Color.FromArgb(255, 222, 89), // 노랑 포인트
                Font = roundTitleFont,
                Location = new Point(0, 25),
                Size = new Size(420, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // 글씨 짤림 방지를 위해 크기와 위치 재조정
            lblSubTitle = new Label
            {
                Text = "NoticeApp 및 시스템 모듈 통합 관리",
                ForeColor = Color.FromArgb(230, 240, 230),
                Font = roundSubFont,
                Location = new Point(0, 68),
                Size = new Size(420, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // 3. 공지사항 실행 버튼 (공룡 알 초록색 패턴)
            btnOpenNotice = new Button
            {
                Text = "📢  공지사항 모듈 실행",
                Font = new Font(roundTitleFont.FontFamily, 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 81, 40), // 글자색 다크 그린
                BackColor = Color.FromArgb(120, 198, 85), // 공룡 알 연초록
                FlatStyle = FlatStyle.Flat,
                Size = new Size(260, 52), // 버튼 높이/너비 확장하여 텍스트 짤림 방지
                Location = new Point(80, 118),
                Cursor = Cursors.Hand
            };
            btnOpenNotice.FlatAppearance.BorderSize = 0;
            btnOpenNotice.Click += BtnOpenNotice_Click;

            // 버튼 호버 이벤트 (노란색으로 변화)
            btnOpenNotice.MouseEnter += (s, e) => {
                btnOpenNotice.BackColor = Color.FromArgb(255, 210, 63);
            };
            btnOpenNotice.MouseLeave += (s, e) => {
                btnOpenNotice.BackColor = Color.FromArgb(120, 198, 85);
            };

            cardPanel.Controls.Add(lblTitle);
            cardPanel.Controls.Add(lblSubTitle);
            cardPanel.Controls.Add(btnOpenNotice);

            // 4. 하단 상태 안내 라벨
            lblStatus = new Label
            {
                Text = "상태: 시스템 준비 완료",
                Font = roundSubFont,
                ForeColor = Color.FromArgb(60, 90, 60),
                Location = new Point(22, 250),
                Size = new Size(420, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.Add(cardPanel);
            this.Controls.Add(lblStatus);
        }

        private void BtnOpenNotice_Click(object? sender, EventArgs e)
        {
            string noticeAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NoticeApp.exe");

            if (File.Exists(noticeAppPath))
            {
                Process.Start(noticeAppPath);
                lblStatus.Text = $"상태: NoticeApp 실행됨 ({DateTime.Now:HH:mm:ss})";
                lblStatus.ForeColor = Color.FromArgb(34, 139, 34);
            }
            else
            {
                MessageBox.Show($"NoticeApp.exe를 찾을 수 없습니다.\n경로: {noticeAppPath}",
                                "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "상태: NoticeApp.exe 찾을 수 없음";
                lblStatus.ForeColor = Color.FromArgb(178, 34, 34);
            }
        }
    }
}