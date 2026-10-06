using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Api.Dtos;
using PetShop.Api.Models;
using PetShop.Api.Repositories;

namespace PetShop.Api.Controllers;

[ApiController, Authorize, Route("api/pets")]
public sealed class PetsController(IPetRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PetResponse>>> GetAll() =>
        Ok((await repository.GetAllAsync()).Select(ToResponse));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PetResponse>> Get(Guid id)
    {
        var pet = await repository.GetByIdAsync(id);
        return pet is null ? NotFound(new { message = "Pet not found." }) : Ok(ToResponse(pet));
    }

    [HttpPost]
    public async Task<ActionResult<PetResponse>> Create(SavePetRequest request)
    {
        var now = DateTime.UtcNow;
        var pet = await repository.CreateAsync(
            new Pet
            {
                Name = request.Name.Trim(),
                Species = request.Species.Trim(),
                Breed = request.Breed.Trim(),
                Age = request.Age,
                Price = request.Price,
                Stock = request.Stock,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
            }
        );
        return CreatedAtAction(nameof(Get), new { id = pet.Id }, ToResponse(pet));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PetResponse>> Update(Guid id, SavePetRequest request)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new { message = "Pet not found." });
        existing.Name = request.Name.Trim();
        existing.Species = request.Species.Trim();
        existing.Breed = request.Breed.Trim();
        existing.Age = request.Age;
        existing.Price = request.Price;
        existing.Stock = request.Stock;
        existing.UpdatedAtUtc = DateTime.UtcNow;
        await repository.UpdateAsync(existing);
        return Ok(ToResponse(existing));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        await repository.DeleteAsync(id)
            ? NoContent()
            : NotFound(new { message = "Pet not found." });

    private static PetResponse ToResponse(Pet pet) =>
        new(
            pet.Id,
            pet.Name,
            pet.Species,
            pet.Breed,
            pet.Age,
            pet.Price,
            pet.Stock,
            pet.CreatedAtUtc,
            pet.UpdatedAtUtc
        );
}
