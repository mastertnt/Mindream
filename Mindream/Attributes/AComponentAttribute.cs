using System;

namespace Mindream.Attributes
{
    /// <summary>
    /// Base component attribute abstract class definition.
    /// </summary>
    public abstract class AComponentAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the component category.
        /// </summary>
        public string Category
        {
            get;
            private set;
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AComponentAttribute"/> class.
        /// </summary>
        /// <param name="pCategory">The component category.</param>
        protected AComponentAttribute(string pCategory)
        {
            this.Category = pCategory;
        }

        #endregion Constructor
    }
}