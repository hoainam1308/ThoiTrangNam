using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ThoiTrangNam.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Mã sản phẩm")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc"), StringLength(200, ErrorMessage = "Tên sản phẩm không vượt quá 200 ký tự")]
        [DisplayName("Tên sản phẩm")]
        public string ProductName { get; set; }

        public string? RemovedDiacriticsName { get; set; }

        [Range(1000.000, 1000000000.000, ErrorMessage = "Giá bán phải nằm trong khoảng từ 1000.000 đến 1000000000.000")]
        [DisplayName("Giá bán")]
        public decimal SellPrice { get; set; }
        
        [Range(1000.000, 1000000000.000, ErrorMessage = "Giá mua phải nằm trong khoảng từ 1000.000 đến 1000000000.000")]
        [DisplayName("Giá mua")]
        public decimal BuyPrice { get; set; }
        
        [DisplayName("Mô tả")]     
        public string Description { get; set; }
        
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }

        [DisplayName("Hình ảnh")]
        public string? ImageUrl { get; set; }
        [JsonIgnore]
        public List<ProductImage>? Images { get; set; }

        [ForeignKey("Category")]
        [DisplayName("Mã danh mục")]
        public int CategoryId { get; set; }

        [JsonIgnore]
        [DisplayName("Danh mục")]
        public Category? Category { get; set; }

    }
}
