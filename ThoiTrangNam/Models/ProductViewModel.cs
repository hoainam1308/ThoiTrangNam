namespace ThoiTrangNam.Models
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Shorting { get; set; }
        public string Search { get; set; }
        public int Cate { get; set; }
        public int Classifi { get; set; }
    }
}
