using System;
using System.IO;
using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.Debugs
{
    /// <summary>
    ///     The PrintString node serves as a simple way to log a string into a file.
    /// </summary>
    [FunctionComponent("Debug")]
    public class LogFile : AFunctionComponent
    {
        #region Events

        /// <summary>
        ///     This event is raised when a false is computed.
        /// </summary>
        public event ComponentReturnDelegate End;

        #endregion // Events.

        #region Inputs

        /// <summary>
        ///     Gets or sets the string to log.
        /// </summary>
        /// <value>
        ///     The string.
        /// </value>
        [In]
        public string String
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the filename.
        /// </summary>
        /// <value>
        ///     The filename.
        /// </value>
        [In]
        public string Filename
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
            File.AppendAllText(this.Filename, this.String + Environment.NewLine);
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