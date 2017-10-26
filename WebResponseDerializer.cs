
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ConsoleApplication4
{
    public class WebResponseDerializer<T>
    {
        class MultiPartMessage
        {
            public String ContentType { get; set; }
            public String ContentTransferEncoding { get; set; }
            public String ContentID { get; set; }
            public byte[] Content { get; set; }
        }

        private Stream stream;
        List<MultiPartMessage> messages = new List<MultiPartMessage>();

        public WebResponseDerializer(System.IO.Stream stream)
        {
            // TODO: Complete member initialization
            this.stream = stream;
        }

        public T GetData()
        {
            Byte[] responseData = ToByte(stream);
            if (EqualByte(responseData, Encoding.ASCII.GetBytes("MIME"), 2))
                ParseMimeMessage(responseData);
            else
                ParseMessage(responseData);

            String soapMessage = GetSoapText();
            return FromXml<T>(GetCleanXml(soapMessage, "<soapenv:Body>"));
        }

        public String GetSoapText()
        {
            foreach (var item in messages)
            {
                if (item.ContentType.Contains("text/xml"))
                    return UTF8Encoding.ASCII.GetString(item.Content);
            }
            return String.Empty;
        }

        public Byte[] GetAttachment(String contentId)
        {
            foreach (var item in messages)
            {
                if (item.ContentID == contentId)
                    return item.Content;
            }
            return null;
        }

        private void ParseMessage(byte[] responseData)
        {
            messages.Add(new MultiPartMessage()
            {
                ContentType = "text/xml; charset=UTF-8",
                Content = responseData
            });
        }

        private void ParseMimeMessage(Byte[] responseData)
        {
            Byte[] delimiter = Encoding.ASCII.GetBytes("\r\n");
            int nameLen = IndexOf(responseData, delimiter);
            Byte[] mineName = new Byte[nameLen + 2];
            Array.Copy(responseData, 0, mineName, 0, nameLen + 2);
            String name = UTF8Encoding.ASCII.GetString(mineName);
            Byte[][] separate = Separate(responseData, mineName);
            foreach (var item in separate)
            {
                if (item.Length > 5)
                {
                    messages.Add(GetMessage(item));
                }
            }
        }

        private static T FromXml<T>(String xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader);
        }

        private String GetCleanXml(String soapMessage, String bodyText)
        {
            int startIndex = soapMessage.IndexOf(bodyText) + bodyText.Length;
            bodyText = bodyText.Replace("<", "</");
            int len = soapMessage.IndexOf(bodyText) - startIndex;
            string innerObject = soapMessage.Substring(startIndex, len).Trim();
            //innerObject = innerObject.Replace(" xmlns=\"http://www.visa.com/ROLSI\"", "");
            //innerObject = innerObject.Replace(" xmlns=\"http://www.visa.com/rtsi\"", "");
            return innerObject;
        }

        private MultiPartMessage GetMessage(byte[] messageStream)
        {
            MultiPartMessage message = new MultiPartMessage();
            Byte[] delimiter = Encoding.ASCII.GetBytes("\r\n\r\n");
            int nameLen = IndexOf(messageStream, delimiter) + 4;
            Byte[] content = new Byte[messageStream.Length - nameLen];
            Array.Copy(messageStream, nameLen, content, 0, messageStream.Length - nameLen);
            String messageDef = UTF8Encoding.ASCII.GetString(messageStream, 0, nameLen);
            message.ContentType = GetStringMessage(messageDef, "Content-Type: ");
            message.ContentTransferEncoding = GetStringMessage(messageDef, "Content-Transfer-Encoding: ");
            message.ContentID = GetStringMessage(messageDef, "Content-ID: ");
            message.Content = content;
            return message;
        }

        private static string GetStringMessage(string messageDef, string stringParam)
        {
            messageDef = messageDef.Substring(messageDef.IndexOf(stringParam) + stringParam.Length);
            messageDef = messageDef.Substring(0, messageDef.IndexOf("\r\n"));
            return messageDef;
        }

        private static int IndexOf(byte[] arrayToSearchThrough, byte[] patternToFind)
        {
            if (patternToFind.Length > arrayToSearchThrough.Length)
                return -1;
            for (int i = 0; i < arrayToSearchThrough.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (arrayToSearchThrough[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        private static byte[][] Separate(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            byte[] Part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (EqualByte(source, separator, I))
                {
                    Part = new byte[I - Index];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = I + separator.Length;
                    I += separator.Length - 1;
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts.ToArray();
        }

        private static bool EqualByte(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }

        private static byte[] ToByte(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
