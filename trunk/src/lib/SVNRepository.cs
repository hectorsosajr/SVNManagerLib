//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNRepository.cs
// Author:			Hector Sosa, Jr
// Date:			5/8/2005
//**********************************************************

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace SVNManagerLib
{
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

	/// <summary>
	/// This represents a single repository in the current Subversion server.
	/// </summary>
	public class SVNRepository
	{
		#region Member Variables

		private SVNRepoConfig _repositoryConfiguration;
		private string _name = string.Empty;
		private SVNUserCollection _users = new SVNUserCollection();
		private SVNUserCollection _admins = new SVNUserCollection();
		private string _fullPath = string.Empty;
		private RepositoryTypes _createRepoType;		
		private StringBuilder _NewConfFile = new StringBuilder();
		private string _AdminUserName = string.Empty;
		private string _AdminUserPassword = string.Empty;
        private Hashtable _files = new Hashtable();
	    private string _serverCommandsPath = string.Empty;

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNRepository"/> class.
        /// </summary>
        public SVNRepository() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNRepository"/> class.
        /// </summary>
        /// <param name="RepositoryPath">The repository path.</param>
		/// <param name="ServerCommandPath">Path to where the Subversion commands are located.</param>
		public SVNRepository( string RepositoryPath, string ServerCommandPath )
		{
            _serverCommandsPath = ServerCommandPath;
            _fullPath = RepositoryPath;
            LoadConfig( RepositoryPath );
        }

		#endregion

		#region Properties

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
		public SVNUserCollection Users
		{
			get
			{
				return _users;
			}
		}

		/// <summary>
		/// A list of <see cref="SVNUser"/> that are associated with
		/// this repository and have administrative rights.
		/// </summary>
		public SVNUserCollection Administrators
		{
			get
			{
				return _admins;
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

                if ( Equals( null, _repositoryConfiguration ) )
                {
                    repoType = _createRepoType;
                }
                else
                {
                    if ( _repositoryConfiguration.RepositoryType.Length == 0 && _createRepoType != 0 )
                    {
                        repoType = _createRepoType;
                    }
                    else
                    {
                        switch ( _repositoryConfiguration.RepositoryType )
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
        /// Holds a list of files and folders for this repository
        /// </summary>
        public Hashtable Files
        {
            get
            {
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
        /// Gets or sets the name of the first repo user.
        /// </summary>
        /// <value>The name of the first repo user.</value>
	    public string FirstRepoUserName
	    {
	        get
	        {
	            return _AdminUserName;
	        }
            set
            {
                _AdminUserName = value;
            }
	    }

        /// <summary>
        /// Gets or sets the first repo user password.
        /// </summary>
        /// <value>The first repo user password.</value>
	    public string FirstRepoUserPassword
	    {
	        get
	        {
	            return _AdminUserPassword;
	        }
            set
            {
                _AdminUserPassword = value;
            }
	    }

		#endregion

		#region Public Members

		/// <summary>
		/// Creates a repository with a Berkeley database backend.
		/// </summary>
		/// <param name="repoName">The name of the desired new repository.</param>
		/// <returns>Whether or not this operation was successful.</returns>
		public bool CreateBerkeleyDbRepository( string repoName )
		{
			bool retval;

			_createRepoType = RepositoryTypes.BerkeleyDatabase;

			retval = CreateRepository( repoName );

            if ( Equals( null, _repositoryConfiguration ) )
            {
                _repositoryConfiguration.RepositoryType = "bdb";
            }

			return retval;
		}

		/// <summary>
		/// Creates a repository with the OS filesystem as the backend.
		/// </summary>
		/// <param name="repoName">The name of the desired new repository.</param>
		/// <returns>Whether or not this operation was successful.</returns>
		public bool CreateFSFSRepository( string repoName )
		{
			bool retval;

			_createRepoType = RepositoryTypes.FileSystem;

            retval = CreateRepository(repoName);

            if ( Equals( null, _repositoryConfiguration ) )
            {
                _repositoryConfiguration.RepositoryType = "fsfs";
            }

			return retval;
		}

		/// <summary>
		/// Deletes the repository and all its subfolders from the hard drive.
		/// </summary>
		/// <returns>Whether or not this operation was successful.</returns>
		public bool DeleteRepository()
		{
			bool retval;
            string formatFilePath = _fullPath + Path.DirectorySeparatorChar + "format";

            // TODO: Make sure that this file exists.

			// The "format" file was set to read only. Need to remove
			// that file attribute in order for the directory delete
			// to work.
			File.SetAttributes( formatFilePath, FileAttributes.Normal );
			
			try
			{
				Directory.Delete( _fullPath, true );
				retval = true;
			}
			catch ( Exception )
			{
			    retval = false;
			}

			return retval;
		}

		/// <summary>
		/// Creates a dumpfile portable format of this repository.
		/// </summary>
		/// <param name="args">An instance of <see cref="DumpArgs"/>.</param>
		/// <returns>Whether or not this operation was successful.</returns>
		public bool DumpRepository( DumpArgs args )
		{
			bool cmdResult;
		    string errors;
			StringBuilder arg = new StringBuilder();

			arg.Append( "dump " );
            arg.Append( _fullPath );

            // Processing struct arguments for command-line switches
            if (Equals(args, null))
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

            if ( args.UseIncremental )
            {
                arg.Append( " --incremental" );
            }

            if ( args.UseQuiet )
            {
                arg.Append( " --quiet" );
            }

            cmdResult = Common.ExecuteWritesToDiskSvnCommand( Common.GetWellFormedSVNCommand( "svnadmin" ), arg.ToString(), args.DumpFileName, out errors );

            return cmdResult;
		}

		/// <summary>
		/// Creates an exact copy of the repository to a different folder.
		/// </summary>
		/// <param name="args">An instance of <see cref="HotCopyArgs"/>.</param>
		/// <returns>Whether or not this operation was successful.</returns>
		public bool HotCopyRepository( HotCopyArgs args )
		{
			bool cmdResult;
            string lines;
		    string errors;
			StringBuilder arg = new StringBuilder();

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
                        arg.Append(" --clean-logs");
                    }
                }
            }

		    cmdResult = Common.ExecuteSvnCommand( Common.GetWellFormedSVNCommand( "svnadmin" ), arg.ToString(), out lines, out errors );

		    return cmdResult;
		}

        /// <summary>
        /// Creates a new directory using the "svn mkdir" command.
        /// </summary>
        /// <param name="directoryName">This includes the current directory plus the new name. This is a directory fragment and not a full path.</param>
        /// <param name="message">The "svn mkdir" command commits immediately and it requires a comment.</param>
        /// <returns>Returns whether or not the command was successful.</returns>
        public bool CreateDirectory( string directoryName, string message )
        {
            bool cmdResult;
            string lines;
            string errors;
            StringBuilder args = new StringBuilder();
            StringBuilder pathUrl = new StringBuilder();
            string url;
            string tmp = directoryName;

            pathUrl.Append( _repositoryConfiguration.RepositoryRootDirectory );
            if (!_repositoryConfiguration.RepositoryRootDirectory.EndsWith( Path.DirectorySeparatorChar.ToString() ))
            {
                pathUrl.Append( Path.DirectorySeparatorChar.ToString() );
            }

            if ( tmp.StartsWith( Path.DirectorySeparatorChar.ToString() ) )
            {
                int pos = tmp.IndexOf( Path.DirectorySeparatorChar.ToString() );
                tmp = tmp.Substring( pos );

            }
            pathUrl.Append( tmp );

            url = Common.PathToFileUrl( pathUrl.ToString() );

            args.Append( "mkdir -m " );
            args.Append( ((Char)34).ToString() );
            args.Append( message );
            args.Append( ((Char)34).ToString() );
            args.Append( " " );
            args.Append( url );

            cmdResult = Common.ExecuteSvnCommand( Common.GetWellFormedSVNCommand( "svn" ), args.ToString(), out lines, out errors );

            return cmdResult;
        }

        /// <summary>
        /// Deletes a directory or file using the "svn delete" command.
        /// </summary>
        /// <param name="directoryPath">The full path to a directory or file.</param>
        /// <param name="message">The "svn delete" command commits immediately and it requires a comment.</param>
        /// <returns>Returns whether or not the command was successful.</returns>
        public bool DeleteDirectory( string directoryPath, string message )
        {
            bool cmdResult;
            string lines;
            string errors;
            StringBuilder args = new StringBuilder();
            StringBuilder pathUrl = new StringBuilder();
            string url;
            string tmp = directoryPath;

            pathUrl.Append( _repositoryConfiguration.RepositoryRootDirectory );
            if ( !_repositoryConfiguration.RepositoryRootDirectory.EndsWith( Path.DirectorySeparatorChar.ToString() ) )
            {
                pathUrl.Append( Path.DirectorySeparatorChar.ToString() );
            }

            if ( tmp.StartsWith( Path.DirectorySeparatorChar.ToString() ) )
            {
                int pos = tmp.IndexOf( Path.DirectorySeparatorChar.ToString() );
                tmp = tmp.Substring( pos );

            }
            pathUrl.Append( tmp );

            url = Common.PathToFileUrl( pathUrl.ToString() );

            args.Append( "delete -m " );
            args.Append( ((Char)34).ToString() );
            args.Append( message );
            args.Append( ((Char)34).ToString() );
            args.Append( " " );
            args.Append( url );

            cmdResult = Common.ExecuteSvnCommand( Common.GetWellFormedSVNCommand("svn"), args.ToString(), out lines, out errors );

            return cmdResult;
        }

		#endregion

		#region Private Members

        private void LoadConfig( string RepositoryPath )
        {
            _repositoryConfiguration = new SVNRepoConfig( RepositoryPath );
            string rootDir = _repositoryConfiguration.RepositoryRootDirectory;

            GetUsers( RepositoryPath );

            _files = Common.GetFileList( rootDir, _serverCommandsPath );
        }

		private bool CreateRepository( string repoName )
		{
			bool retval;

		    string rootRepoDir = Directory.GetParent( _fullPath ).ToString();

            string newRepoPath = Common.GetCorrectedPath( rootRepoDir, true ) + repoName;
			string svnCommand;
			string fileOptions = " --fs-type ";
			string svnBDB = "bdb";
			string svnFSFS = "fsfs";
            string lines;
		    string errors;
			StringBuilder arg = new StringBuilder();			

            svnCommand = Path.Combine( _serverCommandsPath, "svnadmin" );

			// Start setting up the svn command
			arg.Append( "create " );
			arg.Append( newRepoPath + " " );
			arg.Append( fileOptions );

			// decide what type of repository backend is wanted
			switch( _createRepoType )
			{
				case RepositoryTypes.BerkeleyDatabase:

					// Berkely DB switch
					arg.Append( svnBDB );
					break;

				case RepositoryTypes.FileSystem:

					// FSFS (File System) switch
					arg.Append( svnFSFS );
					break;
			}

			retval = Common.ExecuteSvnCommand( svnCommand, arg.ToString(), out lines, out errors );

			ProcessNewConfFile( newRepoPath );

            LoadConfig( newRepoPath );

			return retval;
		}

		private void GetUsers( string RepositoryPath  )
		{
		    string userDbFullPath;
		    FileInfo repo = new FileInfo( RepositoryPath );
			string root;

			root = repo.DirectoryName;
			Common.GetCorrectedPath( root, false );

			userDbFullPath = _repositoryConfiguration.UserDatabaseFileName;

			if( userDbFullPath != "" )
			{
			    StreamReader userRead;
			    userRead = File.OpenText( userDbFullPath );
	
				while (userRead.Peek() != -1)
				{
				    string currLine;
				    currLine = userRead.ReadLine();
	
					switch(currLine.ToUpper().Trim())
					{
						case "[USERS]":
							break;
						case "":
							break;
						default:
							SVNUser newUser;
							newUser = GetUserData( currLine );
							_users.Add( newUser );
							break;
					}
				}
	
				userRead.Close();				
			}
		}

		private SVNUser GetUserData( string CurrentLine )
		{
			SVNUser retval = new SVNUser();
			string[] data;

			data = CurrentLine.Split( '=' );

			retval.UserName = data[0].Trim();
			retval.Password = data[1].Trim();
			retval.ParentRepositoryPath = _repositoryConfiguration.RepositoryRootDirectory;
			retval.UserDatabasePath = _repositoryConfiguration.UserDatabaseFileName; 

			return retval;
		}

		private void ProcessNewConfFile( string newRepoPath )
		{
			StreamReader reader;
			StreamWriter writer;
			string lineString = string.Empty;
			string confPath;

            confPath = Common.GetCorrectedPath( newRepoPath, true ) + "conf" + Path.DirectorySeparatorChar;

			reader = new StreamReader( confPath + "svnserve.conf" );

			try
			{				
				while ( lineString != null )
				{
					lineString = reader.ReadLine();
					ProcessLine( lineString );
				}
			}
			finally
			{
				reader.Close();
			}

			writer = new StreamWriter( confPath + "svnserve.conf" );
			writer.Write( _NewConfFile.ToString() );
			writer.Close();

			CreateUserFile( confPath );
		}

		private void ProcessLine( string line )
		{
			switch( line )
			{
				case "# [general]":

					_NewConfFile.Append( "[general]" + Environment.NewLine );
					break;

				case "# anon-access = read":

					_NewConfFile.Append( Environment.NewLine + "anon-access = none" + Environment.NewLine );
					break;

				case "# auth-access = write":

					_NewConfFile.Append( "auth-access = write" + Environment.NewLine + Environment.NewLine );
					break;

				case "# password-db = passwd":

					_NewConfFile.Append( Environment.NewLine + "password-db = password.txt" + Environment.NewLine + Environment.NewLine );
					break;

				default:

					_NewConfFile.Append( line + Environment.NewLine );
					break;
			}
		}

		private void CreateUserFile( string confPath )
		{
			StreamWriter writer;
			StringBuilder userfile = new StringBuilder();

			writer = new StreamWriter( confPath + "password.txt" );

			userfile.Append( "[users]" + Environment.NewLine );
			userfile.Append( _AdminUserName + " = " + _AdminUserPassword );

			writer.Write( userfile.ToString() );
			writer.Close();
		}

	    #endregion
	}
}