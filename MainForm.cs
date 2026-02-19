using Microsoft.Data.Sqlite;
using System.Data;

namespace PetShotelApp;

public sealed class MainForm : Form
{
    private readonly string _connectionString;

    private readonly ComboBox _knownDogsComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 380 };
    private readonly TextBox _dogNameTextBox = new() { Width = 220 };
    private readonly TextBox _ownerNameTextBox = new() { Width = 220 };
    private readonly TextBox _ownerMobileTextBox = new() { Width = 220 };
    private readonly DateTimePicker _arrivalDatePicker = new() { Format = DateTimePickerFormat.Short, Width = 120 };
    private readonly DateTimePicker _departureDatePicker = new() { Format = DateTimePickerFormat.Short, Width = 120 };
    private readonly NumericUpDown _feedingsPerDayNumeric = new() { Minimum = 1, Maximum = 12, Value = 2, Width = 100 };
    private readonly NumericUpDown _portionSizeNumeric = new() { Minimum = 1, Maximum = 2000, Value = 200, Width = 100 };

    private readonly NumericUpDown _dailyTariffNumeric = new() { Minimum = 1, Maximum = 5000, Value = 35, Width = 100 };
    private readonly ComboBox _activeStayComboBox = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 380 };
    private readonly Label _checkoutSummaryLabel = new() { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

    private readonly DataGridView _staysGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false
    };

    public MainForm()
    {
        Text = "Pets Hotel - Dog Stays";
        Width = 1000;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        _connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "petshotel.db")}";

        BuildUi();
        EnsureDatabase();
        LoadDogProfiles();
        LoadActiveStays();
        LoadStaysGrid();
    }

    private void BuildUi()
    {
        var tabControl = new TabControl { Dock = DockStyle.Fill };
        tabControl.TabPages.Add(CreateCheckInTab());
        tabControl.TabPages.Add(CreateCheckOutTab());
        tabControl.TabPages.Add(CreateAllStaysTab());

        Controls.Add(tabControl);
    }

    private TabPage CreateCheckInTab()
    {
        var page = new TabPage("Check in");

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            RowCount = 9,
            AutoSize = true,
            Padding = new Padding(16)
        };

        var loadProfileButton = new Button { Text = "Load profile", Width = 160, Height = 30 };
        loadProfileButton.Click += (_, _) => ApplySelectedDogProfile();

        var profileRow = new FlowLayoutPanel { AutoSize = true };
        profileRow.Controls.Add(_knownDogsComboBox);
        profileRow.Controls.Add(loadProfileButton);

        layout.Controls.Add(new Label { Text = "Known dogs", AutoSize = true }, 0, 0);
        layout.Controls.Add(profileRow, 1, 0);

        layout.Controls.Add(new Label { Text = "Dog name", AutoSize = true }, 0, 1);
        layout.Controls.Add(_dogNameTextBox, 1, 1);

        layout.Controls.Add(new Label { Text = "Owner name", AutoSize = true }, 0, 2);
        layout.Controls.Add(_ownerNameTextBox, 1, 2);

        layout.Controls.Add(new Label { Text = "Owner mobile", AutoSize = true }, 0, 3);
        layout.Controls.Add(_ownerMobileTextBox, 1, 3);

        layout.Controls.Add(new Label { Text = "Arrival date", AutoSize = true }, 0, 4);
        layout.Controls.Add(_arrivalDatePicker, 1, 4);

        layout.Controls.Add(new Label { Text = "Departure date", AutoSize = true }, 0, 5);
        layout.Controls.Add(_departureDatePicker, 1, 5);

        layout.Controls.Add(new Label { Text = "Food times/day", AutoSize = true }, 0, 6);
        layout.Controls.Add(_feedingsPerDayNumeric, 1, 6);

        layout.Controls.Add(new Label { Text = "Portion size (grams)", AutoSize = true }, 0, 7);
        layout.Controls.Add(_portionSizeNumeric, 1, 7);

        var checkInButton = new Button { Text = "Check in dog", Width = 160, Height = 35 };
        checkInButton.Click += (_, _) => CheckInDog();
        layout.Controls.Add(checkInButton, 1, 8);

        page.Controls.Add(layout);
        return page;
    }

    private TabPage CreateCheckOutTab()
    {
        var page = new TabPage("Check out");

        var wrapper = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(16),
            WrapContents = false,
            AutoScroll = true
        };

        wrapper.Controls.Add(new Label { Text = "Active stay", AutoSize = true });
        wrapper.Controls.Add(_activeStayComboBox);

        wrapper.Controls.Add(new Label { Text = "Daily tariff", AutoSize = true });
        wrapper.Controls.Add(_dailyTariffNumeric);

        var previewButton = new Button { Text = "Preview total", Width = 150, Height = 35 };
        previewButton.Click += (_, _) => PreviewCheckout();
        wrapper.Controls.Add(previewButton);

        wrapper.Controls.Add(_checkoutSummaryLabel);

        var checkoutButton = new Button { Text = "Close account (Check out)", Width = 220, Height = 40 };
        checkoutButton.Click += (_, _) => CheckOutDog();
        wrapper.Controls.Add(checkoutButton);

        page.Controls.Add(wrapper);
        return page;
    }

    private TabPage CreateAllStaysTab()
    {
        var page = new TabPage("All stays");
        var refreshButton = new Button { Text = "Refresh", Dock = DockStyle.Top, Height = 36 };
        refreshButton.Click += (_, _) =>
        {
            LoadDogProfiles();
            LoadActiveStays();
            LoadStaysGrid();
        };

        page.Controls.Add(_staysGrid);
        page.Controls.Add(refreshButton);
        return page;
    }

    private void EnsureDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS DogStay (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DogName TEXT NOT NULL,
                OwnerName TEXT NOT NULL,
                OwnerMobile TEXT NOT NULL,
                ArrivalDate TEXT NOT NULL,
                DepartureDate TEXT NOT NULL,
                FeedingsPerDay INTEGER NOT NULL,
                PortionSizeGrams INTEGER NOT NULL,
                IsCheckedOut INTEGER NOT NULL DEFAULT 0,
                DailyTariff REAL,
                TotalAmount REAL,
                CheckedOutAt TEXT
            );

            CREATE TABLE IF NOT EXISTS DogProfile (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DogName TEXT NOT NULL,
                OwnerName TEXT NOT NULL,
                OwnerMobile TEXT NOT NULL,
                FeedingsPerDay INTEGER NOT NULL,
                PortionSizeGrams INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                UNIQUE (DogName, OwnerMobile)
            );
            """;

        command.ExecuteNonQuery();
    }

    private void CheckInDog()
    {
        if (string.IsNullOrWhiteSpace(_dogNameTextBox.Text) ||
            string.IsNullOrWhiteSpace(_ownerNameTextBox.Text) ||
            string.IsNullOrWhiteSpace(_ownerMobileTextBox.Text))
        {
            MessageBox.Show("Dog name, owner name, and owner mobile are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var arrival = _arrivalDatePicker.Value.Date;
        var departure = _departureDatePicker.Value.Date;
        if (departure < arrival)
        {
            MessageBox.Show("Departure date must be on or after arrival date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var dogName = _dogNameTextBox.Text.Trim();
        var ownerName = _ownerNameTextBox.Text.Trim();
        var ownerMobile = _ownerMobileTextBox.Text.Trim();
        var feedingsPerDay = (int)_feedingsPerDayNumeric.Value;
        var portionSizeGrams = (int)_portionSizeNumeric.Value;

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO DogStay
                (DogName, OwnerName, OwnerMobile, ArrivalDate, DepartureDate, FeedingsPerDay, PortionSizeGrams)
            VALUES
                ($dog, $owner, $mobile, $arrival, $departure, $feedings, $portion);
            """;

        command.Parameters.AddWithValue("$dog", dogName);
        command.Parameters.AddWithValue("$owner", ownerName);
        command.Parameters.AddWithValue("$mobile", ownerMobile);
        command.Parameters.AddWithValue("$arrival", arrival.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$departure", departure.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$feedings", feedingsPerDay);
        command.Parameters.AddWithValue("$portion", portionSizeGrams);
        command.ExecuteNonQuery();

        SaveOrUpdateDogProfile(dogName, ownerName, ownerMobile, feedingsPerDay, portionSizeGrams);

        MessageBox.Show("Dog checked in successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        ClearCheckInInputs();
        LoadDogProfiles();
        LoadActiveStays();
        LoadStaysGrid();
    }

    private void SaveOrUpdateDogProfile(string dogName, string ownerName, string ownerMobile, int feedingsPerDay, int portionSizeGrams)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO DogProfile
                (DogName, OwnerName, OwnerMobile, FeedingsPerDay, PortionSizeGrams, CreatedAt, UpdatedAt)
            VALUES
                ($dog, $owner, $mobile, $feedings, $portion, $createdAt, $updatedAt)
            ON CONFLICT(DogName, OwnerMobile) DO UPDATE SET
                OwnerName = excluded.OwnerName,
                FeedingsPerDay = excluded.FeedingsPerDay,
                PortionSizeGrams = excluded.PortionSizeGrams,
                UpdatedAt = excluded.UpdatedAt;
            """;

        var now = DateTime.UtcNow.ToString("O");
        command.Parameters.AddWithValue("$dog", dogName);
        command.Parameters.AddWithValue("$owner", ownerName);
        command.Parameters.AddWithValue("$mobile", ownerMobile);
        command.Parameters.AddWithValue("$feedings", feedingsPerDay);
        command.Parameters.AddWithValue("$portion", portionSizeGrams);
        command.Parameters.AddWithValue("$createdAt", now);
        command.Parameters.AddWithValue("$updatedAt", now);
        command.ExecuteNonQuery();
    }

    private void LoadDogProfiles()
    {
        var items = new List<DogProfileItem> { DogProfileItem.Empty };

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, DogName, OwnerName, OwnerMobile, FeedingsPerDay, PortionSizeGrams
            FROM DogProfile
            ORDER BY DogName, OwnerName;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(
                new DogProfileItem(
                    reader.GetInt64(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetInt32(4),
                    reader.GetInt32(5)));
        }

        _knownDogsComboBox.DataSource = items;
        _knownDogsComboBox.DisplayMember = nameof(DogProfileItem.Display);
        _knownDogsComboBox.ValueMember = nameof(DogProfileItem.Id);
        _knownDogsComboBox.SelectedIndex = 0;
    }

    private void ApplySelectedDogProfile()
    {
        if (_knownDogsComboBox.SelectedItem is not DogProfileItem selected || selected.Id == 0)
        {
            MessageBox.Show("Select a known dog profile first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _dogNameTextBox.Text = selected.DogName;
        _ownerNameTextBox.Text = selected.OwnerName;
        _ownerMobileTextBox.Text = selected.OwnerMobile;
        _feedingsPerDayNumeric.Value = selected.FeedingsPerDay;
        _portionSizeNumeric.Value = selected.PortionSizeGrams;
    }

    private void ClearCheckInInputs()
    {
        _knownDogsComboBox.SelectedIndex = 0;
        _dogNameTextBox.Clear();
        _ownerNameTextBox.Clear();
        _ownerMobileTextBox.Clear();
        _arrivalDatePicker.Value = DateTime.Today;
        _departureDatePicker.Value = DateTime.Today;
        _feedingsPerDayNumeric.Value = 2;
        _portionSizeNumeric.Value = 200;
    }

    private void LoadActiveStays()
    {
        var items = new List<StayItem>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, DogName, OwnerName, ArrivalDate, DepartureDate
            FROM DogStay
            WHERE IsCheckedOut = 0
            ORDER BY ArrivalDate;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var id = reader.GetInt64(0);
            var dogName = reader.GetString(1);
            var ownerName = reader.GetString(2);
            var arrival = reader.GetString(3);
            var departure = reader.GetString(4);

            items.Add(new StayItem(id, $"#{id} - {dogName} ({ownerName}) [{arrival} to {departure}]"));
        }

        _activeStayComboBox.DataSource = items;
        _activeStayComboBox.DisplayMember = nameof(StayItem.Display);
        _activeStayComboBox.ValueMember = nameof(StayItem.Id);
        _checkoutSummaryLabel.Text = items.Count == 0 ? "No active stays." : string.Empty;
    }

    private void PreviewCheckout()
    {
        if (_activeStayComboBox.SelectedItem is not StayItem selected)
        {
            _checkoutSummaryLabel.Text = "Select an active stay first.";
            return;
        }

        var pricing = CalculatePricing(selected.Id, (decimal)_dailyTariffNumeric.Value);
        _checkoutSummaryLabel.Text =
            $"Stay days: {pricing.StayDays} | Daily tariff: {pricing.DailyTariff:C} | Total: {pricing.Total:C}";
    }

    private void CheckOutDog()
    {
        if (_activeStayComboBox.SelectedItem is not StayItem selected)
        {
            MessageBox.Show("Select an active stay first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var pricing = CalculatePricing(selected.Id, (decimal)_dailyTariffNumeric.Value);

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE DogStay
            SET IsCheckedOut = 1,
                DailyTariff = $tariff,
                TotalAmount = $total,
                CheckedOutAt = $checkedOutAt
            WHERE Id = $id;
            """;

        command.Parameters.AddWithValue("$tariff", pricing.DailyTariff);
        command.Parameters.AddWithValue("$total", pricing.Total);
        command.Parameters.AddWithValue("$checkedOutAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$id", selected.Id);

        var affected = command.ExecuteNonQuery();
        if (affected == 0)
        {
            MessageBox.Show("Could not close account; record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        MessageBox.Show($"Account closed. Total charge: {pricing.Total:C}", "Check out complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        _checkoutSummaryLabel.Text = string.Empty;
        LoadActiveStays();
        LoadStaysGrid();
    }

    private PricingResult CalculatePricing(long stayId, decimal dailyTariff)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT ArrivalDate, DepartureDate FROM DogStay WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", stayId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new InvalidOperationException("Stay record not found.");
        }

        var arrival = DateTime.Parse(reader.GetString(0)).Date;
        var departure = DateTime.Parse(reader.GetString(1)).Date;
        var stayDays = Math.Max(1, (departure - arrival).Days + 1);
        var total = stayDays * dailyTariff;

        return new PricingResult(stayDays, dailyTariff, total);
    }

    private void LoadStaysGrid()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                Id,
                DogName,
                OwnerName,
                OwnerMobile,
                ArrivalDate,
                DepartureDate,
                FeedingsPerDay,
                PortionSizeGrams,
                CASE WHEN IsCheckedOut = 1 THEN 'Closed' ELSE 'Open' END AS Status,
                DailyTariff,
                TotalAmount,
                CheckedOutAt
            FROM DogStay
            ORDER BY Id DESC;
            """;

        using var adapter = new SqliteDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);

        _staysGrid.DataSource = table;
    }

    private sealed record StayItem(long Id, string Display);
    private sealed record PricingResult(int StayDays, decimal DailyTariff, decimal Total);

    private sealed record DogProfileItem(
        long Id,
        string DogName,
        string OwnerName,
        string OwnerMobile,
        int FeedingsPerDay,
        int PortionSizeGrams)
    {
        public static DogProfileItem Empty => new(0, string.Empty, string.Empty, string.Empty, 2, 200);

        public string Display => Id == 0
            ? "-- Select known dog --"
            : $"{DogName} - {OwnerName} ({OwnerMobile})";
    }
}

internal sealed class SqliteDataAdapter : IDisposable
{
    private readonly SqliteCommand _command;

    public SqliteDataAdapter(SqliteCommand command)
    {
        _command = command;
    }

    public int Fill(DataTable table)
    {
        using var reader = _command.ExecuteReader();
        table.Load(reader);
        return table.Rows.Count;
    }

    public void Dispose()
    {
        _command.Dispose();
    }
}
