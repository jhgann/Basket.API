using System.ComponentModel.DataAnnotations;

namespace ClientApp.Models
{
    /// <summary>
    /// A single item in a shopping basket
    /// </summary>
    public class BasketItem
    {
        [Required]
        [StringLength(50)]
        public string ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        [Required]
        [Range(1, 999, ErrorMessage = "Quantity must be between {1} and {2}.")]
        public int Quantity { get; set; }
    }
}
