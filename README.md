A Collection of Powershell modules to interact with K2 products (blackpearl)

Also included are Solutions from K2 Underground for Packaging and Deploying Solutions and Service brokers.

This gives you a way to include K2 blackpearl workflows, smartObjects and Service brokers as part of you Continuous Integration Build.

There are helper scripts available, but you will need to manually edit them to change to your development path.

See my [FAQ](FAQ.txt) https://github.com/k2workflow/K2PowershellLibaries/blob/master/FAQ.txt page for more details.

# Scripts

## Tested in Production (At enterprise client Sites)

__WARNING: EDIT THE PATH IN THESE FILES BEFORE RUNNING__

- A.RegisterPowerscriptModules.ps1 - Registers the K2 modules in the All Users profile. SORRY IT OVERWITES THE EXISTING PROFILE (although it will prompt you)
- C.RegisterServiceBrokers.ps1 - Registers Service brokers (and default SmartObjects) based on an XML file

## Untested in Production (Needs tidy-up)

- B.BuildandDeployAllK2SharedComponents.ps1 - Builds the Dependant .Net Solutions using the below scripts
- B.1.BuildAllK2SharedComponents.ps1
- B.2.DeployAllK2SharedComponents.ps1

# .NET Solutions 2010

## Tested in Production (At enterprise client Sites)

K2Field.Utilities.Deploy.sln - Uses 2 k2underground projects to Package (Simulate Right Click - Create Deploy Package)
These have been modified from the k2underground projects to fix bugs

http://www.k2underground.com/groups/msbuild_tasks_to_handle_deployment_of_custom_service_brokers_and_services_instances/default.aspx
http://www.k2underground.com/groups/k2_build_and_deploy_msbuild_tasks/default.aspx
