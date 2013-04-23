Param($runTimeDir=$pwd)
$strInputFile=$profile.AllUsersAllHosts
$strInputFile

# Import the modules so that script below this can actually run.
Import-Module $runTimeDir\K2Module.psm1
Import-Module $runTimeDir\GenericModule.psm1

### The content between @" "@ will be put into the allusers profile
### This is so we do not have to keep finding and registering these modules
$profileContent = @"

Import-Module $runTimeDir\K2Module.psm1
Import-Module $runTimeDir\GenericModule.psm1
Set-K2BlackPearlDirectory

"@

if (!(test-path $strInputFile)) 
{
	new-item -type file -path $strInputFile -force
	$strInputFile=$profile.AllUsersAllHosts
	$strInputFile
}
if ( (get-childitem $strInputFile).length -eq 0 )
{
	write-host 'Cannot find AllUsers powershell profileInput text file $strInputFile is empty.' -forecolor Red
	[bool]$DoNotReplace=$true
}
else
{
	$title = "Replace contents of the powershell profile?"
	$message= "Would you like to replace the contents of the all user powershell profile? This will enable the rest of the scripts to run"
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


