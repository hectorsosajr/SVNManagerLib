namespace SVNManagerLib
{
    /// <summary>
    /// Represents an alias inside the authz file.
    /// </summary>
    public class SVNAuthorizationAlias
    {
        ///<summary>
        /// Stores the name for the alias that was created for this instance.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// Stores that repository path that is represented by this alias.
        ///</summary>
        public string AliasPath { get; set; }
    }
}