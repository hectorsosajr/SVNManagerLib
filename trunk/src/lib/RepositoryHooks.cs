//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		RepositoryHooks.cs
// Author:			Hector Sosa, Jr
// Date:			8/23/2007
//**********************************************************

using System.Collections.Generic;
using System.IO;

namespace SVNManagerLib
{
    ///<summary>
    /// This represents a list of hook scripts in the current Subversion repository.
	///</summary>
	public class RepositoryHooks
	{
        #region Member Variables

        private List<SVNHook> _hooks = new List<SVNHook>();
        private string _repoPath = string.Empty;
        private string _hookPath = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryHooks"/> class.
        /// </summary>
        /// <param name="repositoryPath">The repository path.</param>
        public RepositoryHooks( string repositoryPath )
        {
            _repoPath = repositoryPath;

            LoadHooks();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the hook files.
        /// </summary>
        /// <value>The hook files.</value>
        public List<SVNHook> HookFiles
        {
            get
            {
                return _hooks;
            }
        }

        #endregion

        #region Private Members

        private void LoadHooks()
        {
            _hookPath = Path.Combine( _repoPath, "hooks" );
            DirectoryInfo hookDir = new DirectoryInfo( _hookPath );
            FileInfo[] hooks = hookDir.GetFiles();

            foreach ( FileInfo fi in hooks )
            {
                if ( !Equals( fi, null ) )
                {
                    SVNHook newHook = new SVNHook( fi.FullName );

                    _hooks.Add( newHook );
                }
            }
        } 

        #endregion
    }
}
