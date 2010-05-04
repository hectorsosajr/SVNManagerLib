using System.Collections.Generic;
using Nini.Config;
using Nini.Ini;

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
    public class SVNAuthorizationProvider
    {
        #region Member Variables

        private readonly string _authzPath;
        private List<SVNAuthorizationAlias> _aliases = new List<SVNAuthorizationAlias>();
        private List<SVNAuthorizationGroup> _groups = new List<SVNAuthorizationGroup>();

        #endregion

        #region Constructors

        ///<summary>
        /// Initializes an instance of the <see cref="SVNAuthorizationProvider"/> class.
        ///</summary>
        ///<param name="authzFilePath">The path to the authz file.</param>
        public SVNAuthorizationProvider( string authzFilePath )
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

        #region Public Members

        ///<summary>
        /// Adds a new group to the authz file.
        ///</summary>
        ///<param name="groupName"></param>
        public void AddGroup(string groupName)
        {
        }

        ///<summary>
        /// Deletes a group from the authz file.
        ///</summary>
        ///<param name="groupName">The name of the group that will be deleted.</param>
        public void DeleteGroup(string groupName)
        {
        }

        ///<summary>
        /// Adds a new alias to the authz file.
        ///</summary>
        ///<param name="alias">The name of the new alias.</param>
        ///<param name="aliasPath">The path that the new alias will be representing.</param>
        public void AddAlias(string alias, string aliasPath)
        {
        }

        ///<summary>
        /// Deletes an alias from the authz file.
        ///</summary>
        ///<param name="alias">The name of the alias that will be deleted.</param>
        public void DeleteAlias(string alias)
        {
        }

        ///<summary>
        /// Adds a new member to an existing authz group.
        ///</summary>
        ///<param name="groupName">The group name which will have the new member added.</param>
        ///<param name="newMemberName">The name of the new group member.</param>
        public void AddGroupMember(string groupName, string newMemberName)
        {
        }
 
        ///<summary>
        /// Removes a member from an existing authz group.
        ///</summary>
        ///<param name="groupName">The group name which will have the member deleted.</param>
        ///<param name="memberNameToDelete">The name of the group member that will be removed from the group.</param>
        public void RemoveGroupMember(string groupName, string memberNameToDelete)
        {
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Processes the authz file.
        /// </summary>
        private void ProcessFile()
        {
            var iniDoc = new IniDocument( _authzPath, IniFileType.SambaStyle );
            var authzConfig = new IniConfigSource( iniDoc );

            var configs = authzConfig.Configs;

            foreach ( var config in configs )
            {
                var currConfig = (IniConfig)config;

                if ( currConfig.Name == "groups" )
                {
                    ProcessGroup( currConfig );
                }
            }
        }

        private void ProcessGroup( IConfig config )
        {
            foreach (var group in config.GetKeys())
            {
                var currGroup = new SVNAuthorizationGroup( group );
                ProcessGroupMembers( config, currGroup, group );

                Groups.Add( currGroup );
            }
        }

        private static void ProcessGroupMembers( IConfig config, SVNAuthorizationGroup currGrp, string group )
        {
            string[] members = config.Get( group ).Split( ',' );

            foreach (var member in members)
            {
                var currMember = new SVNAuthorizationMember( member );
                currGrp.Members.Add( currMember );
            }
        }

        #endregion
    }
}
