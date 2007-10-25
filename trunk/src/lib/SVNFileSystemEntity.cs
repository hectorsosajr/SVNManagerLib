//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNFileSystemEntity.cs
// Author:			Hector Sosa, Jr
// Date:			10/3/2006
//**********************************************************

using System.Collections;
using System.IO;
using System.Text;

namespace SVNManagerLib
{
	/// <summary>
	/// Represents a file or folder inside a Subversion repository.
	/// </summary>
    public class SVNFileSystemEntity
	{
        #region Member Variables

		private Hashtable _properties = new Hashtable();
        private Hashtable _information = new Hashtable();
        private string _entityName;        
        private string _fullPath;
	    private string _serverCommandsPath;
 
	    #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNFileSystemEntity"/> class.
        /// </summary>
        public SVNFileSystemEntity()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNFileSystemEntity"/> class.
        /// </summary>
        /// <param name="serverCommandsPath">The path to the Subversion command line programs.</param>
        /// <param name="pathToEntity">The path to entity.</param>
        /// <param name="entityName">Name of the entity.</param>
        public SVNFileSystemEntity( string serverCommandsPath, string pathToEntity, string entityName )
        {
            _serverCommandsPath = serverCommandsPath;
			_fullPath = pathToEntity;
			_entityName = entityName;
            LoadEntityInfo();
            LoadEntityProperties();
		}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                _fullPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file system entity.
        /// </summary>
        /// <value>The name of the file or folder.</value>
        public string EntityName
        {
            get
            {
                return _entityName;
            }
            set
            {
                _entityName = value;
            }
        }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>The information.</value>
        public Hashtable Information
        {
            get
            {
                return _information;
            }
            set
            {
                _information = value;
            }
        }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public Hashtable Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        } 

        #endregion

        #region Public Members 

        /// <summary>
        /// Loads the information for this file or folder.
        /// </summary>
        /// <returns></returns>
        public bool LoadInfo( string pathToEntity )
        {
			bool retval;

			_fullPath = pathToEntity;

			retval = LoadEntityInfo();

			return retval;
        }

        /// <summary>
        /// Loads the properties for this file or folder.
        /// </summary>
        /// <returns></returns>
        public bool LoadProperties( string pathToEntity )
		{
			bool retval;

			_fullPath = pathToEntity;

			retval = LoadEntityProperties();

            return retval;
        } 

        #endregion

		#region Private Members

		private bool LoadEntityInfo()
		{
		    bool retval;
            string svnCommand;
            string convPath;
            string lines;
		    string errors;
		    string[] parsedLines;
            StringBuilder arg = new StringBuilder();

            convPath = Common.PathToFileUrl( _fullPath );

            svnCommand = Path.Combine( _serverCommandsPath, "svn" );

            // Start setting up the svn command
            arg.Append( "info " );
            arg.Append( convPath );

            retval = Common.ExecuteSvnCommand( svnCommand, arg.ToString(), out lines, out errors );

		    parsedLines = Common.ParseOutputIntoLines( lines );

            if ( !Equals( parsedLines,null ) )
            {
                foreach ( string s in parsedLines )
                {
                    if ( s != "" )
                    {
                        string[] data;
                        data = s.Split(':');

                        _information.Add(data[0], data[1]);
                    }
                } 
            }

			return retval;
		}

		private bool LoadEntityProperties()
        {
            bool retval;
            string svnCommand;
            string convPath;
            string lines;
		    string errors;
            string[] parsedLines;
            StringBuilder arg = new StringBuilder();

            convPath = Common.PathToFileUrl( _fullPath );

            svnCommand = Path.Combine( _serverCommandsPath, "svn" );

            // Start setting up the svn command
            arg.Append( "proplist " );
		    arg.Append( "--verbose " );
            arg.Append( convPath );

            retval = Common.ExecuteSvnCommand( svnCommand, arg.ToString(), out lines, out errors );

            parsedLines = Common.ParseOutputIntoLines( lines );

            if ( !Equals( parsedLines,null ) )
            {
                foreach ( string s in parsedLines )
                {
                    string[] data;
                    string key;
                    string value;

                    data = s.Split( ':' );
                    key = data[0] + data[1];
                    value = data[2];

                    _properties.Add( key, value );
                } 
            }

            return retval;
        } 

        #endregion
    }
}
