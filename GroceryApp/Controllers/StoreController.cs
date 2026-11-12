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

    // GET: api/store?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var res = new ApiResponse<PagedApiResponse<Store>>();
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                res.Message = "Invalid pagination parameters.";
                return BadRequest(res);
            }
            var stores = await _storeService.GetPagedAsync(
                pageNumber,
                pageSize
            );

            res.Status = true;
            res.Message = "Stores fetched successfully.";
            res.Result = stores;
            return Ok(res);
        }
        catch (Exception)
        {
            res.Message = "An error occurred while fetching stores.";
            return StatusCode(500, res);
        }
    }

    // GET: api/store/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var res = new ApiResponse<Store>();
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                res.Message = "Invalid store ID.";
                return BadRequest(res);
            }

            var store = await _storeService.GetByIdAsync(id);
            if (store == null)
            {
                res.Message = "Store not found.";
                return NotFound(res);
            }

            res.Status = true;
            res.Message = "Store fetched successfully.";
            res.Result = store;
            return Ok(res);
        }
        catch (Exception)
        {
            res.Message = "An error occurred while fetching the store.";
            return StatusCode(500, res);
        }
    }

    // POST: api/store
    [HttpPost()]
    public async Task<IActionResult> Create([FromBody] StoreDto storeDto)
    {
        var res = new ApiResponse<Store>();
        try
        {
            var store = await _storeService.CreateAsync(storeDto);

            res.Status = true;
            res.Message = "Store created successfully.";
            res.Result = store;
            return StatusCode(201, res);
        }
        catch (Exception)
        {
            res.Message = "An error occurred while creating the store.";
            return StatusCode(500, res);
        }
    }

    // PUT: api/store/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] StoreDto storeDto)
    {
        var res = new ApiResponse<Store>();
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                res.Message = "Invalid store ID.";
                return BadRequest(res);
            }

            var updated = await _storeService.UpdateAsync(id, storeDto);
            if (!updated)
            {
                res.Message = "Store not found.";
                return NotFound(res);
            }

            res.Status = true;
            res.Message = "Store updated successfully.";
            return Ok(res);
        }
        catch (Exception)
        {
            res.Message = "An error occurred while updating the store.";
            return StatusCode(500, res);
        }
    }

    // DELETE: api/store/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var res = new ApiResponse<Store>();
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                res.Message = "Invalid store ID.";
                return BadRequest(res);
            }

            var deleted = await _storeService.DeleteAsync(id);
            if (!deleted)
            {
                res.Message = "Store not found.";
                return NotFound(res);
            }

            res.Status = true;
            res.Message = "Store deleted successfully.";
            return Ok(res);
        }
        catch (Exception)
        {
            res.Message = "An error occurred while deleting the store.";
            return StatusCode(500, res);
        }
    }
}