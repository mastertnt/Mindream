using System;
using System.Windows.Threading;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;

namespace DemoApplication.Components
{
    /// <summary>
    ///     The PrintString node serves as a simple way to display a string in the console.
    /// </summary>
    [FunctionComponent("Debug")]
    public class PrintString : AComponent
    {
        #region Inputs

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="PrintString" /> is condition.
        /// </summary>
        /// <value>
        ///     <c>true</c> if condition; otherwise, <c>false</c>.
        /// </value>
        [In]
        public string String
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a new line must be printed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [new line]; otherwise, <c>false</c>.
        /// </value>
        [In]
        public bool NewLine
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
            MainWindow.Instance.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => MainWindow.Instance.mOutput.Text += this.String));
            if (this.NewLine)
            {
                MainWindow.Instance.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => MainWindow.Instance.mOutput.Text += Environment.NewLine));    
            }
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