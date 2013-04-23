[CmdletBinding()]
Param($runTimeDir="C:\Users\Administrator\Documents\GitHub\K2PowershellLibaries",
	[bool]$DoNotStop=$false
   )

$CURRENTDIR=pwd
$DebugPreference="Continue"
write-debug "A.RegisterPowerscriptModules.ps1"
write-verbose "Script designed to be double-clicked and just work with default parameters, not closing when finished or erroring."
trap {write-host "error"+ $error[0].ToString() + $error[0].InvocationInfo.PositionMessage  -Foregroundcolor Red; cd "$CURRENTDIR"; read-host 'There has been an error'; break}


$strInputFile=$profile.AllUsersAllHosts
$strInputFile

# Import the modules so that script below this can actually run.
Import-Module $runTimeDir\scripts\GenericModule.psm1
Import-Module $runTimeDir\scripts\K2Module.psm1

### The content between @" "@ will be put into the allusers profile
### This is so we do not have to keep finding and registering these modules
$profileContent = @"

Import-Module $runTimeDir\scripts\GenericModule.psm1
Import-Module $runTimeDir\scripts\K2Module.psm1

Set-K2BlackPearlDirectory
Add-GlobalVariables

"@

if (!(test-path $strInputFile)) 
{
	write-debug "all users profile is not found: $strInputFile"
	new-item -type file -path $strInputFile -force
	$strInputFile=$profile.AllUsersAllHosts
	$strInputFile
}
if ( (get-childitem $strInputFile).length -eq 0 )
{
	write-debug " AllUsers powershell profileInput text file: $strInputFile is empty."
	[bool]$DoNotReplace=$false
}
else
{
	$title = "Replace contents of the powershell profile?"
	$message= "Would you like to replace the contents of the all user powershell profile? This will enable the rest of the scripts to run, but override all existing settings"
	$options =@('&Yes','&No')
	$PromptOptions = [System.Management.Automation.Host.ChoiceDescription[]]($options)
	$DoNotReplace= $host.ui.PromptForChoice($title, $message, $PromptOptions, 0) 
	if($DoNotReplace)
	{
		$title = "ERROR"
		$message= "You will need to add the following to $strInputFile manually $profileContent"
		$options =@('&OK')
		$PromptOptions = [System.Management.Automation.Host.ChoiceDescription[]]($options)
		$throwaway= $host.ui.PromptForChoice($title, $message, $PromptOptions, 0) 
	}
}
if(!$DoNotReplace)
{
	Write-Verbose "Overwriting $strInputFile with $profileContent"
	Set-Content -Value $profileContent -Path $strInputFile
}
if (!(test-path $strInputFile)) 
{
		$title = "ERROR"
        $message= "Script cannot find powershell profile path. You will need to run the following manually $profileContent"
        $options =@('&OK')
        $PromptOptions = [System.Management.Automation.Host.ChoiceDescription[]]($options)
        $DoNotReplace= $host.ui.PromptForChoice($title, $message, $PromptOptions, 0) 
}


If($DoNotStop){Write-Host "======Finished======"} else {Read-Host "======Finished======"}

