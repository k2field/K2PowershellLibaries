[CmdletBinding()]
Param(
	[string]$Environment=$null,
	[string]$ManifestFilePath="C:\Users\Administrator\Documents\GitHub\K2PowershellLibaries\Deployment\",
	[string]$ManifestFileName="EnvironmentMetaDataWorklistBroker.xml",
	[bool]$RestartK2Server=$true ,
	[bool]$DoNotStop=$false
   )

$CURRENTDIR=pwd
$DebugPreference="Continue"
write-debug "C.RegisterServiceBrokers.ps1"
write-verbose "Script designed to be double-clicked and just work with default parameters, not closing when finished or erroring."
write-verbose " Turn UAC OFF. It will change to the wrong directory"
trap {write-host "error"+ $error[0].ToString() + $error[0].InvocationInfo.PositionMessage  -Foregroundcolor Red; cd "$CURRENTDIR"; read-host 'There has been an error'; break}

Publish-K2ServiceBrokers -RestartK2Server $RestartK2Server -Environment $Environment -RootFilePath $ManifestFilePath -ManifestFileName $ManifestFileName -prompt $true -verbose ##-debug
   
### Remove this setting if you want to prompt for an environment
###-Environment $Environment

#####TODO: CALLING CODE HERE

If($DoNotStop){Write-Host "======Finished======"} else {Read-Host "======Finished======"}