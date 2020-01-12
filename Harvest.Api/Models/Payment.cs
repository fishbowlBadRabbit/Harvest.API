using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class Payment : BaseModel
    {
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public DateTime PaidDate { get; set; }
        public string RecordedBy { get; set; }
        public string RecordedByEmail { get; set; }
        public string Notes { get; set; }
        public string TransactionId { get; set; }
    }

    public class PaymentsResponse : PagedList
    {        
        public Payment[] InvoicePayments { get; set; }
    }
}
