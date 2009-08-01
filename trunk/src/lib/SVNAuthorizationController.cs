using System.Collections.Generic;
using System;
using System.IO;
using Nini.Config;

namespace SVNManagerLib
{
    ///<summary>
    /// Authorization states for the authz file.
    ///</summary>
    public enum PathAuthorizations
    {
        /// <summary>
        /// This user has no rights.
        /// </summary>
        None,

        /// <summary>
        /// This user has read rights.
        /// </summary>
        Read,

        /// <summary>
        /// This user has write rights.
        /// </summary>
        Write
    }

    /// <summary>
    /// Represents the contents of the authz file.
    /// </summary>
    public class SVNAuthorizationController
    {
        #region Member Variables

        private readonly string _authzPath;
        private List<SVNAuthorizationAlias> _aliases = new List<SVNAuthorizationAlias>();
        private List<SVNAuthorizationGroup> _groups = new List<SVNAuthorizationGroup>();

        #endregion

        #region Constructors

        ///<summary>
        /// Initializes an instance of the <see cref="SVNAuthorizationController"/> class.
        ///</summary>
        ///<param name="authzFilePath">The path to the authz file.</param>
        public SVNAuthorizationController( string authzFilePath )
        {
            _authzPath = authzFilePath;

            ProcessFile();
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the authorization file path.
        /// </summary>
        /// <value>The authorization file path.</value>
        public string AuthorizationFilePath
        {
            get
            {
                return _authzPath;
            }
        }

        /// <summary>
        /// Gets the aliases contained in the authz file.
        /// </summary>
        /// <value>The aliases.</value>
        public List<SVNAuthorizationAlias> Aliases
        {
            get
            {
                return _aliases;
            }
        }

        /// <summary>
        /// Gets the groups contained in the authz file.
        /// </summary>
        /// <value>The groups.</value>
        public List<SVNAuthorizationGroup> Groups
        {
            get
            {
                return _groups;
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Processes the authz file.
        /// </summary>
        private void ProcessFile()
        {
            var authzConfig = new IniConfigSource( _authzPath );

            var configs = authzConfig.Configs;
        }

        #endregion
    }
}
