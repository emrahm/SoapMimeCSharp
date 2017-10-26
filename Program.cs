using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApplication4
{


    class Program
    {
        static void Main(string[] args)
        {
            Byte[] file = File.ReadAllBytes("..\\..\\Data\\ccc.xxx");
            Stream stream = new MemoryStream(file);
            WebResponseDerializer<SIGetImageResponse> deserilizer = new WebResponseDerializer<SIGetImageResponse>(stream);
            SIGetImageResponse ddd = deserilizer.GetData();
            foreach (var item in ddd.ResponseData.AttachmentDescriptor.Attachment)
            {
                String contentId = "<<" + item.ImageData.Include.Href + ">>";
                contentId = contentId.Replace("%40", "@").Replace("cid:", "");
                item.ImageData.Include.XopData = deserilizer.GetAttachment(contentId);
            }
        }
    }
}
