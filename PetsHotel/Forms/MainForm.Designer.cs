namespace PetsHotel.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private TabControl tabControl;
    private TabPage tabActiveStays;
    private TabPage tabDogs;
    private TabPage tabHistory;

    // Active Stays tab
    private DataGridView dgvActiveStays;
    private Button btnCheckIn;
    private Button btnCheckOut;
    private Button btnRefreshActive;
    private Label lblActiveCount;

    // Dogs tab
    private DataGridView dgvDogs;
    private Button btnAddDog;
    private Button btnDeleteDog;
    private Button btnRefreshDogs;
    private Label lblDogCount;

    // History tab
    private DataGridView dgvHistory;
    private Button btnRefreshHistory;

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
        Text          = "Pets Hotel - Admin";
        ClientSize    = new Size(900, 580);
        MinimumSize   = new Size(700, 480);
        StartPosition = FormStartPosition.CenterScreen;
        Font          = new Font("Segoe UI", 9F);
        Icon          = SystemIcons.Application;

        // ── TabControl ────────────────────────────────────────────────────────
        tabControl = new TabControl
        {
            Dock     = DockStyle.Fill,
            TabIndex = 0
        };

        // ─── Active Stays tab ─────────────────────────────────────────────────
        tabActiveStays = new TabPage { Text = "  Active Stays  " };

        dgvActiveStays = MakeGrid();
        dgvActiveStays.Location = new Point(8, 44);
        dgvActiveStays.Anchor   = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        btnCheckIn = MakeButton("Check In", new Point(8, 8), Color.FromArgb(0, 120, 215), 0);
        btnCheckIn.Click += btnCheckIn_Click;

        btnCheckOut = MakeButton("Check Out", new Point(100, 8), Color.FromArgb(0, 128, 0), 1);
        btnCheckOut.Click += btnCheckOut_Click;

        btnRefreshActive = MakeButton("Refresh", new Point(200, 8), SystemColors.Control, 2);
        btnRefreshActive.ForeColor = SystemColors.ControlText;
        btnRefreshActive.Click += btnRefreshActive_Click;

        lblActiveCount = new Label
        {
            Text      = "Active stays: 0",
            Location  = new Point(304, 14),
            Size      = new Size(200, 20),
            ForeColor = Color.Gray
        };

        tabActiveStays.Controls.AddRange(new Control[]
            { btnCheckIn, btnCheckOut, btnRefreshActive, lblActiveCount, dgvActiveStays });
        tabControl.TabPages.Add(tabActiveStays);

        // ─── Dogs tab ─────────────────────────────────────────────────────────
        tabDogs = new TabPage { Text = "  All Dogs  " };

        dgvDogs = MakeGrid();
        dgvDogs.Location = new Point(8, 44);
        dgvDogs.Anchor   = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        btnAddDog = MakeButton("Add Dog", new Point(8, 8), Color.FromArgb(0, 120, 215), 0);
        btnAddDog.Click += btnAddDog_Click;

        btnDeleteDog = MakeButton("Delete Dog", new Point(100, 8), Color.FromArgb(200, 0, 0), 1);
        btnDeleteDog.Click += btnDeleteDog_Click;

        btnRefreshDogs = MakeButton("Refresh", new Point(200, 8), SystemColors.Control, 2);
        btnRefreshDogs.ForeColor = SystemColors.ControlText;
        btnRefreshDogs.Click += btnRefreshDogs_Click;

        lblDogCount = new Label
        {
            Text      = "Total dogs: 0",
            Location  = new Point(304, 14),
            Size      = new Size(200, 20),
            ForeColor = Color.Gray
        };

        tabDogs.Controls.AddRange(new Control[]
            { btnAddDog, btnDeleteDog, btnRefreshDogs, lblDogCount, dgvDogs });
        tabControl.TabPages.Add(tabDogs);

        // ─── History tab ──────────────────────────────────────────────────────
        tabHistory = new TabPage { Text = "  Stay History  " };

        dgvHistory = MakeGrid();
        dgvHistory.Location = new Point(8, 44);
        dgvHistory.Anchor   = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        btnRefreshHistory = MakeButton("Refresh", new Point(8, 8), SystemColors.Control, 0);
        btnRefreshHistory.ForeColor = SystemColors.ControlText;
        btnRefreshHistory.Click += btnRefreshHistory_Click;

        tabHistory.Controls.AddRange(new Control[] { btnRefreshHistory, dgvHistory });
        tabControl.TabPages.Add(tabHistory);

        Controls.Add(tabControl);

        ResumeLayout(false);
    }

    private static DataGridView MakeGrid() =>
        new DataGridView
        {
            Size                  = new Size(876, 490),
            ReadOnly              = true,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.None,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible     = false,
            BackgroundColor       = Color.White,
            BorderStyle           = BorderStyle.None,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            TabIndex              = 9
        };

    private static Button MakeButton(string text, Point loc, Color back, int tab) =>
        new Button
        {
            Text      = text,
            Location  = loc,
            Size      = new Size(86, 28),
            BackColor = back,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            TabIndex  = tab
        };
}
