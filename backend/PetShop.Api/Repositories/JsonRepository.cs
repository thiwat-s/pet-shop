using System.Text.Json;
using PetShop.Api.Models;

namespace PetShop.Api.Repositories;

public sealed class JsonPetRepository(IWebHostEnvironment environment) : IPetRepository
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private readonly string _path = Path.Combine(environment.ContentRootPath, "Data", "pets.json");

    public async Task<IReadOnlyList<Pet>> GetAllAsync() =>
        (await ReadAsync()).OrderBy(x => x.Name).ToList();

    public async Task<Pet?> GetByIdAsync(Guid id) =>
        (await ReadAsync()).FirstOrDefault(x => x.Id == id);

    public async Task<Pet> CreateAsync(Pet pet)
    {
        await Gate.WaitAsync();
        try
        {
            var pets = await ReadAsync();
            pet.Id = Guid.NewGuid();
            pets.Add(pet);
            await WriteAsync(pets);
            return pet;
        }
        finally
        {
            Gate.Release();
        }
    }

    public async Task<bool> UpdateAsync(Pet pet)
    {
        await Gate.WaitAsync();
        try
        {
            var pets = await ReadAsync();
            var index = pets.FindIndex(x => x.Id == pet.Id);
            if (index < 0)
                return false;
            pets[index] = pet;
            await WriteAsync(pets);
            return true;
        }
        finally
        {
            Gate.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await Gate.WaitAsync();
        try
        {
            var pets = await ReadAsync();
            if (pets.RemoveAll(x => x.Id == id) == 0)
                return false;
            await WriteAsync(pets);
            return true;
        }
        finally
        {
            Gate.Release();
        }
    }

    private async Task<List<Pet>> ReadAsync()
    {
        if (!File.Exists(_path))
            return [];
        try
        {
            await using var stream = File.OpenRead(_path);
            return await JsonSerializer.DeserializeAsync<List<Pet>>(stream, Options) ?? [];
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Pet data cannot be read.");
        }
    }

    private async Task WriteAsync(List<Pet> pets)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temp = _path + ".tmp";
        await File.WriteAllTextAsync(temp, JsonSerializer.Serialize(pets, Options));
        File.Move(temp, _path, true);
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
}

public sealed class JsonUserRepository(IWebHostEnvironment environment) : IUserRepository
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private readonly string _path = Path.Combine(environment.ContentRootPath, "Data", "users.json");

    public async Task<AppUser?> FindByUsernameAsync(string username) =>
        (await ReadAsync()).FirstOrDefault(x =>
            string.Equals(x.Username, username, StringComparison.OrdinalIgnoreCase)
        );

    public async Task<bool> HasUsersAsync() => (await ReadAsync()).Count > 0;

    public async Task AddAsync(AppUser user)
    {
        await Gate.WaitAsync();
        try
        {
            var users = await ReadAsync();
            users.Add(user);
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            await File.WriteAllTextAsync(_path, JsonSerializer.Serialize(users, Options));
        }
        finally
        {
            Gate.Release();
        }
    }

    private async Task<List<AppUser>> ReadAsync()
    {
        if (!File.Exists(_path))
            return [];
        await using var stream = File.OpenRead(_path);
        return await JsonSerializer.DeserializeAsync<List<AppUser>>(stream, Options) ?? [];
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
}
