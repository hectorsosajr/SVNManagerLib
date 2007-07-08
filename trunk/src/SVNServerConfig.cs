//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNServerConfig.cs
// Author:			Hector Sosa
// Date:			5/10/2005
//**********************************************************

using System;
using System.IO;
using Nini.Config;

namespace SVNManagerLib
{
	/// <summary>
	/// This class provides read/write access to server-wide configuration
    /// settings. This uses the Nini configuration library at
    /// http://nini.sourceforge.net/
	/// </summary>
	public class SVNServerConfig
	{
		#region Member Variables

		private string _repositoryRootDirectory = string.Empty;
		private string _serverRootDirectory = string.Empty;
		private string _commandRootDirectory = string.Empty;
        private string _configFileName = string.Empty;
        private string _defaultConfigFileName = "svnmanagerlib.ini";
		private IConfigSource _config;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor
		/// This class defaults the configuration file 
		/// to "svnmanagerlib.ini"
		/// </summary>
		public SVNServerConfig()
		{
			try
			{
				LoadServerConfiguration();
			}
			catch ( Exception ex )
			{
				Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// This class defaults to "svnmanagerlib.ini"
		/// This allows for a different configuration file.
		/// </summary>
		/// <param name="ConfigFilePath">The path to the user selected config file.</param>
		public SVNServerConfig( string ConfigFilePath )
		{
			try
			{
				LoadServerConfiguration( ConfigFilePath );
			}
			catch ( Exception ex )
			{
				Console.WriteLine( ex.Message );
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// This is the directory where the actual command-line
		/// programs reside.
		/// </summary>
		/// <remarks>It looks like some Linux distributions install the
		/// command-line programs to different locations.</remarks>
		public string CommandRootDirectory
		{
			get
			{
				return _commandRootDirectory;
			}
			set
			{
				_commandRootDirectory = value;
			}
		}

		/// <summary>
		/// This is the topmost directory. This is where the
		/// Subversion server binaries get installed.
		/// </summary>
		public string ServerRootDirectory
		{
			get
			{
				return _serverRootDirectory;
			}
			set
			{
				_serverRootDirectory = value;
			}
		}

		/// <summary>
		/// This is the root directory for all the repositories.
		/// </summary>
		/// <remarks>svnserve.exe should be configured with the -r argument on this
		/// directory in order for this to work correctly.</remarks>
		public string RepositoryRootDirectory
		{
			get
			{
				return _repositoryRootDirectory;
			}
			set
			{
				_repositoryRootDirectory = value;
			}
		}

        /// <summary>
        /// Gets or sets the name of the config file.
        /// </summary>
        /// <value>The name of the config file.</value>
        public string ConfigFileName
        {
            get
            {
                return _configFileName;
            }
            set
            {
                _configFileName = value;
            }
        }

		#endregion

		#region Public Members

		/// <summary>
		/// Saves the current values to the config file.
		/// </summary>
		/// <returns>Whether or not the save action was successful.</returns>
		public bool Save()
		{
			bool retval;

			try
			{
				_config.Configs["subversion"].Set( "serverdir", _serverRootDirectory );
				_config.Configs["subversion"].Set( "reporoot", _repositoryRootDirectory );
				_config.Configs["subversion"].Set( "commanddir", _commandRootDirectory );
				_config.Save();
				retval = true;
			}
            catch ( NullReferenceException )
            {
                FileInfo fi = new FileInfo( System.Reflection.Assembly.GetExecutingAssembly().FullName );
                string path = fi.DirectoryName + Path.PathSeparator + _defaultConfigFileName;

                StreamWriter configWriter = new StreamWriter(path);
                configWriter.WriteLine( "[subversion]" );
                configWriter.WriteLine( "; This is the root directory for the Subversion installation." );
                configWriter.WriteLine( "serverdir = " + _serverRootDirectory );
                configWriter.WriteLine( "; This is the root directory for the repositories" );
                configWriter.WriteLine( "reporoot = " + _repositoryRootDirectory );
                configWriter.WriteLine( "; This is where the actual command executables are" );
                configWriter.WriteLine( "commanddir = " + _commandRootDirectory );

                configWriter.Close();

                retval = true;
            }
			catch ( Exception )
			{
			    retval = false;
			}

			return retval;
		}

		#endregion

		#region Private members

		private void LoadServerConfiguration()
		{
            _config = new IniConfigSource( _defaultConfigFileName );

			_repositoryRootDirectory = _config.Configs["subversion"].GetString("reporoot");
			_serverRootDirectory =  _config.Configs["subversion"].GetString("serverdir");
			_commandRootDirectory = _config.Configs["subversion"].GetString("commanddir");
		    _configFileName = _defaultConfigFileName;
		}

		private void LoadServerConfiguration( string configFileName )
		{
			_config = new IniConfigSource( configFileName );

			_repositoryRootDirectory = _config.Configs["subversion"].GetString("reporoot");
			_serverRootDirectory =  _config.Configs["subversion"].GetString("serverdir");
			_commandRootDirectory = _config.Configs["subversion"].GetString("commanddir");
		    _configFileName = configFileName;
		}

		#endregion
	}
}
