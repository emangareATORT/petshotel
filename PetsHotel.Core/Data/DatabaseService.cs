using Microsoft.Data.Sqlite;
using PetsHotel.Models;

namespace PetsHotel.Data;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string dbPath = "petshotel.db")
    {
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Dogs (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Name     TEXT NOT NULL,
                Breed    TEXT NOT NULL,
                Age      INTEGER NOT NULL,
                OwnerName  TEXT NOT NULL,
                OwnerPhone TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Stays (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                DogId       INTEGER NOT NULL REFERENCES Dogs(Id),
                CheckInDate TEXT NOT NULL,
                CheckOutDate TEXT,
                Notes       TEXT NOT NULL DEFAULT ''
            );";
        cmd.ExecuteNonQuery();
    }

    // ── Dogs ─────────────────────────────────────────────────────────────────

    public int AddDog(Dog dog)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Dogs (Name, Breed, Age, OwnerName, OwnerPhone)
            VALUES (@name, @breed, @age, @owner, @phone);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", dog.Name);
        cmd.Parameters.AddWithValue("@breed", dog.Breed);
        cmd.Parameters.AddWithValue("@age", dog.Age);
        cmd.Parameters.AddWithValue("@owner", dog.OwnerName);
        cmd.Parameters.AddWithValue("@phone", dog.OwnerPhone);

        dog.Id = Convert.ToInt32(cmd.ExecuteScalar());
        return dog.Id;
    }

    public void DeleteDog(int dogId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        using var tx = conn.BeginTransaction();

        var deleteStays = conn.CreateCommand();
        deleteStays.Transaction = tx;
        deleteStays.CommandText = "DELETE FROM Stays WHERE DogId = @id";
        deleteStays.Parameters.AddWithValue("@id", dogId);
        deleteStays.ExecuteNonQuery();

        var deleteDog = conn.CreateCommand();
        deleteDog.Transaction = tx;
        deleteDog.CommandText = "DELETE FROM Dogs WHERE Id = @id";
        deleteDog.Parameters.AddWithValue("@id", dogId);
        deleteDog.ExecuteNonQuery();

        tx.Commit();
    }

    public List<Dog> GetAllDogs()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Breed, Age, OwnerName, OwnerPhone FROM Dogs ORDER BY Name";

        var dogs = new List<Dog>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            dogs.Add(new Dog
            {
                Id         = reader.GetInt32(0),
                Name       = reader.GetString(1),
                Breed      = reader.GetString(2),
                Age        = reader.GetInt32(3),
                OwnerName  = reader.GetString(4),
                OwnerPhone = reader.GetString(5)
            });
        }
        return dogs;
    }

    // ── Stays ────────────────────────────────────────────────────────────────

    public int CheckIn(int dogId, string notes = "")
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Stays (DogId, CheckInDate, Notes)
            VALUES (@dogId, @checkIn, @notes);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@dogId", dogId);
        cmd.Parameters.AddWithValue("@checkIn", DateTime.Now.ToString("o"));
        cmd.Parameters.AddWithValue("@notes", notes);

        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void CheckOut(int stayId, string notes = "")
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Stays
            SET CheckOutDate = @checkOut,
                Notes = @notes
            WHERE Id = @id";
        cmd.Parameters.AddWithValue("@checkOut", DateTime.Now.ToString("o"));
        cmd.Parameters.AddWithValue("@notes", notes);
        cmd.Parameters.AddWithValue("@id", stayId);
        cmd.ExecuteNonQuery();
    }

    private static Stay MapStay(SqliteDataReader r) => new Stay
    {
        Id           = r.GetInt32(0),
        DogId        = r.GetInt32(1),
        DogName      = r.GetString(2),
        Breed        = r.GetString(3),
        OwnerName    = r.GetString(4),
        OwnerPhone   = r.GetString(5),
        CheckInDate  = DateTime.Parse(r.GetString(6)),
        CheckOutDate = r.IsDBNull(7) ? null : DateTime.Parse(r.GetString(7)),
        Notes        = r.GetString(8)
    };

    private const string StayQuery = @"
        SELECT s.Id, s.DogId, d.Name, d.Breed, d.OwnerName, d.OwnerPhone,
               s.CheckInDate, s.CheckOutDate, s.Notes
        FROM Stays s
        JOIN Dogs d ON d.Id = s.DogId";

    public List<Stay> GetActiveStays()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = StayQuery + " WHERE s.CheckOutDate IS NULL ORDER BY s.CheckInDate";

        var stays = new List<Stay>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) stays.Add(MapStay(reader));
        return stays;
    }

    public List<Stay> GetAllStays()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = StayQuery + " ORDER BY s.CheckInDate DESC";

        var stays = new List<Stay>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) stays.Add(MapStay(reader));
        return stays;
    }
}
