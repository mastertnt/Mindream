using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream.Attributes
{
    public class ParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether this instance is in.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is in; otherwise, <c>false</c>.
        /// </value>
        public bool IsInput
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is out.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is out; otherwise, <c>false</c>.
        /// </value>
        public bool IsOutput
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterAttribute"/> class.
        /// </summary>
        /// <param name="pIsInput">if set to <c>true</c> [the parameter is input].</param>
        /// <param name="pIsOutput">if set to <c>true</c> [the parameter is output].</param>
        public ParameterAttribute(bool pIsInput, bool pIsOutput)
        {
            this.IsInput = pIsInput;
            this.IsOutput = pIsOutput;
        }

    }
}
