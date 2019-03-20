namespace PipelineDecode
{
    using System;
    using System.Collections.Generic;
    using Microsoft.BizTalk.PipelineOM;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    
    
    public sealed class ReceivePipeline : Microsoft.BizTalk.PipelineOM.ReceivePipeline
    {
        
        private const string _strPipeline = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instanc"+
"e\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MajorVersion=\"1\" MinorVersion=\"0\">  <Description /> "+
" <CategoryId>f66b9f5e-43ff-4f5f-ba46-885348ae1b4e</CategoryId>  <FriendlyName>Receive</FriendlyName>"+
"  <Stages>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"1\" Name=\"Decode\" minOccurs=\""+
"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4103-4cce-4536-83fa-4a5040674ad6\" />      <Component"+
"s>        <Component>          <Name>PipelineComponent.ExcelToFlatFile,ExcelToFlatFile, Version=1.0."+
"0.0, Culture=neutral, PublicKeyToken=fac3fe6804f7dbf7</Name>          <ComponentName>ExcelToFlatFile"+
"</ComponentName>          <Description>BizTalk Receive Pipeline Component to convert .xlsx to Flat F"+
"iles</Description>          <Version>1.0</Version>          <Properties>            <Property Name=\""+
"TempFolder\">              <Value xsi:type=\"xsd:string\" />            </Property>            <Propert"+
"y Name=\"TargetFolder\">              <Value xsi:type=\"xsd:string\" />            </Property>          "+
"  <Property Name=\"FieldDelimiter\">              <Value xsi:type=\"xsd:string\" />            </Propert"+
"y>            <Property Name=\"NameSpace\">              <Value xsi:type=\"xsd:string\" />            </"+
"Property>          </Properties>          <CachedDisplayName>ExcelToFlatFile</CachedDisplayName>    "+
"      <CachedIsManaged>true</CachedIsManaged>        </Component>      </Components>    </Stage>    "+
"<Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"2\" Name=\"Disassemble\" minOccurs=\"0\" maxOcc"+
"urs=\"-1\" execMethod=\"FirstMatch\" stageId=\"9d0e4105-4cce-4536-83fa-4a5040674ad6\" />      <Components "+
"/>    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"3\" Name=\"Validate\" minOc"+
"curs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e410d-4cce-4536-83fa-4a5040674ad6\" />      <Com"+
"ponents />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"4\" Name=\"Resolve"+
"Party\" minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e410e-4cce-4536-83fa-4a5040674ad6\" "+
"/>      <Components />    </Stage>  </Stages></Document>";
        
        private const string _versionDependentGuid = "04458eee-420e-476d-b236-bf40dd8d8ffe";
        
        public ReceivePipeline()
        {
            Microsoft.BizTalk.PipelineOM.Stage stage = this.AddStage(new System.Guid("9d0e4103-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.all);
            IBaseComponent comp0 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("PipelineComponent.ExcelToFlatFile,ExcelToFlatFile, Version=1.0.0.0, Culture=neutral, PublicKeyToken=fac3fe6804f7dbf7");;
            if (comp0 is IPersistPropertyBag)
            {
                string comp0XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"TempFolder\">   "+
"   <Value xsi:type=\"xsd:string\" />    </Property>    <Property Name=\"TargetFolder\">      <Value xsi:"+
"type=\"xsd:string\" />    </Property>    <Property Name=\"FieldDelimiter\">      <Value xsi:type=\"xsd:st"+
"ring\" />    </Property>    <Property Name=\"NameSpace\">      <Value xsi:type=\"xsd:string\" />    </Pro"+
"perty>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp0XmlProperties);;
                ((IPersistPropertyBag)(comp0)).Load(pb, 0);
            }
            this.AddComponent(stage, comp0);
        }
        
        public override string XmlContent
        {
            get
            {
                return _strPipeline;
            }
        }
        
        public override System.Guid VersionDependentGuid
        {
            get
            {
                return new System.Guid(_versionDependentGuid);
            }
        }
    }
}
