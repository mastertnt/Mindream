using System.Collections.Generic;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;
using SharpDX.DirectInput;

namespace Mindream.SharpDX.Components
{
    [FunctionComponent("Input")]
    public class JoystickController : AFunctionComponent
    {
        #region Fields

        /// <summary>
        ///     The device
        /// </summary>
        private Joystick mDevice;

        #endregion // Fields.

        #region Inputs

        /// <summary>
        ///     Gets or sets the updates.
        /// </summary>
        /// <value>
        ///     The updates.
        /// </value>
        [Out]
        public List<JoystickUpdate> Updates
        {
            get;
            set;
        }

        #endregion // Inputs.

        #region Events

        /// <summary>
        ///     This event is raised when the updates are ready.
        /// </summary>
        public event ComponentReturnDelegate Updated;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     This method is called when the component is initialized.
        /// </summary>
        protected override void ComponentInitilialized()
        {
            base.ComponentInitilialized();
            InputManager.Instance.Initialize();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStarted()
        {
            if (this.mDevice == null)
            {
                this.mDevice = InputManager.Instance.AddJoystick();
            }

            this.Updates = InputManager.Instance.GetJoystickStates(this.mDevice);
            if (this.Updated != null)
            {
                this.Updated();
            }
            this.Stop();
        }
        
        /// <summary>
        ///     This method is called when the component is stopped.
        /// </summary>
        protected override void ComponentStopped()
        {
            this.Updates.Clear();
        }

        /// <summary>
        ///     This method is called when the component is initialized.
        /// </summary>
        protected void ComponentFinalized()
        {
            base.ComponentInitilialized();
            InputManager.Instance.Initialize();

        }

        #endregion // Methods.
    }
}