
function Test-K2Connection
{
   [CmdletBinding()]
   param($k2Connection)
	trap {write-debug "$error[0]"; write-Output $false; continue}
	[bool]$IsConnected = $k2Connection.Connection.IsConnected
	Write-Verbose	 "Test-SmoConnection: K2Server Connected? $IsConnected"
	Write-Output $IsConnected
}

function Get-SmoConnection
{

    param([Parameter(Position=0, Mandatory=$true)] [string]$sqlserver)

 begin
 {
Write-host "** SourceCode.SmartObjects.Management"
[Reflection.Assembly]::LoadWithPartialName(“SourceCode.SmartObjects.Management”) | out-null

   $sqlconn = New-Object SourceCode.SmartObjects.Management.SmartObjectManagementServer
   ###new-object ("Microsoft.SqlServer.Management.Common.ServerConnection") $sqlserver

 #Write-host "SmartObjectManagementServer"
	###[SourceCode.SmartObjects.Management.SmartObjectManagementServer]$k2SMOServer 

	#Write-host "Creating the connection"
	$sqlconn.CreateConnection();
	
	#Write-host "Opening the connection"
	$sqlconn.Connection.Open($sqlserver);

	#$IsConnected = $sqlconn.Connection.IsConnected
	
	$IsConnected = Test-K2Connection $sqlconn
	Write-host	 "Is the SmartObjectManagementServer Connected? $IsConnected"
	##Write-Output $sqlconn

    ###$sqlconn.Connect()

 }
 
 process
 {

    Write-Output $sqlconn
}
 

}

function Get-CategoryConnection
{

    param([Parameter(Position=0, Mandatory=$true)] [string]$sqlserver)

 begin
 {
Write-host "** SourceCode.SmartObjects.Management"
[Reflection.Assembly]::LoadWithPartialName(“SourceCode.Categories.Client”) | out-null

   $sqlconn = New-Object SourceCode.Categories.Client.CategoryServer
   ###new-object ("Microsoft.SqlServer.Management.Common.ServerConnection") $sqlserver

 #Write-host "SmartObjectManagementServer"
	###[SourceCode.SmartObjects.Management.SmartObjectManagementServer]$k2SMOServer 

	#Write-host "Creating the connection"
	$sqlconn.CreateConnection();
	
	#Write-host "Opening the connection"
	#$sqlconn.Connection.Open($sqlserver);
#$sqlconn.Connection.Close();
	#$IsConnected = $sqlconn.Connection.IsConnected
	
	$IsConnected = Test-K2Connection $sqlconn
	Write-host	 "Is the CategoryManagementServer Connected? $IsConnected"
	##Write-Output $sqlconn

    ###$sqlconn.Connect()

 }
 
 process
 {

    Write-Output $sqlconn
}
 

}



$SmoConnection = Get-SmoConnection "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host=localhost;Port=5555" 
$CategoryConnection = Get-CategoryConnection "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host=localhost;Port=5555" 
$SmartObjectType =  [SourceCode.Categories.Client.CategoryServer+dataType]::SmartObject 
$SmartObjectType | Get-Member

###$IsConnected = $sqlconn.Connection.IsConnected
###Write-host	 "Is the SmartObjectManagementServer Connected? $IsConnected"
###$sqlconn | Get-Member
###$sqlconn[2] | Get-Member
Write-host	 "1111Is the SmartObjectManagementServer Connected? $IsConnected"
	Test-K2Connection $SmoConnection[2] -debug
Write-host	 "222Is the CategoryManagementServer Connected? $IsConnected"
	Test-K2Connection $CategoryConnection[2]  -debug

	