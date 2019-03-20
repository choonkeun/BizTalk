using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.ComponentModel;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Xml;

namespace PipelineComponent
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Decoder)]
    [System.Runtime.InteropServices.Guid("8B684775-8957-4D78-A7E6-838380F9FA68")]
    public class ExcelToFlatFile :
                IBaseComponent,
                Microsoft.BizTalk.Component.Interop.IComponent,
                Microsoft.BizTalk.Component.Interop.IPersistPropertyBag,
                IComponentUI
    {


#region Properties
        private string tempFolder = null;
        [System.ComponentModel.Description("Temp Folder for Dropping ODBC Files.")]
        public string TempFolder
        {
            get { return tempFolder; }
            set { tempFolder = value; }
        }

        private string targetFolder = null;
        [System.ComponentModel.Description("Destination Folder for Dropping Files.")]
        public string TargetFolder
        {
            get { return targetFolder; }
            set { targetFolder = value; }
        }

        private string fieldDelimiter = null;
        [System.ComponentModel.Description("field Delimiter for putput text file")]
        public string FieldDelimiter
        {
            get { return fieldDelimiter; }
            set { fieldDelimiter = value; }
        }

        private string fNameSpace = null;
        [System.ComponentModel.Description("NameSpace for result XML Message")]
        public string NameSpace
        {
            get { return fNameSpace; }
            set { fNameSpace = value; }
        }
#endregion

        #region IBaseComponent Members

        [Browsable(false)]
        string IBaseComponent.Description
        {
            get { return "BizTalk Receive Pipeline Component to convert .xlsx to Flat Files"; }
        }

        [Browsable(false)]
        string IBaseComponent.Name
        {
            get { return "ExcelToFlatFile"; }
        }

        [Browsable(false)]
        string IBaseComponent.Version
        {
            get { return "1.0"; }
        }
        #endregion

        #region IPersistPropertyBag Members

        void IPersistPropertyBag.GetClassID(out Guid classID)
        {
            classID = new Guid("9B684775-8957-4D78-A7E6-838380F9FA68");
        }

        void IPersistPropertyBag.InitNew()
        {

        }

        void IPersistPropertyBag.Load(IPropertyBag propertyBag, int errorLog)
        {
            object valTempFolder = null,
                    valTargetFolder = null,
                    valFieldDelimiter = null,
                    valNameSpace = null;
            try
            {
                propertyBag.Read("TempFolder", out valTempFolder, 0);
                propertyBag.Read("TargetFolder", out valTargetFolder, 0);
                propertyBag.Read("FieldDelimiter", out valFieldDelimiter, 0);
                propertyBag.Read("NameSpace", out valNameSpace, 0);
            }
            catch (ArgumentException argEx)
            {
                // throw argEx;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading propertybag: " + ex.Message);
            }
            if (valTempFolder != null)
                TempFolder = (string)valTempFolder;
            else
                TempFolder = "";

            if (valTargetFolder != null)
                TargetFolder = (string)valTargetFolder;
            else
                TargetFolder = "";

            if (valFieldDelimiter != null)
                FieldDelimiter = (string)valFieldDelimiter;
            else
                FieldDelimiter = "";

            if (valNameSpace != null)
                NameSpace = (string)valNameSpace;
            else
                NameSpace = "";
        }
        void IPersistPropertyBag.Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object valTempFolder = (object)TempFolder;
            propertyBag.Write("TempFolder", ref valTempFolder);

            object valTargetFolder = (object)TargetFolder;
            propertyBag.Write("TargetFolder", ref valTargetFolder);

            object valFieldDelimiter = (object)FieldDelimiter;
            propertyBag.Write("FieldDelimiter", ref valFieldDelimiter);

            object valNameSpace = (object)NameSpace;
            propertyBag.Write("NameSpace", ref valNameSpace);

        }
        #endregion        #endregion

        #region IComponentUI Members

        IntPtr IComponentUI.Icon
        {
            get
            {
                return new System.IntPtr();
                //ResourceManager rm = new ResourceManager("ODBCPipelineComponent.Resource", Assembly.GetExecutingAssembly());
                //Bitmap bm = (Bitmap)rm.GetObject("odbc");
                //return bm.GetHicon();
            }
        }

        System.Collections.IEnumerator IComponentUI.Validate(object projectSystem)
        {
            return null;
        }

        #endregion

        #region IComponent Members

        IBaseMessage Microsoft.BizTalk.Component.Interop.IComponent.Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string sFilePath = pInMsg.Context.Read("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties").ToString();
            string fileFullPath = System.IO.Path.GetFileName(sFilePath);
            string filenameOnly = System.IO.Path.GetFileNameWithoutExtension(fileFullPath);

            //extract messageBody from 'pInMsg'
            IBaseMessagePart bodyPart = pInMsg.BodyPart;
            Stream originalStream = pInMsg.BodyPart.GetOriginalDataStream();

            //copy to tempFilePath
            string tempFilePath = Path.Combine(TempFolder, fileFullPath);
            var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write);
            originalStream.CopyTo(fileStream);
            fileStream.Dispose();

            //return message
            XmlDocument Result = new XmlDocument();

            try
            {
                //Open Exel file using OLEDB 12.0 - AccessRuntime_2007.exe has success to install
                string ConStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + tempFilePath + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=0\"";
                OleDbConnection conn = new OleDbConnection(ConStr);
                conn.Open();
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string sheetname = string.Empty;
                Int16 sheetcnt = 0;
                Int32 rowsProcessed = 0;

                foreach (DataRow drSheet in dtSheet.Rows)
                {
                    sheetcnt += 1;
                    if (drSheet["TABLE_NAME"].ToString().Contains("$"))
                    {
                        sheetname = drSheet["TABLE_NAME"].ToString();
                        OleDbCommand oconn = new OleDbCommand("select * from [" + sheetname + "]", conn);
                        OleDbDataAdapter adp = new OleDbDataAdapter(oconn);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);

                        sheetname = sheetname.Replace("$", "");

                        //if (sheetname.ToLower() == "detail")
                        //{
                        rowsProcessed = 0;
                        StreamWriter sw = new StreamWriter(targetFolder + "\\" + filenameOnly + "_" + sheetname + ".txt", false);
                        int ColumnCount = dt.Columns.Count;

                        //Header Line
                        if (sheetcnt == 1)
                        {
                            // Write the Header Row to File
                            for (int i = 0; i < ColumnCount; i++)
                            {
                                sw.Write(dt.Columns[i]);
                                if (i < ColumnCount - 1)
                                {
                                    sw.Write("|");
                                }
                            }
                            sw.Write(sw.NewLine);
                        }

                        //Data Line
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (!dr.IsNull(0))
                            {
                                rowsProcessed++;
                                for (int i = 0; i < ColumnCount; i++)
                                {
                                    if (!Convert.IsDBNull(dr[i]))
                                    {
                                        sw.Write(dr[i].ToString());
                                    }
                                    if (i < ColumnCount - 1)
                                    {
                                        sw.Write("|");
                                    }
                                }
                                sw.Write(sw.NewLine);
                            }
                        }
                        sw.Close();
                        //} //if (sheetname.ToLower() == "detail")
                    }
                }
                Result.LoadXml("<Status>" + "Success" + "</Status>");
            }
            catch (Exception ex)
            {
                Result.LoadXml("<Status>" + ex.Message.ToString() + "</Status>");
            }
            //return results 
            Stream ms = new MemoryStream();

            Result.Save(ms);
            ms.Position = 0;
            IBaseMessage outMsg = pInMsg;
            outMsg.BodyPart.Data = ms;
            return outMsg;
        }
        // return pInMsg;
    }
    #endregion
}

