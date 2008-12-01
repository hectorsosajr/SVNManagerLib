//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		RepositoryRootDirectoryDoesNotExistException.cs
// Author:			Hector Sosa, Jr
// Date:			11/30/2008
//**********************************************************

using System;

namespace SVNManagerLib
{
    /// <summary>
    /// Exception thrown when the root directory for Subversion repositories does
    /// not exists at run time. The most probable cause is that it was moved
    /// since the last time SVNManagerLib was run.
    /// </summary>
    public class RepositoryRootDirectoryDoesNotExistException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryRootDirectoryDoesNotExistException"/> class.
        /// </summary>
        /// <param name="directoryPath">The missing directory path.</param>
        public RepositoryRootDirectoryDoesNotExistException( string directoryPath )
            : base( directoryPath + " has been moved or deleted." )
        {
        }
    }
}
