using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

public class FocusDialog : Form
{
    private ListBox listBoxWindows;
    private Button buttonOK;
    private Button buttonCancel;
    private Button buttonRefresh;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public WindowInfo SelectedWindow { get; private set; }

    public FocusDialog()
    {
        InitializeComponents();
        LoadWindowList();
    }

    private void InitializeComponents()
    {
        this.Text = "실행 중인 창 목록";
        this.ClientSize = new System.Drawing.Size(400, 300);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        listBoxWindows = new ListBox();
        listBoxWindows.Location = new System.Drawing.Point(10, 10);
        listBoxWindows.Size = new System.Drawing.Size(380, 210);
        this.Controls.Add(listBoxWindows);

        buttonOK = new Button();
        buttonOK.Text = "확인";
        buttonOK.Location = new System.Drawing.Point(220, 230);
        buttonOK.Click += ButtonOK_Click;
        this.Controls.Add(buttonOK);

        buttonCancel = new Button();
        buttonCancel.Text = "취소";
        buttonCancel.Location = new System.Drawing.Point(310, 230);
        buttonCancel.Click += (s, e) =>
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        };
        this.Controls.Add(buttonCancel);

        buttonRefresh = new Button();
        buttonRefresh.Text = "새로고침";
        buttonRefresh.Location = new System.Drawing.Point(10, 230);
        buttonRefresh.Click += ButtonRefresh_Click;
        this.Controls.Add(buttonRefresh);
    }

    private void LoadWindowList()
    {
        listBoxWindows.Items.Clear();
        List<WindowInfo> windows = WindowEnumerator.GetOpenWindows();
        foreach (var win in windows)
        {
            listBoxWindows.Items.Add(win);
        }
    }

    private void ButtonRefresh_Click(object sender, EventArgs e)
    {
        LoadWindowList();
    }

    private void ButtonOK_Click(object sender, EventArgs e)
    {
        if (listBoxWindows.SelectedItem is WindowInfo win)
        {
            SelectedWindow = win;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            MessageBox.Show("창을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}



public class WindowInfo
{
    public IntPtr Handle { get; set; }
    public string Title { get; set; }
    public override string ToString() => $"{Title} (0x{Handle.ToString("X")})";
}

// Win32 API를 이용해 창을 열거하는 헬퍼 클래스
public static class WindowEnumerator
{
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    public static List<WindowInfo> GetOpenWindows()
    {
        List<WindowInfo> windows = new List<WindowInfo>();

        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                int length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    string title = sb.ToString();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        windows.Add(new WindowInfo { Handle = hWnd, Title = title });
                    }
                }
            }
            return true; // 계속 열거
        }, IntPtr.Zero);

        return windows;
    }
}