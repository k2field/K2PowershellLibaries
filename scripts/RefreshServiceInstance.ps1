# This script creates a connection to the SmartObjectServer and then performs a refresh
# on the ServiceInstance that you specify by its guid.

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$smartObjectServerConnectionString, 
    [Parameter(Mandatory=$True)]
    [string]$serviceInstanceGuid
    )

Add-Type -Path 'C:\Program Files (x86)\K2 blackpearl\Bin\SourceCode.SmartObjects.Management.dll' -ErrorAction Stop

try {
    $result    = $False
    $stopWatch = New-Object System.Diagnostics.StopWatch

    $managementServer = New-Object Sourcecode.SmartObjects.Management.SmartObjectManagementServer
    $managementServer.CreateConnection() >$null

    if ($managementServer.Connection.Open($smartObjectServerConnectionString)) {
        $serviceGuid = [System.Guid]::Parse($serviceInstanceGuid);

        $stopWatch.Start();
        $result = $managementServer.RefreshService($serviceGuid);
        $stopWatch.Stop();

        $elapsed = $stopWatch.ElapsedMilliseconds.ToString();

        if ($result) {        
            Write-Host "Service $serviceGuid refreshed in $elapsed ms"
        }
        else {
            Write-Error "Refresh of service $serviceGuid ran for $elapsed ms and failed"
        }
    }
    else {
        Write-Error "Failed to open connection to SmartObject server"
    }
}
finally {    
    $managementServer.Connection.Close();
}
