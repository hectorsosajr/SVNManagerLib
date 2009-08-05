namespace SVNManagerLib
{
    /// <summary>
    /// This represents a single user in the authz file.
    /// </summary>
    public class SVNAuthorizationMember
    {
        ///<summary>
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
