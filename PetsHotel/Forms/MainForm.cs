using PetsHotel.Data;
using PetsHotel.Models;

namespace PetsHotel.Forms;

public partial class MainForm : Form
{
    private readonly DatabaseService _db;

    public MainForm()
    {
        _db = new DatabaseService();
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        LoadActiveStays();
        LoadDogs();
        LoadStayHistory();
    }

    // ── Active Stays tab ─────────────────────────────────────────────────────

    private void LoadActiveStays()
    {
        var stays = _db.GetActiveStays();
        dgvActiveStays.DataSource = stays.Select(s => new
        {
            s.Id,
            Dog         = s.DogName,
            Breed       = s.Breed,
            Owner       = s.OwnerName,
            Phone       = s.OwnerPhone,
            s.CheckInDate,
            Days        = s.DaysStayed,
            s.Notes
        }).ToList();

        dgvActiveStays.Columns["Id"].Visible = false;
        SetColumnWidths(dgvActiveStays, ("Dog", 120), ("Breed", 110), ("Owner", 120),
            ("Phone", 110), ("CheckInDate", 130), ("Days", 50), ("Notes", 180));

        lblActiveCount.Text = $"Active stays: {stays.Count}";
        btnCheckOut.Enabled = stays.Count > 0;
    }

    private void btnCheckIn_Click(object sender, EventArgs e)
    {
        var dogs = _db.GetAllDogs();
        using var form = new CheckInForm(dogs);
        if (form.ShowDialog(this) != DialogResult.OK || form.SelectedDog == null) return;

        var dog = form.SelectedDog;
        if (dog.Id == 0)
            dog.Id = _db.AddDog(dog);

        _db.CheckIn(dog.Id, form.Notes);
        LoadActiveStays();
        LoadDogs();
        LoadStayHistory();
        tabControl.SelectedIndex = 0;
    }

    private void btnCheckOut_Click(object sender, EventArgs e)
    {
        var stay = GetSelectedActiveStay();
        if (stay == null) return;

        using var form = new CheckOutForm(stay);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        _db.CheckOut(stay.Id, form.Notes);
        LoadActiveStays();
        LoadStayHistory();
    }

    private void btnRefreshActive_Click(object sender, EventArgs e) => LoadActiveStays();

    private Stay? GetSelectedActiveStay()
    {
        if (dgvActiveStays.CurrentRow == null) return null;

        var row = dgvActiveStays.CurrentRow;
        var stays = _db.GetActiveStays();
        int id = (int)row.Cells["Id"].Value;
        return stays.FirstOrDefault(s => s.Id == id);
    }

    // ── Dogs tab ─────────────────────────────────────────────────────────────

    private void LoadDogs()
    {
        var dogs = _db.GetAllDogs();
        dgvDogs.DataSource = dogs.Select(d => new
        {
            d.Id,
            d.Name,
            d.Breed,
            d.Age,
            Owner = d.OwnerName,
            Phone = d.OwnerPhone
        }).ToList();

        dgvDogs.Columns["Id"].Visible = false;
        SetColumnWidths(dgvDogs, ("Name", 130), ("Breed", 130), ("Age", 50),
            ("Owner", 140), ("Phone", 120));

        lblDogCount.Text = $"Total dogs: {dogs.Count}";
        btnDeleteDog.Enabled = dogs.Count > 0;
    }

    private void btnAddDog_Click(object sender, EventArgs e)
    {
        using var form = new CheckInForm(new List<Dog>());
        form.Text = "Add New Dog";
        if (form.ShowDialog(this) != DialogResult.OK || form.SelectedDog == null) return;

        var dog = form.SelectedDog;
        if (dog.Id == 0) _db.AddDog(dog);
        LoadDogs();
    }

    private void btnDeleteDog_Click(object sender, EventArgs e)
    {
        if (dgvDogs.CurrentRow == null) return;

        var row  = dgvDogs.CurrentRow;
        var name = row.Cells["Name"].Value?.ToString() ?? "this dog";
        int id   = (int)row.Cells["Id"].Value;

        if (MessageBox.Show($"Delete '{name}'? This will also remove their stay records.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        _db.DeleteDog(id);
        LoadDogs();
        LoadActiveStays();
        LoadStayHistory();
    }

    private void btnRefreshDogs_Click(object sender, EventArgs e) => LoadDogs();

    // ── Stay History tab ─────────────────────────────────────────────────────

    private void LoadStayHistory()
    {
        var stays = _db.GetAllStays();
        dgvHistory.DataSource = stays.Select(s => new
        {
            s.Id,
            Dog        = s.DogName,
            Breed      = s.Breed,
            Owner      = s.OwnerName,
            CheckIn    = s.CheckInDate,
            CheckOut   = s.CheckOutDate.HasValue ? s.CheckOutDate.Value.ToString("dd MMM yyyy HH:mm") : "—",
            Days       = s.DaysStayed,
            Status     = s.IsActive ? "Active" : "Checked out",
            s.Notes
        }).ToList();

        dgvHistory.Columns["Id"].Visible = false;
        SetColumnWidths(dgvHistory, ("Dog", 110), ("Breed", 110), ("Owner", 110),
            ("CheckIn", 130), ("CheckOut", 130), ("Days", 50), ("Status", 90), ("Notes", 160));
    }

    private void btnRefreshHistory_Click(object sender, EventArgs e) => LoadStayHistory();

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static void SetColumnWidths(DataGridView grid, params (string Name, int Width)[] cols)
    {
        foreach (var (name, width) in cols)
        {
            if (grid.Columns.Contains(name))
                grid.Columns[name].Width = width;
        }
    }
}
