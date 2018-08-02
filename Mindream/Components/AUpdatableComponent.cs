using System.ComponentModel;
namespace Mindream.Components
{
    /// <summary>
    ///     This class is the base class for all updatable components.
    /// </summary>
    public abstract class AUpdatableComponent : ABaseComponent
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AUpdatableComponent" /> class.
        /// </summary>
        protected AUpdatableComponent()
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
        public override bool IsUpdatable
        {
            get
            {
                return true;
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
            get { return int.MaxValue; }
        }

        #endregion // Properties.
    }
}