//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNProperties.cs
// Author:			Hector Sosa, Jr
// Date:			8/23/2007
//**********************************************************

using System.Collections.Generic;

namespace SVNManagerLib
{
    ///<summary>
    ///</summary>
    public class SVNProperty
    {
        #region Member Variables

        private string _value;
        private string _name; 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        } 

        #endregion
    }

    ///<summary>
    ///</summary>
    public class SVNProperties : List<SVNProperty>
    {}
}
