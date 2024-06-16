using System;
using System.ComponentModel.DataAnnotations;

namespace ThoiTrangNam.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public decimal DiscountAmount { get; set; }

        public bool IsPercentage { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int? UsageLimit { get; set; }

        public int TimesUsed { get; set; }
    }
}
