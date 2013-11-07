<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet 
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ns="http://schemas.microsoft.com/developer/msbuild/2003"
	xmlns="http://schemas.microsoft.com/developer/msbuild/2003"
	exclude-result-prefixes="ns">

	<xsl:output omit-xml-declaration="no" indent="yes" method="xml"/>

	<xsl:template match="node()|@*">
      <xsl:copy>
         <xsl:apply-templates select="node()|@*"/>
      </xsl:copy>
    </xsl:template>


    <xsl:template match='ns:Project/ns:PropertyGroup/ns:OutputPath'>
		<OutputPath><xsl:value-of select="translate(text(), 'net45', 'net40')"/></OutputPath>
	</xsl:template>

	<xsl:template match='ns:Project/ns:PropertyGroup/ns:DocumentationFile'>
		<DocumentationFile><xsl:value-of select="translate(text(), 'net45', 'net40')"/></DocumentationFile>
	</xsl:template>

	<xsl:template match="ns:Project/ns:PropertyGroup/ns:TargetFrameworkVersion">
		<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
	</xsl:template>

	<xsl:template match='ns:Project/ns:PropertyGroup/ns:DefineConstants'>
		<DefineConstants><xsl:value-of select="translate(text(), 'NET45', 'NET40')"/></DefineConstants>
	</xsl:template>

	<xsl:template match="ns:Project/ns:ItemGroup/ns:ProjectReference/@Include">
		<xsl:attribute name="Include">
			<xsl:value-of select="concat(substring-before(.,'.csproj'),'.NET40.csproj')"/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="ns:Project/ns:ItemGroup/ns:Compile[contains(@Include,'Async')]" />
</xsl:stylesheet>