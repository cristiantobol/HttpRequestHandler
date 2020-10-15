using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HttpRequestHandler.Tremol
{
    public class TremolBillNumber
    {
        public string GetBillNumber(string[] args)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(args[0]);
            XmlNode node = xmldoc.SelectSingleNode("folio/head/folio_print_task/BILLNUMBER");

            float billNumber = float.Parse(node.InnerText);

            return billNumber.ToString();
        }
    }
}
