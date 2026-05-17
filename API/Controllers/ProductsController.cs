using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Application.DTOs;
using InventoryAPI.Application.Services;

namespace InventoryAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<ProductDto>>(true, "Products retrieved.", products));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product is null)
            return NotFound(new ApiResponse<ProductDto>(false, $"Product {id} not found.", null));

        return Ok(new ApiResponse<ProductDto>(true, "Product found.", product));
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetByCategory(string category)
    {
        var products = await _service.GetByCategoryAsync(category);
        return Ok(new ApiResponse<IEnumerable<ProductDto>>(true, $"Products in {category}.", products));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest(new ApiResponse<object>(false, "Keyword is required.", null));

        var products = await _service.SearchAsync(keyword);
        return Ok(new ApiResponse<IEnumerable<ProductDto>>(true, "Search results.", products));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id },
                new ApiResponse<ProductDto>(true, "Product created.", product));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse<object>(false, ex.Message, null));
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _service.UpdateAsync(id, dto);
        if (product is null)
            return NotFound(new ApiResponse<object>(false, $"Product {id} not found.", null));

        return Ok(new ApiResponse<ProductDto>(true, "Product updated.", product));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new ApiResponse<object>(false, $"Product {id} not found.", null));

        return Ok(new ApiResponse<object>(true, "Product deleted.", null));
    }
}
