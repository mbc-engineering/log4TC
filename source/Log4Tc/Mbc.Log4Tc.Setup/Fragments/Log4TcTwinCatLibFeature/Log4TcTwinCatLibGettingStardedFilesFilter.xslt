<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">

  <xsl:output method="xml" indent="yes" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- Exclude filter pdb and .xml files-->
  <xsl:key name="service-search"
           match="wix:Component[contains(wix:File/@Source, '.pdb') or contains(wix:File/@Source, '.suo') or contains(wix:File/@Source, '.library')]"
           use="@Id" />
  <xsl:template match="wix:Component[key('service-search', @Id)]" />

</xsl:stylesheet>