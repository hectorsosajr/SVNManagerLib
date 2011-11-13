//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		SVNUserCollection.cs
// Author:			Hector Sosa, Jr
// Date:			5/7/2005
//**********************************************************

using System.Collections;

namespace SVNManagerLib
{
    /// <summary>
    /// This represents a list of <see cref="SVNUser"/> objects.
    /// </summary>
    public class SVNUserCollection : CollectionBase
    {
        #region Constructors

        #endregion

        #region  Properties

        /// <summary>
        /// Gets the <see cref="SVNUser"/> at the specified index.
        /// </summary>
        /// <value></value>
        public SVNUser this[int Index]
        {
            get
            {
                return ( (SVNUser)(List[Index]) );
            }
        }

        #endregion

        #region  Public Members

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="Items">The items.</param>
        /// <returns></returns>
        public int[] Add( SVNUserCollection Items )
        {
            ArrayList indexes = new ArrayList();

            foreach ( object Item in Items )
            {
                indexes.Add( List.Add( Item ) );
            }

            return ((int[])(indexes.ToArray(typeof(int))));
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="Item">The item.</param>
        /// <returns></returns>
        public int Add( SVNUser Item )
        {
            return List.Add( Item );
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="Index">The index.</param>
        /// <param name="Item">The item.</param>
        public void Insert( int Index, SVNUser Item )
        {
            List.Insert( Index, Item );
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="Item">The item.</param>
        public void Remove( SVNUser Item )
        {
            List.Remove( Item );
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="Item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains( SVNUser Item )
        {
            return List.Contains( Item );
        }

        #endregion
    }
}
