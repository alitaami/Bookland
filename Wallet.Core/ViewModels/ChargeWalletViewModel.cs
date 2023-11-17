using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Core.ViewModels
{
    public class ChargeWalletViewModel
    {
        [Required]
        public int amount { get; set; }
     
        [Required]
        public int userId { get; set; }
    }
}
