using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRequestHandler.Opera.Models
{
    public class Payment
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string TaxInclusiveYN { get; set; }
    }
}
