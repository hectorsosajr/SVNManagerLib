//**********************************************************
// Project Name:	SVNManagerLib.csproj
// File Name:		Repositories.cs
// Author:			Hector Sosa, Jr
// Date:			5/8/2005
//**********************************************************

using System.Collections;

namespace SVNManagerLib
{
    /// <summary>
    /// This represents a list of repositories in the current Subversion server.
    /// </summary>
    public class Repositories : CollectionBase
    {
        #region Constructors

        #endregion

        #region  Properties

        /// <summary>
        /// Gets the <see cref="SVNRepository"/> at the specified index.
        /// </summary>
        /// <value></value>
        public SVNRepository this[int Index]
        {
            get
            {
                return ((SVNRepository)(List[Index]));
            }
        }

        /// <summary>
        /// Gets the <see cref="SVNRepository"/> with the specified repo name.
        /// </summary>
        /// <value></value>
        public SVNRepository this[string repoName]
        {
            get
            {
                foreach( SVNRepository tmpRepo in List )
                {
                    if( tmpRepo.Name == repoName )
                    {
                        return tmpRepo;
                    }
                }
                
                return null;
            }
        }

        #endregion

        #region  Public Members

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="Items">The items.</param>
        /// <returns></returns>
        public int[] Add(SVNUserCollection Items)
        {
            ArrayList indexes = new ArrayList();

            foreach (object Item in Items)
            {
                indexes.Add(List.Add(Item));
            }

            return ((int[])(indexes.ToArray(typeof(int))));
        }

        /// <summary>
        /// Strongly typed Add method.
        /// </summary>
        /// <param name="Item">An instance of <see cref="SVNRepository"/> class.</param>
        /// <returns>The position index where this <see cref="SVNRepository"/> instance was inserted.</returns>
        public int Add(SVNRepository Item)
        {
            return List.Add(Item);
        }

        /// <summary>
        /// Strongly typed Insert method.
        /// </summary>
        /// <param name="Index">The index to where this <see cref="SVNRepository"/> instance should be inserted.</param>
        /// <param name="Item">An instance of <see cref="SVNRepository"/> class.</param>
        public void Insert(int Index, SVNRepository Item)
        {
            List.Insert(Index, Item);
        }

        /// <summary>
        /// Removes a specific <see cref="SVNRepository"/> instance.
        /// </summary>
        /// <param name="Item">An instance of <see cref="SVNRepository"/> class.</param>
        public void Remove(SVNRepository Item)
        {
            List.Remove(Item);
        }

        /// <summary>
        /// Checks if this collection has the specified <see cref="SVNRepository"/> instance.
        /// </summary>
        /// <param name="Item">An instance of <see cref="SVNRepository"/> class.</param>
        /// <returns></returns>
        public bool Contains(SVNRepository Item)
        {
            return List.Contains(Item);
        }

        #endregion
    }
}
