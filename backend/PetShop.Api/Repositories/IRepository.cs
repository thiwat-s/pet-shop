using PetShop.Api.Models;

namespace PetShop.Api.Repositories;

public interface IPetRepository
{
    Task<IReadOnlyList<Pet>> GetAllAsync();
    Task<Pet?> GetByIdAsync(Guid id);
    Task<Pet> CreateAsync(Pet pet);
    Task<bool> UpdateAsync(Pet pet);
    Task<bool> DeleteAsync(Guid id);
}

public interface IUserRepository
{
    Task<AppUser?> FindByUsernameAsync(string username);
    Task<bool> HasUsersAsync();
    Task AddAsync(AppUser user);
}
