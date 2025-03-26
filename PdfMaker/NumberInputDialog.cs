using System;
using System.ComponentModel;
using System.Windows.Forms;

public sealed class NumberInputDialog : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int SelectNumber { get; private set; }

    FlowLayoutPanel flowLayoutPanel;
    NumericUpDown numericUpDown;

    public NumberInputDialog()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.DoubleBuffered = true;
        this.KeyPreview = true; // 키 이벤트 수신

        StartPosition = FormStartPosition.CenterScreen;
        AutoSize = true;                  // 폼 크기 자동조정 활성화
        AutoSizeMode = AutoSizeMode.GrowAndShrink;


        flowLayoutPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(10),
            AutoSize = true,             // FlowLayoutPanel 크기도 자동조정
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false         // 자동 줄바꿈 방지 (세로 정렬이므로 false 권장)
        };

        flowLayoutPanel.Controls.Add(new Label()
        {
            Text = @"Input Repeat Count",
            TextAlign = ContentAlignment.MiddleCenter,
            Width = 150
        });

        numericUpDown = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 5000,
            DecimalPlaces = 0,
            Dock = DockStyle.Top,
            Font = new System.Drawing.Font("Segoe UI", 12),
            TextAlign = HorizontalAlignment.Center
        };

        flowLayoutPanel.Controls.Add(numericUpDown);
        Controls.Add(flowLayoutPanel);
    }


    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Enter)
        {
            this.DialogResult = DialogResult.OK;
            this.SelectNumber = (int)numericUpDown.Value;
            this.Close();
        }
        else if (e.KeyCode == Keys.Escape)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
