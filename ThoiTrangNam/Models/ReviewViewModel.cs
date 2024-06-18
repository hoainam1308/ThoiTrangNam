namespace ThoiTrangNam.Models
{
    public class ReviewViewModel
    {
        public int OrderDetailId { get; set; }
        public int Rating { get; set; }
        public string? UserReview { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? UserName { get; set; }
    }
}
