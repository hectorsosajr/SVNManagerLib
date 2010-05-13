//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNUserProvider.cs
// Author:			Hector Sosa, Jr
// Date:			5/8/2005
//**********************************************************

using System;
using System.IO;
using Nini.Config;

namespace SVNManagerLib
{
	/// <summary>
    /// This class provides read/write access to individual user
    /// settings. This uses the Nini configuration library at
    /// http://nini.sourceforge.net/
	/// </summary>
	public class SVNUserProvider
	{
		#region Member Variables

		private string _Password;
		private string _UserName;
		private string _RepositoryPath;
	    private readonly string _UserDatabaseFileName;

	    #endregion

		#region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
		public SVNUserProvider()
		{}

		/// <summary>
		/// Constructor to set the username and password.
		/// </summary>
		/// <param name="UserName">The user's account name.</param>
		/// <param name="Password">The user's account password.</param>
		public SVNUserProvider( string UserName, string Password )
		{
			_UserName = UserName;
			_Password = Password;
		}

		/// <summary>
		/// Constructor to set the username, password, and repository path.
		/// </summary>
        /// <param name="UserName">The user's account name.</param>
        /// <param name="Password">The user's account password.</param>
		/// <param name="RepositoryPath">The path to the repository where this user will be created.</param>
		public SVNUserProvider( string UserName, string Password, string RepositoryPath )
		{
			_UserName = UserName;
			_Password = Password;
			_RepositoryPath = RepositoryPath;
        }

        ///<summary>
        ///</summary>
        ///<param name="UserName">The user's account name.</param>
        ///<param name="Password">The user's account password</param>
        ///<param name="UserDatabaseFileName"></param>
        public SVNUserProvider( string UserName, string Password, FileSystemInfo UserDatabaseFileName )
        {
            _UserName = UserName;
            _Password = Password;
            _UserDatabaseFileName = UserDatabaseFileName.FullName;
        }

		#endregion

		#region Properties

		/// <summary>
		/// The account's user name.
		/// </summary>
		public string UserName
		{
			get
			{
				return _UserName;
			}
			set
			{
				_UserName = value;
			}
		}

		/// <summary>
		/// The account's password.
		/// </summary>
		public string Password
		{
			get
			{
				return _Password;
			}
			set
			{
				_Password = value;
			}
		}

		/// <summary>
		/// The physical path to the repository where this account resides.
		/// </summary>
		public string RepositoryPath
		{
			get
			{
				return _RepositoryPath;
			}
			set
			{
				_RepositoryPath = value;
			}
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Saves this user's information to the user's database file.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool Save()
		{
			bool result;

            if ( !Equals( _UserDatabaseFileName, null ) )
            {
                result = InsertUpdateUser( _UserDatabaseFileName );
            }
            else
            {
                result = InsertUpdateUser();
            }

		    return result;
		}

		/// <summary>
		/// Adds a new user to the repository's user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool Add()
		{
            bool result;

            if (!Equals(_UserDatabaseFileName, null))
            {
                result = InsertUpdateUser(_UserDatabaseFileName);
            }
            else
            {
                result = InsertUpdateUser();
            }

			return result;
		}

		/// <summary>
		/// Deletes this user from the repository's user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool Delete()
		{
			bool result;

            if ( !Equals( _UserDatabaseFileName, null ) )
            {
                result = DeleteUser( _UserDatabaseFileName );
            }
            else
            {
                result = DeleteUser();
            }

		    return result;
		}

		/// <summary>
		/// Adds this user to the repository's admin user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool AddAdmin()
		{
		    bool result = InsertUpdateAdminUser();

		    return result;
		}

	    /// <summary>
		/// Deletes this user from the repository's admin user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool DeleteAdmin()
	    {
	        bool result = DeleteAdminUser();

	        return result;
	    }

	    #endregion

		#region Private Members
		
		private bool InsertUpdateUser()
		{
			var repoConfig = new SVNRepoConfig( _RepositoryPath );

		    string userFile = repoConfig.UserDatabaseFileName;

		    bool result = InsertUpdateUser( userFile );

		    return result;
		}

        private bool InsertUpdateUser( string userFile )
        {
            bool result;

            var userConfig = new IniConfigSource( userFile );

            userConfig.Configs["users"].Set( _UserName, _Password );

            try
            {
                userConfig.Save();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
		
		private bool InsertUpdateAdminUser()
		{
		    bool result;

			string userFile = Common.GetCorrectedPath( _RepositoryPath, true ) + "administrators.txt";
			var userConfig = new IniConfigSource( userFile );

			userConfig.Configs["users"].Set( _UserName, _Password );

			try
			{
				userConfig.Save();
				result = true;
			}
			catch ( Exception )
			{
			    result = false;
			}

		    return result;
		}		
		
		private bool DeleteUser()
		{
			var repoConfig = new SVNRepoConfig( _RepositoryPath );

		    string userFile = repoConfig.UserDatabaseFileName;

		    bool result = DeleteUser( userFile );

		    return result;
		}
		
        private bool DeleteUser( string userFile )
        {
            var userConfig = new IniConfigSource( userFile );

            userConfig.Configs["users"].Remove( _UserName );

            bool result;

            try
            {
                userConfig.Save();
                result = true;
            }
            catch ( Exception )
            {
                result = false;
            }

            return result;
        }
		
		private bool DeleteAdminUser()
		{
		    bool result;

		    string userFile = Common.GetCorrectedPath( _RepositoryPath, true ) + "administrators.txt";
			var userConfig = new IniConfigSource( userFile );

			userConfig.Configs["users"].Remove( _UserName );

			try
			{
				userConfig.Save();
				result = true;
			}
			catch ( Exception )
			{
			    result = false;
			}

		    return result;
		}

		#endregion
	}
}
