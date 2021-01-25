using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    /// Mutex component.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class WaitForSignal : AComponent
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
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
            TaskManager.Instance.SignalOccured += this.OnSignalRaised;
        }

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            this.Ended?.Invoke();
        }

        /// <summary>
        /// Called when a signal is raised.
        /// </summary>
        /// <param name="pManager">The owner of the signal.</param>
        /// <param name="pSignalName">The name of signal raised.</param>
        private void OnSignalRaised(TaskManager pManager, string pSignalName)
        {
            if (pSignalName == this.Name)
            {
                TaskManager.Instance.SignalOccured -= this.OnSignalRaised;
                this.Stop();
            }
        }

        #endregion // Methods.
    }
}