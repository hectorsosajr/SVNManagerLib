namespace SVNManagerLib
{
    /// <summary>
    /// This represents a single user in the authz file.
    /// </summary>
    public class SVNAuthorizationMember
    {
        ///<summary>
        /// Initializes a new instance of the <see cref="SVNAuthorizationMember"/> class.
        ///</summary>
        ///<param name="memberName"></param>
        public SVNAuthorizationMember( string memberName )
        {
            MemberName = memberName;
        }

        ///<summary>
        /// The name for this group member.
        ///</summary>
        public string MemberName { set; get; }
    }
}
