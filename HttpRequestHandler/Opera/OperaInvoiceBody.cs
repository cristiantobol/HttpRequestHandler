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
using HttpRequestHandler.Utils;

namespace HttpRequestHandler.Opera
{
    public class OperaInvoiceBody
    {
        public string GrossAccommodation = "";
        public string CityTaxPrice = "";
        public List<PositiveCharge> ReadXML(string[] args)
        {
            //String URLString = "http://localhost:81/xml/fp_buhhanota11324.xml";
            String URLString = args[0];
            var configIni = new IniFile("config.ini");
            string accommodation = configIni.Read("ACCOMMODATION");

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
                positiveCharge.FiscalTrxCodeType = node["FISCALTRXCODETYPE"].InnerText;
                positiveCharge.Type = node["TYPE"].InnerText;
                positiveCharge.TotalInclTaxAmount = node["TOTALINCLTAXAMOUNT"].InnerText;
                positiveCharge.Tax6Amount = node["TAX6AMOUNT"].InnerText;
                positiveCharge.CodeType = node["CODETYPE"].InnerText;
                positiveCharge.TaxCodeNo = node["TAX_CODE_NO"].InnerText;
                positiveCharge.GrossAmount = node["GROSSAMOUNT"].InnerText;

                positiveCharge.isTaxIncuded = false;

                if((positiveCharge.TaxCodeNo == "02,06" || positiveCharge.TaxCodeNo == "02,05") && positiveCharge.TotalInclTaxAmount != "")
                {
                    positiveCharge.FiscalTrxCodeType = "B";
                    positiveCharge.isTaxIncuded = true;
                    GrossAccommodation = (decimal.Parse(positiveCharge.GrossAmount) - decimal.Parse(positiveCharge.Tax6Amount)).ToString();
                    CityTaxPrice = (decimal.Parse(positiveCharge.Tax6Amount)).ToString();
                }

                if(positiveCharge.TotalInclTaxAmount == "" || positiveCharge.TotalInclTaxAmount == "0.00")
                {
                    if(positiveCharge.FiscalTrxCodeType == "_")
                    {
                        positiveCharge.FiscalTrxCodeType = "E";
                    }
                    else
                    {
                        positiveCharge.FiscalTrxCodeType = "B";
                    }             
                }

                if(positiveCharge.Description == "VAT B")
                {
                    positiveCharge.Quantity = "1";
                }

                if (positiveCharge.FiscalTrxCodeType == "_")
                {
                    positiveCharge.FiscalTrxCodeType = "E";
                }
                
                positiveChargeList.Add(positiveCharge);
            }
            return positiveChargeList;
        }
    }
}
