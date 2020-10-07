using HttpRequestHandler.Opera;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HttpRequestHandler.Tremol
{
    public class TremolXmlBuilder : ITremolXmlBuilder
    {
        private XmlDocument doc = new XmlDocument();

        public void BuildTremolXml(string[] args)
        {
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

            XmlElement textArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
            textArgEl.SetAttribute("Name", "Text");
            textArgEl.SetAttribute("Value", "CONFORM DOCUMENT:11324");
            textArgEl.SetAttribute("Compulsory", "false");
            openReceiptArgs.AppendChild(textArgEl);

            XmlElement sellPLUwithSpecifiedVATCommandEl = doc.CreateElement(string.Empty, "Command", string.Empty);
            sellPLUwithSpecifiedVATCommandEl.SetAttribute("Name", "SellPLUwithSpecifiedVAT");
            rootNode.AppendChild(sellPLUwithSpecifiedVATCommandEl);

            OperaInvoiceBody operaInvoiceBody = new OperaInvoiceBody();
            var positiveChargesList = operaInvoiceBody.ReadXML(args);

            foreach (var positiveCharge in positiveChargesList)
            {
                XmlElement argsElement = doc.CreateElement(string.Empty, "Args", string.Empty);
                sellPLUwithSpecifiedVATCommandEl.AppendChild(argsElement);

                XmlElement descriptionArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                descriptionArgEl.SetAttribute("Name", "NamePLU");
                descriptionArgEl.SetAttribute("Value", positiveCharge.Description);
                argsElement.AppendChild(descriptionArgEl);

                XmlElement optionVatClassArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                optionVatClassArgEl.SetAttribute("Name", "OptionVATClass");
                optionVatClassArgEl.SetAttribute("Value", positiveCharge.TaxAmount);
                argsElement.AppendChild(optionVatClassArgEl);

                XmlElement priceArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                priceArgEl.SetAttribute("Name", "Price");
                priceArgEl.SetAttribute("Value", positiveCharge.Price);
                argsElement.AppendChild(priceArgEl);

                XmlElement quantityArgEl = doc.CreateElement(string.Empty, "Arg", string.Empty);
                quantityArgEl.SetAttribute("Name", "Quantity");
                quantityArgEl.SetAttribute("Value", positiveCharge.Quantity);
                argsElement.AppendChild(quantityArgEl);
            }

            // TODO: Create payment type command

            doc.Save("C:\\fiscalxml\\document.xml");
        }
    }
}
