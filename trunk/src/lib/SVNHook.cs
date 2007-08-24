//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNHook.cs
// Author:			Hector Sosa, Jr
// Date:			July 21, 2007
//**********************************************************

using System.IO;

namespace SVNManagerLib
{
	///<summary>
	/// Represents a single Subversion hook file.
	///</summary>
	public class SVNHook
	{
        #region Member Variables

        private string _contents = string.Empty;
        private string _hookPath = string.Empty;
	    private FileInfo _hookInfo = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNHook"/> class.
        /// </summary>
        /// <param name="hookPath">The hook file's path.</param>
        public SVNHook( string hookPath )
        {
            _hookPath = hookPath;

            _hookInfo = new FileInfo( hookPath );

            LoadContents();
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the contents.
        /// </summary>
        /// <value>The contents.</value>
        public string Contents
        {
            get
            {
                return _contents;
            }
            set
            {
                _contents = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get
            {
                return _hookInfo.Name;
            }
        }

        /// <summary>
        /// Gets the file path to this hook script.
        /// </summary>
        /// <value>The file path.</value>
	    public string FilePath
	    {
	        get
	        {
	            return _hookInfo.FullName;
	        }
	    }

        #endregion

        #region Private Members

        private void LoadContents()
        {
            StreamReader sr = new StreamReader( _hookPath );

            _contents = sr.ReadToEnd();
        } 

        #endregion
    }
}
