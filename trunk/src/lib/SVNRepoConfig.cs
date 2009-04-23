//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNRepoConfig.cs
// Author:			Hector Sosa, Jr
// Date:			5/7/2005
//**********************************************************

using System;
using System.IO;

using Nini.Config;
using Nini.Ini;

namespace SVNManagerLib
{
	/// <summary>
    /// This class provides read/write access to repository-wide configuration
    /// settings. This uses the Nini configuration library at
    /// http://nini.sourceforge.net/
	/// </summary>
	public class SVNRepoConfig
	{
        #region Member Variables

		private string _UserDatabaseFileName;
		private IniConfigSource _ServerConfig;
		private string _RepositoryRootDirectory;
        private RepositoryAuthorization _AnonymousAccess;
        private RepositoryAuthorization _AuthorizedAccess;
		private string _AnonAcc;
		private string _AuthAcc;
		private string _repositoryType;
        private string _repositoryUUID;
        private string _fullPathToConfFile;
        private string _authorizationRulesFile;
        private string _repositoryRealm;
	    private string _repositorySchemaVersion;
	    private bool _isSaslAvailable;
	    private int _minSaslEncryption;
	    private int _maxSaslEncryption;

	    #endregion

		#region Constructors

		/// <summary>
		/// Empty constructor
		/// </summary>
		public SVNRepoConfig()
		{}

		/// <summary>
		/// Loads a repository configuration information.
		/// </summary>
		/// <param name="RepositoryPath">The path to the repository.</param>
		public SVNRepoConfig( string RepositoryPath )
		{
			LoadRepositoryConfigurationSettings( RepositoryPath );
		}

        ///<summary>
        ///</summary>
        ///<param name="GlobalConfigFileInfo"></param>
        ///<param name="RepositoryPath"></param>
        public SVNRepoConfig( FileInfo GlobalConfigFileInfo, string RepositoryPath )
        {
            _RepositoryRootDirectory = RepositoryPath;

            LoadRepositoryConfigurationSettings( GlobalConfigFileInfo );
        }

		#endregion

		#region Properties

		/// <summary>
		/// The file that contains the user accounts.
		/// </summary>
		public string UserDatabaseFileName
		{
			get
			{
				return _UserDatabaseFileName;
			}
			set
			{
				_UserDatabaseFileName = value;
			}
		}

		/// <summary>
		/// The root directory for this repository.
		/// </summary>
		public string RepositoryRootDirectory
		{
			get
			{
				return _RepositoryRootDirectory;
			}
			set
			{
				_RepositoryRootDirectory = value;
			}
		}

        /// <summary>
        /// Gets or sets the anonymous access.
        /// </summary>
        /// <value>The anonymous access.</value>
		public RepositoryAuthorization AnonymousAccess
		{
			get
			{
				return _AnonymousAccess;
			}
			set
			{
				_AnonymousAccess = value;
			}
		}

        /// <summary>
        /// Gets or sets the authorized access.
        /// </summary>
        /// <value>The authorized access.</value>
		public RepositoryAuthorization AuthorizedAccess
		{
			get
			{
				return _AuthorizedAccess;
			}
			set
			{
				_AuthorizedAccess = value;
			}
		}

		/// <summary>
		/// Holds the string contained in the fs-type file
		/// inside the db folder.
		/// </summary>
		public string RepositoryType
		{
			get
			{
				return _repositoryType;
			}
			set
			{
				_repositoryType = value;
			}
		}

        /// <summary>
        /// Holds the UUID (Universal Unique IDentifier) for this repository.
        /// </summary>
        /// <remarks>
        /// This seems to be in GUID format.
        /// </remarks>
        public string RepositoryUUID
        {
            get
            {
                return _repositoryUUID;
            }
        }

        ///<summary>
        /// This stores the realm this repository belongs to.
        /// </summary>
        public string RepositoryRealm
        {
            get
            {
                return _repositoryRealm;
            }
            set
            {
                _repositoryRealm = value;
            }
        }

        ///<summary>
        /// This holds the path to the authorization file that contain rules 
        /// for path-based access control.
        /// </summary>
        ///<remarks>The default file name is authz. This uses the same format 
        /// as the authz file from the Apache setups.
        /// </remarks>
        public string AuthorizationRulesFile
        {
            get
            {
                return _authorizationRulesFile;
            }
            set
            {
                _authorizationRulesFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the repository schema version.
        /// </summary>
        /// <value>The repository schema version.</value>
	    public string RepositorySchemaVersion
	    {
	        get
	        {
	            return _repositorySchemaVersion;
	        }
            set
            {
                _repositorySchemaVersion = value;
            }
	    }


        /// <summary>
        /// Gets a value indicating whether this repository has SASL available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this this repository has SASL available; otherwise, <c>false</c>.
        /// </value>
        public bool IsSaslAvailable
	    {
	        get
	        {
	            return _isSaslAvailable;
	        }
	    }

        /// <summary>
        /// Gets the sasl minimun encryption.
        /// </summary>
        /// <value>The sasl minimun encryption.</value>
	    public Int32 SaslMinimunEncryption
	    {
	        get
	        {
	            return _minSaslEncryption;
	        }
	    }

        /// <summary>
        /// Gets the sasl maximum encryption.
        /// </summary>
        /// <value>The sasl maximum encryption.</value>
	    public Int32 SaslMaximumEncryption
	    {
	        get
	        {
	            return _maxSaslEncryption;
	        }
	    }

	    #endregion
		
		#region Public Members

        /// <summary>
        /// Updates the repository authorization settings in the svnserve.conf file.
        /// </summary>
        /// <returns></returns>
        public bool UpdateAuthorization()
        {
            bool retval = true;
            string anon = _AnonymousAccess.ToString().ToLower();
            string auth = _AuthorizedAccess.ToString().ToLower();

            var iniDoc = new IniDocument( _fullPathToConfFile, IniFileType.SambaStyle );
            var source = new IniConfigSource( iniDoc );

            source.Configs["general"].Set( "anon-access", anon );
            source.Configs["general"].Set( "auth-access", auth );

            try
            {
                source.Save( _fullPathToConfFile );
            }
            catch
           {
                retval = false;
           }

            return retval;
        }

        /// <summary>
        /// Updates the repository realm.
        /// </summary>
        /// <returns></returns>
        public bool UpdateRealm()
        {
            bool retval = true;

            var iniDoc = new IniDocument( _fullPathToConfFile, IniFileType.SambaStyle );
            var source = new IniConfigSource( iniDoc );

            source.Configs["general"].Set( "realm", _repositoryRealm );

            try
            {
                source.Save( _fullPathToConfFile );
            }
            catch
            {
                retval = false;
            }

            return retval;
        }

        /// <summary>
        /// Updates the sasl data.
        /// </summary>
        /// <returns></returns>
        public bool UpdateSaslData()
        {
            bool retval = true;

            var iniDoc = new IniDocument( _fullPathToConfFile, IniFileType.SambaStyle );
            var source = new IniConfigSource(iniDoc);

            source.Configs["sasl"].Set( "use-sasl", _isSaslAvailable );
            source.Configs["sasl"].Set( "min-encryption", _minSaslEncryption );
            source.Configs["sasl"].Set( "max-encryption", _maxSaslEncryption );

            try
            {
                source.Save( _fullPathToConfFile );
            }
            catch
            {
                retval = false;
            }

            return retval;
        }

        /// <summary>
        /// Converts a string to RepositoryAuthorization enum.
        /// </summary>
        /// <param name="authenticationString">The authentication string to convert.</param>
        /// <returns></returns>
        public RepositoryAuthorization ConvertStringToAuth( string authenticationString )
        {
            RepositoryAuthorization retVal;

            switch ( authenticationString.ToLower() )
            {
                case "write":

                    retVal = RepositoryAuthorization.Write;
                    break;

                case "read":

                    retVal = RepositoryAuthorization.Read;
                    break;

                case "none":

                    retVal = RepositoryAuthorization.None;
                    break;

                default:

                    retVal = RepositoryAuthorization.None;
                    break;
            }

            return retVal;
        }

		#endregion

		#region Private Members

		private void LoadRepositoryConfigurationSettings( string RepositoryPath )
		{
		    string fullPath = RepositoryPath;
		    string newFullPath;

            string confPart = "conf" + Path.DirectorySeparatorChar + "svnserve.conf";

            if ( !fullPath.EndsWith( confPart ) )
            {
                _RepositoryRootDirectory = fullPath;
                newFullPath = Path.Combine( fullPath, confPart );
            }
            else
            {
                _RepositoryRootDirectory += RepositoryPath.Replace( confPart, "" );
                newFullPath = fullPath;
            }

            _fullPathToConfFile = newFullPath;

            var iniDoc = new IniDocument( newFullPath, IniFileType.SambaStyle );

			_ServerConfig = new IniConfigSource( iniDoc );

            _UserDatabaseFileName = Path.GetDirectoryName( newFullPath );
			
			try
			{
                string fileName = _ServerConfig.Configs["general"].GetString( "password-db" );

                if ( fileName.Length > 0 )
                {
                    _UserDatabaseFileName = Path.Combine( _UserDatabaseFileName, fileName );
                } 
                else
                {
                    _UserDatabaseFileName = "";
                }

			}
			catch
			{
				_UserDatabaseFileName = "";
			}

			ParseConfigKeys();
		}

	    private void LoadRepositoryConfigurationSettings( FileInfo globalConfigFileInfo )
        {
            var iniDoc = new IniDocument( globalConfigFileInfo.FullName, IniFileType.SambaStyle );

            _ServerConfig = new IniConfigSource( iniDoc );

            try
            {
                string fileName = _ServerConfig.Configs["general"].GetString( "password-db" );

                bool passwordDbExists = File.Exists( fileName );

                if ( !passwordDbExists )
                {
// ReSharper disable PossibleNullReferenceException
                    string configPath = globalConfigFileInfo.Directory.ToString();
// ReSharper restore PossibleNullReferenceException
                    fileName = Path.Combine( configPath, fileName );
                }

                if (fileName.Length > 0)
                {
                    _UserDatabaseFileName = fileName;
                }
                else
                {
                    _UserDatabaseFileName = "";
                }

            }
            catch
            {
                _UserDatabaseFileName = "";
            }

            ParseConfigKeys();
        }

        private void ParseConfigKeys()
        {
            try
            {
                _AnonAcc = _ServerConfig.Configs["general"].GetString( "anon-access") ;
                _AnonymousAccess = ConvertStringToAuth(_AnonAcc);
            }
            catch
            {
                _AnonAcc = "";
                _AnonymousAccess = ConvertStringToAuth(_AnonAcc);
            }

            try
            {
                _AuthAcc = _ServerConfig.Configs["general"].GetString( "auth-access" );
                _AuthorizedAccess = ConvertStringToAuth(_AuthAcc);
            }
            catch
            {
                _AuthAcc = "";
                _AuthorizedAccess = ConvertStringToAuth(_AuthAcc);
            }

            try
            {
                _authorizationRulesFile = _ServerConfig.Configs["general"].GetString( "authz-db" );
            }
            catch
            {
                _authorizationRulesFile = "";
            }

            try
            {
                _repositoryRealm = _ServerConfig.Configs["general"].GetString( "realm" );
            }
            catch
            {
                _repositoryRealm = "";
            }

            try
            {
                _isSaslAvailable = _ServerConfig.Configs["sasl"].GetBoolean( "use-sasl" );
            }
            catch
            {
                _isSaslAvailable = false;
            }

            if ( _isSaslAvailable )
            {
                try
                {
                    _minSaslEncryption = _ServerConfig.Configs["sasl"].GetInt( "min-encryption" );
                }
                catch
                {
                    _minSaslEncryption = 0;
                }

                try
                {
                    _maxSaslEncryption = _ServerConfig.Configs["sasl"].GetInt( "max-encryption" ) ;
                }
                catch
                {
                    _maxSaslEncryption = 0;
                }
            }

            _repositoryType = GetRepositoryType();
            _repositoryUUID = GetRepositoryUUID();
            _repositorySchemaVersion = GetRepositorySchemaVersion();
        }

	    private string GetRepositoryType()
		{
		    string lineString;

	        string fixedPath = Common.GetCorrectedPath( _RepositoryRootDirectory, true );

            string typeFile = fixedPath + "db" + Path.DirectorySeparatorChar + "fs-type";

            try
            {
                var reader = new StreamReader( typeFile );

			    try
			    {
				    lineString = reader.ReadToEnd();
			    }
			    finally
			    {
				    reader.Close();
			    }

			    lineString = lineString.Replace( "\n", "" );
			    lineString = lineString.Trim();
            }
            catch ( FileNotFoundException )
            {
                lineString = string.Empty;
            }
			
			return lineString;
		}

        private string GetRepositoryUUID()
        {
            string lineString;

            string fixedPath = Common.GetCorrectedPath( _RepositoryRootDirectory, true );

            string uuidFile = fixedPath + "db" + Path.DirectorySeparatorChar + "uuid";

            try
            {
                var reader = new StreamReader( uuidFile );

                try
                {
                    lineString = reader.ReadToEnd();
                }
                finally
                {
                    reader.Close();
                }

                lineString = lineString.Replace( "\n", "" );
                lineString = lineString.Trim();
            }
            catch ( FileNotFoundException )
            {
                lineString = string.Empty;
            }
			
			return lineString;
		}

        private string GetRepositorySchemaVersion()
        {
            string lineString;
            string fixedPath = _RepositoryRootDirectory;
            string schemaPath = Path.Combine( fixedPath, "format" );

            try
            {
                var reader = new StreamReader( schemaPath );

                try
                {
                    lineString = reader.ReadToEnd();
                }
                finally
                {
                    reader.Close();
                }

                lineString = lineString.Replace( "\n", "" );
                lineString = lineString.Trim();
            }
            catch ( FileNotFoundException )
            {
                lineString = string.Empty;
            }

            return lineString;
        }

	    #endregion
	}
}
