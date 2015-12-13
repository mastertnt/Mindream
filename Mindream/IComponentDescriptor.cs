using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This interface describes a component descriptor.
    /// A component descriptor can instanciate a component.
    /// </summary>
    public interface IComponentDescriptor
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the inputs.
        /// </summary>
        /// <value>
        /// The inputs.
        /// </value>
        List<ParameterInfo> Inputs { get; }

        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>
        /// The outputs.
        /// </value>
        List<ParameterInfo> Outputs { get; }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <returns>The created instance of the component.</returns>
        IComponent Create();
    }
}
