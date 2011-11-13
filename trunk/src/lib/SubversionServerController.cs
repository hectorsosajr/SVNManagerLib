//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SubversionServerController.cs
// Author:			Hector Sosa, Jr
// Date:			11/30/2008
//**********************************************************

using System;
using System.IO;
using System.Collections.Generic;

namespace SVNManagerLib
{
    /// <summary>
    /// This class controls actions in SVNManagerLib. This is the top
    /// level class/object in this namespace.
    /// </summary>
    public class SubversionServerController
    {
        #region Member Variables

        private SVNServerConfig _serverConfiguration;
        private List<ISubversionRepository> _repositoryCollection = new List<ISubversionRepository>();
        private SVNUserCollection _adminUsers = new SVNUserCollection();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubversionServerController"/> class.
        /// </summary>
        /// <param name="serverRootPath">The server root path.</param>
        /// <param name="severCommandPath">The sever command path.</param>
        /// <param name="serverRepositoriesPath">The server repositories path.</param>
        public SubversionServerController ( string serverRootPath, string severCommandPath, string serverRepositoriesPath )
        {
            _serverConfiguration = new SVNServerConfig( serverRootPath );
            _serverConfiguration.CommandRootDirectory = severCommandPath;
            _serverConfiguration.RepositoryRootDirectory = serverRepositoriesPath;

            LoadRepositories();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubversionServerController"/> class
        /// using a <see cref="SVNServerConfig"/> object.
        /// </summary>
        /// <param name="config">The configuration object for the server.</param>
        public SubversionServerController ( SVNServerConfig config )
        {
            _serverConfiguration = config;
            LoadRepositories();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubversionServerController"/> class.
        /// </summary>
        /// <param name="RepositoryPaths">A list of repository paths.</param>
        /// <param name="CommandPath">The command path to the Subversion command-line utilities.</param>
        public SubversionServerController ( IEnumerable<string> RepositoryPaths, string CommandPath )
        {
            _serverConfiguration = new SVNServerConfig();
            _serverConfiguration.CommandRootDirectory = CommandPath;
            LoadRepositories( RepositoryPaths );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds a <see cref="SVNServerConfig"/> object for this server.
        /// </summary>
        public SVNServerConfig ServerConfiguration
        {
            get
            {
                return _serverConfiguration;
            }
            set
            {
                _serverConfiguration = value;
            }
        }

        /// <summary>
        /// Holds a collection of SVNRepsitory objects, one
        /// for each repository in this server.
        /// </summary>
        public List<ISubversionRepository> RepositoryCollection
        {
            get
            {
                return _repositoryCollection;
            }
            set
            {
                _repositoryCollection = value;
            }
        }
        
        /// <summary>
        /// Holds a list of <see cref="SVNUser"/> objects that have administrative rights.
        /// </summary>
        /// <remarks>This is not part of the default Subversion functionality. This is a function
        /// of this library. This will be used to synchronize users between user silos in
        /// external systems such as Active Directory, LADP, etc.</remarks>
        public SVNUserCollection AdminUsers
        {
            get
            {
                return _adminUsers;
            }
        }
        
        #endregion

        #region Public Members

        #endregion

        #region Private Members

        private void LoadRepositories()
        {
            if ( !Equals( _serverConfiguration, null ) )
            {
                string repoRoot = _serverConfiguration.RepositoryRootDirectory;

                try
                {
                    var rootRepoInfo = new DirectoryInfo( repoRoot );

                    // Get an array of DirectoryInfo objects. These objects represent
                    // the roots for each repository. One root directory is a repository.
                    if ( rootRepoInfo.Exists )
                    {
                        DirectoryInfo[] childrenRepo = rootRepoInfo.GetDirectories();

                        foreach ( DirectoryInfo childRepo in childrenRepo )
                        {
                            ProcessRepository( childRepo );
                        }
                    }
                    else
                    {
                        throw new RepositoryRootDirectoryDoesNotExistException( rootRepoInfo.Name );
                    }
                }
                catch ( ArgumentException )
                {}
            }
        }

        private void LoadRepositories( IEnumerable<string> RepositoryPaths )
        {
            try
            {
                foreach( string repoPath in RepositoryPaths )
                {
                    var currRepo = new DirectoryInfo( repoPath );
                    ProcessRepository( currRepo );
                }
            }
            catch ( ArgumentException )
            {}
        }

        private void ProcessRepository( DirectoryInfo childRepo )
        {
            if ( _serverConfiguration.UsesGlobalConfigFile )
            {
                ProcessRepoConfig( _serverConfiguration.GlobalConfigFilePath, childRepo );
            }
            else
            {
                DirectoryInfo[] repoDirs = childRepo.GetDirectories();

                foreach ( DirectoryInfo repoDir in repoDirs )
                {
                    if ( repoDir.Name == "conf" )
                    {
                        ProcessConfigFiles( repoDir );
                    }
                }
            }
        }

        private void ProcessConfigFiles( DirectoryInfo currRepoDir )
        {
            FileInfo[] configFiles = currRepoDir.GetFiles();

            foreach ( FileInfo config in configFiles )
            {
                if( config.Name == "svnserve.conf" )
                {
                    ProcessRepoConfig( config );
                }
            }
        }

        private void ProcessRepoConfig( FileInfo RepoConfig )
        {
            var currRepo = new SvnServeRepository( RepoConfig.FullName, _serverConfiguration.CommandRootDirectory )
                               {
                                   Name = RepoConfig.Directory.Parent.Name,
                                   FullPath = RepoConfig.Directory.Parent.FullName
                               };

            _repositoryCollection.Add( currRepo );
        }

        private void ProcessRepoConfig( string pathToGlobalConfig, FileSystemInfo RepoDir )
        {
            var globalConfigDir = new FileInfo( pathToGlobalConfig );
            var globalRepoConfig = new SVNRepoConfig( globalConfigDir, RepoDir.FullName );
            var currRepo = new SvnServeRepository( globalRepoConfig, _serverConfiguration.CommandRootDirectory )
                               {
                                   Name = RepoDir.Name,
                                   FullPath = RepoDir.FullName,
                                   IsUsingGlobalConfigFile = true
                               };

            _repositoryCollection.Add( currRepo );
        }

        #endregion
    }
}
