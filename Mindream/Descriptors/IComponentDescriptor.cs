using System.Collections.Generic;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Reflection;

namespace Mindream.Descriptors
{
    /// <summary>
    ///     This delegate represents the end of a component (by default, just a simple return).
    /// </summary>
    public delegate void ComponentReturnDelegate();

    /// <summary>
    ///     This interface describes a component descriptor.
    ///     A component descriptor can instanciate a component.
    /// </summary>
    public interface IComponentDescriptor
    {
        /// <summary>
        ///     Gets the name (must be unique along all descriptors).
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Id
        {
            get;
        }

        /// <summary>
        ///     Gets the inputs.
        /// </summary>
        /// <value>
        ///     The inputs.
        /// </value>
        List<IComponentMemberInfo> Inputs
        {
            get;
        }

        /// <summary>
        ///     Gets the outputs.
        /// </summary>
        /// <value>
        ///     The outputs.
        /// </value>
        List<IComponentMemberInfo> Outputs
        {
            get;
        }

        /// <summary>
        ///     Gets the results.
        /// </summary>
        /// <value>
        ///     The results (By default, the result is ended).
        /// </value>
        List<IComponentReturnInfo> Results
        {
            get;
        }

        /// <summary>
        ///     Gets the component attribute.
        /// </summary>
        /// <value>
        ///     The component attribute.
        /// </value>
        AComponentAttribute ComponentAttribute
        {
            get;
        }


        /// <summary>
        ///     Creates an instance.
        /// </summary>
        /// <returns>The created instance of the component.</returns>
        IComponent Create();
    }
}