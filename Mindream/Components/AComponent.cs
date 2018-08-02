using System;
using System.ComponentModel;

namespace Mindream.Components
{
    /// <summary>
    ///     This class is the base class for all non-updatable components.
    /// </summary>
    public abstract class AComponent : ABaseComponent
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        protected AComponent()
        {
        }

        #endregion // Constructors.

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is updatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is updatable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public override bool IsUpdatable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the maximum start count.
        /// </summary>
        /// <value>
        /// The maximum start count.
        /// </value>
        [Browsable(false)]
        public override int MaxStartCount
        {
            get 
            { 
                return int.MaxValue; 
            }
        }

        #endregion // Properties.

        #region Methods

        /// <summary>
        /// This method is called to update the component.
        /// </summary>
        /// <param name="pDeltaTime">The delta time.</param>
        public override void Update(TimeSpan pDeltaTime)
        {
            // Nothing to do.
        }

        #endregion // Methods
    }
}