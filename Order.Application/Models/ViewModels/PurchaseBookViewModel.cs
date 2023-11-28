using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Models.ViewModels
{
    public class PurchaseBookViewModel
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int DiscountId { get; set; }

    }
}
