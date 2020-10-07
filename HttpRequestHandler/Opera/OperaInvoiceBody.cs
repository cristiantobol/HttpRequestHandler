using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;

namespace HttpRequestHandler.Opera
{
    public class OperaInvoiceBody : OperaInvoiceDetails
    {
        public List<PositiveCharge> ReadXML(string[] args)
        {
            //String URLString = "http://localhost:81/xml/fp_buhhanota11324.xml";
            String URLString = args[0];

            List<PositiveCharge> positiveChargeList = new List<PositiveCharge>();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(URLString);

            PositiveCharge positiveCharge;
            XmlNodeList positiveChargeNodes = xmldoc.SelectNodes("folio/body/positive_charge");

            foreach (XmlNode node in positiveChargeNodes)
            {
                positiveCharge = new PositiveCharge();
                positiveCharge.Code = node["CODE"].InnerText;
                positiveCharge.Description = node["DESCRIPTION"].InnerText;
                positiveCharge.Quantity = node["QUANTITY"].InnerText;
                positiveCharge.Price = node["GROSSAMOUNT"].InnerText;

                if((node["CODE"].InnerText == "7500" && node["TAX2AMOUNT"].InnerText.Length > 0) || node["CODE"].InnerText == "7101")
                {
                    positiveCharge.TaxAmount = "B";
                }
                else if(node["CODE"].InnerText == "7105")
                {
                    positiveCharge.TaxAmount = "D";
                }
                
                positiveCharge.FiscalTrxCodeType = node["FISCALTRXCODETYPE"].InnerText;
                positiveChargeList.Add(positiveCharge);
            }
            return positiveChargeList;
        }
    }
}
