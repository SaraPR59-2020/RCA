<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="RedditService" generation="1" functional="0" release="0" Id="833c7859-93e5-4fec-a691-d9bfc6de99f6" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="RedditServiceGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="HealthStatusServiceWeb:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/RedditService/RedditServiceGroup/LB:HealthStatusServiceWeb:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="RedditServiceWeb:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/RedditService/RedditServiceGroup/LB:RedditServiceWeb:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="HealthMonitoringServiceWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapHealthMonitoringServiceWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthMonitoringServiceWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapHealthMonitoringServiceWorkerInstances" />
          </maps>
        </aCS>
        <aCS name="HealthStatusServiceWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapHealthStatusServiceWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthStatusServiceWebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapHealthStatusServiceWebInstances" />
          </maps>
        </aCS>
        <aCS name="NotificationServiceWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapNotificationServiceWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="NotificationServiceWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapNotificationServiceWorkerInstances" />
          </maps>
        </aCS>
        <aCS name="RedditServiceWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapRedditServiceWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="RedditServiceWebInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/RedditService/RedditServiceGroup/MapRedditServiceWebInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:HealthStatusServiceWeb:Endpoint1">
          <toPorts>
            <inPortMoniker name="/RedditService/RedditServiceGroup/HealthStatusServiceWeb/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:RedditServiceWeb:Endpoint1">
          <toPorts>
            <inPortMoniker name="/RedditService/RedditServiceGroup/RedditServiceWeb/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapHealthMonitoringServiceWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/RedditService/RedditServiceGroup/HealthMonitoringServiceWorker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapHealthMonitoringServiceWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/HealthMonitoringServiceWorkerInstances" />
          </setting>
        </map>
        <map name="MapHealthStatusServiceWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/RedditService/RedditServiceGroup/HealthStatusServiceWeb/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapHealthStatusServiceWebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/HealthStatusServiceWebInstances" />
          </setting>
        </map>
        <map name="MapNotificationServiceWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/RedditService/RedditServiceGroup/NotificationServiceWorker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapNotificationServiceWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/NotificationServiceWorkerInstances" />
          </setting>
        </map>
        <map name="MapRedditServiceWeb:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/RedditService/RedditServiceGroup/RedditServiceWeb/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapRedditServiceWebInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/RedditServiceWebInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="HealthMonitoringServiceWorker" generation="1" functional="0" release="0" software="C:\Users\stame\source\repos\RCAReddit\RCA\backend\RedditService\RedditService\csx\Debug\roles\HealthMonitoringServiceWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;HealthMonitoringServiceWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringServiceWorker&quot; /&gt;&lt;r name=&quot;HealthStatusServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationServiceWorker&quot; /&gt;&lt;r name=&quot;RedditServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/HealthMonitoringServiceWorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/RedditService/RedditServiceGroup/HealthMonitoringServiceWorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/RedditService/RedditServiceGroup/HealthMonitoringServiceWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="HealthStatusServiceWeb" generation="1" functional="0" release="0" software="C:\Users\stame\source\repos\RCAReddit\RCA\backend\RedditService\RedditService\csx\Debug\roles\HealthStatusServiceWeb" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="8080" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;HealthStatusServiceWeb&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringServiceWorker&quot; /&gt;&lt;r name=&quot;HealthStatusServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationServiceWorker&quot; /&gt;&lt;r name=&quot;RedditServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/HealthStatusServiceWebInstances" />
            <sCSPolicyUpdateDomainMoniker name="/RedditService/RedditServiceGroup/HealthStatusServiceWebUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/RedditService/RedditServiceGroup/HealthStatusServiceWebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="NotificationServiceWorker" generation="1" functional="0" release="0" software="C:\Users\stame\source\repos\RCAReddit\RCA\backend\RedditService\RedditService\csx\Debug\roles\NotificationServiceWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;NotificationServiceWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringServiceWorker&quot; /&gt;&lt;r name=&quot;HealthStatusServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationServiceWorker&quot; /&gt;&lt;r name=&quot;RedditServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/NotificationServiceWorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/RedditService/RedditServiceGroup/NotificationServiceWorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/RedditService/RedditServiceGroup/NotificationServiceWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="RedditServiceWeb" generation="1" functional="0" release="0" software="C:\Users\stame\source\repos\RCAReddit\RCA\backend\RedditService\RedditService\csx\Debug\roles\RedditServiceWeb" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;RedditServiceWeb&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringServiceWorker&quot; /&gt;&lt;r name=&quot;HealthStatusServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationServiceWorker&quot; /&gt;&lt;r name=&quot;RedditServiceWeb&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/RedditService/RedditServiceGroup/RedditServiceWebInstances" />
            <sCSPolicyUpdateDomainMoniker name="/RedditService/RedditServiceGroup/RedditServiceWebUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/RedditService/RedditServiceGroup/RedditServiceWebFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="RedditServiceWebUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="HealthStatusServiceWebUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="NotificationServiceWorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="HealthMonitoringServiceWorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="HealthMonitoringServiceWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="HealthStatusServiceWebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="NotificationServiceWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="RedditServiceWebFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="HealthMonitoringServiceWorkerInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="HealthStatusServiceWebInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="NotificationServiceWorkerInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="RedditServiceWebInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="4e65bbc8-67a9-47f7-84be-0001afa7764e" ref="Microsoft.RedDog.Contract\ServiceContract\RedditServiceContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="d15d115d-9c1b-4df2-8d10-3421d221eaf5" ref="Microsoft.RedDog.Contract\Interface\HealthStatusServiceWeb:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/RedditService/RedditServiceGroup/HealthStatusServiceWeb:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="a086aef4-ead5-4c02-aaad-016f7f8204b5" ref="Microsoft.RedDog.Contract\Interface\RedditServiceWeb:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/RedditService/RedditServiceGroup/RedditServiceWeb:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>