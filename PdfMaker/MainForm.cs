using System.Diagnostics;
using System.Drawing.Imaging;
using System.Reflection.Metadata;

public class MainForm : Form
{
    List<Button> toggleButtonList = [];
    CancellationTokenSource? cancellationTokenSource = null;
    ProgressBar ProgressBar { get; }
    public MainForm()
    {
        this.Text = "캡쳐 매크로";

        AutoSize = true;                  // 폼 크기 자동조정 활성화
        AutoSizeMode = AutoSizeMode.GrowAndShrink;


        var flowLayoutPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(10),
            AutoSize = true,             // FlowLayoutPanel 크기도 자동조정
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false         // 자동 줄바꿈 방지 (세로 정렬이므로 false 권장)
        };

        var startButton = new Button();
        startButton.Text = "시작하기";        
        startButton.Click += StartButton_Click;
        flowLayoutPanel.Controls.Add(startButton);
        toggleButtonList.Add(startButton);

        var startSelectProcessButton = new Button()
        {
            Width = 150,
        };
        startSelectProcessButton.Text = "시작하기(앱선택)";
        startSelectProcessButton.Click += StartSelectProcessButton_Click;
        flowLayoutPanel.Controls.Add(startSelectProcessButton);
        toggleButtonList.Add(startSelectProcessButton);

        var cancelButton = new Button();
        cancelButton.Text = "취소하기";
        cancelButton.Click += CancelButton_Click;
        cancelButton.Enabled = false;
        flowLayoutPanel.Controls.Add(cancelButton);
        toggleButtonList.Add(cancelButton);

        ProgressBar = new ProgressBar()
        {
            Width = 150,
        };
        flowLayoutPanel.Controls.Add(ProgressBar);

        this.Controls.Add(flowLayoutPanel);
    }
    
    private void CancelButton_Click(object? sender, EventArgs e)
    {
        cancellationTokenSource?.Cancel();
    }


    private void ToggleButtonEnable()
    {
        foreach (var button in toggleButtonList)
        {
            button.Enabled = !button.Enabled;
        }
    }

    private async void StartSelectProcessButton_Click(object? sender, EventArgs e)
    {
        IntPtr handle;
        int repeatCount;

        using (var numberInput = new NumberInputDialog())
        {
            if (numberInput.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            repeatCount = numberInput.SelectNumber;
        }
        if (repeatCount == 0)
        {
            return;
        }

        using (var focusDialog = new FocusDialog())
        {
            if (focusDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            handle = focusDialog.SelectedWindow.Handle;
        }

        using (cancellationTokenSource = new CancellationTokenSource())
        {
            await this.Start(handle, repeatCount, cancellationTokenSource.Token);
        }
    }

    private async void StartButton_Click(object sender, EventArgs e)
    {
        Rectangle selectRegion;
        IntPtr handle;
        int repeatCount;

        using (var regionSelector = new RegionSelector())
        {
            if (regionSelector.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            selectRegion = regionSelector.SelectedRegion;
        }

        using (var numberInput = new NumberInputDialog())
        {
            if (numberInput.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            repeatCount = (int)numberInput.SelectNumber;
        }
        if (repeatCount == 0)
        {
            return;
        }

        using (var focusDialog = new FocusDialog())
        {
            if (focusDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            handle = focusDialog.SelectedWindow.Handle;
        }

        using (cancellationTokenSource = new CancellationTokenSource())
        {
            await this.Start(handle, selectRegion, repeatCount, cancellationTokenSource.Token);
        }
    }

    public Task Start(IntPtr handle, int repeatCount, CancellationToken token)
    {
        return this.Start(handle, NativeMethods.GetWindowRectangle(handle), repeatCount, token);
    }

    public Task Start(IntPtr handle, Rectangle selectRegion, int repeatCount, CancellationToken token)
    {
        NativeMethods.SetForegroundWindow(handle);
        this.ToggleButtonEnable();
        return Task.Run(async () =>
        {
            var path = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmSS"));
            Directory.CreateDirectory(path);
            ProgressBar.Maximum = repeatCount;
            try
            {
                var rect = selectRegion with { Width = selectRegion.Width - 440, X = selectRegion.X + 220 };
                for (var i = 0; i < repeatCount; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Capture(rect, path, i);
                    await Task.Delay(500, token);
                    KeySender.SendRightArrow();
                    await Task.Delay(500, token);
                    ProgressBar.Value = i + 1;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                this.Invoke(this.ToggleButtonEnable);
            }

            Process.Start("explorer.exe", path);
            return;

            void Capture(Rectangle rectangle, string path, int index)
            {
                using Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height);
                using Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rectangle.Location, Point.Empty, rectangle.Size);

                bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                bmp.Save($"{path}/capture{index:D4}.png", ImageFormat.Png);
            }
        }, token);
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new MainForm());
    }
}