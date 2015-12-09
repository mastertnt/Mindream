using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This class reflects all the methods, events and property availabled for a component.
    /// </summary>
    public class ComponentType : IEquatable<ComponentType>
    {
        #region Fields

        /// <summary>
        /// The inner type
        /// </summary>
        private Type mInnerType = null;

        #endregion // Fields.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentType"/> class.
        /// </summary>
        /// <param name="pInnerType">The inner type.</param>
        public ComponentType(Type pInnerType)
        {
            this.mInnerType = pInnerType;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// This mthod checks if the instance is equal to an other one.
        /// </summary>
        /// <param name="pOther">The other instance.</param>
        /// <returns>True if the two instances are equal, false otherwise.</returns>
        public bool Equals(ComponentType pOther)
        {
            if (this.mInnerType == pOther.mInnerType)
            {
                return true;
            }
            return false;
        }

        #endregion // Methods.
    }
}
