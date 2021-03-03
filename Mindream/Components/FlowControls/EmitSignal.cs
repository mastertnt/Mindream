using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    /// Emit a signal.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class EmitSignal : AComponent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [InOut]
        public string SignalName
        {
            get;
            set;
        }

        #endregion // Properties.

        #region Events

        /// <summary>
        /// This event is raised when the loop is ended.
        /// </summary>
        public event ComponentReturnDelegate Ended;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            TaskManager.Instance.EmitSignal(this.SignalName);
            this.Stop();
        }

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            this.Ended?.Invoke();
        }

        #endregion // Methods.
    }
}