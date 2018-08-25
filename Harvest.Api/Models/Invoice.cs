using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class Invoice : BaseModel
    {
        public Client Client { get; set; }
        public List<InvoiceLineItem> LineItems { get; set; }
        public Estimate Estimate { get; set; }
        public Retainer Retainer { get; set; }
        public Creator Creator { get; set; }
        public string ClientKey { get; set; }
        public string Number { get; set; }
        public string PurchaseOrder { get; set; }
        public decimal Amount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Tax2 { get; set; }
        public decimal Tax2Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Subject { get; set; }
        public string Notes { get; set; }
        public string Currency { get; set; }
        public string State { get; set; }
        public DateTime PeroidStart { get; set; }
        public DateTime PeroidEnd { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string PaymentTerm { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime PaidAt { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime ClosedAt { get; set; }
    }

    public class InvoiceLineItem : BaseModel
    {
        public Project Project { get; set; }
        public string Kind { get; set;  }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public bool Taxed { get; set; }
        public bool Taxed2 { get; set; }
    }
}
