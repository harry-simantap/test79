using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApplication.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Status")]
        public string DebitCreditStatus { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Amount { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
