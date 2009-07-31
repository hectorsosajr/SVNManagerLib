using System.Collections.Generic;

namespace SVNManagerLib
{
    /// <summary>
    /// Represents a group inside the authz file.
    /// </summary>
    public class SVNAuthorizationGroup
    {
        #region Properties

        /// <summary>
        /// Gets or sets the members of this group.
        /// </summary>
        /// <value>The members.</value>
        public List<SVNAuthorizationMember> Members
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name for this group.
        /// </summary>
        /// <value>The name given to this group.</value>
        public string Name
        {
            get;
            set;
        } 

        #endregion
    }
}