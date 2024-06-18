using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoiTrangNam.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục không vượt quá 100 ký tự")]
        public string CategoryName { get; set; }

        [ForeignKey("Classification")]
        [DisplayName("Mã phân loại")]
        public int ClassificationId { get; set; }

        [DisplayName("Phân loại")]
        public Classification? Classification { get; set; }
        public List<Product>? Products { get; set; }
    }
}
