using System;
using System.IO;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Ionic.Zip;
using System.Collections.Generic;
using System.Text;

//https://www.codeproject.com/Articles/152405/Zip-Files-in-a-Custom-Pipeline-Component - prog structure
//https://biztalklive.blogspot.com/2018/01/zip-file-in-custom-send-pipeline.html - zip 
//Custom pipeline component to zip messages in the encode stage in a BizTalk send pipeline.

namespace XO.ZipEncoder
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    [System.Runtime.InteropServices.Guid("2cbc0379-c573-4506-9980-da69aad1496c")]
    public class ZipEncode : IBaseComponent, IComponentUI, IComponent, IPersistPropertyBag
    {

        #region IBaseComponent - Default
        private const string _description = "SendPort Pipeline component used to zip a message";
        private const string _name = "ZipEncode";
        private const string _version = "1.0.0.0";

        public string Description
        {
            get { return _description; }
        }
        public string Name
        {
            get { return _name; }
        }
        public string Version
        {
            get { return _version; }
        }
        #endregion

        #region IComponentUI - Default
        private IntPtr _icon = new IntPtr();
        public IntPtr Icon
        {
            get { return _icon; }
        }
        public System.Collections.IEnumerator Validate(object projectSystem)
        {
            return null;
        }
        #endregion

        #region IPersistPropertyBag - Parameters from Admin Console

        #region Property
        private string _password;
        private string _fileExtension;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string FileExtension
        {
            get { return _fileExtension; }
            set { _fileExtension = value; }
        }
        public void GetClassID(out Guid classID)
        {
            classID = new Guid("0c2cc78f-c88e-40db-a9ba-072c354af57f");
        }
        #endregion

        #region Method
        public void InitNew()
        {
        }
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            object val1 = null;
            object val2 = null;
            try
            {
                propertyBag.Read("Password", out val1, 0);
                propertyBag.Read("FileExtension", out val2, 0);
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading PropertyBag: " + ex.Message);
            }
            if (val1 != null)
                _password = (string)val1;
            if (val2 != null)
                _fileExtension = (string)val2;
            else
                _fileExtension = "xml";
        }
        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object val1 = (object)_password;
            object val2 = (object)_fileExtension;
            propertyBag.Write("Password", ref val1);
            propertyBag.Write("FileExtension", ref val2);
        }
        #endregion

        #endregion

        #region IComponent - One message at a time

        //https://docs.microsoft.com/en-us/biztalk/core/how-to-use-message-context-properties
        //https://docs.microsoft.com/en-us/biztalk/core/file-adapter-property-schema-and-properties
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            IBaseMessageContext context = pInMsg.Context;
            object obj = context.Read("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties");
            //fileName = ((string)obj).Substring(((string)obj).LastIndexOf("\\") + 1);
            string fullPath = Path.GetFullPath((string)obj);
            string fileName = Path.GetFileName(fullPath);
            string filePath = Path.GetDirectoryName(fullPath);

            Byte[] TextFileBytes = null;
            IBaseMessagePart msgBodyPart = pInMsg.BodyPart;

            //Creating outMessage
            IBaseMessage outMessage;
            outMessage = pContext.GetMessageFactory().CreateMessage();

            if (msgBodyPart != null)
            {
                outMessage.Context = PipelineUtil.CloneMessageContext(pInMsg.Context);
                Stream msgBodyPartStream = msgBodyPart.GetOriginalDataStream();
                TextFileBytes = StreamToByteArray(msgBodyPartStream);
                outMessage.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);


                IDictionary<string, Byte[]> lst = new Dictionary<string, Byte[]>();
                lst.Add(fileName, TextFileBytes);

                MemoryStream ms = new MemoryStream();
                ms.Seek(0, SeekOrigin.Begin);

                using (ZipFile zip = new ZipFile())
                {
                    //zip.Password = _password;
                    if (_password != null)
                        zip.Password = _password;
                    zip.Encryption = Ionic.Zip.EncryptionAlgorithm.WinZipAes256;
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                    foreach (KeyValuePair<string, Byte[]> item in (IDictionary<string, Byte[]>)lst)
                    {
                        zip.AddEntry(item.Key, item.Value); //Key:fileName, Value:TextFileBytes
                    }
                    zip.Save(ms);
                }

                ms.Position = 0;
                outMessage.BodyPart.Data = ms;
                outMessage.Context.Promote("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", fileName.Substring(0, fileName.IndexOf(".")));
            }
            return outMessage;
        }

        private Byte[] StreamToByteArray(Stream sourceStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                sourceStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        #endregion

    }
}
