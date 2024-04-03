using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoiTrangNam.Models
{
    public class Classification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassificationId { get; set; }
        [Required(ErrorMessage = "Tên phân loại là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên phân loại không vượt quá 50 ký tự")]
        public string ClassificationName { get; set; }

        public List<Category> Categories { get; set; }
    }
}
