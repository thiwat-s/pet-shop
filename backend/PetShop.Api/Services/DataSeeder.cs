using PetShop.Api.Models;
using PetShop.Api.Repositories;

namespace PetShop.Api.Services;

public sealed class DataSeeder(IUserRepository users, IPetRepository pets, PasswordHasher hasher)
{
    public async Task SeedAsync()
    {
        if (!await users.HasUsersAsync())
        {
            var credential = hasher.Hash("PetShop123!");
            await users.AddAsync(
                new AppUser
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = credential.Hash,
                    PasswordSalt = credential.Salt,
                    Role = "Administrator",
                }
            );
        }
        if ((await pets.GetAllAsync()).Count == 0)
        {
            var now = DateTime.UtcNow;
            foreach (
                var pet in new[]
                {
                    new Pet
                    {
                        Name = "Milo",
                        Species = "Dog",
                        Breed = "Golden Retriever",
                        Age = 2,
                        Price = 850,
                        Stock = 1,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now,
                    },
                    new Pet
                    {
                        Name = "Luna",
                        Species = "Cat",
                        Breed = "British Shorthair",
                        Age = 1,
                        Price = 600,
                        Stock = 2,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now,
                    },
                    new Pet
                    {
                        Name = "Kiwi",
                        Species = "Bird",
                        Breed = "Cockatiel",
                        Age = 1,
                        Price = 140,
                        Stock = 4,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now,
                    },
                }
            )
                await pets.CreateAsync(pet);
        }
    }
}
