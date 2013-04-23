using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using SourceCode.EnvironmentSettings.Client;
using SourceCode.Framework.Deployment;
using SourceCode.ProjectSystem;
using SourceCode.Workflow.Design.EnvironmentSettings;

//References to 
//•         Microsoft.Build.Engine
//•         Microsoft.Build.Framework
//•         SourceCode.EnvironmentSettings.Client
//•         SourceCode.Framework
//•         SourceCode.Workflow.Design

//NOTE: You'll need to build and copy this and the above .targets file into your C:\Program Files\MSBuild folder.
namespace K2Field.Utilities.Build
{

    /// <summary>
    /// Author: http://www.danielflippance.com
    /// 
    /// This class is an MSBuild task used to deploy a K2 Blackpearl project to a K2 server
    /// K2 requires that all files in the K2 project are writable in order to perform a deployment
    /// so we copy the entire project folder and all it's containing files to a Temp folder.
    /// We then make all those files writable and then perform the deployment.
    /// 
    /// Output files, including MSBuild files are copied to the OutputPath 
    /// 
    /// This code was based on the example at:
    /// http://www.k2underground.com/blogs/pitchblack/archive/2008/04/30/automatic-deployment-of-k2-process-and-k2-smartobject-artefacts-using-thesourcecode-deployment-framework-and-msbuild-assemblies.aspx
    /// http://www.csharp411.com/c-copy-folder-recursively/
    /// http://www.west-wind.com/weblog/posts/4072.aspx
    /// </summary>
    public class K2Deploy : Task
    {
        #region Properties
        private string server = string.Empty;

        [Required]
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        private int port;

        [Required]
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private string environment = string.Empty;

        /// <summary>
        /// This is no longer required. The project will try to find settings for ALL environments
        /// Lee Adams 23rd Nov
        /// </summary>
        public string Environment
        {
            get { return environment; }
            set { environment = value; }
        }

        /// <summary>
        /// The location of the .csproj file containing the K2 SmartObjects or Workflow Processes
        /// </summary>
        private string projectPath;

        [Required]
        public string ProjectPath
        {
            get { return projectPath; }
            set { projectPath = value; }
        }

        /// <summary>
        /// The folder name where the output files will be created
        /// </summary>
        private string outputPath;

        [Required]
        public string OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; }
        }

        private string ConnectionString
        {
            get
            {
                return string.Format(
                    "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host={0};Port={1}",
                    server, port);
            }
        }

        #endregion

        public override bool Execute()
        {
            Console.WriteLine("Environment {0}", Environment);
            Console.WriteLine("Server {0}", Server);
            Console.WriteLine("Port {0}", Port);
            Console.WriteLine("ProjectPath {0}", ProjectPath);
            Console.WriteLine("OutputPath {0}", OutputPath);

            Project project;
            EnvironmentSettingsManager environmentManager;
            DeploymentResults results;
            DeploymentPackage package;

            bool result = false;

            //Create a temporary folder for the K2 project files
            string tempPath = Path.GetTempPath().Trim('\\').Trim('/');
            string k2DeployFolder = tempPath + @"\K2Deploy";
            DeleteDirectory(k2DeployFolder);

            try
            {

                Console.WriteLine("\n\n\n************** BEGIN PACKAGE ******************\n");

                //Check parameters
                if (!ProjectPath.EndsWith(".k2proj")) throw new ArgumentException("ProjectPath must end with .k2proj");

                //Create a temporary folder for the code
                string projectFolder = ProjectPath.Substring(0, ProjectPath.LastIndexOf('\\'));

                Console.WriteLine("\nProject Folder: " + projectFolder);

                //Copy the files to the temp folder

                Console.WriteLine("\nCreating temporary folder: " + k2DeployFolder);

                CopyFolder(projectFolder, k2DeployFolder);

                //Ensure we have access to all the files.

                Console.WriteLine("\nSetting writable permissions for folder: " + tempPath);

                bool success = SetAcl(k2DeployFolder, "F", true);
                if (!success) throw new Exception("Failed to set ACLs on folder " + tempPath);

                //Ensure the feils are all writable
                SetWritable(k2DeployFolder);

                //Load the project file
                string newProjectFile = k2DeployFolder + @"\" + ProjectPath.Substring(1 + ProjectPath.LastIndexOf('\\'));
                Console.WriteLine("\nLoading project file: " + newProjectFile);
                project = new Project();
                project.Load(newProjectFile);

                // Compile the K2 Project
                Console.WriteLine("\nCompiling project file: " + newProjectFile);
                results = project.Compile();

                //Grab the deployment package
                environmentManager = GetEnvironmentManager();
                package = GetDeploymentPackage(project, environmentManager);


                Console.WriteLine("\nSetting writable permissions for folder: " + OutputPath);

                success = SetAcl(OutputPath, "F", true);
                if (!success) throw new Exception("Failed to set ACLs on folder " + OutputPath);
                //Ensure the feils are all writable
                SetWritable(OutputPath);


                Console.WriteLine("\nSaving deployment package to folder: " + OutputPath);
                package.Save(OutputPath, "K2DeploymentPackage");
                result = true;
                //////Console.WriteLine("\nExecuting deployment package...");

                //////results = package.Execute();
                //////Console.WriteLine("\nSuccessful = " + results);

                //////result = results.Successful;

                //////Console.WriteLine("\nSuccessful = " + result);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw;
            }
            finally
            {
                Console.WriteLine("\nDeleting k2DeployFolder " + k2DeployFolder);
                DeleteDirectory(k2DeployFolder);
            }

            Console.WriteLine("\n\n\n************** END PACKAGE ******************\n\n\n");

            return result;
        }

        private EnvironmentSettingsManager GetEnvironmentManager()
        {
            EnvironmentSettingsManager environmentManager = new EnvironmentSettingsManager(false, false);

            // This is weird but the only way I could get it to work!
            environmentManager.ConnectToServer(ConnectionString);
            environmentManager.InitializeSettingsManager();
            environmentManager.ConnectToServer(ConnectionString);

            if (!string.IsNullOrEmpty(environment))
                environmentManager.ChangeEnvironment(environment);
            environmentManager.InitializeSettingsManager();
            environmentManager.GetEnvironmentFields(environmentManager.CurrentEnvironment);

            //////////foreach (EnvironmentInstance env in environmentManager.CurrentTemplate.Environments)
            //////////{

            //////////    InitaliseEnvironmentForSpecifiedEnvironment(environmentManager, env.EnvironmentName);
            //////////}

            return environmentManager;
        }

        private void InitaliseEnvironmentForSpecifiedEnvironment(EnvironmentSettingsManager environmentManager, string devString)
        {
            Console.WriteLine("\n**************** GetEnvironmentFields({0})*********************", devString);
            environmentManager.ConnectToServer(ConnectionString);
            environmentManager.InitializeSettingsManager();
            environmentManager.ConnectToServer(ConnectionString);

            environmentManager.ChangeEnvironment(devString);
            environmentManager.InitializeSettingsManager();
            environmentManager.GetEnvironmentFields(environmentManager.CurrentEnvironment);
        }


        private DeploymentPackage GetDeploymentPackage(Project project, EnvironmentSettingsManager environmentManager)
        {
            DeploymentPackage package;

            package = project.CreateDeploymentPackage();

            var listEnvirnonments = new List<string>();
            // Populate Environment Fields 
            foreach (EnvironmentInstance env in environmentManager.CurrentTemplate.Environments)
            {
                listEnvirnonments.Add(env.EnvironmentName);
            }

            foreach (var envName in listEnvirnonments)
            {
                DeploymentEnvironment depEnv = package.AddEnvironment(envName);

                Console.WriteLine("\n****************Environment Fields********************* FOR {0}", envName);


                //////environmentManager.ChangeEnvironment(env.EnvironmentName);
                InitaliseEnvironmentForSpecifiedEnvironment(environmentManager, envName);
                foreach (EnvironmentInstance env in environmentManager.CurrentTemplate.Environments)
                {
                   ////// Console.WriteLine("\n foreach (EnvironmentInstance env in environmentManager.CurrentTemplate.Environments) FOR {0}", env.EnvironmentName);
                    if (env.EnvironmentName.Equals(envName))
                    {
                        //////Console.WriteLine("\n****env.EnvironmentName.Equals(envName)***** FOR {0} {1}", env.EnvironmentName, envName);
                        foreach (EnvironmentField field in env.EnvironmentFields)
                        {
                            //////Console.WriteLine("\n****************Environment Fields********************* FOR {0}", envName);
                            Console.WriteLine("{0} = {1}", depEnv.Properties[field.FieldName], field.Value);

                            depEnv.Properties[field.FieldName] = field.Value;
                        }
                    }
                }
                //////Console.WriteLine("\n**************** InitializeSettingsManager()*********************");
                //////environmentManager.InitializeSettingsManager();
                //////Console.WriteLine("\n**************** GetEnvironmentFields({0})*********************", environmentManager.CurrentEnvironment.EnvironmentName);
                //////environmentManager.GetEnvironmentFields(environmentManager.CurrentEnvironment);

            }


            package.SelectedEnvironment = environmentManager.CurrentEnvironment.EnvironmentName;
            package.DeploymentLabelName = DateTime.Now.ToString();
            package.DeploymentLabelDescription = string.Empty;
            package.TestOnly = false;


            Console.WriteLine("\nEnvironment Info:\n\n");
            Console.WriteLine("\nSelectedEnvironment = " + package.SelectedEnvironment);
            Console.WriteLine("\nDeploymentLabelName = " + package.DeploymentLabelName);
            Console.WriteLine("\nDeploymentLabelDescription = " + package.DeploymentLabelDescription);
            Console.WriteLine("\nTestOnly = " + package.TestOnly);

            // Get the Default SmartObject Server in the Environment
            // The prefix "$Field=" is when the value of the SmartObject server is registered in the environment fields collection.
            // this will do a lookup in the environment with the display name of the field, and use the value.
            // If you set the value directly, no lookups will be performed.
            EnvironmentField smartObjectServerField =
                environmentManager.CurrentEnvironment.GetDefaultField(typeof(SmartObjectField));
            package.SmartObjectConnectionString = "$Field=" + smartObjectServerField.DisplayName;


            Console.WriteLine("\npackage.SmartObjectConnectionString = " + package.SmartObjectConnectionString);

            // Get the Default Workflow Management Server in the Environment
            EnvironmentField workflowServerField =
                environmentManager.CurrentEnvironment.GetDefaultField(typeof(WorkflowManagementServerField));

            package.WorkflowManagementConnectionString = "$Field=" + workflowServerField.DisplayName;

            Console.WriteLine("\npackage.WorkflowManagementConnectionString = " + package.WorkflowManagementConnectionString);

            return package;
        }

        /// <summary>
        /// Recursively copy the source folder to the destination folder
        /// Created destination folder if necessary
        /// From: http://www.csharp411.com/c-copy-folder-recursively/
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        /// <summary>
        /// Recursively set the ACL on a folder
        /// From: http://www.west-wind.com/weblog/posts/4072.aspx
        /// </summary>
        /// <returns></returns>
        private bool SetAcl(string folderName, string userRights, bool inheritSubDirectories)
        {

            if (folderName == null || folderName == "")
            {
                Console.WriteLine("Path cannot be empty.");
                return false;
            }

            // *** Strip off trailing backslash which isn't supported
            folderName = folderName.TrimEnd('\\');
            FileSystemRights rights = (FileSystemRights)0;

            if (userRights == "R")
                rights = FileSystemRights.ReadAndExecute;
            else if (userRights == "C")
                rights = FileSystemRights.ChangePermissions;
            else if (userRights == "F")
                rights = FileSystemRights.FullControl;

            // *** Add Access Rule to the actual directory itself
            string currentUserName = System.Environment.UserDomainName + @"\" + System.Environment.UserName;
            FileSystemAccessRule accessRule = new FileSystemAccessRule(currentUserName, rights,
                          InheritanceFlags.None,
                          PropagationFlags.NoPropagateInherit,
                          AccessControlType.Allow);

            DirectoryInfo Info = new DirectoryInfo(folderName);
            DirectorySecurity Security = Info.GetAccessControl(AccessControlSections.Access);

            bool Result = false;
            Security.ModifyAccessRule(AccessControlModification.Set, accessRule, out Result);

            if (!Result)
                return false;

            // *** Always allow objects to inherit on a directory 
            InheritanceFlags iFlags = InheritanceFlags.ObjectInherit;
            if (inheritSubDirectories)
                iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

            // *** Add Access rule for the inheritance
            accessRule = new FileSystemAccessRule(currentUserName, rights,
                          iFlags,
                          PropagationFlags.InheritOnly,
                          AccessControlType.Allow);
            Result = false;
            Security.ModifyAccessRule(AccessControlModification.Add, accessRule, out Result);

            if (!Result) return false;

            Info.SetAccessControl(Security);

            return true;
        }

        private void SetWritable(string folder)
        {
            foreach (string f in Directory.GetFiles(folder)) File.SetAttributes(f, FileAttributes.Normal);
            foreach (string d in Directory.GetDirectories(folder)) SetWritable(d);
        }

        private void DeleteDirectory(string folder)
        {
            if (Directory.Exists(folder))
            {
                foreach (string f in Directory.GetFiles(folder)) File.Delete(f);
                foreach (string d in Directory.GetDirectories(folder)) DeleteDirectory(d);
                Directory.Delete(folder, true);
            }
        }
    }

}
