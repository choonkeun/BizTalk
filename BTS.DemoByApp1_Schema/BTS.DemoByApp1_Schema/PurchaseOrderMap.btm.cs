namespace BTS.DemoByApp1_Schema {
    
    
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BTS.DemoByApp1_Schema.PurchaseOrder", typeof(global::BTS.DemoByApp1_Schema.PurchaseOrder))]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BTS.DemoByApp1_Schema.PurchaseOrderAccepted", typeof(global::BTS.DemoByApp1_Schema.PurchaseOrderAccepted))]
    public sealed class PurchaseOrderMap : global::Microsoft.XLANGs.BaseTypes.TransformBase {
        
        private const string _strMap = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:var=""http://schemas.microsoft.com/BizTalk/2003/var"" exclude-result-prefixes=""msxsl var s0 userCSharp"" version=""1.0"" xmlns:s0=""http://BTS.DemoByApp1_Schema.PurchaseOrder"" xmlns:ns0=""http://BTS.DemoByApp1_Schema.PurchaseOrderAccepted"" xmlns:userCSharp=""http://schemas.microsoft.com/BizTalk/2003/userCSharp"">
  <xsl:output omit-xml-declaration=""yes"" method=""xml"" version=""1.0"" />
  <xsl:template match=""/"">
    <xsl:apply-templates select=""/s0:PurchaseOrder"" />
  </xsl:template>
  <xsl:template match=""/s0:PurchaseOrder"">
    <xsl:variable name=""var:v1"" select=""userCSharp:DateCurrentDateTime()"" />
    <ns0:PurchaseOrderAccepted>
      <xsl:attribute name=""OrderDate"">
        <xsl:value-of select=""$var:v1"" />
      </xsl:attribute>
      <xsl:for-each select=""Address"">
        <Address>
          <xsl:if test=""@Type"">
            <xsl:attribute name=""Type"">
              <xsl:value-of select=""@Type"" />
            </xsl:attribute>
          </xsl:if>
          <Name>
            <xsl:value-of select=""Name/text()"" />
          </Name>
          <Street>
            <xsl:value-of select=""Street/text()"" />
          </Street>
          <City>
            <xsl:value-of select=""City/text()"" />
          </City>
          <State>
            <xsl:value-of select=""State/text()"" />
          </State>
          <Zip>
            <xsl:value-of select=""Zip/text()"" />
          </Zip>
          <Country>
            <xsl:value-of select=""Country/text()"" />
          </Country>
          <xsl:value-of select=""./text()"" />
        </Address>
      </xsl:for-each>
      <DeliveryNotes>
        <xsl:value-of select=""DeliveryNotes/text()"" />
      </DeliveryNotes>
      <Items>
        <xsl:for-each select=""Items/Item"">
          <Item>
            <xsl:if test=""@PartNumber"">
              <xsl:attribute name=""PartNumber"">
                <xsl:value-of select=""@PartNumber"" />
              </xsl:attribute>
            </xsl:if>
            <ProductName>
              <xsl:value-of select=""ProductName/text()"" />
            </ProductName>
            <Quantity>
              <xsl:value-of select=""Quantity/text()"" />
            </Quantity>
            <USPrice>
              <xsl:value-of select=""USPrice/text()"" />
            </USPrice>
            <Comment>
              <xsl:value-of select=""Comment/text()"" />
            </Comment>
            <xsl:value-of select=""./text()"" />
          </Item>
        </xsl:for-each>
        <xsl:value-of select=""Items/text()"" />
      </Items>
    </ns0:PurchaseOrderAccepted>
  </xsl:template>
  <msxsl:script language=""C#"" implements-prefix=""userCSharp""><![CDATA[
public string DateCurrentDateTime()
{
	DateTime dt = DateTime.Now;
	string curdate = dt.ToString(""yyyy-MM-dd"", System.Globalization.CultureInfo.InvariantCulture);
	string curtime = dt.ToString(""T"", System.Globalization.CultureInfo.InvariantCulture);
	string retval = curdate + ""T"" + curtime;
	return retval;
}



]]></msxsl:script>
</xsl:stylesheet>";
        
        private const int _useXSLTransform = 0;
        
        private const string _strArgList = @"<ExtensionObjects />";
        
        private const string _strSrcSchemasList0 = @"BTS.DemoByApp1_Schema.PurchaseOrder";
        
        private const global::BTS.DemoByApp1_Schema.PurchaseOrder _srcSchemaTypeReference0 = null;
        
        private const string _strTrgSchemasList0 = @"BTS.DemoByApp1_Schema.PurchaseOrderAccepted";
        
        private const global::BTS.DemoByApp1_Schema.PurchaseOrderAccepted _trgSchemaTypeReference0 = null;
        
        public override string XmlContent {
            get {
                return _strMap;
            }
        }
        
        public override int UseXSLTransform {
            get {
                return _useXSLTransform;
            }
        }
        
        public override string XsltArgumentListContent {
            get {
                return _strArgList;
            }
        }
        
        public override string[] SourceSchemas {
            get {
                string[] _SrcSchemas = new string [1];
                _SrcSchemas[0] = @"BTS.DemoByApp1_Schema.PurchaseOrder";
                return _SrcSchemas;
            }
        }
        
        public override string[] TargetSchemas {
            get {
                string[] _TrgSchemas = new string [1];
                _TrgSchemas[0] = @"BTS.DemoByApp1_Schema.PurchaseOrderAccepted";
                return _TrgSchemas;
            }
        }
    }
}
