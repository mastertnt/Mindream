using System;
using System.Collections.Generic;

namespace Mindream
{
    /// <summary>
    /// This interface describes a component instance.
    /// </summary>
    public interface IComponent
    {
        #region Properties

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <value>
        /// The descriptor.
        /// </value>
        IComponentDescriptor Descriptor
        {
            get;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        Dictionary<string, object> Inputs
        {
            get;
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        Dictionary<string, object> Outputs
        {
            get;
        }

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised when the component is started.
        /// </summary>
        event Action<IComponent> Started;

        /// <summary>
        /// This event is raised when the component succeed.
        /// </summary>
        event Action<IComponent, string> Ended;

        /// <summary>
        /// This event is raised when the component failed.
        /// </summary>
        event Action<IComponent> Failed;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified p parameter.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="pParameterName">The parameter name.</param>
        /// <returns></returns>
        object this[string pParameterName]
        {
            get; 
            set;
        }

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        void Start();

        /// <summary>
        /// This method is called to stop the component.
        /// </summary>
        void Stop();

        #endregion // Methods.
    }
}
