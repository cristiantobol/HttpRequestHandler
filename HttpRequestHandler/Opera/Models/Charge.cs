using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestHandler.Opera
{
    public class Charge
    {
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string TaxAmount { get; set; }
        public string FiscalTrxCodeType { get; set; }
        public string TotalInclTaxAmount { get; set; }
        public string GrossAmount { get; set; }
        public string Tax6Amount { get; set; }
        public string CodeType { get; set; }
        public string TaxCodeNo { get; set; }
        public bool IsTaxIncluded { get; set; }
    }
}
