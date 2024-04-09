namespace ThoiTrangNam.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(Product product, int quatity)
        {
            CartItem? item = Items.Where(p => p.Product.ProductId == product.ProductId).FirstOrDefault();
            if (item == null)
            {
                Items.Add(new CartItem { Product = product, Quantity = quatity });
            }
            else
            {
                item.Quantity += quatity;
            }
        }

        public void RemoveItem(Product product)
        {
            Items.RemoveAll(i => i.Product.ProductId == product.ProductId);
        }
        public decimal ComputeToTalValue()
        {
            return Items.Sum(e => e.Product.SellPrice * e.Quantity);
        }

        public void Clear() { Items.Clear(); }
    }
    public class CartItem
    {
        public int CartItemID { get; set; }
        public Product Product { get; set; } = new();
        public int Quantity { get; set; }
        
    }
}
