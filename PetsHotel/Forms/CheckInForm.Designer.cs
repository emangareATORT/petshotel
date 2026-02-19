namespace PetsHotel.Forms;

partial class CheckInForm
{
    private System.ComponentModel.IContainer components = null;

    // Existing dog controls
    private RadioButton radExisting;
    private RadioButton radNew;
    private Panel pnlExisting;
    private Label lblDog;
    private ComboBox cmbDog;

    // New dog controls
    private Panel pnlNewDog;
    private Label lblName;
    private TextBox txtName;
    private Label lblBreed;
    private TextBox txtBreed;
    private Label lblAge;
    private TextBox txtAge;
    private Label lblOwnerName;
    private TextBox txtOwnerName;
    private Label lblOwnerPhone;
    private TextBox txtOwnerPhone;

    // Notes
    private Label lblNotes;
    private TextBox txtNotes;

    // Buttons
    private Button btnCheckIn;
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
        Text            = "Check In Dog";
        ClientSize      = new Size(440, 480);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        StartPosition   = FormStartPosition.CenterParent;
        Font            = new Font("Segoe UI", 9F);
        Padding         = new Padding(12);

        // ── Radio buttons ─────────────────────────────────────────────────────
        radExisting = new RadioButton
        {
            Text     = "Select existing dog",
            Checked  = true,
            Location = new Point(12, 14),
            Size     = new Size(180, 22),
            TabIndex = 0
        };
        radExisting.CheckedChanged += radExisting_CheckedChanged;

        radNew = new RadioButton
        {
            Text     = "Register new dog",
            Location = new Point(200, 14),
            Size     = new Size(160, 22),
            TabIndex = 1
        };
        radNew.CheckedChanged += radExisting_CheckedChanged;

        // ── Existing dog panel ────────────────────────────────────────────────
        pnlExisting = new Panel { Location = new Point(12, 44), Size = new Size(410, 36) };

        lblDog = new Label { Text = "Dog:", Location = new Point(0, 8), Size = new Size(50, 20), TextAlign = ContentAlignment.MiddleLeft };
        cmbDog = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(56, 4), Size = new Size(350, 24), TabIndex = 2 };

        pnlExisting.Controls.AddRange(new Control[] { lblDog, cmbDog });

        // ── New dog panel ─────────────────────────────────────────────────────
        pnlNewDog = new Panel { Location = new Point(12, 44), Size = new Size(410, 200), Visible = false };

        int row = 0;
        int inputX = 96, inputW = 310;

        lblName      = MakeLabel("Name:",        row); txtName      = MakeTextBox(inputX, row, inputW, 3);  row += 30;
        lblBreed     = MakeLabel("Breed:",       row); txtBreed     = MakeTextBox(inputX, row, inputW, 4);  row += 30;
        lblAge       = MakeLabel("Age:",         row); txtAge       = MakeTextBox(inputX, row, 60,    5);   row += 30;
        lblOwnerName = MakeLabel("Owner Name:",  row); txtOwnerName = MakeTextBox(inputX, row, inputW, 6);  row += 30;
        lblOwnerPhone= MakeLabel("Owner Phone:", row); txtOwnerPhone= MakeTextBox(inputX, row, inputW, 7);

        pnlNewDog.Controls.AddRange(new Control[]
        {
            lblName, txtName, lblBreed, txtBreed, lblAge, txtAge,
            lblOwnerName, txtOwnerName, lblOwnerPhone, txtOwnerPhone
        });

        // ── Notes ─────────────────────────────────────────────────────────────
        lblNotes = new Label
        {
            Text     = "Notes:",
            Location = new Point(12, 258),
            Size     = new Size(60, 20),
            TextAlign = ContentAlignment.MiddleLeft
        };
        txtNotes = new TextBox
        {
            Location  = new Point(12, 280),
            Size      = new Size(410, 80),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            TabIndex  = 8
        };

        // ── Buttons ───────────────────────────────────────────────────────────
        btnCheckIn = new Button
        {
            Text      = "Check In",
            Location  = new Point(238, 378),
            Size      = new Size(88, 30),
            TabIndex  = 9,
            BackColor = Color.FromArgb(0, 120, 215),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCheckIn.Click += btnCheckIn_Click;

        btnCancel = new Button
        {
            Text     = "Cancel",
            Location = new Point(334, 378),
            Size     = new Size(88, 30),
            TabIndex = 10
        };
        btnCancel.Click += btnCancel_Click;

        Controls.AddRange(new Control[]
        {
            radExisting, radNew, pnlExisting, pnlNewDog,
            lblNotes, txtNotes, btnCheckIn, btnCancel
        });

        ResumeLayout(false);
    }

    private static Label MakeLabel(string text, int y) =>
        new Label { Text = text, Location = new Point(0, y + 4), Size = new Size(90, 20), TextAlign = ContentAlignment.MiddleLeft };

    private static TextBox MakeTextBox(int x, int y, int w, int tab) =>
        new TextBox { Location = new Point(x, y), Size = new Size(w, 24), TabIndex = tab };
}
