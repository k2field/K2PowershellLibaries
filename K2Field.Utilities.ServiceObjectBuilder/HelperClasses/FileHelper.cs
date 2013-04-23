using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.IO;

namespace K2Field.Utilities.ServiceObjectBuilder.HelperClasses
{
    public class FileHelper
    {
        public FileHelper()
        { }

        /// <summary>
        /// Recursively copy the source folder to the destination folder
        /// Created destination folder if necessary
        /// From: http://www.csharp411.com/c-copy-folder-recursively/
        /// </summary>
        /// <param name="sourceFolder">The folder to copy from</param>
        /// <param name="destFolder">The folder to copy to</param>
        public void CopyFolder(string sourceFolder, string destFolder)
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
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public bool SetAcl(string folderName, string userRights, bool inheritSubDirectories)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentException("A valid Folder name must be specified to set ACL.");
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

        /// <summary>
        /// Sets the files specifid folder and all folders and files within it to writable.
        /// </summary>
        /// <param name="folder">The folder to set rights on.</param>
        public void SetWritable(string folder)
        {
            foreach (string f in Directory.GetFiles(folder)) File.SetAttributes(f, FileAttributes.Normal);
            foreach (string d in Directory.GetDirectories(folder)) SetWritable(d);
        }

        /// <summary>
        /// Deletes a directory.
        /// </summary>
        /// <param name="folder">The folder to delete.</param>   
        public void DeleteDirectory(string folder)
        {
            if (Directory.Exists(folder))
            {
                foreach (string f in Directory.GetFiles(folder)) File.Delete(f);
                foreach (string d in Directory.GetDirectories(folder)) DeleteDirectory(d);
                Directory.Delete(folder, true);
            }
        }

        /// <summary>
        /// Gets the folder from path.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <returns>The path to the folder</returns>
        public string GetFolderFromPath(string FilePath)
        {
            return FilePath.Substring(0, FilePath.LastIndexOf('\\'));
        }

        /// <summary>
        /// Gets the file name from path.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <returns>The file name.</returns>
        public string GetFileNameFromPath(string FilePath)
        {
            return FilePath.Substring(1 + FilePath.LastIndexOf('\\'));
        }

        /// <summary>
        /// Gets a temporary directory.
        /// </summary>
        /// <returns>Path to the temp directory</returns>
        public string GetTempDirectory()
        {
            return Path.GetTempPath().Trim('\\').Trim('/');
        }

        /// <summary>
        /// Replace file with no back up
        /// </summary>
        /// <param name="oldFileNameToReplace"></param>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        public bool ReplaceFile(string oldFileNameToReplace, string newFileName)
        {
            //no back up
            System.IO.File.Replace(newFileName, oldFileNameToReplace, null);

            return true;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourFileName"></param>
        /// <param name="destinationFileName"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public bool CopyFile(string sourFileName, string destinationFileName, bool overwrite)
        {
            System.IO.File.Copy(sourFileName, destinationFileName, overwrite);
            return true;
        }

    }
}
