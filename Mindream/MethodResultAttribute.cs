using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodResultAttribute :  Attribute
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public MethodResult Result
        {
            get; 
            private set;
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodResultAttribute"/> class.
        /// </summary>
        /// <param name="pResultName">Name of the result.</param>
        public MethodResultAttribute(string pResultName)
        {
            this.Result = new MethodResult(pResultName);
        }

        #endregion // Constructors.
    }
}
