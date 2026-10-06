using System.Data.Common;
using Dapper;
using PetShop.Api.Models;

namespace PetShop.Api.Repositories;

// SQL implementation seam for a future database migration; JSON remains the active mock persistence.
public sealed class DapperPetRepository(DbConnection connection)
{
    public Task<IEnumerable<Pet>> GetAllAsync() =>
        connection.QueryAsync<Pet>(
            "select Id, Name, Species, Breed, Age, Price, Stock, CreatedAtUtc, UpdatedAtUtc from Pets order by Name"
        );
}
