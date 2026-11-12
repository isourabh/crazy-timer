namespace CrazyTimer;

public sealed class StopwatchForm : Form
{
    private readonly StopwatchModel stopwatch = new();
    private readonly System.Windows.Forms.Timer refreshTimer = new() { Interval = 30 };
    private readonly Font displayFont = new("Consolas", 30, FontStyle.Regular);
    private readonly Label display = new()
    {
        Text = "00:00:00.00",
        AccessibleName = "Elapsed time",
        AutoSize = true,
        Anchor = AnchorStyles.None,
        TextAlign = ContentAlignment.MiddleCenter,
        Margin = new Padding(12, 24, 12, 24)
    };
    private readonly Button start = CreateButton("&Start", 0);
    private readonly Button stop = CreateButton("S&top", 1);
    private readonly Button reset = CreateButton("&Reset", 2);

    public StopwatchForm()
    {
        Text = "Crazy Timer";
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleDimensions = new SizeF(96, 96);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Padding = new Padding(20);

        display.Font = displayFont;
        var buttons = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty
        };
        buttons.Controls.Add(start, 0, 0);
        buttons.Controls.Add(stop, 1, 0);
        buttons.Controls.Add(reset, 2, 0);

        var layout = new TableLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty
        };
        layout.Controls.Add(display, 0, 0);
        layout.Controls.Add(buttons, 0, 1);
        Controls.Add(layout);

        start.Click += (_, _) => { stopwatch.Start(); UpdateState(); };
        stop.Click += (_, _) => { stopwatch.Stop(); UpdateState(); };
        reset.Click += (_, _) => { stopwatch.Reset(); UpdateState(); };
        refreshTimer.Tick += (_, _) => UpdateDisplay();
        UpdateState();
    }

    private static Button CreateButton(string text, int tabIndex) => new()
    {
        Text = text,
        TabIndex = tabIndex,
        AutoSize = true,
        MinimumSize = new Size(100, 40),
        Margin = new Padding(6),
        Dock = DockStyle.Fill,
        UseVisualStyleBackColor = true
    };

    private void UpdateDisplay() => display.Text = StopwatchModel.FormatElapsed(stopwatch.Elapsed);

    private void UpdateState()
    {
        start.Enabled = !stopwatch.IsRunning;
        stop.Enabled = stopwatch.IsRunning;
        refreshTimer.Enabled = stopwatch.IsRunning;
        UpdateDisplay();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            refreshTimer.Dispose();

        base.Dispose(disposing);

        if (disposing)
            displayFont.Dispose();
    }
}
