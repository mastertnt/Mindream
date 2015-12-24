using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream
{
    public class MethodResult
    {
        /// <summary>
        /// Gets the name of the action.
        /// </summary>
        /// <value>
        /// The name of the action.
        /// </value>
        public string ResultName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodResult"/> class.
        /// </summary>
        /// <param name="pResultName">Name of the result.</param>
        public MethodResult(string pResultName)
        {
            this.ResultName = pResultName;
        }
    }
}
