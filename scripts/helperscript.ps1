[CmdletBinding()]
Param()
New-K2Packages -K2ServerWithAllEnvSettings localhost -SourceCodePathToDiscoverK2ProjFiles C:\tfs\K2.Shared -DeploymentPath C:\tfs\K2.Shared\Deployment -debug -verbose
###. .\4.BuildAndPAckageK2SharedSolution.ps1 -K2ServerWithAllEnvSettings localhost -SourceCodePathToDiscoverK2ProjFiles C:\tfs\K2.Shared -DeploymentPath C:\tfs\K2.Shared\Deployment -DoNotStop $true

