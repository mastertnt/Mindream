using System;
using Mindream.Descriptors;
using System.ComponentModel;

namespace Mindream.Components
{
    /// <summary>
    ///     This interface describes a component instance.
    /// </summary>
    public interface IComponent : IDisposable, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        ///     Gets the state of the component.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        TaskState State
        {
            get;
        }

        /// <summary>
        ///     Gets the descriptor.
        /// </summary>
        /// <value>
        ///     The descriptor.
        /// </value>
        IComponentDescriptor Descriptor
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is updatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is updatable; otherwise, <c>false</c>.
        /// </value>
        bool IsUpdatable
        {
            get;
        }

        /// <summary>
        /// Gets the maximum start count.
        /// </summary>
        /// <value>
        /// The maximum start count.
        /// </value>
        int MaxStartCount
        {
            get;
        }

        #endregion // Properties

        #region Events

        /// <summary>
        ///     This event is raised when the component is initialized.
        /// </summary>
        event Action<IComponent> Initialized;

        /// <summary>
        ///     This event is raised when the component is started.
        ///     If the value is empty or null, the default port is started.
        /// </summary>
        event Action<IComponent, string> Started;

        /// <summary>
        ///     This event is raised when the component is suspended.
        /// </summary>
        event Action<IComponent> Suspended;

        /// <summary>
        ///     This event is raised when the component is resumed.
        /// </summary>
        event Action<IComponent> Resumed;

        /// <summary>
        ///     This event is raised when the component is stopped.
        /// </summary>
        event Action<IComponent> Stopped;

        /// <summary>
        ///     This event is raised when the component is aborted.
        /// </summary>
        event Action<IComponent> Aborted;

        /// <summary>
        ///     This event is raised when the component has returned.
        /// </summary>
        event Action<IComponent, string> Returned;

        /// <summary>
        ///     This event is raised when the component failed.
        /// </summary>
        event Action<IComponent> Failed;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     Gets or sets the <see cref="System.Object" /> with the specified p parameter.
        /// </summary>
        /// <value>
        ///     The <see cref="System.Object" />.
        /// </value>
        /// <param name="pParameterName">The parameter name.</param>
        /// <returns></returns>
        object this[string pParameterName]
        {
            get;
            set;
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        void Start(string pPortName = "");

        /// <summary>
        ///     This method is called to suspend the component.
        /// </summary>
        void Suspend();

        /// <summary>
        ///     This method is called to update the component.
        /// </summary>
        void Update(TimeSpan pDeltaTime);

        /// <summary>
        ///     This method is called to resume the component.
        /// </summary>
        void Resume();

        /// <summary>
        ///     This method is called to stop the component.
        /// </summary>
        void Stop();

        /// <summary>
        ///     This method is called to abort the component.
        /// </summary>
        void Abort();

        #endregion // Methods.
    }
}