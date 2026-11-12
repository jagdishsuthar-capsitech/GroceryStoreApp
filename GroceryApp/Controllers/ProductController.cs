using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using GroceryApp.Services;
using GroceryApp.DTOs;
using GroceryApp.Models;

namespace GroceryApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    // GET: api/product?pageNumber=1&pageSize=10
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
    {
        var res = new ApiResponse<PagedApiResponse<Product>>();
        try
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                res.Message = "Invalid pagination parameters.";
                return BadRequest(res);
            }

            var products = await _productService.GetPagedAsync(pageNumber,pageSize);
            res.Status = true;
            res.Message = "Products fetched successfully.";
            res.Result = products;
            return Ok(res);
        }
        catch (Exception)
        {
            res.Message = "An error occurred while fetching products.";
            return StatusCode(500, res);
        }
    }


    // GET: api/product/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var res = new ApiResponse<Product>();
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                res.Message = "Invalid product ID.";
                return BadRequest(res);
            }

            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                res.Message = "Product not found.";
                return NotFound(res);
            }

            res.Status = true;
            res.Message = "Product fetched successfully.";
            res.Result = product;
            return Ok(res);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            return StatusCode(500, res);
        }
    }

    // POST: api/product/add
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto productDto)
    {
        var res = new ApiResponse<Product>();
        try
        {
            var product = await _productService.CreateAsync(productDto);

            res.Status = true;
            res.Message = "Product created successfully.";
            res.Result = product;
            return StatusCode(201, res);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            return StatusCode(500, res);
        }
    }

    // PUT: api/product/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ProductDto productDto)
    {
        var res = new ApiResponse<Product>();
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                res.Message = "Invalid product ID.";
                return BadRequest(res);
            }

            var updated = await _productService.UpdateAsync(id, productDto);
            if (!updated)
            {
                res.Message = "Product not found.";
                return NotFound(res);
            }

            res.Status = true;
            res.Message = "Product updated successfully.";
            return Ok(res);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            return StatusCode(500, res);
        }
    }

    // DELETE: api/product/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var res = new ApiResponse<Product>();
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                res.Message = "Invalid product ID.";
                return BadRequest(res);
            }

            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
            {
                res.Message = "Product not found.";
                return NotFound(res);
            }

            res.Status = true;
            res.Message = "Product deleted successfully.";
            return Ok(res);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            return StatusCode(500, res);
        }
    }
}