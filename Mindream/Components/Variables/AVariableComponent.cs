using Mindream.Attributes;

namespace Mindream.Components.Variables
{
    /// <summary>
    ///     This component cane be used to get/set a variable.
    /// </summary>
    /// <typeparam name="TNativeType">The type of the native type.</typeparam>
    public abstract class AVariableComponent<TNativeType> : AMethodComponent
    {
        #region Inputs

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [InOut]
        public TNativeType Value
        {
            get;
            set;
        }

        #endregion // Inputs

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStarted()
        {
            this.Stop();
        }

        #endregion // Methods
    }
}