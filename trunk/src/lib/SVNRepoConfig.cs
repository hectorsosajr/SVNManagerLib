//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNRepoConfig.cs
// Author:			Hector Sosa, Jr
// Date:			5/7/2005
//**********************************************************

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
			LoadServerConfigs( RepositoryPath );
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

            IniDocument iniDoc = new IniDocument( _fullPathToConfFile, IniFileType.SambaStyle );
            IniConfigSource source = new IniConfigSource(iniDoc);

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

		private void LoadServerConfigs( string RepositoryPath )
		{
		    string fullPath = RepositoryPath;
			
            //OperatingSystem myOS = Environment.OSVersion;
			
            ////Code Checks to see which OS is being used 128 indicates Linux
            //if( (int)myOS.Platform == 128 )
            //{				
            //    if ( !fullPath.EndsWith( "/conf/svnserve.conf" ) )
            //    {
            //        _RepositoryRootDirectory = fullPath;
            //        fullPath += "/conf/svnserve.conf";
            //    }
            //    else
            //    {
            //        _RepositoryRootDirectory += RepositoryPath.Replace( "conf/svnserve.conf", "" );

            //    }				
            //}
            //else
            //{			
            //    if (!fullPath.EndsWith( "\\conf\\svnserve.conf" ))
            //    {
            //        _RepositoryRootDirectory = fullPath;
            //        fullPath += "\\conf\\svnserve.conf";
            //    }
            //    else
            //    {
            //        _RepositoryRootDirectory += RepositoryPath.Replace( "conf\\svnserve.conf", string.Empty );
            //    }				
            //}

            string confPart = Path.DirectorySeparatorChar + "conf" + Path.DirectorySeparatorChar + "svnserve.conf";

            if ( !fullPath.EndsWith( confPart ) )
            {
                _RepositoryRootDirectory = fullPath;
                fullPath = Path.Combine( fullPath, confPart );
            }
            else
            {
                _RepositoryRootDirectory += RepositoryPath.Replace( confPart, "" );
            }	

		    _fullPathToConfFile = fullPath;

			IniDocument iniDoc = new IniDocument( fullPath, IniFileType.SambaStyle );

			_ServerConfig = new IniConfigSource( iniDoc );

			_UserDatabaseFileName = Path.GetDirectoryName( fullPath );
			
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

			try
			{
				_AnonAcc = _ServerConfig.Configs["general"].GetString( "anon-access" );
				_AnonymousAccess = ConvertStringToAuth( _AnonAcc );
			}
			catch
			{
				_AnonAcc = "";
				_AnonymousAccess = ConvertStringToAuth( _AnonAcc );
			}

			try
			{
				_AuthAcc = _ServerConfig.Configs["general"].GetString( "auth-access" );
				_AuthorizedAccess = ConvertStringToAuth( _AuthAcc );
			}
			catch
			{
				_AuthAcc = "";
				_AuthorizedAccess = ConvertStringToAuth( _AuthAcc );
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

			_repositoryType = GetRepositoryType();
            _repositoryUUID = GetRepositoryUUID();
		}

	    private string GetRepositoryType()
		{
		    string lineString;

	        string fixedPath = Common.GetCorrectedPath( _RepositoryRootDirectory, true );

            string typeFile = fixedPath + "db" + Path.DirectorySeparatorChar + "fs-type";

            try
            {
                StreamReader reader = new StreamReader( typeFile );

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
                StreamReader reader = new StreamReader(uuidFile);

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
