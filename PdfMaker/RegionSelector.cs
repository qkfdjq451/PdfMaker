using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

public class RegionSelector : Form
{

    public Rectangle SelectedRegion { get; private set; }
    // 이전 영역 정보를 받아서 초기화
    public RegionSelector()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.Opacity = 0.3; // 반투명 배경
        this.BackColor = Color.Black;
        this.DoubleBuffered = true;
        this.KeyPreview = true; // 키 이벤트 수신
    }

    // WS_SIZEBOX 스타일 추가하여 창 크기 조절 가능하게 함
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style |= 0x00040000; // WS_SIZEBOX
            return cp;
        }
    }

    // 창 이동을 위해 Win32 API 함수 사용
    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();
    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    // 중앙 영역(모서리 20px 제외)에서 마우스 다운 시 창 이동 처리
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        int margin = 20; // 모서리 영역은 크기 조절 용도로 남김
        if (e.Button == MouseButtons.Left)
        {
            if (e.X > margin && e.X < this.Width - margin && e.Y > margin && e.Y < this.Height - margin)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Enter)
        {
            this.DialogResult = DialogResult.OK;
            this.SelectedRegion = this.Bounds;
            this.Close();
        }
        else if (e.KeyCode == Keys.Escape)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    // 모서리만 표시 (각 모서리 20px)
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using (Pen pen = new Pen(Color.Red, 2))
        {
            int cornerSize = 20;
            // 좌측 상단
            e.Graphics.DrawLine(pen, 0, 0, cornerSize, 0);
            e.Graphics.DrawLine(pen, 0, 0, 0, cornerSize);
            // 우측 상단
            e.Graphics.DrawLine(pen, this.Width, 0, this.Width - cornerSize, 0);
            e.Graphics.DrawLine(pen, this.Width, 0, this.Width, cornerSize);
            // 좌측 하단
            e.Graphics.DrawLine(pen, 0, this.Height, cornerSize, this.Height);
            e.Graphics.DrawLine(pen, 0, this.Height, 0, this.Height - cornerSize);
            // 우측 하단
            e.Graphics.DrawLine(pen, this.Width, this.Height, this.Width - cornerSize, this.Height);
            e.Graphics.DrawLine(pen, this.Width, this.Height, this.Width, this.Height - cornerSize);
        }
    }
}
