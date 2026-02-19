using PetsHotel.Data;
using PetsHotel.Models;

namespace PetsHotel.Tests;

public class DatabaseServiceTests : IDisposable
{
    private readonly string _dbPath;
    private readonly DatabaseService _db;

    public DatabaseServiceTests()
    {
        // Each test gets its own temp database
        _dbPath = Path.GetTempFileName();
        _db = new DatabaseService(_dbPath);
    }

    public void Dispose()
    {
        try { File.Delete(_dbPath); } catch { }
        try { File.Delete(_dbPath + "-shm"); } catch { }
        try { File.Delete(_dbPath + "-wal"); } catch { }
    }

    // ── Dog tests ─────────────────────────────────────────────────────────────

    [Fact]
    public void AddDog_AssignsId()
    {
        var dog = new Dog { Name = "Rex", Breed = "Lab", Age = 3, OwnerName = "Alice", OwnerPhone = "555-1111" };
        int id = _db.AddDog(dog);
        Assert.True(id > 0);
        Assert.Equal(id, dog.Id);
    }

    [Fact]
    public void GetAllDogs_ReturnsAddedDogs()
    {
        _db.AddDog(new Dog { Name = "Rex", Breed = "Lab", Age = 3, OwnerName = "Alice", OwnerPhone = "555-1111" });
        _db.AddDog(new Dog { Name = "Buddy", Breed = "Poodle", Age = 5, OwnerName = "Bob", OwnerPhone = "555-2222" });

        var dogs = _db.GetAllDogs();
        Assert.Equal(2, dogs.Count);
    }

    [Fact]
    public void GetAllDogs_ReturnsSortedByName()
    {
        _db.AddDog(new Dog { Name = "Zeus", Breed = "Husky", Age = 2, OwnerName = "C", OwnerPhone = "" });
        _db.AddDog(new Dog { Name = "Arlo", Breed = "Beagle", Age = 1, OwnerName = "D", OwnerPhone = "" });

        var dogs = _db.GetAllDogs();
        Assert.Equal("Arlo", dogs[0].Name);
        Assert.Equal("Zeus", dogs[1].Name);
    }

    [Fact]
    public void DeleteDog_RemovesDog()
    {
        var dog = new Dog { Name = "Rex", Breed = "Lab", Age = 3, OwnerName = "Alice", OwnerPhone = "" };
        _db.AddDog(dog);

        _db.DeleteDog(dog.Id);

        Assert.Empty(_db.GetAllDogs());
    }

    // ── Stay tests ────────────────────────────────────────────────────────────

    [Fact]
    public void CheckIn_CreatesActiveStay()
    {
        var dog = new Dog { Name = "Max", Breed = "Labrador", Age = 4, OwnerName = "Dave", OwnerPhone = "555-3333" };
        _db.AddDog(dog);

        int stayId = _db.CheckIn(dog.Id, "No special needs");

        Assert.True(stayId > 0);
        var active = _db.GetActiveStays();
        Assert.Single(active);
        Assert.Equal("Max", active[0].DogName);
        Assert.Equal("No special needs", active[0].Notes);
    }

    [Fact]
    public void CheckOut_EndsStay()
    {
        var dog = new Dog { Name = "Biscuit", Breed = "Spaniel", Age = 2, OwnerName = "Eve", OwnerPhone = "" };
        _db.AddDog(dog);
        int stayId = _db.CheckIn(dog.Id);

        _db.CheckOut(stayId, "Healthy stay");

        Assert.Empty(_db.GetActiveStays());

        var all = _db.GetAllStays();
        Assert.Single(all);
        Assert.False(all[0].IsActive);
        Assert.Equal("Healthy stay", all[0].Notes);
    }

    [Fact]
    public void GetActiveStays_ExcludesCheckedOut()
    {
        var dog = new Dog { Name = "Scout", Breed = "Retriever", Age = 6, OwnerName = "Frank", OwnerPhone = "" };
        _db.AddDog(dog);

        int stay1 = _db.CheckIn(dog.Id);
        int stay2 = _db.CheckIn(dog.Id);
        _db.CheckOut(stay1);

        var active = _db.GetActiveStays();
        Assert.Single(active);
        Assert.Equal(stay2, active[0].Id);
    }

    [Fact]
    public void GetAllStays_ReturnsAllStays()
    {
        var dog = new Dog { Name = "Daisy", Breed = "Pug", Age = 3, OwnerName = "Grace", OwnerPhone = "" };
        _db.AddDog(dog);

        int s1 = _db.CheckIn(dog.Id);
        _db.CheckOut(s1);
        _db.CheckIn(dog.Id);

        Assert.Equal(2, _db.GetAllStays().Count);
    }

    // ── Model tests ───────────────────────────────────────────────────────────

    [Fact]
    public void Stay_IsActive_TrueWhenNoCheckOut()
    {
        var stay = new Stay { CheckInDate = DateTime.Today };
        Assert.True(stay.IsActive);
    }

    [Fact]
    public void Stay_IsActive_FalseWhenCheckedOut()
    {
        var stay = new Stay { CheckInDate = DateTime.Today.AddDays(-2), CheckOutDate = DateTime.Today };
        Assert.False(stay.IsActive);
    }

    [Fact]
    public void Stay_DaysStayed_AtLeastOne()
    {
        var stay = new Stay { CheckInDate = DateTime.Today };
        Assert.Equal(1, stay.DaysStayed);
    }

    [Fact]
    public void Stay_DaysStayed_CountsCorrectly()
    {
        var stay = new Stay
        {
            CheckInDate  = DateTime.Today.AddDays(-5),
            CheckOutDate = DateTime.Today
        };
        Assert.Equal(5, stay.DaysStayed);
    }

    [Fact]
    public void DeleteDog_AlsoDeletesRelatedStays()
    {
        var dog = new Dog { Name = "Fido", Breed = "Terrier", Age = 2, OwnerName = "Hank", OwnerPhone = "" };
        _db.AddDog(dog);
        _db.CheckIn(dog.Id, "test stay");

        _db.DeleteDog(dog.Id);

        Assert.Empty(_db.GetAllDogs());
        Assert.Empty(_db.GetAllStays());
    }

    [Fact]
    public void Dog_ToString_FormattedCorrectly()
    {
        var dog = new Dog { Name = "Rex", Breed = "Lab", Age = 3, OwnerName = "Alice", OwnerPhone = "" };
        Assert.Equal("Rex (Lab) - Owner: Alice", dog.ToString());
    }
}
