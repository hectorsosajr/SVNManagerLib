//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNUser.cs
// Author:			Hector Sosa, Jr
// Date:			5/7/2005
//**********************************************************

using System;
using System.IO;

namespace SVNManagerLib
{
	/// <summary>
	/// This represents a user in a Subversion repository.
	/// </summary>
	public class SVNUser
	{
		#region Member Variables

		private bool _IsDeleted;
		private bool _IsDirty;
		private bool _IsNew;
		private string _Password				= string.Empty;
		private string _UserName				= string.Empty;
		private string _ParentRepositoryPath	= string.Empty;
        private bool _IsAdmin;
        private FileInfo _UserDatabaseFileName;

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNUser"/> class.
        /// </summary>
		public SVNUser()
		{
			_IsNew = true;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNUser"/> class.
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <param name="Password">The password.</param>
        /// <param name="ParentRepositoryPath">The parent repository path.</param>
		public SVNUser( string UserName, string Password, string ParentRepositoryPath )
		{
			_UserName = UserName;
			_Password = Password;
			_ParentRepositoryPath = ParentRepositoryPath;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNUser"/> class.
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <param name="Password">The password.</param>
        /// <param name="ParentRepositoryPath">The parent repository path.</param>
        /// <param name="IsAdmin">if set to <c>true</c> [is admin].</param>
		public SVNUser( string UserName, string Password, string ParentRepositoryPath, bool IsAdmin )
		{
			_UserName = UserName;
			_Password = Password;
			_ParentRepositoryPath = ParentRepositoryPath;
			_IsAdmin = IsAdmin;
		}

        ///<summary>
        ///</summary>
        ///<param name="UserName"></param>
        ///<param name="Password"></param>
        ///<param name="UserDatabaseFileName"></param>
        public SVNUser( string UserName, string Password, FileInfo UserDatabaseFileName )
        {
            _UserName = UserName;
            _Password = Password;
            _UserDatabaseFileName = UserDatabaseFileName;
        }

		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
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
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
		public string Password
		{
			get
			{
				return _Password;
			}
			set
			{
				_Password = value;
				_IsDirty = true;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this user is an admin.
        /// </summary>
        /// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get
			{
				return _IsAdmin;
			}
			set
			{
				_IsAdmin = value;
				_IsDirty = true;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
		public bool IsNew
		{
			get
			{
				return _IsNew;
			}
			set
			{
				_IsNew = value;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
		public bool IsDirty
		{
			get
			{
				return _IsDirty;
			}
			set
			{
				_IsDirty = value;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
		public bool IsDeleted
		{
			get
			{
				return _IsDeleted;
			}
			set
			{
				_IsDeleted = value;
				_IsDirty = true;
			}
		}

        /// <summary>
        /// Gets or sets the parent repository path.
        /// </summary>
        /// <value>The parent repository path.</value>
		public string ParentRepositoryPath
		{
			get
			{
				return _ParentRepositoryPath;
			}
			set
			{
				_ParentRepositoryPath = value;
				_IsDirty = true;
			}
		}

        /// <summary>
        /// Gets or sets the user database path.
        /// </summary>
        /// <value>The user database path.</value>
		public string UserDatabasePath
		{
			get
			{
                return _UserDatabaseFileName.FullName;
			}
			set
			{
                _UserDatabaseFileName  = new FileInfo( value );
				_IsDirty = true;
			}
		}

		#endregion

		#region Public Members

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
		public bool Save()
		{
			bool retVal;

			retVal = UpdateUser();

			if ( retVal )
			{
				_IsNew = false;
				_IsDirty = false;
			}

			return retVal;
		}

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns></returns>
		public bool Delete()
		{
			bool retVal;

			retVal = DeleteUser();

			if ( retVal )
			{
				_IsDeleted = true;
			}

			return retVal;
		}

        /// <summary>
        /// Grants the admin status to this user.
        /// </summary>
        /// <returns></returns>
		public bool GrantAdminStatus()
		{
			bool retVal;
			SVNUserProvider userData;

			userData = new SVNUserProvider( _UserName, _Password, _ParentRepositoryPath );

			try
			{
				userData.AddAdmin();
				retVal = true;
				_IsAdmin = true;
			}
			catch ( Exception )
			{
			    retVal = false;
			}

			return retVal;
		}

        /// <summary>
        /// Revokes the admin status for this user.
        /// </summary>
        /// <returns></returns>
		public bool RevokeAdminStatus()
		{
			bool retVal;
			SVNUserProvider userData;

			userData = new SVNUserProvider( _UserName, _Password, _ParentRepositoryPath );

			try
			{
				userData.DeleteAdmin();
				retVal = true;
				_IsAdmin = false;
			}
			catch ( Exception )
			{
			    retVal = false;
			}

			return retVal;
		}

		#endregion

		#region Private Members

		private bool UpdateUser()
		{
			bool retVal;
			SVNUserProvider userData;

            if ( !Equals( _UserDatabaseFileName, null ) )
            {
                userData = new SVNUserProvider( _UserName, _Password, _UserDatabaseFileName );
            }
            else
            {
                userData = new SVNUserProvider( _UserName, _Password, _ParentRepositoryPath );
            }

		    try
			{
				userData.Save();

				retVal = true;
			}
			catch ( Exception )
			{
			    retVal = false;
            }

			return retVal;
		}

		private bool DeleteUser()
		{
			bool retVal;
            SVNUserProvider userData;

            if ( !Equals( _UserDatabaseFileName, null ) )
            {
                userData = new SVNUserProvider( _UserName, _Password, _UserDatabaseFileName );
            }
            else
            {
                userData = new SVNUserProvider( _UserName, _Password, _ParentRepositoryPath );
            }

			try
			{
				userData.Delete();
				retVal = true;
			}
			catch ( Exception )
			{
			    retVal = false;
			}

			return retVal;
		}

		#endregion
	}
}
