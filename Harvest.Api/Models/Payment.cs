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
        public PaymentGateway PaymentGateway { get; set; }
    }

    public class PaymentGateway
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
