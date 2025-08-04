using RuleWay.ECommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace RuleWay.ECommerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        public required string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsLive { get; set; }
    }
}
