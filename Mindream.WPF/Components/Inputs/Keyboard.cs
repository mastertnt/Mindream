using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;
using XSerialization.Attributes;

namespace Mindream.WPF.Components.Inputs
{
    /// <summary>
    /// A component to catch all the keyboards.
    /// </summary>
    [FunctionComponent("Input")]
    public class Keyboard : AUpdatableComponent
    {
        #region Fields

        private bool mIsKeyDown;

        private bool mIsKeyUp;

        #endregion // Fields.

        #region Events

        /// <summary>
        ///     This event is raised when a false is computed.
        /// </summary>
        public event ComponentReturnDelegate KeyPressed;

        /// <summary>
        ///     This event is raised when a false is computed.
        /// </summary>
        public event ComponentReturnDelegate KeyReleased;

        #endregion // Events.

        #region Inputs

        /// <summary>
        ///     Gets or sets the event notifier.
        /// </summary>
        /// <value>
        ///     The event notifier.
        /// </value>
        [SkipXSerialization]
        public static Window EventNotifier
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the string to log.
        /// </summary>
        /// <value>
        ///     The string.
        /// </value>
        [In]
        public Key KeyToTest
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key pressed buffer.
        /// </summary>
        /// <value>
        /// The key pressed buffer.
        /// </value>
        [Out]
        public List<Key> KeyPressedBuffer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key released buffer.
        /// </summary>
        /// <value>
        /// The key released buffer.
        /// </value>
        [Out]
        public List<Key> KeyReleasedBuffer
        {
            get;
            set;
        }

        #endregion // Inputs

        #region Methods

        /// <summary>
        /// This method is called when the component is initialized.
        /// </summary>
        protected override void ComponentInitialized()
        {
            base.ComponentInitialized();
            this.KeyPressedBuffer = new List<Key>();
            this.KeyReleasedBuffer = new List<Key>();
        }

        /// <summary>
        ///     Events the notifier on key up.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pKeyEventArgs">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void EventNotifierOnKeyUp(object pSender, KeyEventArgs pKeyEventArgs)
        {
            if (pKeyEventArgs.Key == this.KeyToTest || this.KeyToTest == Key.None)
            {
                this.mIsKeyUp = true;
                this.KeyReleasedBuffer.Add(pKeyEventArgs.Key);
            }
        }

        /// <summary>
        ///     Events the notifier on key down.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pKeyEventArgs">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void EventNotifierOnKeyDown(object pSender, KeyEventArgs pKeyEventArgs)
        {
            if (pKeyEventArgs.Key == this.KeyToTest || this.KeyToTest == Key.None)
            {
                this.mIsKeyDown = true;
                this.KeyPressedBuffer.Add(pKeyEventArgs.Key);
            }
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            EventNotifier.KeyDown += this.EventNotifierOnKeyDown;
            EventNotifier.KeyUp += this.EventNotifierOnKeyUp;
        }

        /// <summary>
        ///     This method is called to update the component.
        /// </summary>
        public override void Update(TimeSpan pDeltaTime)
        {
            if (this.mIsKeyDown)
            {
                if (this.KeyPressed != null)
                {
                    this.KeyPressed();
                }
            }
            if (this.mIsKeyUp)
            {
                if (this.KeyReleased != null)
                {
                    this.KeyReleased();
                }
            }

            this.KeyReleasedBuffer.Clear();
            this.KeyPressedBuffer.Clear();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            EventNotifier.KeyDown -= this.EventNotifierOnKeyDown;
            EventNotifier.KeyUp -= this.EventNotifierOnKeyUp;
        }

        #endregion // Methods
    }
}