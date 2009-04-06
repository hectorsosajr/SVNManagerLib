using System;
using System.Collections.Generic;
using System.Text;

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
    public class SVNAuthorization
    {
        #region Member Variables

        private readonly string _authzPath;
        private List<SVNAuthorizationAlias> _aliases;
        private List<SVNAuthorizationGroup> _groups;

        #endregion

        #region Constructors

        ///<summary>
        /// Initializes an instance of the <see cref="SVNAuthorization"/> class.
        ///</summary>
        ///<param name="authzFilePath">The path to the authz file.</param>
        public SVNAuthorization( string authzFilePath )
        {
            _authzPath = authzFilePath;
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
    }

    /// <summary>
    /// Represents an alias inside the authz file.
    /// </summary>
    public class SVNAuthorizationAlias
    {
    }

    /// <summary>
    /// Represents a group inside the authz file.
    /// </summary>
    public class SVNAuthorizationGroup
    {
    }
}
