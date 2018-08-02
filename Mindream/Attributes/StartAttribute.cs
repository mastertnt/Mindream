using System;

namespace Mindream.Attributes
{
    /// <summary>
    /// This attribute can be declared on the start method to add multiple start value.
    /// </summary>
    public class StartAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StartAttribute" /> class.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        public StartAttribute(string pPortName)
        {
            this.PortName = pPortName;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is in.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is in; otherwise, <c>false</c>.
        /// </value>
        public string PortName
        {
            get;
            private set;
        }
    }
}