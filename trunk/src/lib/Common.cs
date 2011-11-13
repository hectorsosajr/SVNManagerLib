//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		Common.cs
// Author:			Hector Sosa, Jr
// Date:			1/7/2006
//**********************************************************

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace SVNManagerLib
{
    /// <summary>
    /// A list of possible authorization modes for a
    /// Subversion repository.
    /// </summary>
    public enum RepositoryAuthorization
    {
        /// <summary>
        /// This repository will allow both read and write access.
        /// </summary>
        Write	= 0,
        /// <summary>
        /// This repository will allow read-only access.
        /// </summary>
        Read	= 1,
        /// <summary>
        /// This is a private repository.
        /// </summary>
        None	= 2
    }

    /// <summary>
    /// A list of currently supported Subversion repository backends.
    /// </summary>
    public enum RepositoryTypes
    {
        /// <summary>
        /// This repository uses the Berekely database.
        /// </summary>
        BerkeleyDatabase = 1,
        /// <summary>
        /// This repository uses the file system.
        /// </summary>
        FileSystem = 2
    }

    ///<summary>
    /// A list to tell the SVNFileSystemEntity what
    /// type of item it is dealing with.
    ///</summary>
    public enum FileSystemEntityType
    {
        ///<summary>
        /// This represents a folder/directory in the
        /// specified Subversion repository.
        ///</summary>
        Folder,
        ///<summary>
        /// This represents a file in the
        /// specified Subversion repository.
        ///</summary>
        File
    }

    /// <summary>
    /// Functions that are common across several classes.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Returns a fully qualified path for the current operating system.
        /// This also fixes the path separators according to the current
        /// Operating System.
        /// </summary>
        /// <param name="FullPath">The path to verify.</param>
        /// <param name="EndSeparator">A switch to tell the function whether it should check for a separator at the end of the path.</param>
        public static string GetCorrectedPath( string FullPath, bool EndSeparator )
        {
            string retval;

            OperatingSystem myOS = Environment.OSVersion;
            
            //Code Checks to see which OS is being used 128 indicates Linux
            if( (int)myOS.Platform == 128 )
            {
                retval = FullPath.Replace( "\\", "/" );

                if ( EndSeparator ) 
                {
                    if ( !retval.EndsWith("/") )
                    {
                        retval += "/";
                    }
                }
            }
            else
            {
                retval = FullPath.Replace( "/", "\\" );

                if ( EndSeparator )
                {
                    if ( !retval.EndsWith("\\")  )
                    {
                        retval += "\\";
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Executes a well formed Subversion command against a command-line
        /// program.
        /// </summary>
        /// <param name="command">Well formed Subversion command.</param>
        /// <param name="arguments">Arguments that will be used for the Subversion command.</param>
        /// <param name="result">This is the output for the command after execution.</param>
        /// <param name="errors">This is the output for any errors during execution.</param>
        /// <returns>bool - Whether or not the command was successfully executed.</returns>
        public static bool ExecuteSvnCommand( string command, string arguments, out string result, out string errors )
        {
            bool retval = false;
            string output = string.Empty;
            string errorLines = string.Empty;
            Process svnCommand = null;
            var psi = new ProcessStartInfo( command );

            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try
            {
                Process.Start( psi );
                psi.Arguments = arguments;
                svnCommand = Process.Start( psi );

                StreamReader myOutput = svnCommand.StandardOutput;
                StreamReader myErrors = svnCommand.StandardError;
                svnCommand.WaitForExit();

                if ( svnCommand.HasExited )
                {
                    output = myOutput.ReadToEnd();
                    errorLines = myErrors.ReadToEnd();
                }

                // Check for errors
                if ( errorLines.Trim().Length == 0 )
                {
                    retval = true;
                }
            }
            catch ( Exception ex )
            {
                string msg = ex.Message;
                errorLines += Environment.NewLine + msg;
            }
            finally
            {
                if (svnCommand != null)
                {
                    svnCommand.Close();
                }
            }

            result = output;
            errors = errorLines;

            return retval;
        }

        /// <summary>
        /// Executes a well formed Subversion command that writes output to disk 
        /// against a command-line program.
        /// </summary>
        /// <param name="command">Well formed Subversion command.</param>
        /// <param name="arguments">Arguments that will be used for the Subversion command.</param>
        /// <param name="destinationFile">This is the file that this command will write output to.</param>
        /// <param name="errors">This is the output for any errors during execution.</param>
        /// <returns>bool - Whether or not the command was successfully executed.</returns>
        public static bool ExecuteWritesToDiskSvnCommand( string command, string arguments, string destinationFile, out string errors )
        {
            bool retval = false;
            string errorLines = string.Empty;
            Process svnCommand = null;
            var psi = new ProcessStartInfo( command );

            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try
            {
                Process.Start( psi );
                psi.Arguments = arguments;
                svnCommand = Process.Start( psi );

                StreamReader myOutput = svnCommand.StandardOutput;
                StreamReader myErrors = svnCommand.StandardError;
                
                File.AppendAllText( destinationFile, myOutput.ReadToEnd() );
                svnCommand.WaitForExit();
                File.AppendAllText( destinationFile, myOutput.ReadToEnd() );

                if ( svnCommand.HasExited )
                {
                    errorLines = myErrors.ReadToEnd();
                }

                // Check for errors
                if ( errorLines.Trim().Length == 0 )
                {
                    retval = true;
                }
            }
            catch ( Exception ex )
            {
                string msg = ex.Message;
                errorLines += Environment.NewLine + msg;
            }
            finally
            {
                if ( svnCommand != null )
                {
                    svnCommand.Close();
                }
            }

            errors = errorLines;

            return retval;
        }

        /// <summary>
        /// Executes a well formed Subversion command against a command-line
        /// where input, either as a file or text, is requested.
        /// </summary>
        /// <param name="command">Well formed Subversion command.</param>
        /// <param name="arguments">Arguments that will be used for the Subversion command.</param>
        /// <param name="filePath">The file path that will be processed by this Subversion command.</param>
        /// <param name="result">This is the output for the command after execution.</param>
        /// <param name="errors">This is the output for any errors during execution.</param>
        /// <returns>bool - Whether or not the command was successfully executed.</returns>
        public static bool ExecuteSvnCommandWithFileInput( string command, string arguments, string filePath, out string result, out string errors )
        {
            bool retval = false;
            string output = string.Empty;
            string errorLines = string.Empty;
            Process svnCommand = null;
            var psi = new ProcessStartInfo( command );

            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            try
            {
                Process.Start( psi );
                psi.Arguments = arguments;
                svnCommand = Process.Start( psi );

                var file = new FileInfo(filePath);
                StreamReader reader = file.OpenText();
                string fileContents = reader.ReadToEnd();
                reader.Close();

                StreamWriter myWriter = svnCommand.StandardInput;
                StreamReader myOutput = svnCommand.StandardOutput;
                StreamReader myErrors = svnCommand.StandardError;

                myWriter.AutoFlush = true;
                myWriter.Write(fileContents);
                myWriter.Close();

                output = myOutput.ReadToEnd();
                errorLines = myErrors.ReadToEnd();

                // Check for errors
                if ( errorLines.Trim().Length == 0 )
                {
                    retval = true;
                }
            }
            catch ( Exception ex )
            {
                string msg = ex.Message;
                errorLines += Environment.NewLine + msg;
            }
            finally
            {
                if (svnCommand != null)
                {
                    svnCommand.Close();
                }
            }

            result = output;
            errors = errorLines;

            return retval;
        }

        /// <summary>
        /// Returns a list of files for the directory that was passed in as a parameter.
        /// </summary>
        /// <param name="currentDirectory">The directory path to act upon.</param>
        /// <returns>Returns a hashtable with a list of files and directories under the directory that 
        /// was used for the parameter. The keys are the file/directory name without a path, the value
        /// is the full path to this directory or file.</returns>
        /// <param name="serverCmdPath">Path to where the Subversion command folder resides.</param>
        public static Hashtable GetFileList( string currentDirectory, string serverCmdPath )
        {
            bool cmdResult;
            string lines;
            string errors;
            var fileList = new Hashtable();
            string parsedDir;
            string fullCmdPath = Path.Combine( serverCmdPath, "svn" );

            parsedDir = PathToFileUrl( currentDirectory );

            cmdResult = ExecuteSvnCommand( fullCmdPath, "list " + parsedDir, out lines, out errors );

            if ( cmdResult )
            {
                string[] files = ParseOutputIntoLines( lines );

                string fullPath = currentDirectory;

                if ( !fullPath.EndsWith( Path.DirectorySeparatorChar.ToString() ) )
                {
                    fullPath += Path.DirectorySeparatorChar.ToString();
                }

                if ( files != null )
                {
                    foreach ( string file in files )
                    {
                        fileList.Add( file, fullPath + file );
                    }
                }
            }

            return fileList;
        }

        /// <summary>
        /// Parses the output from a Subversion command.
        /// </summary>
        /// <param name="consoleOutput">The raw output from a Subversion command.</param>
        /// <returns>A string array containing the parsed Subversion command output.</returns>
        public static string[] ParseOutputIntoLines( string consoleOutput )
        {
            string[] retval = null;

            if ( consoleOutput.Trim().Length > 0 )
            {
                string output;
                output = consoleOutput.Replace( Environment.NewLine, "\n" );

                if ( output.EndsWith( "\n" ) )
                {
                    output = output.Substring( 0, output.Length - 1 );
                }

                retval = output.Split( '\n' );
            }

            return retval;
        }

        /// <summary>
        /// Converts a path to a "file:///" format.
        /// </summary>
        /// <param name="pathToConvert">That full path that will be converted to a file url.</param>
        /// <returns></returns>
        public static string PathToFileUrl( string pathToConvert )
        {
            string fileURL = "";
            bool isUNC;
            UriBuilder fileURI = null;

            // This is to capture whether the incoming path is
            // an UNC path or not.
            if ( pathToConvert.StartsWith( @"\\" ) )
            {
                isUNC = true;
            }
            else
            {
                isUNC = false;
            }

            try
            {
                fileURI = new UriBuilder( pathToConvert );
            }
            catch( UriFormatException )
            {}

            if ( isUNC )
            {
                if ( fileURI != null ) fileURL = fileURI.ToString();
            }
            else
            {
                if ( fileURI != null ) fileURL = fileURI.ToString().Replace( "file://", "file:///" );
            }

            return fileURL;
        }

        /// <summary>
        /// Scans a directory and subdirectories and removes any read-only attributes that it
        /// finds in any file under the root directory.
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <returns></returns>
        public static bool SafeDelete( FileSystemInfo rootDirectory )
        {
            bool retval;

            try
            {
                rootDirectory.Attributes = FileAttributes.Normal;
                var di = rootDirectory as DirectoryInfo;

                if (di != null)
                {
                    foreach (var dirInfo in di.GetFileSystemInfos())
                        SafeDelete( dirInfo );
                }

                rootDirectory.Delete();

                retval = true;
            }
            catch ( System.Exception ex )
            {
                Console.WriteLine( ex.Message );
                retval = false;
            }

            return retval;
        }
    }
}