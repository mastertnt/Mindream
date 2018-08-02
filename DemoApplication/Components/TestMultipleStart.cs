using System;
using System.Windows.Threading;
using Mindream;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;

namespace DemoApplication.Components
{
    /// <summary>
    ///     The PrintString node serves as a simple way to display a string in the console.
    /// </summary>
    [FunctionComponent("Debug")]
    public class TestMultipleStart : AComponent
    {
        #region Inputs

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TestMultipleStart"/> is a.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a; otherwise, <c>false</c>.
        /// </value>
        [Start("A")]
        public bool A
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TestMultipleStart"/> is b.
        /// </summary>
        /// <value>
        ///   <c>true</c> if b; otherwise, <c>false</c>.
        /// </value>
        [Start("B")]
        public bool B
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the port called.
        /// </summary>
        /// <value>
        /// The port called.
        /// </value>
        [Out]
        public string PortCalled
        {
            get;
            set;
        }

        #endregion // Inputs

        #region Events

        /// <summary>
        ///     This event is raised when a false is computed.
        /// </summary>
        public event ComponentReturnDelegate End;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.PortCalled = pPortName;
            this.Stop();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            if (this.End != null)
            {
                this.End();
            }
        }

        #endregion // Methods
    }
}