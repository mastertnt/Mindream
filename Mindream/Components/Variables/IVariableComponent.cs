using System;

namespace Mindream.Components.Variables
{
    /// <summary>
    /// This interface describes a variable component.
    /// </summary>
    public interface IVariableComponent
    {
        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <value>
        /// The type of the variable.
        /// </value>
        Type VariableType
        {
            get;
        }
    }
}
