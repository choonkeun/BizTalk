using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Ionic.Zip;
using System.IO;

namespace PipelineUnZipDisassemble
{

    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [System.Runtime.InteropServices.Guid("90E45487-0AFC-49F5-81EB-737635678A17")]
    public class UnzipComponent : IBaseComponent, IDisassemblerComponent, IComponentUI, IPersistPropertyBag
    {

#region IBaseComponent
        private const string _description = "Pipeline component used to unzip messages";
        private const string _name = "UnzipDisassembler";
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

#region IComponentUI
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

#region IPersistPropertyBag - Password
        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public void GetClassID(out Guid classID)
        {
            classID = new Guid("625BBF88-0F86-419A-83AE-B15A976A6715");
        }
        public void InitNew()
        {
        }
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            object val1 = null;
            try
            {
                propertyBag.Read("Password", out val1, 0);
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
        }
        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object val1 = (object)_password;
            propertyBag.Write("Password", ref val1);
        }
        #endregion

        #region IDisassemblerComponent

        //https://docs.microsoft.com/en-us/biztalk/core/how-to-use-message-context-properties
        //https://docs.microsoft.com/en-us/biztalk/core/file-adapter-property-schema-and-properties
        //https://biztalklive.blogspot.com/2017/12/unzip-disassemble-pipeline-component-in.html

        private System.Collections.Queue _qOutMessages = new System.Collections.Queue();
        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            IBaseMessagePart bodyPart = pInMsg.BodyPart;

            if (bodyPart != null)
            {
                Stream originalStream = bodyPart.GetOriginalDataStream();

                if (originalStream != null)
                {
                    using (ZipInputStream zipInputStream = new ZipInputStream(originalStream))
                    {
                        if (_password != null && _password.Length > 0)
                            zipInputStream.Password = _password;

                        ZipEntry entry = zipInputStream.GetNextEntry();

                        while (entry != null)
                        {
                            MemoryStream memStream = new MemoryStream();
                            byte[] buffer = new Byte[1024];

                            int bytesRead = 1024;
                            while (bytesRead != 0)
                            {
                                bytesRead = zipInputStream.Read(buffer, 0, buffer.Length);
                                memStream.Write(buffer, 0, bytesRead);
                            }

                            string fileName = entry.FileName.ToString();    //file name in zip file
                            string extension = Path.GetExtension(fileName);

                            IBaseMessage outMessage;
                            outMessage = pContext.GetMessageFactory().CreateMessage();
                            outMessage.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);
                            memStream.Position = 0;
                            outMessage.BodyPart.Data = memStream;


                            IBaseMessageContext context = pInMsg.Context;
                            string receivePortName = context.Read("ReceivePortName", "http://schemas.microsoft.com/BizTalk/2003/system-properties").ToString();
                            string fullPath = context.Read("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties").ToString();
                            string filePath = Path.GetDirectoryName(fullPath);

                            outMessage.Context = PipelineUtil.CloneMessageContext(pInMsg.Context);
                            outMessage.Context.Promote("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", fileName);
                            outMessage.Context.Promote("ReceivePortName", "http://schemas.microsoft.com/BizTalk/2003/system-properties", receivePortName);

                            _qOutMessages.Enqueue(outMessage);

                            entry = zipInputStream.GetNextEntry();
                        }
                    }
                }
            }
        }
        public IBaseMessage GetNext(IPipelineContext pContext)
        {
            if (_qOutMessages.Count > 0)
                return (IBaseMessage)_qOutMessages.Dequeue();
            else
                return null;
        }
#endregion

    } // Ends the class

} // Ends the namespace
