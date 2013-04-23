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
using SourceCode.Hosting.Client.BaseAPI;

using K2Field.Utilities.ServiceObjectBuilder.HelperClasses;

namespace K2Field.Utilities.ServiceObjectBuilder
{
    /// <summary>
    /// This class is an MSBuild task used to deploy a K2 Blackpearl project to a K2 server
    /// K2 requires that all files in the K2 project are writable in order to perform a deployment
    /// so we copy the entire project folder and all it's containing files to a Temp folder.
    /// We then make all those files writable and then perform the deployment.
    /// Output files, including MSBuild files are copied to the OutputPath
    /// This code was based on the examples at:
    /// http://www.k2underground.com/blogs/pitchblack/archive/2008/04/30/automatic-deployment-of-k2-process-and-k2-smartobject-artefacts-using-thesourcecode-deployment-framework-and-msbuild-assemblies.aspx
    /// http://www.csharp411.com/c-copy-folder-recursively/
    /// http://www.west-wind.com/weblog/posts/4072.aspx
    /// </summary>
    public class K2Build : Task
    {
        #region Member Variables

        private string server = string.Empty;
        private string environment = string.Empty;
        private string projectFilePath = string.Empty;
        private string outputPath = string.Empty;
        private string deployLabel = string.Empty;
        private string deployDescription = string.Empty;
        private int hostServerPort;
        private bool testOnly;
        private bool useEnvironmentCache;
        private string windowsDomain = string.Empty;
        private string windowsUsername = string.Empty;
        private string windowsPassword = string.Empty;
        private string binfiletodelete = string.Empty;  // TODO: this value needs to be set
        //Edgar 6/3/2010
        private string tempDeployFolderPath = string.Empty;
        private bool compileArtifactsIndividually =false;

        #endregion

        #region Properties

        [Required]
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        [Required]
        public int HostServerPort
        {
            get { return hostServerPort; }
            set { hostServerPort = value; }
        }

        [Required]
        public string Environment
        {
            get { return environment; }
            set { environment = value; }
        }

        /// <summary>
        /// The location of the .csproj file containing the K2 SmartObjects or Workflow Processes
        /// </summary>
        [Required]
        public string ProjectFilePath
        {
            get { return projectFilePath; }
            set { projectFilePath = value; }
        }

        /// <summary>
        /// The folder name where the output files will be created
        /// </summary>
        [Required]
        public string OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; }
        }

        public string BinFileToDelete
        {
            get { return binfiletodelete; }
            set { binfiletodelete = value; }
        }

        public bool CompileProjectArtifactsIndividually
        {
            get { return compileArtifactsIndividually; }
            set { compileArtifactsIndividually = value; }
        }

        public string DeployLabel
        {
            get { return deployLabel; }
            set { deployLabel = value; }
        }

        public string DeployDescription
        {
            get { return deployDescription; }
            set { deployDescription = value; }
        }

        public bool TestOnly
        {
            get { return testOnly; }
            set { testOnly = value; }
        }

        public bool UseEnvironmentCache
        {
            get { return useEnvironmentCache; }
            set { useEnvironmentCache = value; }
        }

        public string WindowsDomain
        {
            get
            {
                return windowsDomain;
            }
            set
            {
                windowsDomain = value;
            }
        }

        public string WindowsUsername
        {
            get
            {
                return windowsUsername;
            }
            set
            {
                windowsUsername = value;
            }
        }

        public string WindowsPassword
        {
            get
            {
                return windowsPassword;
            }
            set
            {
                windowsPassword = value;
            }
        }

        private string ConnectionString
        {
            get
            {
                return K2Helper.GetK2ConnectionString(
                    Server, HostServerPort.ToString(), WindowsDomain, WindowsUsername, WindowsPassword);
            }
        }

        //Edgar 6/3/2010
        private string TempDeployFolderPath
        {
            get { return tempDeployFolderPath; }
            set { tempDeployFolderPath = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            EnvironmentSettingsManager environmentManager;
            
            string newProjectFilePath = string.Empty;
            bool result = false;

            int currentStep=0;


            // below if block added to check for and purge old cache file if it exists
            if (BinFileToDelete != string.Empty)
            {
                if (System.IO.File.Exists(BinFileToDelete))
                {
                    try
                    {
                        System.IO.File.Delete(BinFileToDelete);
                    }
                    catch (System.IO.IOException e)
                    {
                        LogHelper.LogMessage("\n\n*****Environment Cache Could Not Be Deleted*******\n");
                        LogHelper.LogMessage(e.Message.ToString());
                    }
                }
            }


            LogHelper.LogMessage("\n\n*****     BEGIN       *******\n");

            try
            {

                //check if file exists
                    //delete file

                #region 1. Validation
                LogHelper.LogStep(++currentStep, "Validating");
                ValidateK2Project();

                #endregion

                #region 2. Gather Environment Information
                LogHelper.LogStep(++currentStep, "Preparing Environment");
                LogHelper.LogMessage("   -- Getting K2 Environment Manager");

                environmentManager =
                    K2Helper.GetEnvironmentManager(ConnectionString, Environment, UseEnvironmentCache);

                #endregion

                #region 3. File Preparation
                LogHelper.LogStep(++currentStep,"Preparing Project files");
               
                //Edgar
                Dictionary<string, string> dictFiles = null;
                //check if the user want to compile the project as a whole or artifacts individually
                if (CompileProjectArtifactsIndividually)
                {
                    dictFiles = GetNewProjectFiles(ref newProjectFilePath);
                    LogHelper.LogMessage("\n3.1 Project files created in Temporary folder");

                    //we next need to create backup of the original file
                    FileHelper fileHelper = new FileHelper();

                    fileHelper.CopyFile(newProjectFilePath, newProjectFilePath + ".bak", true);
                    LogHelper.LogMessage("File has been backed up = "+ newProjectFilePath);

                    foreach (KeyValuePair<string, string> kvp in dictFiles)
                    {
                      //  currentStep += 1;
                        string artifactName = kvp.Key;
                        string newOutputPath = outputPath + @"\" + artifactName;

                        //we need to replace the original file w/ this one.
                       fileHelper.ReplaceFile(newProjectFilePath, kvp.Value);  

                        //it will always compile the the project under the original name;
                        result = BuildCompileAndSaveProject(newProjectFilePath, newOutputPath, environmentManager,currentStep);
                        currentStep += 1;
                    }

                    fileHelper = null;
                }
                else { 
                //default behavior as originally coded
                    newProjectFilePath = GetNewProjectFile();
                    LogHelper.LogMessage("\n3.1 Project files created in Temporary folder");
                    
                    BuildCompileAndSaveProject(newProjectFilePath, outputPath, environmentManager, currentStep);
                    currentStep += 1;
                }               
                //Edgar//
                #endregion
/* 
                #region 7. Execute Deployment Package

                
                // Execute the Deployment Package
                LogHelper.LogMessage("\nExecuting deployment package...");
                results = package.Execute();

                // Record success result
                result = results.Successful;
                LogHelper.LogMessage("\nSuccessful = " + result);
                 

                #endregion
*/

                result = true;
            }
            catch(Exception ex)
            {
                LogHelper.LogMessage(ex.ToString());
                throw;
            }
            finally
            {
                LogHelper.LogMessage("\n*****       END       *******\n\n");
            }


            return result;
        }

        /// <summary>
        /// Validates the K2 project.
        /// </summary>
        private void ValidateK2Project()
        {
            // Make sure that it's a K2 project
            if (!ProjectFilePath.EndsWith(".k2proj"))
                throw new ArgumentException("Project file must end with .k2proj.\n");

            // If not using the local environment cache then make sure that credentials are available
            if (!UseEnvironmentCache &&
                (string.IsNullOrEmpty(WindowsDomain) ||
                string.IsNullOrEmpty(WindowsUsername) ||
                string.IsNullOrEmpty(WindowsPassword)))
            {
                throw new ArgumentException("Valid User credentials must be entered if not using local Environment cache.\n");
            }
        }

        /// <summary>
        /// Manipulates the files.
        /// </summary>
        /// <returns>The path to the project file</returns>
        private string GetNewProjectFile()
        {
            string tempPath = string.Empty;
            // string k2DeployFolder = string.Empty;
            FileHelper fileHelper = new FileHelper();

            //Get the project folder
            string projectFolder = fileHelper.GetFolderFromPath(ProjectFilePath);
            LogHelper.LogMessage("      Project Folder: " + projectFolder);

            // Get the Project File Path
            string projectFile = fileHelper.GetFileNameFromPath(ProjectFilePath);
            LogHelper.LogMessage("      Project File: " + projectFile);

            LogHelper.LogMessage("   -- Getting path to a temporary folder for the K2 project files");
            tempPath = fileHelper.GetTempDirectory();

            TempDeployFolderPath = tempPath + @"\K2Deploy";

            LogHelper.LogMessage("      Temp Folder: " + TempDeployFolderPath);
            LogHelper.LogMessage("   -- Cleaning up files from any previous builds");
            fileHelper.DeleteDirectory(TempDeployFolderPath);

            LogHelper.LogMessage("   -- Copying files to the temp folder");
            fileHelper.CopyFolder(projectFolder, TempDeployFolderPath);
            LogHelper.LogMessage("      Files copied from '" + projectFolder + "' to '" + TempDeployFolderPath + "'");

            //Ensure we have access to all the files.
            LogHelper.LogMessage("   -- Setting ACL on folders and files in '" + TempDeployFolderPath + "'");
            bool success = fileHelper.SetAcl(TempDeployFolderPath, "F", true);
            if (!success) throw new Exception("Failed to set ACLs on folder " + TempDeployFolderPath);

            //Ensure the files are all writable
            LogHelper.LogMessage("   -- Setting writable permissions for folder: " + TempDeployFolderPath);
            fileHelper.SetWritable(TempDeployFolderPath);

            LogHelper.LogMessage("   -- Getting the Project File Path in Temp Folder");
            string newProjectFilePath = TempDeployFolderPath + @"\" + ProjectFilePath.Substring(1 + ProjectFilePath.LastIndexOf('\\'));
            LogHelper.LogMessage("      New Project File: " + newProjectFilePath);

            return newProjectFilePath;
        }


        //EDGAR 6/1/2010
        #region Edgar additions

        private bool BuildCompileAndSaveProject(string projectFilePath,string outputPath,EnvironmentSettingsManager environmentManager, int stepNumber)
        {
            Project project;
            DeploymentPackage package;
            bool result = false;

            #region 4. Build

            LogHelper.LogMessage(String.Format("\n{0}.1 Beginning Build",stepNumber));
            LogHelper.LogMessage("   -- Loading project file: " + projectFilePath);
            project = new Project();
            project.Load(projectFilePath);

            LogHelper.LogMessage("   -- Building Project");
            result = K2Helper.CompileK2Project(project);

            if (!result)
                throw new Exception("The Project did not Compile successfully.\n");

            #endregion

            #region 5. Deployment Package Creation

            LogHelper.LogMessage(String.Format("\n{0}.2 Creating the Deployment Package",stepNumber));
            package = K2Helper.CreateDeploymentPackage(
                project, environmentManager, DeployLabel, DeployDescription, testOnly);

            #endregion

            #region 6. Save Deployment Package

            LogHelper.LogMessage(String.Format("\n{0}.3. Saving Deployment Package to '" + OutputPath + "'",stepNumber));
            FileHelper fileHelper = new FileHelper();
            fileHelper.DeleteDirectory(outputPath);

            package.Save(outputPath, "K2DeploymentPackage");
            LogHelper.LogMessage("   -- Package Saved");

            #endregion

            return result;
            
        }

        /// <summary>
        /// Based on the Original K2 project file, this methods will generate a separate project file for each artifact.
        /// </summary>
        /// <returns>An Array of project files to compile</returns>
        private Dictionary<string, string> GetNewProjectFiles(ref string refTempfilePathPath)
        {
            Dictionary<string, string> dictFileNames = new Dictionary<string, string>();

            string newProjectFilePath = GetNewProjectFile();

            refTempfilePathPath = newProjectFilePath;

            dictFileNames = CreateProjectForEachProjectArtifact(TempDeployFolderPath, newProjectFilePath);

            return dictFileNames;
        }

        /// <summary>
        /// This method splits a k2 project in to multiple files.  One for each artifac.  The artifacts must be flagged as included in the original project, else they are omitted
        /// </summary>
        /// <param name="k2DeploymentFolder">The current temporary location of the files</param>
        /// <param name="k2ProjectFilePath">The current path to the original file to use</param>
        /// <returns></returns>
        private Dictionary<string, string> CreateProjectForEachProjectArtifact(string k2DeploymentFolder, string k2ProjectFilePath)
        {
            //stores a key value pair,  The key will be the name of the artifact and the value is the path to its k2project.
            Dictionary<string, string> dictFileNames = new Dictionary<string, string>();

            //stores the list of folders in the solution
            List<String> folders = new List<string>();
            //stores the list of artifacts in the solution.  The artifacts must be "included"
            List<String> k2Artifacts = new List<string>();

            //stores the list of un-necessary nodes "ItemGroup" that need to be removed.  For this solution we only need 2, one for the folders and one for the artifacts.
            List<System.Xml.XmlNode> oldNodes = new List<System.Xml.XmlNode>();

            //use xml to create each file.
            //Load the original file for manipulation
            System.Xml.XmlDocument origK2ProjXML = new System.Xml.XmlDocument();
            origK2ProjXML.Load(k2ProjectFilePath);


            //counter to track the number of ItemGroup nodes encountered
            int countItemGroups = 1;
            //loop through all of the notes in the Project document
            foreach (System.Xml.XmlNode child in origK2ProjXML.FirstChild.ChildNodes)
            {
                //especifically we are looking for the ItemGroup node.
                if (child.Name == "ItemGroup")
                {
                    if (countItemGroups > 2)
                    {
                        //we only need two
                        //add it to the list of oldNodes that will be removed.
                        oldNodes.Add(child);
                    }

                    //We need to get the list of folders and process that have been included by default
                    FillFolderAndProcessList(child, ref folders, ref k2Artifacts);
                    countItemGroups += 1;
                }
            }

            //remove any ItemGroup reference above the ones that are necessary
            foreach (System.Xml.XmlNode oldNode in oldNodes)
            {
                origK2ProjXML.FirstChild.RemoveChild(oldNode);
            }

            //For each k2Artifact included in the original project, we need to create its own project file.
            foreach (string artifact in k2Artifacts)
            {
                //this will be the new file name for this solution
                string fileName = string.Empty;
                //Get a cleansed name to be use as a subfolder later on.
                //Todo:Not sure if artifacts w/ the same name will cause problems
                string artifactName = GetProjectNameFromArtifactName(artifact);

                //Generate new ItemGroup Nodes, with the setting that we need.
                System.Xml.XmlElement foldersItemGroupNode = CreateItemGroupFolderNode(folders, origK2ProjXML);
                System.Xml.XmlElement artifactItemGroupNode = CreateItemGroupContentNode(artifact, origK2ProjXML);

                //again, we have to count the ItemGroup nodes.
                //I couldn't get this to work otherwise. Creating a new document create other sideaffects.  Mainly each node is saved with an empty xmlns (namespace), which MSBuild did not like
                int itemCount = 1;

                //for each of the 2 nodes in the project file - reset the inner xml to the values we need.
                //Todo: look in to storing these nodes in a list in previous steps, then reset them here and just do a replace child node.
                foreach (System.Xml.XmlNode child in origK2ProjXML.FirstChild.ChildNodes)
                {
                    //especifically we are looking for the ItemGroup node.
                    if (child.Name == "ItemGroup")
                    {
                        if (itemCount == 1)
                        {
                            child.InnerXml = foldersItemGroupNode.InnerXml;
                        }
                        else
                        {
                            child.InnerXml = artifactItemGroupNode.InnerXml;
                        }
                        itemCount += 1;
                    }
                }

                //create a new file name for this artifact.
                fileName = k2DeploymentFolder + @"\" + artifactName + ".k2proj";

                //Save the project
                origK2ProjXML.Save(fileName);

                //track the name of the artifact and its corresponding project file
                dictFileNames.Add(artifactName, fileName);
            }

            return dictFileNames;
        }

        /// <summary>
        /// Fills two lists, one has teh artifacts found in the project, the other one has the folders
        /// </summary>
        /// <param name="itemGroup">XMLNODE that is parsed</param>
        /// <param name="folders">List<String> of folders</param>
        /// <param name="artifacts">List<String> of artifacts</param>
        private void FillFolderAndProcessList(System.Xml.XmlNode itemGroup, ref List<String> folders, ref List<String> artifacts)
        {
            if (itemGroup != null)
            {
                foreach (System.Xml.XmlNode child in itemGroup.ChildNodes)
                {
                    //looking for Folder or Content
                    if (child.Name == "Folder")
                    {
                        folders.Add(child.Attributes["Include"].Value);
                    }
                    if (child.Name == "Content")
                    {
                        artifacts.Add(child.Attributes["Include"].Value);
                    }
                }
            }
        }

        /// <summary>
        /// Creates an xml Element to be added to a Project xml
        /// </summary>
        /// <param name="contentName">The name of the artifact for which to create a "Content" node</param>
        /// <param name="origXMLDoc">the document that the element will be added to, not by this method</param>
        /// <returns>XMLNode of name "Content"</returns>
        private System.Xml.XmlElement GetProjectContentNode(string contentName, System.Xml.XmlDocument origXMLDoc)
        {
            System.Xml.XmlElement contentNode = origXMLDoc.CreateElement("Content");
            System.Xml.XmlElement subTypeNode = origXMLDoc.CreateElement("SubType");
            System.Xml.XmlElement excludedNode = origXMLDoc.CreateElement("Excluded");
            System.Xml.XmlAttribute includeAttr = origXMLDoc.CreateAttribute("Include");

            subTypeNode.InnerText = "Content";
            includeAttr.Value = contentName;

            contentNode.Attributes.Append(includeAttr);
            contentNode.AppendChild(subTypeNode);
            contentNode.AppendChild(excludedNode);

            return contentNode;
        }

        /// <summary>
        ///  Creates an xml Element to be added to a Project xml
        /// </summary>
        /// <param name="folders">The list of folders that need to go in to this ItemGroup Node</param>
        /// <param name="origXMLDoc">The xml Document </param>
        /// <returns>XMLElement of type ItemGroup</returns>
        private System.Xml.XmlElement CreateItemGroupFolderNode(List<String> folders, System.Xml.XmlDocument origXMLDoc)
        {
            System.Xml.XmlElement itemGroupNode = origXMLDoc.CreateElement("ItemGroup");

            foreach (string folder in folders)
            {
                itemGroupNode.AppendChild(GetProjectFolderNode(folder, origXMLDoc));
            }

            return itemGroupNode;
        }

        /// <summary>
        /// Creates an xml Element to be added to a Project xml
        /// </summary>
        /// <param name="artifactToInclude">Name of the artifact to include</param>
        /// <param name="origXMLDoc"></param>
        /// <returns>XmlElement of type ItemGroup</returns>
        private System.Xml.XmlElement CreateItemGroupContentNode(string artifactToInclude, System.Xml.XmlDocument origXMLDoc)
        {
            System.Xml.XmlElement itemGroupNode = origXMLDoc.CreateElement("ItemGroup");

            itemGroupNode.AppendChild(GetProjectContentNode(artifactToInclude, origXMLDoc));

            return itemGroupNode;
        }

        /// <summary>
        /// Creates an Element of type "Folder" to be added to an ItemGroup node
        /// </summary>
        /// <param name="folderName">Name of the folder</param>
        /// <param name="origXMLDoc">The XML Document</param>
        /// <returns>XmlElement of type Folder</returns>
        private System.Xml.XmlElement GetProjectFolderNode(string folderName, System.Xml.XmlDocument origXMLDoc)
        {
            System.Xml.XmlElement folderNode = origXMLDoc.CreateElement("Folder");
            System.Xml.XmlAttribute includeAttr = origXMLDoc.CreateAttribute("Include");

            includeAttr.Value = folderName;

            folderNode.Attributes.Append(includeAttr);

            return folderNode;
        }

        /// <summary>
        /// Parses the Name of an artifact in to something that can be used as a folder name later on
        /// </summary>
        /// <param name="artifactNameAsStored">Name of the artifact as originally stored in the K2 project file</param>
        /// <returns>Cleansed name, that can be used to create folders</returns>
        private string GetProjectNameFromArtifactName(string artifactNameAsStored)
        {
            //the input would be either
            //processName.kprx or Process.Name.kprx or SODXs
            //the names can also be preceded by a list of folders.
            string newVal = string.Empty;

            if (artifactNameAsStored == string.Empty)
                return "";

            //this should hold the name of the SmO or the Process. 
            newVal = artifactNameAsStored.Substring(1 + artifactNameAsStored.LastIndexOf('\\'));

            if (newVal.ToUpper().EndsWith(".SODX") || newVal.ToUpper().EndsWith(".KPRX"))
            {
                newVal = newVal.Substring(0, newVal.Length - 5);
            }
            return newVal;
        }
        #endregion   

        #endregion Methods
    }
}
