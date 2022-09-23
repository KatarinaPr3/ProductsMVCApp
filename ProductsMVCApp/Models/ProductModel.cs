using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ProductsMVCApp.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductID { get; set; }

        [DisplayName("Product Name")]
        [DataType(DataType.Text, ErrorMessage = "Insert valid name!")]
        [StringLength(100, MinimumLength = 1)]
        [Required]
        public string ProductName { get; set; }

        [DisplayName("Product Description")]
        [DataType(DataType.Text, ErrorMessage = "Insert valid description!")]
        [StringLength(100, MinimumLength = 2)]
        [Required]
        public string ProductDescription { get; set; }

        [DisplayName("Product Category")]
        [DataType(DataType.Text, ErrorMessage = "Insert valid Category for Product!")]
        [StringLength(100, MinimumLength = 1)]
        [Required]
        public string ProductCategory { get; set; }

        [DisplayName("Product Manufacturer")]
        [DataType(DataType.Text, ErrorMessage = "Insert valid manufacturer!")]
        [StringLength(100, MinimumLength = 1)]
        [Required]
        public string ProductManufacturer { get; set; }

        [DisplayName("Product Supplier")]
        [DataType(DataType.Text, ErrorMessage = "Insert valid supplier!")]
        [StringLength(100, MinimumLength = 1)]
        [Required]
        public string ProductSupplier { get; set; }

        [DisplayName("Product Price")]
        [DataType(DataType.Currency, ErrorMessage = "Insert valid value!")]
        [Range(0.01, 1000000, ErrorMessage = "Price of product can not be smaller than 0 or bigger than 1000000.")]
        [Required]
        public decimal ProductPrice { get; set; }
    }
}