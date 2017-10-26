using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApplication4
{
    [XmlRoot(ElementName = "Status", Namespace = "http://www.visa.com/ROLSI")]
    public class Status
    {
        [XmlElement(ElementName = "Code", Namespace = "http://www.visa.com/ROLSI")]
        public string Code { get; set; }
        [XmlElement(ElementName = "Message", Namespace = "http://www.visa.com/ROLSI")]
        public string Message { get; set; }
    }

    [XmlRoot(ElementName = "Include", Namespace = "http://www.w3.org/2004/08/xop/include")]
    public class Include
    {
        [XmlAttribute(AttributeName = "xop", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xop { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        public Byte[] XopData { get; set; }
    }

    [XmlRoot(ElementName = "ImageData", Namespace = "http://www.visa.com/ROLSI")]
    public class ImageData
    {
        [XmlElement(ElementName = "Include", Namespace = "http://www.w3.org/2004/08/xop/include")]
        public Include Include { get; set; }
    }

    [XmlRoot(ElementName = "Attachment", Namespace = "http://www.visa.com/ROLSI")]
    public class Attachment
    {
        [XmlElement(ElementName = "ContentType", Namespace = "http://www.visa.com/ROLSI")]
        public string ContentType { get; set; }
        [XmlElement(ElementName = "Comment", Namespace = "http://www.visa.com/ROLSI")]
        public string Comment { get; set; }
        [XmlElement(ElementName = "ImageData", Namespace = "http://www.visa.com/ROLSI")]
        public ImageData ImageData { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "AttachmentDescriptor", Namespace = "http://www.visa.com/ROLSI")]
    public class AttachmentDescriptor
    {
        [XmlElement(ElementName = "Attachment", Namespace = "http://www.visa.com/ROLSI")]
        public List<Attachment> Attachment { get; set; }
    }

    [XmlRoot(ElementName = "ResponseData", Namespace = "http://www.visa.com/ROLSI")]
    public class ResponseData
    {
        [XmlElement(ElementName = "VisaCaseNumber", Namespace = "http://www.visa.com/ROLSI")]
        public string VisaCaseNumber { get; set; }
        [XmlElement(ElementName = "DocId", Namespace = "http://www.visa.com/ROLSI")]
        public string DocId { get; set; }
        [XmlElement(ElementName = "AttachmentDescriptor", Namespace = "http://www.visa.com/ROLSI")]
        public AttachmentDescriptor AttachmentDescriptor { get; set; }
        [XmlElement(ElementName = "DocType", Namespace = "http://www.visa.com/ROLSI")]
        public string DocType { get; set; }
    }

    [XmlRoot(ElementName = "SIGetImageResponse", Namespace = "http://www.visa.com/ROLSI")]
    public class SIGetImageResponse
    {
        [XmlElement(ElementName = "Status", Namespace = "http://www.visa.com/ROLSI")]
        public Status Status { get; set; }
        [XmlElement(ElementName = "ResponseData", Namespace = "http://www.visa.com/ROLSI")]
        public ResponseData ResponseData { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
