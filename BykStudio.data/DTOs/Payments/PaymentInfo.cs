using System;
using System.Collections.Generic;
using System.Text;

namespace BykStudio.data.DTOs
{
    public class PaymentInfo
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
