//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNUserProvider.cs
// Author:			Hector Sosa, Jr
// Date:			5/8/2005
//**********************************************************

using System;
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

			result = InsertUpdateUser();

			return result;
		}

		/// <summary>
		/// Adds a new user to the repository's user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool Add()
		{
			bool result;

			result = InsertUpdateUser();

			return result;
		}

		/// <summary>
		/// Deletes this user from the repository's user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool Delete()
		{
			bool result;

			result = DeleteUser();

			return result;
		}

		/// <summary>
		/// Adds this user to the repository's admin user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool AddAdmin()
		{
			bool result;

			result = InsertUpdateAdminUser();

			return result;
		}

		/// <summary>
		/// Deletes this user from the repository's admin user database.
		/// </summary>
        /// <returns>Returns whether or not the command was successful.</returns>
		public bool DeleteAdmin()
		{
			bool result;
			
			result = DeleteAdminUser();

			return result;
		}

		#endregion

		#region Private Members
		
		private bool InsertUpdateUser()
		{
			SVNRepoConfig repoConfig = new SVNRepoConfig( _RepositoryPath );
			string userFile;
			IniConfigSource userConfig;
			bool result;

			userFile = repoConfig.UserDatabaseFileName;
			userConfig = new IniConfigSource( userFile );

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
		
		private bool InsertUpdateAdminUser()
		{
			string userFile;
			IniConfigSource userConfig;
			bool result;

			userFile = Common.GetCorrectedPath( _RepositoryPath, true ) + "administrators.txt";
			userConfig = new IniConfigSource( userFile );

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
			SVNRepoConfig repoConfig = new SVNRepoConfig( _RepositoryPath );
			string userFile;
			IniConfigSource userConfig;
			bool result;

			userFile = repoConfig.UserDatabaseFileName;
			userConfig = new IniConfigSource( userFile );

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
		
		private bool DeleteAdminUser()
		{
			string userFile;
			IniConfigSource userConfig;
			bool result;

		    userFile = Common.GetCorrectedPath( _RepositoryPath, true ) + "administrators.txt";
			userConfig = new IniConfigSource( userFile );

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
