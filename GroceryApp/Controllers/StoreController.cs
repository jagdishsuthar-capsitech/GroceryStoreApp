using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using GroceryApp.Services;
using GroceryApp.DTOs;
using GroceryApp.Models;

namespace GroceryApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
    private readonly StoreService _storeService;

    public StoreController(StoreService storeService)
    {
        _storeService = storeService;
    }

    // GET: api/store
    [HttpGet]
    [ProducesResponseType(typeof(List<Store>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var stores = await _storeService.GetAllAsync();
        return Ok(stores);
    }

    // GET: api/store/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid store ID.");

        var store = await _storeService.GetByIdAsync(id);
        if (store == null)
            return NotFound("Store not found.");

        return Ok(store);
    }

    // POST: api/store
    [HttpPost]
    [ProducesResponseType(typeof(Store), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] StoreDto storeDto)
    {
        var store = await _storeService.CreateAsync(storeDto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = store.Id },
            store
        );
    }

    // PUT: api/store/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] StoreDto storeDto)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid store ID.");

        var updated = await _storeService.UpdateAsync(id, storeDto);
        if (!updated)
            return NotFound("Store not found.");

        return NoContent();
    }

    // DELETE: api/store/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid store ID.");

        var deleted = await _storeService.DeleteAsync(id);
        if (!deleted)
            return NotFound("Store not found.");

        return NoContent();
    }
}