//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNHook.cs
// Author:			Hector Sosa, Jr
// Date:			July 21, 2007
//**********************************************************

using System.Collections.Generic;
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
	    private bool _isBinary = false;
        private List<string> _binaryExtensions = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SVNHook"/> class.
        /// </summary>
        /// <param name="hookPath">The hook file's path.</param>
        public SVNHook( string hookPath )
        {
            _hookPath = hookPath;

            SetupExtensions();

            _hookInfo = new FileInfo( hookPath );

            LoadContents();
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the contents of the file, if it is a text file.
        /// </summary>
        /// <value>The text in the file.</value>
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
        /// Gets the name of the file.
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

        /// <summary>
        /// Gets a value indicating whether this instance is binary.
        /// </summary>
        /// <value><c>true</c> if this instance is binary; otherwise, <c>false</c>.</value>
        public bool IsBinary
        {
            get
            {
                return _isBinary;
            }
        }

        #endregion

        #region Private Members

        private void SetupExtensions()
        {
            _binaryExtensions.Add("exe");
            _binaryExtensions.Add("dll");
            _binaryExtensions.Add("chm");
            _binaryExtensions.Add("hlp");
            _binaryExtensions.Add("so");
            _binaryExtensions.Add("doc");
            _binaryExtensions.Add("rtf");
            _binaryExtensions.Add("xls");
            _binaryExtensions.Add("ppt");
            _binaryExtensions.Add("pdb");
            _binaryExtensions.Add("gif");
            _binaryExtensions.Add("png");
            _binaryExtensions.Add("jpg");
            _binaryExtensions.Add("wav");
            _binaryExtensions.Add("mp3");
            _binaryExtensions.Add("aac");
            _binaryExtensions.Add("tif");
            _binaryExtensions.Add("tiff");
        }

        private void LoadContents()
        {
            string ext = _hookInfo.Extension;

            if ( !_binaryExtensions.Contains( ext ) )
            {
                StreamReader sr = new StreamReader( _hookPath );
                _contents = sr.ReadToEnd();
            }
            else
            {
                _isBinary = true;
            }
        } 

        #endregion
    }
}
