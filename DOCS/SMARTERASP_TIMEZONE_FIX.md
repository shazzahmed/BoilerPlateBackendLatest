<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <!-- Add this section to set timezone for your application -->
    <runtime>
      <appDomainManagerAssembly value="System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" />
      <appDomainManagerType value="System.Web.Hosting.AppDomainManager" />
    </runtime>
    
    <!-- Set application timezone -->
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    
    <aspNetCore processPath="dotnet" 
                arguments=".\SMSBACKEND.Presentation.dll" 
                stdoutLogEnabled="false" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess">
      <environmentVariables>
        <!-- Set timezone to Pakistan Standard Time -->
        <environmentVariable name="TZ" value="Asia/Karachi" />
        <environmentVariable name="WEBSITE_TIME_ZONE" value="Pakistan Standard Time" />
      </environmentVariables>
    </aspNetCore>
  </system.webServer>
</configuration>

