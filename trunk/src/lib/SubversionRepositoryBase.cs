//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SubversionRepositoryBase.cs
// Author:			Hector Sosa, Jr
// Date:			11/29/2008
//**********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Text;

namespace SVNManagerLib
{
    /// <summary>
    /// Represents a basic Subversion repository.
    /// </summary>
    public class SubversionRepositoryBase : Component, ISubversionRepository
    {
        #region IComponent Members

        public override ISite Site
        {
            set
            {
                base.Site = value;

                var container = (IServiceContainer)GetService(typeof(IServiceContainer));
                container.AddService(typeof(ISubversionRepository), this);
            }
        }

        #endregion

        #region Member Variables

        private string _name = string.Empty;
        private Hashtable _files = new Hashtable();
        private bool _filesLoaded;
        private List<SVNFileSystemEntity> _entities = new List<SVNFileSystemEntity>();
        private RepositoryHooks _repoHooks;

        protected internal bool _usersLoaded;
        protected internal SVNRepoConfig _repositoryConfiguration;
        protected internal SVNUserCollection _users = new SVNUserCollection();
        protected internal readonly StringBuilder _NewConfFile = new StringBuilder();
        protected internal string _firstUserName = string.Empty;
        protected internal string _firstUserPassword = string.Empty;
        protected internal string _serverCommandsPath = string.Empty;
        protected internal string _fullPath = string.Empty;
        protected internal RepositoryTypes _createRepoType;
        protected internal string _realm = string.Empty;

        #endregion

        #region Subversion Command Structs

        /// <summary>
        /// Arguments for the "svnadmin create" command.
        /// </summary>
        public struct CreateRepositoryArgs
        {
            /// <summary>
            /// The type of back end this repository will be using. This
            /// uses one of the types in <see cref="RepositoryTypes"/>.
            /// </summary>
            public RepositoryTypes RepositoryType;
            /// <summary>
            /// The name of the new repository. This will also be the folder
            /// name under the repository root folder.
            /// </summary>
            public string RepositoryName;
        }

        /// <summary>
        /// Arguments for the "svnadmin dump" command.
        /// </summary>
        public struct DumpArgs
        {
            /// <summary>
            /// The revision number to dump. This can also be
            /// a range in the X:Y format.
            /// </summary>
            public string RevisionArg;
            /// <summary>
            /// The name that will be given to this dump file.
            /// </summary>
            public string DumpFileName;
            /// <summary>
            /// Whether or not to use an incremental dump.
            /// </summary>
            public bool UseIncremental;
            /// <summary>
            /// Whether or not to show progress. Also known as verbose.
            /// </summary>
            public bool UseQuiet;
            /// <summary>
            /// The name of the new repository. This will also be the folder
            /// name under the repository root folder.
            /// </summary>
            public string RepositoryName;
        }

        /// <summary>
        /// Arguments for the "svnadmin hotcopy" command.
        /// </summary>
        public struct HotCopyArgs
        {
            /// <summary>
            /// The destination folder for the hot copy.
            /// </summary>
            public string DestinationPath;
            /// <summary>
            /// Whether or not to remove the redundant log files
            /// from the source repository. This only works for
            /// BerkeleyDB repositories.
            /// </summary>
            public bool UseCleanLogs;
        }

        ///<summary>
        /// Arguments for the "svnadmin load" command.
        ///</summary>
        public struct LoadDumpFileArgs
        {
            /// <summary>
            /// The path to the dump file.
            /// </summary>
            public string DumpFilePath;
            /// <summary>
            /// The destination folder where the dump file will be loaded into.
            /// </summary>
            public string DestinationPath;
            /// <summary>
            /// This is used when the user wants to load the dump file
            /// into another path other than the reporitory's root path.
            /// </summary>
            public string ParentPath;
        }

        #endregion

        #region Properties

        public string RepositoryPath { get; set; }

        /// <summary>
        /// Holds the name or ID of the repository's realm. When no realm is present,
        /// it defaults to the UUID.
        /// </summary>
        public string Realm
        {
            get { return _repositoryConfiguration.RepositoryRealm; }
        }

        /// <summary>
        /// Gets the hook files for this repository.
        /// </summary>
        /// <value>The hook files.</value>
        public List<SVNHook> HookScriptFiles
        {
            get
            {
                if ( Equals( _repoHooks, null ) )
                {
                    if ( !Equals( _repositoryConfiguration, null ) )
                    {
                        _repoHooks = new RepositoryHooks( _repositoryConfiguration.RepositoryRootDirectory );
                    }
                }

                if ( Equals( _repoHooks, null ) )
                {
                    return null;
                }

                return _repoHooks.HookFiles;
            }
        }

        /// <summary>
        /// Holds what <see cref="RepositoryAuthorization">rights</see> do the anonymous users have
        /// for this repository.
        /// </summary>
        public RepositoryAuthorization AnonymousAccess
        {
            get
            {
                return _repositoryConfiguration.AnonymousAccess;
            }
            set
            {
                _repositoryConfiguration.AnonymousAccess = value;
            }
        }

        /// <summary>
        /// Holds what <see cref="RepositoryAuthorization">rights</see> do the authenticated users have 
        /// for this repository.
        /// </summary>
        public RepositoryAuthorization AuthorizedAccess
        {
            get
            {
                return _repositoryConfiguration.AuthorizedAccess;
            }
            set
            {
                _repositoryConfiguration.AuthorizedAccess = value;
            }
        }

        /// <summary>
        /// The full path to this repository.
        /// </summary>
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                _fullPath = value;
            }
        }

        /// <summary>
        /// The name of this repository. This is usually the root folder
        /// name of the repository
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// The <see cref="SVNRepoConfig"/> managing this repository's
        /// configuration settings.
        /// </summary>
        public SVNRepoConfig RepositoryConfiguration
        {
            get
            {
                return _repositoryConfiguration;
            }
            set
            {
                _repositoryConfiguration = value;
            }
        }

        /// <summary>
        /// A list of <see cref="SVNUser"/> that are associated with
        /// this repository.
        /// </summary>
        public virtual SVNUserCollection Users
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the repository file entities.
        /// </summary>
        /// <value>The repository file entities.</value>
        public List<SVNFileSystemEntity> RepositoryFileEntities
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
            }
        }

        /// <summary>
        /// Holds the back end that this repository is using.
        /// </summary>
        public RepositoryTypes RepositoryType
        {
            get
            {
                RepositoryTypes repoType;

                if (Equals(null, _repositoryConfiguration))
                {
                    repoType = _createRepoType;
                }
                else
                {
                    if (_repositoryConfiguration.RepositoryType.Length == 0 && _createRepoType != 0)
                    {
                        repoType = _createRepoType;
                    }
                    else
                    {
                        switch (_repositoryConfiguration.RepositoryType)
                        {
                            case "fsfs":
                                repoType = RepositoryTypes.FileSystem;
                                break;
                            case "bdb":
                                repoType = RepositoryTypes.BerkeleyDatabase;
                                break;
                            default:
                                repoType = RepositoryTypes.FileSystem;
                                break;
                        }
                    }
                }

                return repoType;
            }
        }

        /// <summary>
        /// Holds the Universal Unique IDentifier (UUID) for this repository.
        /// </summary>
        public string RepositoryUUID
        {
            get
            {
                return _repositoryConfiguration.RepositoryUUID;
            }
        }

        /// <summary>
        /// Gets the schema version associated with the repository in question.
        /// </summary>
        /// <value>The repository schema version.</value>
        public string RepositorySchemaVersion
        {
            get
            {
                return _repositoryConfiguration.RepositorySchemaVersion;
            }
        }

        /// <summary>
        /// Holds a list of files and folders for this repository,
        /// as a list of text items.
        /// </summary>
        public Hashtable Files
        {
            get
            {
                if ( !_filesLoaded )
                {
                    LoadFiles();
                    _filesLoaded = true;
                }

                return _files;
            }
        }

        /// <summary>
        /// Gets the sever commands path.
        /// </summary>
        /// <value>The sever commands path.</value>
        public string SeverCommandsPath
        {
            get
            {
                return _serverCommandsPath;
            }
            set
            {
                _serverCommandsPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the first user in the repository.
        /// </summary>
        /// <value>First user name used during repository creation.</value>
        public string FirstRepoUserName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the name of the first user password in the repository.
        /// </summary>
        /// <value>First user password used during repository creation.</value>
        public string FirstRepoUserPassword
        {
            get; set;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Creates a repository with a Berkeley database backend.
        /// </summary>
        /// <param name="repositoryName">The name of the desired new repository.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        public bool CreateBerkeleyDbRepository( string repositoryName )
        {
            _createRepoType = RepositoryTypes.BerkeleyDatabase;

            bool retval = CreateRepository( repositoryName );

            if ( Equals( null, _repositoryConfiguration ) )
            {
                if ( _repositoryConfiguration != null ) _repositoryConfiguration.RepositoryType = "bdb";
            }

            return retval;
        }

        /// <summary>
        /// Creates a repository with the OS filesystem as the backend.
        /// </summary>
        /// <param name="repositoryName">The name of the desired new repository.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        public bool CreateFSFSRepository( string repositoryName )
        {
            _createRepoType = RepositoryTypes.FileSystem;

            bool retval = CreateRepository( repositoryName );

            if ( Equals( null, _repositoryConfiguration ) )
            {
                if ( _repositoryConfiguration != null ) _repositoryConfiguration.RepositoryType = "fsfs";
            }

            return retval;
        }

        /// <summary>
        /// Deletes the repository and all its subfolders from the hard drive.
        /// </summary>
        /// <returns>Whether or not this operation was successful.</returns>
        public bool DeleteRepository( out string errors )
        {
            bool retval;
            string msg = "";
            string formatFilePath = Path.Combine( _fullPath, "format" );
            string formatFilePathdb = Path.Combine( Path.Combine( _fullPath, "db" ), "format");

            // The "format" file was set to read only. Need to remove
            // that file attribute in order for the directory delete
            // to work.
            if ( File.Exists( formatFilePath ) )
            {
                File.SetAttributes( formatFilePath, FileAttributes.Normal );
            }

            if ( File.Exists( formatFilePathdb ) )
            {
                File.SetAttributes( formatFilePathdb, FileAttributes.Normal );
            }

            try
            {
                string hookDir = Path.Combine( _fullPath, "hooks" );
                Directory.Delete( hookDir, true );
                Directory.Delete( _fullPath, true );
                retval = true;
            }
            catch ( Exception ex )
            {
                retval = false;
                msg = ex.Message;
                System.Diagnostics.Debugger.Log(1,"IO", msg);
            }

            errors = msg;

            return retval;
        }

        /// <summary>
        /// Creates a dumpfile portable format of this repository.
        /// </summary>
        /// <param name="args">An instance of <see cref="SVNManagerLib.DumpArgs"/>.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        public bool DumpRepository( SVNManagerLib.DumpArgs args )
        {
            string errors;
            var arg = new StringBuilder();

            arg.Append( "dump " );
            arg.Append( _fullPath );

            // Processing struct arguments for command-line switches
            if ( Equals( args, null ) )
            {
                return false;
            }

            if ( !Equals( args.DumpFileName, null ) && args.DumpFileName.Length > 0 )
            {
                arg.Append( " > " + args.DumpFileName );
            }

            if ( !Equals( args.RevisionArg, null ) && args.RevisionArg.Length > 0 )
            {
                arg.Append( " --revision " + args.RevisionArg );
            }

            // TODO : Incremental dumps need to have a root dump file. Fix this!
            if ( args.UseIncremental )
            {
                arg.Append( " --incremental" );
            }

            if ( args.UseQuiet )
            {
                arg.Append( " --quiet" );
            }

            string svnCommand = Path.Combine( _serverCommandsPath, "svnadmin" );

            bool cmdResult = Common.ExecuteWritesToDiskSvnCommand( svnCommand, arg.ToString(), args.DumpFileName, out errors );

            return cmdResult;
        }

        /// <summary>
        /// Loads a Subversion dump file.
        /// </summary>
        /// <param name="args">This is the <see cref="SVNManagerLib.LoadDumpFileArgs"/> struct with valid information for this action.</param>
        /// <param name="errorMessages">The error messages that come from the "svnadmin load" command.</param>
        /// <returns></returns>
        public bool LoadDumpFile( SVNManagerLib.LoadDumpFileArgs args, out string errorMessages )
        {
            var cmdArgs = new StringBuilder();
            string errors;
            string lines;

            string svnCommand = Path.Combine( _serverCommandsPath, "svnadmin" );

            cmdArgs.Append( " load " );

            if ( !Equals( null, args.ParentPath ) )
            {
                if ( args.ParentPath.Length > 0 )
                {
                    cmdArgs.Append( "--parent-dir " + args.ParentPath + " " );
                }
            }

            cmdArgs.Append( args.DestinationPath );

            OperatingSystem myOS = Environment.OSVersion;

            if (!Equals(myOS.Platform, 128))
            {
                // This is a Windows box. Make sure to surround the path
                // to svnadmin with qoutes. This is to deal with the issue
                // of Windows allowing spaces in directory/folder names.
                svnCommand = @"""" + svnCommand + @"""";
            }

            bool cmdResult = Common.ExecuteSvnCommandWithFileInput( svnCommand, cmdArgs.ToString(), args.DumpFilePath, out lines, out errors );

            errorMessages = errors;

            return cmdResult;
        }

        /// <summary>
        /// Creates an exact copy of the repository to a different folder.
        /// </summary>
        /// <param name="args">An instance of <see cref="SVNManagerLib.HotCopyArgs"/>.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        public bool HotCopyRepository( SVNManagerLib.HotCopyArgs args )
        {
            string lines;
            string errors;
            var arg = new StringBuilder();

            arg.Append( "hotcopy " );
            arg.Append( _fullPath );

            // Processing struct arguments for command-line switches
            if ( Equals( args, null ) )
            {
                return false;
            }

            if ( Equals( args.DestinationPath, null ) )
            {
                return false;
            }

            if ( args.DestinationPath == string.Empty )
            {
                return false;
            }

            arg.Append( " " );
            arg.Append( args.DestinationPath );

            if ( !Equals( args.UseCleanLogs, null ) )
            {
                if ( args.UseCleanLogs )
                {
                    // the clean-logs switch is only useful for Berkeley repositories.
                    if ( RepositoryType == RepositoryTypes.BerkeleyDatabase )
                    {
                        arg.Append( " --clean-logs" );
                    }
                }
            }

            string svnCommand = Path.Combine( _serverCommandsPath, "svnadmin" );

            bool cmdResult = Common.ExecuteSvnCommand( svnCommand, arg.ToString(), out lines, out errors );

            return cmdResult;
        }

        /// <summary>
        /// Creates a new directory using the "svn mkdir" command.
        /// </summary>
        /// <param name="directoryName">This includes the current directory plus the new name. This is a directory fragment and not a full path.</param>
        /// <param name="message">The "svn mkdir" command commits immediately and it requires a comment.</param>
        /// <returns>Returns whether or not the command was successful.</returns>
        /// <param name="errorMessages">This will contain the error text from the Subversion command, if any.</param>
        public bool CreateDirectory( string directoryName, string message, out string errorMessages )
        {
            string lines;
            string errors;
            var args = new StringBuilder();
            var pathUrl = new StringBuilder();
            string tmp = directoryName;

            pathUrl.Append( _repositoryConfiguration.RepositoryRootDirectory );

            if ( !_repositoryConfiguration.RepositoryRootDirectory.EndsWith( Path.DirectorySeparatorChar.ToString() ) )
            {
                pathUrl.Append( Path.DirectorySeparatorChar.ToString() );
            }

            if (tmp.StartsWith( Path.DirectorySeparatorChar.ToString() ) )
            {
                int pos = tmp.IndexOf( Path.DirectorySeparatorChar.ToString() );
                tmp = tmp.Substring( pos + 1 );

            }
            pathUrl.Append( tmp );

            string url = Common.PathToFileUrl( pathUrl.ToString() );

            args.Append( "mkdir -m " );
            args.Append( ( ( Char ) 34 ).ToString() );
            args.Append( message );
            args.Append( ( ( Char )34 ).ToString() );
            args.Append( " " );
            args.Append( url );

            string svnCommand = Path.Combine( _serverCommandsPath, "svn" );
            
            bool cmdResult = Common.ExecuteSvnCommand( svnCommand, args.ToString(), out lines, out errors );

            errorMessages = errors;

            return cmdResult;
        }

        /// <summary>
        /// Deletes a directory or file using the "svn delete" command.
        /// </summary>
        /// <param name="directoryPath">The full path to a directory or file.</param>
        /// <param name="message">The "svn delete" command commits immediately and it requires a comment.</param>
        /// <returns>Returns whether or not the command was successful.</returns>
        /// <param name="errorMessages">This will contain the error text from the Subversion command, if any.</param>
        public bool DeleteDirectory( string directoryPath, string message, out string errorMessages )
        {
            string lines;
            string errors;
            var args = new StringBuilder();
            var pathUrl = new StringBuilder();
            string tmp = directoryPath;

            pathUrl.Append( _repositoryConfiguration.RepositoryRootDirectory );

            if ( !_repositoryConfiguration.RepositoryRootDirectory.EndsWith( Path.DirectorySeparatorChar.ToString() ) )
            {
                pathUrl.Append( Path.DirectorySeparatorChar.ToString() );
            }

            if ( tmp.StartsWith( Path.DirectorySeparatorChar.ToString() ) )
            {
                int pos = tmp.IndexOf( Path.DirectorySeparatorChar.ToString() );
                tmp = tmp.Substring( pos + 1 );
            }

            pathUrl.Append( tmp );

            string url = Common.PathToFileUrl( pathUrl.ToString() );

            args.Append( "delete -m " );
            args.Append( ( ( Char )34 ).ToString() );
            args.Append( message );
            args.Append( ( ( Char )34 ).ToString() );
            args.Append( " " );
            args.Append( url );

            string svnCommand = Path.Combine( _serverCommandsPath, "svn" );

            bool cmdResult = Common.ExecuteSvnCommand( svnCommand, args.ToString(), out lines, out errors );

            errorMessages = errors;

            return cmdResult;
        }

        /// <summary>
        /// Executes the SvnLook command using the specified sub-command and 
        /// revision number and returns a string array consisting of each line 
        /// of the output.
        /// </summary>
        /// <param name="subcommand">Sub command for svnlook</param>
        /// <param name="revision">The revision to grab information about.</param>
        /// <param name="args">Arguments except for the revision and repository.</param>
        /// <returns></returns>
        public string[] SvnLook( string subcommand, int revision, params string[] args )
        {
            string lines;
            string errors;
            string svnCommand = Path.Combine( _serverCommandsPath, "svnlook" );
            Common.ExecuteSvnCommand( svnCommand, String.Join ( " ", args ), out lines, out errors );

            return Common.ParseOutputIntoLines( lines );
        }

        /// <summary>
        /// Executes the SvnLook command using the specified sub-command for the 
        /// current revision and returns a string array consisting of each line 
        /// of the output.
        /// </summary>
        /// <param name="subcommand">Sub command for svnlook</param>
        /// <param name="args">Arguments except for the revision and repository.</param>
        /// <returns></returns>
        public string[] SvnLook( string subcommand, params string[] args )
        {
            string lines;
            string errors;
            string svnCommand = Path.Combine( _serverCommandsPath, "svnlook" );
            Common.ExecuteSvnCommand( svnCommand, String.Join( " ", args ), out lines, out errors );

            return Common.ParseOutputIntoLines( lines );
        }

        /// <summary>
        /// Executes the svn command using the specified arguments.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns></returns>
        public string[] Svn( params string[] args )
        {
            string lines;
            string errors;
            string svnCommand = Path.Combine( _serverCommandsPath, "svnlook" );
            Common.ExecuteSvnCommand( svnCommand, String.Join( " ", args ), out lines, out errors );

            return Common.ParseOutputIntoLines( lines );
        }

        #endregion

        #region Private Members

        private void LoadFiles()
        {
            if ( !Equals( _repositoryConfiguration, null ) )
            {
                string rootDir = _repositoryConfiguration.RepositoryRootDirectory;

                _files = Common.GetFileList( rootDir, _serverCommandsPath );
            }
        }

        protected void LoadConfig( string repositoryPath )
        {
            _repositoryConfiguration = new SVNRepoConfig( repositoryPath );
        }

        private void LoadFileEntities()
        {
            foreach ( object pkey in _files.Keys )
            {
                string fileName = pkey.ToString();
                string filePath = _files[pkey].ToString();

                var entity = new SVNFileSystemEntity( _serverCommandsPath, filePath, fileName );

                _entities.Add( entity );
            }
        }

        protected virtual bool CreateRepository( string repoName )
        {
            return false;
        }

        #endregion
    }
}
