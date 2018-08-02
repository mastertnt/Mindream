using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
