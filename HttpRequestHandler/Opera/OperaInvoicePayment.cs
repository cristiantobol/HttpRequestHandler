using HttpRequestHandler.Opera.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HttpRequestHandler.Opera
{
    class OperaInvoicePayment
    {
        public decimal cardGrouped = 0;
        public decimal cashGrouped = 0;
        public bool hasCash = false;
        public bool hasCardPayment = false;
        public bool hasCheck = false;
        public bool hasVirament = false;

        public void ReadOperaXmlPayments(string[] args)
        {
            String URLString = args[0];
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(URLString);

            Payment payment = new Payment();
            XmlNodeList paymentNodes = xmldoc.SelectNodes("folio/body/payment");

            foreach (XmlNode item in paymentNodes)
            {
                payment.FiscalTrxCodeType = item["FISCALTRXCODETYPE"].InnerText;

                if (payment.FiscalTrxCodeType == "A")
                {
                    hasCardPayment = true;
                    cardGrouped += decimal.Parse(item["POSTEDAMOUNT"].InnerText);
                }
                else if (item["FISCALTRXCODETYPE"].InnerText == "C")
                {
                    hasCash = true;
                    cashGrouped += decimal.Parse(item["POSTEDAMOUNT"].InnerText);
                }
                else if (item["FISCALTRXCODETYPE"].InnerText == "E")
                    hasCheck = true;
                else if (item["FISCALTRXCODETYPE"].InnerText == "X")
                    hasVirament = true;
                else
                    item["FISCALTRXCODETYPE"].InnerText = "C";
            }
        }
    }
}
