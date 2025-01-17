﻿using HttpRequestHandler.Opera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace HttpRequestHandler.Tremol
{
    public class TremolXmlBuilder : ITremolXmlBuilder
    {
        private XmlDocument doc = new XmlDocument();
        
        public void BuildTremolXml(string[] args)
        {
            TremolBillNumber bill = new TremolBillNumber();
            string billNumber = bill.GetBillNumber(args);

            doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlNode rootNode = doc.CreateElement("root");
            doc.AppendChild(rootNode);

            XmlElement openReceiptCommandEl = doc.CreateElement(string.Empty, "Command", string.Empty);
            openReceiptCommandEl.SetAttribute("Name", "OpenReceipt");
            rootNode.AppendChild(openReceiptCommandEl);

            XmlElement openReceiptArgs = doc.CreateElement(string.Empty, "Args", string.Empty);
            openReceiptCommandEl.AppendChild(openReceiptArgs);

            XmlElement operNumArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
            operNumArgEl.SetAttribute("Name", "OperNum");
            operNumArgEl.SetAttribute("Value", "1");
            openReceiptArgs.AppendChild(operNumArgEl);

            XmlElement operPassArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
            operPassArgEl.SetAttribute("Name", "OperPass");
            operPassArgEl.SetAttribute("Value", "0");
            openReceiptArgs.AppendChild(operPassArgEl);

            XmlElement optionFiscalReceiptPrintTypeArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
            optionFiscalReceiptPrintTypeArgEl.SetAttribute("Name", "OptionFiscalReceiptPrintType");
            optionFiscalReceiptPrintTypeArgEl.SetAttribute("Value", "2");
            openReceiptArgs.AppendChild(optionFiscalReceiptPrintTypeArgEl);

            // PrintText
            XmlElement conformDocumentCommandEl = doc.CreateElement(string.Empty, "Command", string.Empty);
            conformDocumentCommandEl.SetAttribute("Name", "PrintText");
            rootNode.AppendChild(conformDocumentCommandEl);

            XmlElement billNumberArgs = doc.CreateElement(string.Empty, "Args", string.Empty);
            conformDocumentCommandEl.AppendChild(billNumberArgs);

            XmlElement textArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
            textArgEl.SetAttribute("Name", "Text");
            textArgEl.SetAttribute("Value", "CONFORM DOCUMENT:" + billNumber);
            billNumberArgs.AppendChild(textArgEl);

            OperaInvoiceBody operaInvoiceBody = new OperaInvoiceBody();
            var positiveChargesList = operaInvoiceBody.ReadXML(args); 

            foreach (var positiveCharge in positiveChargesList)
            {
                XmlElement sellPLUwithSpecifiedVATCommandEl = doc.CreateElement(string.Empty, "Command", string.Empty);
                sellPLUwithSpecifiedVATCommandEl.SetAttribute("Name", "SellPLUwithSpecifiedVAT");
                rootNode.AppendChild(sellPLUwithSpecifiedVATCommandEl);

                XmlElement argsElement = doc.CreateElement(string.Empty, "Args", string.Empty);
                sellPLUwithSpecifiedVATCommandEl.AppendChild(argsElement);

                XmlElement descriptionArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                descriptionArgEl.SetAttribute("Name", "NamePLU");
                descriptionArgEl.SetAttribute("Value", positiveCharge.Description);
                argsElement.AppendChild(descriptionArgEl);             

                XmlElement optionVatClassArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                optionVatClassArgEl.SetAttribute("Name", "OptionVATClass");
                optionVatClassArgEl.SetAttribute("Value", positiveCharge.FiscalTrxCodeType);
                argsElement.AppendChild(optionVatClassArgEl);

                XmlElement priceArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                priceArgEl.SetAttribute("Name", "Price");
                priceArgEl.SetAttribute("Value", positiveCharge.isTaxIncuded ? operaInvoiceBody.GrossAccommodation : positiveCharge.GrossAmount);
                argsElement.AppendChild(priceArgEl);

                XmlElement quantityArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                quantityArgEl.SetAttribute("Name", "Quantity");
                quantityArgEl.SetAttribute("Value", positiveCharge.Quantity);
                argsElement.AppendChild(quantityArgEl);

                if (positiveCharge.isTaxIncuded)
                {
                    XmlElement isTaxCommand = doc.CreateElement(string.Empty, "Command", string.Empty);
                    isTaxCommand.SetAttribute("Name", "SellPLUwithSpecifiedVAT");
                    rootNode.AppendChild(isTaxCommand);

                    XmlElement isTaxArgs = doc.CreateElement(string.Empty, "Args", string.Empty);
                    isTaxCommand.AppendChild(isTaxArgs);

                    XmlElement isTaxDescriptionArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                    isTaxDescriptionArg.SetAttribute("Name", "NamePLU");
                    isTaxDescriptionArg.SetAttribute("Value", "City Tax Incl 2%");
                    isTaxArgs.AppendChild(isTaxDescriptionArg);

                    XmlElement isTaxOptionVatArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                    isTaxOptionVatArg.SetAttribute("Name", "OptionVATClass");
                    isTaxOptionVatArg.SetAttribute("Value", "E");
                    isTaxArgs.AppendChild(isTaxOptionVatArg);

                    XmlElement isTaxPriceArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                    isTaxPriceArg.SetAttribute("Name", "Price");
                    isTaxPriceArg.SetAttribute("Value", operaInvoiceBody.CityTaxPrice);
                    isTaxArgs.AppendChild(isTaxPriceArg);

                    XmlElement isTaxQuantityArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                    isTaxQuantityArg.SetAttribute("Name", "Quantity");
                    isTaxQuantityArg.SetAttribute("Value", positiveCharge.Quantity);
                    isTaxArgs.AppendChild(isTaxQuantityArg);
                }
            }

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(args[0]);

            decimal cardGrouped = 0;
            decimal cashGrouped = 0;
            bool hasCash = false;
            bool hasCardPayment = false;
            bool hasCheck = false;
            bool hasVirament = false;

            XmlNodeList paymentNodes = xmldoc.SelectNodes("folio/body/payment");

            foreach (XmlNode item in paymentNodes)
            {
                if (item["FISCALTRXCODETYPE"].InnerText == "A")
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

            if (hasCardPayment)
            {
                XmlElement cardPaymentCommand = doc.CreateElement(string.Empty, "Command", string.Empty);
                cardPaymentCommand.SetAttribute("Name", "Payment");
                rootNode.AppendChild(cardPaymentCommand);

                XmlElement cardPaymentArgs = doc.CreateElement(string.Empty, "Args", string.Empty);
                cardPaymentCommand.AppendChild(cardPaymentArgs);

                XmlElement cardPaymentArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                cardPaymentArg.SetAttribute("Name", "OptionPaymentType");
                cardPaymentArg.SetAttribute("Value", "1");
                cardPaymentArgs.AppendChild(cardPaymentArg);

                XmlElement amountCardArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                amountCardArg.SetAttribute("Name", "Amount");
                amountCardArg.SetAttribute("Value", cardGrouped.ToString());
                cardPaymentArgs.AppendChild(amountCardArg);
            }          

            if(hasCash)
            {
                XmlElement paymentCashCommand = doc.CreateElement(string.Empty, "Command", string.Empty);
                paymentCashCommand.SetAttribute("Name", "Payment");
                rootNode.AppendChild(paymentCashCommand);

                XmlElement paymentCashArgs = doc.CreateElement(string.Empty, "Args", string.Empty);
                paymentCashCommand.AppendChild(paymentCashArgs);

                XmlElement paymentCashArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                paymentCashArg.SetAttribute("Name", "OptionPaymentType");
                paymentCashArg.SetAttribute("Value", "0");
                paymentCashArgs.AppendChild(paymentCashArg);

                XmlElement amountCashArg = doc.CreateElement(string.Empty, "Arg", string.Empty);
                amountCashArg.SetAttribute("Name", "Amount");
                amountCashArg.SetAttribute("Value", cashGrouped.ToString());
                paymentCashArgs.AppendChild(amountCashArg);
            }

            XmlElement closeReceipt = doc.CreateElement(string.Empty, "Command", string.Empty);
            closeReceipt.SetAttribute("Name", "CloseReceipt");
            rootNode.AppendChild(closeReceipt);

            XDocument xdoc = XDocument.Parse(doc.OuterXml);
            var fragOut = string.Join(Environment.NewLine, xdoc.Root.Elements().Select(ele => ele.ToString()));

            using (StreamWriter writer = new StreamWriter("C:\\TREMOL\\FILE_IN\\bill_" + billNumber + ".xml"))
            {
                writer.WriteLine(fragOut);             
            }

            using (StreamWriter writer = new StreamWriter("C:\\TREMOL\\ARCHIVE\\bill_.xml"))
            {
                writer.WriteLine(fragOut);
            }
        }
    }
}
