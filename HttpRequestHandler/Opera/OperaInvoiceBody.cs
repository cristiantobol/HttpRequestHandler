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
        public List<Charge> ReadOperaXmlCharges(string[] args)
        {
            String URLString = args[0];
            var configIni = new IniFile("config.ini");
            string accommodation = configIni.Read("ACCOMMODATION");

            List<Charge> chargeList = new List<Charge>();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(URLString);

            Charge charge;
            XmlNodeList positiveChargeNodes = xmldoc.SelectNodes("folio/body/positive_charge");
            XmlNodeList negativeChargeNodes = xmldoc.SelectNodes("folio/body/negative_charge");

            var concatList = positiveChargeNodes.Cast<XmlNode>().Concat(negativeChargeNodes.Cast<XmlNode>());

            foreach (XmlNode node in concatList)
            {
                charge = new Charge
                {
                    Description = node["DESCRIPTION"].InnerText,
                    Quantity = node["QUANTITY"].InnerText,
                    FiscalTrxCodeType = node["FISCALTRXCODETYPE"].InnerText,
                    TotalInclTaxAmount = node["TOTALINCLTAXAMOUNT"].InnerText,
                    Tax6Amount = node["TAX6AMOUNT"].InnerText,
                    CodeType = node["CODETYPE"].InnerText,
                    TaxCodeNo = node["TAX_CODE_NO"].InnerText,
                    GrossAmount = node["GROSSAMOUNT"].InnerText,

                    IsTaxIncluded = false
                };

                if ((charge.TaxCodeNo == "02,06" || charge.TaxCodeNo == "02,05") && charge.TotalInclTaxAmount != "")
                {
                    charge.FiscalTrxCodeType = "B";
                    charge.IsTaxIncluded = true;
                    GrossAccommodation = (decimal.Parse(charge.GrossAmount) - decimal.Parse(charge.Tax6Amount)).ToString();
                    CityTaxPrice = (decimal.Parse(charge.Tax6Amount)).ToString();
                }

                if(charge.TotalInclTaxAmount == "" || charge.TotalInclTaxAmount == "0.00")
                {
                    if(charge.FiscalTrxCodeType == "_")
                    {
                        charge.FiscalTrxCodeType = "E";
                    }
                    else
                    {
                        charge.FiscalTrxCodeType = "B";
                    }             
                }

                if(charge.Description == "VAT B")
                {
                    charge.Quantity = "1";
                }

                if (charge.FiscalTrxCodeType == "_")
                {
                    charge.FiscalTrxCodeType = "E";
                }
                else if (charge.FiscalTrxCodeType == "" || charge.FiscalTrxCodeType == null)
                {
                    charge.FiscalTrxCodeType = "A";
                }
                
                chargeList.Add(charge);
            }
            return chargeList;
        }
    }
}
