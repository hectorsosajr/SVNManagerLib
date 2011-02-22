//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SvnServeRepository.cs
// Author:			Hector Sosa, Jr
// Date:			11/29/2008
//**********************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SVNManagerLib
{
    /// <summary>
    /// Represents a single repository managed by svnserve.exe
    /// </summary>
	public class SvnServeRepository : SubversionRepositoryBase
	{
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnServeRepository"/> class.
        /// </summary>
        public SvnServeRepository()
        {
            Groups = new List<SVNAuthorizationGroup>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnServeRepository"/> class.
        /// </summary>
        /// <param name="RepositoryPath">The repository path.</param>
        /// <param name="ServerCommandPath">Path to where the Subversion commands are located.</param>
        public SvnServeRepository( string RepositoryPath, string ServerCommandPath )
        {
            _serverCommandsPath = ServerCommandPath;
            _fullPath = RepositoryPath;
            _usersLoaded = false;
            LoadConfig( RepositoryPath );

            Groups = new List<SVNAuthorizationGroup>();

            if ( _repositoryConfiguration.AuthorizationRulesFile != null )
            {
                if ( _repositoryConfiguration.AuthorizationRulesFile != string.Empty )
                {
                    LoadAuthorizationDatabase();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnServeRepository"/> class.
        /// </summary>
        public SvnServeRepository( SVNRepoConfig RepositoryConfiguration, string ServerCommandPath )
        {
            _serverCommandsPath = ServerCommandPath;
            _repositoryConfiguration = RepositoryConfiguration;
            _usersLoaded = false;

            Groups = new List<SVNAuthorizationGroup>();

            if ( _repositoryConfiguration.AuthorizationRulesFile != null )
            {
                if ( _repositoryConfiguration.AuthorizationRulesFile != string.Empty )
                {
                    LoadAuthorizationDatabase();
                }
            }
        } 

        #endregion

        #region Properties

        ///<summary>
        ///</summary>
        public List<SVNAuthorizationGroup> Groups { get; set; }

        /// <summary>
        /// A list of <see cref="SVNUser"/> that are associated with
        /// this repository.
        /// </summary>
        /// <value></value>
        public override SVNUserCollection Users
        {
            get
            {
                if ( !_usersLoaded )
                {
                    if ( !Equals( _repositoryConfiguration, null ) )
                    {
                        GetUsers( _repositoryConfiguration.RepositoryRootDirectory );
                        _usersLoaded = true;
                    }
                }

                return _users;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is using global config file.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is using global config file; otherwise, <c>false</c>.
        /// </value>
        public bool IsUsingGlobalConfigFile { get; set; }

        #endregion

        #region Private Members

        private void GetUsers( string repositoryPath )
        {
            var repo = new FileInfo( repositoryPath );

            if ( _repositoryConfiguration.UserDatabaseFileName.Length > 0 )
            {
                string root = repo.DirectoryName;

                Common.GetCorrectedPath( root, false );

                string userDbFullPath = _repositoryConfiguration.UserDatabaseFileName;

                if ( userDbFullPath != "" )
                {
                    _users = new SVNUserCollection();
                    StreamReader userRead = File.OpenText(userDbFullPath);

                    while ( userRead.Peek() != -1 )
                    {
                        string currLine = userRead.ReadLine();

                        switch ( currLine.ToUpper().Trim() )
                        {
                            case "[USERS]":
                            case "":
                                break;
                            default:
                                string start = currLine.Substring( 0, 1 );

                                // Ignore comment lines
                                switch ( start )
                                {
                                    case "#":
                                    case ";":
                                        break;
                                    default:
                                        SVNUser newUser;
                                        newUser = GetUserData( currLine );
                                        _users.Add( newUser );
                                        break;
                                }
                                break;
                        }
                    }

                    userRead.Close();
                }
            }
        }

        private SVNUser GetUserData( string CurrentLine )
        {
            var retval = new SVNUser();

            string[] data = CurrentLine.Split( '=' );

            retval.UserName = data[0].Trim();
            retval.Password = data[1].Trim();
            retval.ParentRepositoryPath = _repositoryConfiguration.RepositoryRootDirectory;
            retval.UserDatabasePath = _repositoryConfiguration.UserDatabaseFileName;

            return retval;
        }

        protected override bool CreateRepository( string repoName )
        {
            string rootRepoDir = "";

            try
            {
                rootRepoDir = Directory.GetParent(_fullPath).ToString();
            }
            catch
            {
            }

            string newRepoPath = "\"" + Path.Combine(rootRepoDir, repoName) + "\"";
            string newRepoPathUnescaped = Path.Combine(rootRepoDir, repoName);
            const string fileOptions = " --fs-type ";
            const string svnBDB = "bdb";
            const string svnFSFS = "fsfs";
            string lines;
            string errors;
            var arg = new StringBuilder();

            string svnCommand = Path.Combine( _serverCommandsPath, "svnadmin" );

            // Start setting up the svn command
            arg.Append( "create " );
            arg.Append( newRepoPath + " " );
            arg.Append( fileOptions );

            // decide what type of repository backend is wanted
            switch ( _createRepoType )
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

            bool retval = Common.ExecuteSvnCommand( svnCommand, arg.ToString(), out lines, out errors );

            if ( retval )
            {
                ProcessNewConfFile( newRepoPathUnescaped );

                LoadConfig( newRepoPathUnescaped );
            }

            return retval;
        }

        // TODO : Fix this so that it works with global config files.
        private void ProcessNewConfFile( string newRepoPath )
        {
            string lineString = string.Empty;
            string rootConfPath = Path.Combine( newRepoPath, "conf" );
            string confPath = Path.Combine( rootConfPath, "svnserve.conf" );
            var reader = new StreamReader( confPath );

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

            var writer = new StreamWriter( confPath );
            writer.Write( _NewConfFile.ToString() );
            writer.Close();

            CreateUserFile( rootConfPath );
        }

        // TODO : Fix this so that it works with global config files.
        private void ProcessLine( string line )
        {
            switch ( line )
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

                    _NewConfFile.Append( Environment.NewLine + "password-db = passwd" + Environment.NewLine + Environment.NewLine );
                    break;

                default:

                    _NewConfFile.Append( line + Environment.NewLine );
                    break;
            }
        }

        // TODO : Fix this so that it works with global config files.
        private void CreateUserFile( string confPath )
        {
            var userfile = new StringBuilder();
            string userFileTemp;
            
            if ( Equals( _repositoryConfiguration, null ) )
            {
                userFileTemp = "passwd";
            }
            else
            {
                userFileTemp = _repositoryConfiguration.UserDatabaseFileName;
            }

            var pathToUserFile = Path.Combine( confPath, userFileTemp );
            var writer = new StreamWriter( pathToUserFile );

            userfile.Append( "[users]" + Environment.NewLine );
            userfile.Append( FirstRepoUserName + " = " + FirstRepoUserPassword );

            writer.Write( userfile.ToString() );
            writer.Close();
        }

        private void LoadAuthorizationDatabase()
        {
            var authProvider = new SVNAuthorizationProvider( _repositoryConfiguration.AuthorizationRulesFile );
            Groups = authProvider.Groups;
        }

        #endregion
	}
}
