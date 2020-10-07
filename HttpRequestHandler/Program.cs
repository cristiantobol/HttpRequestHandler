using HttpRequestHandler.Opera;
using HttpRequestHandler.Tremol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace HttpRequestHandler
{
    class Program
    {
        static void Main(string[] args)
        {          
            if (args == null || args.Length == 0)
            {
                throw new ApplicationException("Specify the URI of the resource to retrieve.");
            }

            TremolXmlBuilder tremolXml = new TremolXmlBuilder();
            tremolXml.BuildTremolXml(args);

            WebClient client = new WebClient();

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream data = client.OpenRead(args[0]);
            StreamReader reader = new StreamReader(data);


            string URI3 = args[2];
            string parameters = args[1];
            StringBuilder builder = new StringBuilder(parameters);
            builder.Replace("@in_mesg_status=!null!", "@in_mesg_status=!SUCCESS!");    
            
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(URI3, builder.ToString());
                Console.WriteLine(HtmlResult);
            }

            data.Close();
            reader.Close();
            //Console.ReadKey();
        }
    }
}
