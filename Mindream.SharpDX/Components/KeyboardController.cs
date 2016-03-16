using System.Collections.Generic;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;
using SharpDX.DirectInput;

namespace Mindream.SharpDX.Components
{
    [FunctionComponent("Input")]
    public class KeyboardController : AFunctionComponent
    {
        #region Inputs

        /// <summary>
        ///     Gets or sets the updates.
        /// </summary>
        /// <value>
        ///     The updates.
        /// </value>
        [Out]
        public List<KeyboardUpdate> Updates
        {
            get;
            set;
        }

        #endregion // Inputs

        #region Events

        /// <summary>
        ///     This event is raised when a false is computed.
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
            this.Updates = InputManager.Instance.GetKeyboardState();
            if (this.Updated != null)
            {
                this.Updated();
            }
            this.Stop();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            this.Updates.Clear();
        }

        #endregion // Methods
    }
}