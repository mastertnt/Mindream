namespace Mindream.Components
{
    public abstract class AMethodComponent : AFunctionComponent
    {
        #region Events

        /// <summary>
        ///     This event is raised when the loop is ended.
        /// </summary>
        public event ComponentReturnDelegate Ended;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            if (this.Ended != null)
            {
                this.Ended();
            }
        }

        #endregion // Methods.
    }
}
