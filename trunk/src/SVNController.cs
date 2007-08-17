//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNController.cs
// Author:			Hector Sosa, Jr
// Date:			5/7/2005
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
	public class SVNController
	{
		#region Member Variables

		private SVNServerConfig _serverConfiguration;
		private Repositories _repositoryCollection = new Repositories();
        private SVNUserCollection _adminUsers = new SVNUserCollection();

		#endregion

		#region Constructors

		/// <summary>
		/// Default empty constructor
		/// </summary>
		public SVNController()
		{
			_serverConfiguration = new SVNServerConfig();
			LoadRepositories();
		}

        /// <summary>
        /// Load settings from the specified path.
        /// </summary>
        public SVNController( string serverRootPath )
        {
            _serverConfiguration = new SVNServerConfig( serverRootPath );
            LoadRepositories();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNController"/> class
        /// using a <see cref="SVNServerConfig"/> object.
        /// </summary>
        /// <param name="config">The configuration object for the server.</param>
        public SVNController ( SVNServerConfig config )
        {
            _serverConfiguration = config;
            LoadRepositories();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNController"/> class.
        /// </summary>
        /// <param name="RepositoryPaths">A list of repository paths.</param>
        /// <param name="CommandPath">The command path to the Subversion command-line utilities.</param>
        public SVNController( List<string> RepositoryPaths, string CommandPath )
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
		public Repositories RepositoryCollection
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
                string repoRoot;
                repoRoot = _serverConfiguration.RepositoryRootDirectory;

                try
                {
                    DirectoryInfo rootRepoInfo;
                    rootRepoInfo = new DirectoryInfo( repoRoot );

                    // Get an array of DirectoryInfo objects. These objects represent
                    // the roots for each repository. One root directory is a repository.
                    DirectoryInfo[] childrenRepo;
                    childrenRepo = rootRepoInfo.GetDirectories();

                    foreach ( DirectoryInfo childRepo in childrenRepo )
                    {
                        ProcessRepository( childRepo );
                    }
                }
                catch ( ArgumentException )
                {}
            }
        }

        private void LoadRepositories( List<string> RepositoryPaths )
        {
            try
            {
                foreach( string repoPath in RepositoryPaths )
                {
                    DirectoryInfo currRepo = new DirectoryInfo( repoPath );
                    ProcessRepository( currRepo );
                }
            }
            catch ( ArgumentException )
            {}
        }

		private void ProcessRepository( DirectoryInfo childRepo )
		{
			DirectoryInfo[] repoDirs;

			repoDirs = childRepo.GetDirectories();

			foreach ( DirectoryInfo repoDir in repoDirs )
			{
				if ( repoDir.Name == "conf" )
				{
					ProcessConfigFiles( repoDir );
				}
			}
		}

		private void ProcessConfigFiles( DirectoryInfo currRepoDir )
		{
			FileInfo[] configFiles;

			configFiles = currRepoDir.GetFiles();

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
            SVNRepository currRepo = new SVNRepository(RepoConfig.FullName, _serverConfiguration.CommandRootDirectory);
			currRepo.Name = RepoConfig.Directory.Parent.Name;
			currRepo.FullPath = RepoConfig.Directory.Parent.FullName;

			_repositoryCollection.Add( currRepo );
		}

	    #endregion
	}
}
