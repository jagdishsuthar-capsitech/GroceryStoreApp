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

    // GET: api/product
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    // GET: api/product/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid product ID.");

        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound("Product not found.");

        return Ok(product);
    }

    // POST: api/product
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] ProductDto productDto)
    {
        var product = await _productService.CreateAsync(productDto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product
        );
    }

    // PUT: api/product/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ProductDto productDto)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid product ID.");

        var updated = await _productService.UpdateAsync(id, productDto);
        if (!updated)
            return NotFound("Product not found.");

        return NoContent();
    }

    // DELETE: api/product/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest("Invalid product ID.");

        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound("Product not found.");

        return NoContent();
    }
}