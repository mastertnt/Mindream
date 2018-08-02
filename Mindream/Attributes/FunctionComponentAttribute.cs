using System;

namespace Mindream.Attributes
{
    /// <summary>
    ///     This attribute can be used when a function component is coded by developers.
    ///     The descriptor will be automatically generated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FunctionComponentAttribute : AComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionComponentAttribute"/> class.
        /// </summary>
        /// <param name="pCategory">The category.</param>
        public FunctionComponentAttribute(string pCategory)
            : base(pCategory)
        {
        }
    }
}