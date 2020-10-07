using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestHandler.Opera
{
    public class PositiveCharge
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string TaxAmount { get; set; }
        public string FiscalTrxCodeType { get; set; }
    }
}
