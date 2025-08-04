using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RuleWay.ECommerce.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public int MinStockQuantity { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
