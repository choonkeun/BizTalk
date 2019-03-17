using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SplitXML
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [System.Runtime.InteropServices.Guid("6118B8F0-8684-4ba2-87B4-8336D70BD4F7")]

    public class PipelineComponent : IBaseComponent, IDisassemblerComponent, IComponentUI, IPersistPropertyBag
    {
        private System.Collections.Queue qOutputMsgs = new System.Collections.Queue();

        private string _sender;
        public string SenderID
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("6118B8F0-8684-4ba2-87B4-8336D70BD4F7");
        }

        public void InitNew()
        {

        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            object val = null;

            try
            {
                propertyBag.Read("Sender", out val, 0);
            }
            catch (System.ArgumentException)
            {
            }
            catch (Exception)
            {
            }

            if (val != null)
            {
                _sender = (string)val;
            }
            else
            {
                _sender = "";
            }
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object val = (object)SenderID;
            propertyBag.Write("Sender", ref val);
        }

        public PipelineComponent()
        {

        }

        public string Description
        {
            get
            {
                return "order split disassembler component";
            }
        }
        public string Name
        {
            get
            {
                return "PipelineComponent";
            }
        }
        public string Version
        {
            get
            {
                return "1.0.0.0";
            }
        }
        public IntPtr Icon
        {
            get
            {
                return new System.IntPtr();
            }
        }
        public IEnumerator Validate(object projectSystem)
        {
            return null;
        }

        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {

            string originalDataString;

            try
            {
                //fetch original message

                Stream originalMessageStream = pInMsg.BodyPart.GetOriginalDataStream();

                byte[] bufferOriginalMessage = new byte[originalMessageStream.Length];

                originalMessageStream.Read(bufferOriginalMessage, 0, Convert.ToInt32(originalMessageStream.Length));

                originalDataString = System.Text.ASCIIEncoding.ASCII.GetString(bufferOriginalMessage);

                //reset stream pointer to start of the stream
                originalMessageStream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {

                ex.Source = "PipelineComponent";
                throw ex;
            }

            XmlDocument originalMessageXml = new XmlDocument();

            try
            {
                //load original message xml in XMLDocument type object
                //originalMessageXml.LoadXml(originalDataString);

                string[] xmlDocList = GenerateXMLFiles(originalDataString, SenderID);
                //string[] xmlDocList = GenerateXMLFiles(originalMessageXml, SenderID);

                foreach (var xmlDoc in xmlDocList)
                {
                    //Create outgoing Message for each xml document returned by XML split process
                    CreateOutgoingMessage(pContext, xmlDoc, pInMsg);
                }

            }

            catch (Exception ex)
            {
                ex.Source = "PipelineComponent";
                throw ex;
            }

            finally
            {
                originalMessageXml = null;
            }
        }

        private void CreateOutgoingMessage(IPipelineContext pContext, String messageString, IBaseMessage pInMsg)
        {
            IBaseMessage outMsg;

            try
            {
                //create outgoing message
                outMsg = pContext.GetMessageFactory().CreateMessage();
                outMsg.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);

                byte[] bufferOoutgoingMessage = System.Text.ASCIIEncoding.ASCII.GetBytes(messageString);
                outMsg.BodyPart.Data = new MemoryStream(bufferOoutgoingMessage);
                outMsg.Context = PipelineUtil.CloneMessageContext(pInMsg.Context);
                qOutputMsgs.Enqueue(outMsg);
            }

            catch (Exception ex)
            {
                throw new ApplicationException("Error in queueing outgoing messages: " + ex.Message);
            }
        }

        public IBaseMessage GetNext(IPipelineContext pContext)
        {
            if (qOutputMsgs.Count > 0)
                return (IBaseMessage)qOutputMsgs.Dequeue();
            else
                return null;
        }

        //---
        //add your logic here that will split in coming XML document/message into multiple XML documents/messages 
        //and will return string[] as output of this function
        //---
        private static string[] GenerateXMLFiles(String messageXML, String senderIDList)
        {
            var xDoc = XDocument.Parse(messageXML);
            XElement[] xmls = xDoc.Root.Elements().ToArray();      // split into elements

            //convert XElement to string
            IEnumerable<String> xmlNodes = from n in xmls
                                           select n.ToString();
            String[] arraySplitedXML = xmlNodes.ToArray();

            //for (int i = 0; i < xmls.Length; i++)
            //{
            //    using (var file = File.CreateText(string.Format("xml{0}.xml", i + 1)))
            //    {
            //        file.Write(xmls[i].ToString());   //create text file
            //    }
            //}

            return arraySplitedXML;
        }
    }

#region not used
    //not used
    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
#endregion 


}
