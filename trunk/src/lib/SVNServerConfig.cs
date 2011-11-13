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
        private string _repoMode = string.Empty;
        private string _configFileName = string.Empty;
        private string _configFilePath = string.Empty;
        private bool _usersGlobalConfigFile;
        private string _globalConfigFilePath = string.Empty;
        private readonly string _defaultConfigFileName = "svnmanagerlib.ini";
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

        /// <summary>
        /// Gets or sets the config file path.
        /// </summary>
        /// <value>The config file path.</value>
        public string ConfigFilePath
        {
            get
            {
                return _configFilePath;
            }
            set
            {
                _configFilePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the repository mode.
        /// </summary>
        /// <value>This will contain the word "root" for all the repositories under one
        /// directory, and the word "custom" for each repository in a different directory.</value>
        public string RepositoryMode
        {
            get
            {
                return _repoMode;
            }
            set
            {
                _repoMode = value;
            }
        }

        ///<summary>
        /// Tells whether or not the server is using a global config file.
        ///</summary>
        ///<remarks>
        /// This is the svnserve.conf file. This is only available on Subversion 1.5 or greater.
        /// </remarks>
        public bool UsesGlobalConfigFile
        {
            get
            {
                return _usersGlobalConfigFile;
            }
            set
            {
                _usersGlobalConfigFile = value;
            }
        }

        ///<summary>
        /// Stores the path to the global configuration file.
        ///</summary>
        ///<remarks>
        /// This is only available on Subversion 1.5 or greater.
        /// </remarks>
        public string GlobalConfigFilePath
        {
            get
            {
                return _globalConfigFilePath;
            }
            set
            {
                _globalConfigFilePath = value;
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
                _config.Configs["subversion"].Set( "commanddir", _commandRootDirectory );

                if ( _usersGlobalConfigFile )
                {
                    _config.Configs["subversion"].Set( "globalconfigdir", _globalConfigFilePath );
                }
                else
                {
                    _config.Configs["subversion"].Set( "globalconfigdir", string.Empty );
                }

                _config.Configs["repositories"].Set( "reporoot", _repositoryRootDirectory );
                _config.Configs["repositories"].Set( "mode", _repoMode );
                _config.Save();

                retval = true;
            }
            catch ( NullReferenceException )
            {
                string path = "";

                if ( _configFilePath.Length > 0 && _configFileName.Length > 0 )
                {
                    path = Path.Combine( _configFilePath, _configFileName );
                }
                else
                {
                    FileInfo fi;
                    fi = new FileInfo( System.Reflection.Assembly.GetExecutingAssembly().FullName );
                    if ( fi.DirectoryName != null ) path = Path.Combine( fi.DirectoryName, _defaultConfigFileName );
                }

                var configWriter = new StreamWriter(path);

                configWriter.WriteLine( "[subversion]" );
                configWriter.WriteLine( "; This is the root directory for the Subversion installation." );
                configWriter.WriteLine( "serverdir = " + _serverRootDirectory );
                configWriter.WriteLine( "; This is the actual directory where the command line executables are." );
                configWriter.WriteLine( "commanddir = " + _commandRootDirectory );

                if ( _usersGlobalConfigFile )
                {
                    configWriter.WriteLine( "; This is the location of the global svnserve.conf file." );
                    configWriter.WriteLine( "globalconfigdir = " + _globalConfigFilePath );
                }

                configWriter.WriteLine( "[repositories]" );
                configWriter.WriteLine( "; This is a flag to tell what mode the repositories are setup." );
                configWriter.WriteLine( "; root = all repositories are under one directory" );
                configWriter.WriteLine( "; custom = each repository is in a separate directory" );
                configWriter.WriteLine( "mode = " + _repoMode );
                configWriter.WriteLine( "; This is the root directory for the repositories." );
                configWriter.WriteLine( "reporoot = " + _repositoryRootDirectory );

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
            
            _serverRootDirectory =  _config.Configs["subversion"].GetString( "serverdir" );
            _commandRootDirectory = _config.Configs["subversion"].GetString( "commanddir" );
            _globalConfigFilePath = _config.Configs["subversion"].GetString( "globalconfigdir" );

            if ( !Equals( _globalConfigFilePath, null ) )
            {
                if ( _globalConfigFilePath.Length > 0 )
                {
                    _usersGlobalConfigFile = true;
                }
            }

            _repositoryRootDirectory = _config.Configs["repositories"].GetString( "reporoot" );
            _repoMode = _config.Configs["repositories"].GetString( "mode" );
            _configFileName = _defaultConfigFileName;
        }

        private void LoadServerConfiguration( string configFileName )
        {
            _config = new IniConfigSource( configFileName );

            _serverRootDirectory = _config.Configs["subversion"].GetString( "serverdir" );
            _commandRootDirectory = _config.Configs["subversion"].GetString( "commanddir" );
            _globalConfigFilePath = _config.Configs["subversion"].GetString( "globalconfigdir" );

            if ( !Equals( _globalConfigFilePath, null ) )
            {
                if ( _globalConfigFilePath.Length > 0 )
                {
                    _usersGlobalConfigFile = true;
                }
            }

            _repositoryRootDirectory = _config.Configs["repositories"].GetString( "reporoot" );
            _repoMode = _config.Configs["repositories"].GetString( "mode" );
            _configFileName = configFileName;
        }

        #endregion
    }
}
