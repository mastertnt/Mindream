using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    /// The FlipFlop node takes in an execution output and toggles between two execution outputs.
    /// The first time it is called, output A executes.
    /// The second time, B. Then A, then B, and so on. The node also has a boolean output allowing you to track when Output
    /// A has been called.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class FlipFlop : AComponent
    {
        #region Inputs

        /// <summary>
        /// Outputs a boolean value indicating whether Output A is being triggered or not.
        /// This, in effect, will toggle between true and false each time the FlipFlop node is triggered.
        /// </summary>
        [Out]
        public bool IsA
        {
            get;
            private set;
        }

        #endregion // Inputs.

        #region Events

        /// <summary>
        /// This event is raised when a false is computed.
        /// </summary>
        public event ComponentReturnDelegate A;

        /// <summary>
        /// This event is raised when a true is computed.
        /// </summary>
        public event ComponentReturnDelegate B;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            if (this.IsA)
            {
                this.IsA = false;
            }
            else
            {
                this.IsA = true;
            }
            this.Stop();
        }

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            if (this.IsA)
            {
                if (this.A != null)
                {
                    this.A();
                }
            }
            else
            {
                if (this.B != null)
                {
                    this.B();
                }
            }
        }

        #endregion // Methods.
    }
}