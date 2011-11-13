//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		ISubversionRepository.cs
// Author:			Hector Sosa, Jr
// Date:			11/29/2008
//**********************************************************

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SVNManagerLib
{
    /// <summary>
    /// Represents the most basic representation of a Subversion repository,
    /// which could be used for both svnserve and Apache servers.
    /// </summary>
    public interface ISubversionRepository : IComponent
    {
        /// <summary>
        /// Creates a repository with a Berkeley database backend.
        /// </summary>
        /// <param name="repositoryName">The name of the desired new repository.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        bool CreateBerkeleyDbRepository( string repositoryName );

        /// <summary>
        /// Creates a new directory using the "svn mkdir" command.
        /// </summary>
        /// <param name="directoryName">This includes the current directory plus the new name. This is a directory fragment and not a full path.</param>
        /// <param name="message">The "svn mkdir" command commits immediately and it requires a comment.</param>
        /// <returns>Returns whether or not the command was successful.</returns>
        /// <param name="errorMessages">This will contain the error text from the Subversion command, if any.</param>
        bool CreateDirectory( string directoryName, string message, out string errorMessages );

        /// <summary>
        /// Creates a repository with the OS filesystem as the backend.
        /// </summary>
        /// <param name="repositoryName">The name of the desired new repository.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        bool CreateFSFSRepository( string repositoryName );

        /// <summary>
        /// Deletes a directory or file using the "svn delete" command.
        /// </summary>
        /// <param name="directoryPath">The full path to a directory or file.</param>
        /// <param name="message">The "svn delete" command commits immediately and it requires a comment.</param>
        /// <returns>Returns whether or not the command was successful.</returns>
        /// <param name="errorMessages">This will contain the error text from the Subversion command, if any.</param>
        bool DeleteDirectory( string directoryPath, string message, out string errorMessages );

        /// <summary>
        /// Deletes the repository and all its subfolders from the hard drive.
        /// </summary>
        /// <returns>Whether or not this operation was successful.</returns>
        bool DeleteRepository( out string errors );

        /// <summary>
        /// Creates a dumpfile portable format of this repository.
        /// </summary>
        /// <param name="args">An instance of <see cref="DumpArgs"/>.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        bool DumpRepository( DumpArgs args );

        /// <summary>
        /// Creates an exact copy of the repository to a different folder.
        /// </summary>
        /// <param name="args">An instance of <see cref="HotCopyArgs"/>.</param>
        /// <returns>Whether or not this operation was successful.</returns>
        bool HotCopyRepository( HotCopyArgs args );

        /// <summary>
        /// Loads a Subversion dump file.
        /// </summary>
        /// <param name="args">This is the <see cref="LoadDumpFileArgs"/> struct with valid information for this action.</param>
        /// <param name="errorMessages">The error messages that come from the "svnadmin load" command.</param>
        /// <returns></returns>
        bool LoadDumpFile( LoadDumpFileArgs args, out string errorMessages );

        /// <summary>
        /// Executes the svn command using the specified arguments.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns></returns>
        string[] Svn( params string[] args );

        /// <summary>
        /// Executes the SvnLook command using the specified sub-command and 
        /// revision number and returns a string array consisting of each line 
        /// of the output.
        /// </summary>
        /// <param name="subcommand">Sub command for svnlook</param>
        /// <param name="revision">The revision to grab information about.</param>
        /// <param name="args">Arguments except for the revision and repository.</param>
        /// <returns></returns>
        string[] SvnLook( string subcommand, int revision, params string[] args );

        /// <summary>
        /// Executes the SvnLook command using the specified sub-command for the 
        /// current revision and returns a string array consisting of each line 
        /// of the output.
        /// </summary>
        /// <param name="subcommand">Sub command for svnlook</param>
        /// <param name="args">Arguments except for the revision and repository.</param>
        /// <returns></returns>
        string[] SvnLook( string subcommand, params string[] args );

        /// <summary>
        /// Holds what <see cref="RepositoryAuthorization">rights</see> do the anonymous users have
        /// for this repository.
        /// </summary>
        RepositoryAuthorization AnonymousAccess { get; set; }

        /// <summary>
        /// Holds what <see cref="RepositoryAuthorization">rights</see> do the authenticated users have 
        /// for this repository.
        /// </summary>
        RepositoryAuthorization AuthorizedAccess { get; set; }

        /// <summary>
        /// Holds a list of files and folders for this repository,
        /// as a list of text items.
        /// </summary>
        Hashtable Files { get; }

        /// <summary>
        /// Gets or sets the name of the first user in the repository.
        /// </summary>
        /// <value>First user name used during repository creation.</value>
        string FirstRepoUserName { get; set; }

        /// <summary>
        /// Gets or sets the name of the first user password in the repository.
        /// </summary>
        /// <value>First user password used during repository creation.</value>
        string FirstRepoUserPassword { get; set; }

        /// <summary>
        /// The full path to this repository.
        /// </summary>
        string FullPath { get; set; }

        /// <summary>
        /// Gets the hook files for this repository.
        /// </summary>
        /// <value>The hook files.</value>
        List<SVNHook> HookScriptFiles { get; }

        /// <summary>
        /// The name of this repository. This is usually the root folder
        /// name of the repository
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Holds the name or ID of the repository's realm. When no realm is present,
        /// it defaults to the UUID.
        /// </summary>
        string Realm { get; }

        /// <summary>
        /// The <see cref="SVNRepoConfig"/> managing this repository's
        /// configuration settings.
        /// </summary>
        SVNRepoConfig RepositoryConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the repository file entities.
        /// </summary>
        /// <value>The repository file entities.</value>
        List<SVNFileSystemEntity> RepositoryFileEntities { get; set; }
        /// <summary>
        /// The path to the Subversion repository.
        /// </summary>
        string RepositoryPath { get; }

        /// <summary>
        /// Holds the back end that this repository is using.
        /// </summary>
        RepositoryTypes RepositoryType { get; }

        /// <summary>
        /// Holds the Universal Unique IDentifier (UUID) for this repository.
        /// </summary>
        string RepositoryUUID { get; }

        /// <summary>
        /// Gets or sets the sever commands path.
        /// </summary>
        /// <value>The sever commands path.</value>
        string SeverCommandsPath { get; set; }

        /// <summary>
        /// A list of <see cref="SVNUser"/> that are associated with
        /// this repository.
        /// </summary>
        SVNUserCollection Users { get; }
    }
}
