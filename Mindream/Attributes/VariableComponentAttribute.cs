using System;

namespace Mindream.Attributes
{
    /// <summary>
    ///     This attribute can be used when a variable component.
    ///     The descriptor will be automatically generated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VariableComponentAttribute : AComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionComponentAttribute"/> class.
        /// </summary>
        /// <param name="pCategory">The category.</param>
        public VariableComponentAttribute(string pCategory)
            : base(pCategory)
        {
        }
    }
}
