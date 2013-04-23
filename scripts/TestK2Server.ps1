Param(
$RuntimeDir="",

	[parameter(Mandatory=$true)]          
    [ValidateNotNullOrEmpty()]     
	[string]$Environment

)
###cd "C:\Program Files (x86)\MSBuild\SourceCode\v3.5\"
$ErrorActionPreference ="Stop"
$CURRENTDIR=pwd

"current directory is $CURRENTDIR"
"Environment selected is $Environment"

trap {write-host "error"+ $error[0].ToString() + $error[0].InvocationInfo.PositionMessage  -Foregroundcolor Red; cd "$CURRENTDIR"; read-host 'There has been an error'; break}

$pwd=pwd
$RuntimeDir="$pwd\Scripts"
$pwd
$RuntimeDir
Import-Module $RuntimeDir\K2Module.psm1
Import-Module $RuntimeDir\GenericModule.psm1

$K2Host=Get-EnvironmentSettingFromXML -ManifestFile "$RuntimeDir\ListOfServers.xml" -Setting K2Host -Environment $Environment -ParentNode ListOfServers
"Trying to connect to $K2Host"
Test-K2Server -k2Host $K2Host 




# # # # write-host "Remove this read line"
# # # # read-host "did it work"

