namespace PetsHotel.Forms;

partial class CheckOutForm
{
    private System.ComponentModel.IContainer components = null;

    private Label lblDog;
    private Label lblDogValue;
    private Label lblOwner;
    private Label lblOwnerValue;
    private Label lblCheckIn;
    private Label lblCheckInValue;
    private Label lblDays;
    private Label lblDaysValue;
    private Label lblNotes;
    private TextBox txtNotes;
    private Button btnCheckOut;
    private Button btnCancel;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();

        // ── Form ─────────────────────────────────────────────────────────────
        Text            = "Check Out Dog";
        ClientSize      = new Size(400, 320);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        Font            = new Font("Segoe UI", 9F);

        // ── Info labels ───────────────────────────────────────────────────────
        int valueX = 96, valueW = 286;

        lblDog        = MakeLabel("Dog:",       12);
        lblDogValue   = MakeValue(valueX,       12, valueW);
        lblOwner      = MakeLabel("Owner:",     40);
        lblOwnerValue = MakeValue(valueX,       40, valueW);
        lblCheckIn    = MakeLabel("Check-in:",  68);
        lblCheckInValue = MakeValue(valueX,     68, valueW);
        lblDays       = MakeLabel("Stay:",      96);
        lblDaysValue  = MakeValue(valueX,       96, valueW);

        // ── Notes ─────────────────────────────────────────────────────────────
        lblNotes = new Label
        {
            Text      = "Notes:",
            Location  = new Point(12, 136),
            Size      = new Size(60, 20),
            TextAlign = ContentAlignment.MiddleLeft
        };
        txtNotes = new TextBox
        {
            Location   = new Point(12, 158),
            Size       = new Size(370, 80),
            Multiline  = true,
            ScrollBars = ScrollBars.Vertical,
            TabIndex   = 0
        };

        // ── Buttons ───────────────────────────────────────────────────────────
        btnCheckOut = new Button
        {
            Text      = "Confirm Check Out",
            Location  = new Point(180, 256),
            Size      = new Size(120, 30),
            TabIndex  = 1,
            BackColor = Color.FromArgb(0, 128, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCheckOut.Click += btnCheckOut_Click;

        btnCancel = new Button
        {
            Text     = "Cancel",
            Location = new Point(308, 256),
            Size     = new Size(74, 30),
            TabIndex = 2
        };
        btnCancel.Click += btnCancel_Click;

        Controls.AddRange(new Control[]
        {
            lblDog, lblDogValue, lblOwner, lblOwnerValue,
            lblCheckIn, lblCheckInValue, lblDays, lblDaysValue,
            lblNotes, txtNotes, btnCheckOut, btnCancel
        });

        ResumeLayout(false);
    }

    private static Label MakeLabel(string text, int y) =>
        new Label
        {
            Text      = text,
            Location  = new Point(12, y + 2),
            Size      = new Size(80, 20),
            TextAlign = ContentAlignment.MiddleLeft,
            Font      = new Font("Segoe UI", 9F, FontStyle.Bold)
        };

    private static Label MakeValue(int x, int y, int w) =>
        new Label { Location = new Point(x, y + 2), Size = new Size(w, 20), TextAlign = ContentAlignment.MiddleLeft };
}
