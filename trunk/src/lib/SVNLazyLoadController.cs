//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNLazyLoadController.cs
// Author:			Hector Sosa, Jr
// Date:			11/5/2008
//**********************************************************

using System;
using System.IO;

namespace SVNManagerLib
{
	///<summary>
	///</summary>
	public class SVNLazyLoadController
    {
        #region Member Variables

        private SVNServerConfig _serverConfiguration;
        private Repositories _repositoryCollection = new Repositories();

        #endregion

        #region Constructors

        ///<summary>
        ///</summary>
        ///<param name="config"></param>
        public SVNLazyLoadController( SVNServerConfig config )
        {
            _serverConfiguration = config;
            LoadRepositories();
        } 

        #endregion

        #region Private Members

        private void LoadRepositories()
        {
            if ( !Equals( _serverConfiguration, null ) )
            {
                string repoRoot = _serverConfiguration.RepositoryRootDirectory;

                try
                {
                    DirectoryInfo rootRepoInfo = new DirectoryInfo( repoRoot );

                    // Get an array of DirectoryInfo objects. These objects represent
                    // the roots for each repository. One root directory is a repository.
                    if (rootRepoInfo.Exists)
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
                { }
            }
        }

        private void ProcessRepository( DirectoryInfo childRepo )
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

        private void ProcessConfigFiles( DirectoryInfo currRepoDir )
        {
            FileInfo[] configFiles = currRepoDir.GetFiles();

            foreach ( FileInfo config in configFiles )
            {
                if ( config.Name == "svnserve.conf" )
                {
                    ProcessRepoConfig( config );
                }
            }
        }

        private void ProcessRepoConfig( FileInfo RepoConfig )
        {
            var currRepo = new SVNRepository();

            if ( RepoConfig.Directory != null )
            {
                currRepo.Name = RepoConfig.Directory.Parent.Name;
                currRepo.FullPath = RepoConfig.Directory.Parent.FullName;
            }

            _repositoryCollection.Add( currRepo );
        }

        #endregion
    }
}
