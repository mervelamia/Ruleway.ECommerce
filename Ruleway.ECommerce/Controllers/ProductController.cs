using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleWay.ECommerce.Data;
using RuleWay.ECommerce.Models;

namespace RuleWay.ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return product;
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product == null || string.IsNullOrWhiteSpace(product.Title) || product.Title.Length > 200)
                return BadRequest("Başlık gereklidir ve 200 karakterden az olmalıdır.");

            var category = await _context.Categories.FindAsync(product.CategoryId);
            if (category == null)
                return BadRequest("Ürünün geçerli bir kategorisi olmalıdır.");

            if (product.StockQuantity < category.MinStockQuantity)
                return BadRequest("Stok miktarı, kategori için belirlenen minimum stok değerinin altındadır.");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product updatedProduct)
        {
            if (id != updatedProduct.Id)
                return BadRequest();

            var category = await _context.Categories.FindAsync(updatedProduct.CategoryId);
            if (category == null)
                return BadRequest("Kategori geçersiz.");

            if (updatedProduct.StockQuantity < category.MinStockQuantity)
                return BadRequest("Stok miktarı, kategori için belirlenen minimum stok değerinin altındadır.");

            _context.Entry(updatedProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 🔍 FILTER: api/Product/filter?keyword=zeka&minStock=5&maxStock=20
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Product>>> FilterProducts(
            [FromQuery] string? keyword,
            [FromQuery] int? minStock,
            [FromQuery] int? maxStock)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                    p.Title.Contains(keyword) ||
                    p.Description.Contains(keyword) ||
                    p.Category.Name.Contains(keyword));
            }

            if (minStock.HasValue)
                query = query.Where(p => p.StockQuantity >= minStock.Value);

            if (maxStock.HasValue)
                query = query.Where(p => p.StockQuantity <= maxStock.Value);

            return await query.ToListAsync();
        }
    }
}
