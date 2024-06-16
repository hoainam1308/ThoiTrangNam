using System;
using System.Collections.Generic;
using System.Linq;

namespace ThoiTrangNam.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        // Coupon related properties
        public Coupon AppliedCoupon { get; set; } // Store applied coupon
        public decimal TotalDiscount { get; set; } // Total discount amount applied

        public void AddItem(Product product, int quantity)
        {
            CartItem item = Items.FirstOrDefault(p => p.Product.ProductId == product.ProductId);
            if (item == null)
            {
                Items.Add(new CartItem { Product = product, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }
        }

        public void RemoveItem(Product product)
        {
            Items.RemoveAll(i => i.Product.ProductId == product.ProductId);
        }

        public decimal ComputeToTalValue()
        {
            decimal subtotal = Items.Sum(e => e.Product.SellPrice * e.Quantity);
            return subtotal - TotalDiscount;
        }

        public void ApplyCoupon(Coupon coupon)
        {
            if (coupon != null && coupon.ExpirationDate >= DateTime.Now)
            {
                AppliedCoupon = coupon;
                TotalDiscount = AppliedCoupon.IsPercentage ?
                    (Items.Sum(e => e.Product.SellPrice * e.Quantity) * AppliedCoupon.DiscountAmount / 100) :
                    AppliedCoupon.DiscountAmount;
            }
        }

        public void RemoveCoupon()
        {
            AppliedCoupon = null;
            TotalDiscount = 0;
        }

        public decimal ComputeToTotal()
        {
            decimal total = ComputeToTalValue();

            // Add shipping fee if total is less than 1000000
            if (total < 1000000)
            {
                total += 50000;
            }

            return total;
        }

        public void Clear()
        {
            Items.Clear();
            RemoveCoupon(); // Clear coupon when cart is cleared
        }
    }

    public class CartItem
    {
        public int CartItemID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
