using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class Transaction
    {
        [Required]
        public string Id { get; set; }

        public string GroupId { get; set; }

        [Required]
        public TransferTypes TransferType { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        public Exchange Exchange { get; set; }
        
        //[Required]
        public Sender Sender { get; set; }
  
        public ReceiverBank ReceiverBank { get; set; }

        public IntermediateBank IntermediateBank { get; set; }

        //[Required]
        public Receiver Receiver { get; set; }

        public ChargeTypes? Charges { get; set; }

        public Remittance Remittance { get; set; }

        public Person Person { get; set; }

        [Required]
        public string PaymentPurpose { get; set; }

        public string Remarks { get; set; }

        public bool? IsSTP { get; set; }

        public string CreatorUser { get; set; }

        public string Channel { get; set; }

        public bool? DoNotAuthorizeFully { get; set; }

        public double? TransferFee { get; set; }
    }

    public enum TransferTypes
    {
        OWN_ACCOUNTS,
        EXCHANGE,
        INSIDE_BANK,
        RTGS,
        BUDJET,
        SWIFT,
        CASH_IN,
        CASH_OUT,
        MEM_ORDER,
        MEM_ORDER_CLRNG,
        REMITTANCE
    }

    public enum ChargeTypes
    {
        OUR,
        SHA
    }
}
