using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mindream.Attributes;
using Mindream.Reflection;
using IComponent = Mindream.Components.IComponent;

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
        [Browsable(false)]
        string Id
        {
            get;
        }

        /// <summary>
        ///     Gets the additional start ports.
        /// </summary>
        /// <value>
        ///     The additional start port.
        /// </value>
        [Browsable(false)]
        List<string> AdditionalStartPorts
        {
            get;
        }

        /// <summary>
        ///     Gets the inputs.
        /// </summary>
        /// <value>
        ///     The inputs.
        /// </value>
        [Browsable(false)]
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
        [Browsable(false)]
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
        [Browsable(false)]
        List<IComponentReturnInfo> Results
        {
            get;
        }

        /// <summary>
        ///     Gets the component attribute.
        /// </summary>
        /// <value>
        ///     The component attribute  (can be null)
        /// </value>
        [Browsable(false)]
        AComponentAttribute ComponentAttribute
        {
            get;
        }

        /// <summary>
        ///     Gets or sets the component type.
        /// </summary>
        /// <value>
        ///     The component type (can be null)
        /// </value>
        [Browsable(false)]
        Type ComponentType
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an operator.
        /// An operator has no start and end.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an operator; otherwise, <c>false</c>.
        /// </value>
        bool IsOperator
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance needs to instanciate an internal object.
        /// Useful for variable components.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance needs to create an internal object; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        bool NeedCreate
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