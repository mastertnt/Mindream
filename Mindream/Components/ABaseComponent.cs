using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Mindream.Descriptors;
using Mindream.Structures;
using XSerialization.Attributes;

namespace Mindream.Components
{
    /// <summary>
    ///     This class is the base class for all components.
    /// </summary>
    public abstract class ABaseComponent : IComponent 
    {
        #region Events

        /// <summary>
        /// Notify a property change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        protected ABaseComponent()
        {
        }

        #endregion // Constructors.

        #region Indexers

        /// <summary>
        ///     Gets or sets the <see cref="System.Object" /> with the specified p parameter.
        /// </summary>
        /// <value>
        ///     The <see cref="System.Object" />.
        /// </value>
        /// <param name="pParameterName">The parameter name.</param>
        /// <returns></returns>
        [SkipXSerialization]
        public virtual object this[string pParameterName]
        {
            get
            {
                var lComponentMemberInfo = this.Descriptor.Outputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    return lComponentMemberInfo.GetValue(this);
                }

                lComponentMemberInfo = this.Descriptor.Inputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    return lComponentMemberInfo.GetValue(this);
                }

                return null;
            }
            set
            {
                var lComponentMemberInfo = this.Descriptor.Inputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    lComponentMemberInfo.SetValue(this, value);
                }
                else
                {
                    lComponentMemberInfo = this.Descriptor.Outputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                    if (lComponentMemberInfo != null)
                    {
                        lComponentMemberInfo.SetValue(this, value);
                    }
                }
            }
        }

        #endregion // Indexers.

        #region Properties

        /// <summary>
        ///     Gets the state of the component.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        [SkipXSerialization]
        [Browsable(false)]
        public TaskState State
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets the name of the result.
        /// </summary>
        /// <value>
        ///     The name of the result.
        /// </value>
        protected string ResultName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the descriptor.
        /// </summary>
        /// <value>
        ///     The descriptor.
        /// </value>
        [Browsable(false)]
        [SkipXSerialization]
        public IComponentDescriptor Descriptor
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the parameters values.
        /// </summary>
        /// <value>
        ///     The parameters.
        /// </value>
        internal ValueStorage Inputs
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the results values.
        /// </summary>
        /// <value>
        ///     The results.
        /// </value>
        [Browsable(false)]
        internal ValueStorage Outputs
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is updatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is updatable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)] 
        public abstract bool IsUpdatable
        {
            get;
        }

        /// <summary>
        /// Gets the maximum start count.
        /// </summary>
        /// <value>
        /// The maximum start count.
        /// </value>
        [Browsable(false)]
        public abstract int MaxStartCount
        {
            get;
        }

        #endregion // Properties.

        #region Events

        /// <summary>
        /// This event is raised when the component is initialized.
        /// </summary>
        public event Action<IComponent> Initialized;

        /// <summary>
        /// This event is raised when the component is started.
        /// If the value is empty or null, the default port is started.
        /// </summary>
        public event Action<IComponent, string> Started;

        /// <summary>
        /// This event is raised when the component is suspended.
        /// </summary>
        public event Action<IComponent> Suspended;

        /// <summary>
        ///     This event is raised when the component is resumed.
        /// </summary>
        public event Action<IComponent> Resumed;

        /// <summary>
        ///     This event is raised when the component is stopped.
        /// </summary>
        public event Action<IComponent> Stopped;

        /// <summary>
        /// This event is raised when the component has returned.
        /// </summary>
        public event Action<IComponent, string> Returned;

        /// <summary>
        /// This event is raised when the component is aborted (started.
        /// </summary>
        public event Action<IComponent> Aborted;

        /// <summary>
        /// This event is raised when the component failed.
        /// </summary>
        public event Action<IComponent> Failed;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// Initializes the specified p descriptor.
        /// </summary>
        /// <param name="pDescriptor">The descriptor.</param>
        public void Initialize(IComponentDescriptor pDescriptor)
        {
            this.Descriptor = pDescriptor;
            this.Inputs = new ValueStorage();
            this.Outputs = new ValueStorage();

            foreach (var lReturnInfo in this.Descriptor.Results)
            {
                var lEventInfo = this.GetType().GetEvent(lReturnInfo.Name);
                if (lEventInfo != null)
                {
                    ComponentReturnDelegate lDelegateForMethod = delegate
                    {
                        if (this.Returned != null)
                        {
                            this.Returned(this, lEventInfo.Name);
                        }
                    };
                    lEventInfo.AddEventHandler(this, lDelegateForMethod);
                }
            }

            if (this.Initialized != null)
            {
                this.Initialized(this);
            }
            this.ComponentInitialized();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        public void Start(string pPortName)
        {
            this.State = TaskState.Running;
            if (this.Started != null)
            {
                this.Started(this, pPortName);
            }
            try
            {
                this.ComponentStarted(pPortName);
            }
            catch (Exception lEx)
            {
                Console.WriteLine(lEx);
                if (this.Failed != null)
                {
                    this.Failed(this);
                }
            }
        }

        /// <summary>
        /// This method is called to suspend the component.
        /// </summary>
        public void Suspend()
        {
            this.State = TaskState.Suspended;
            if (this.Suspended != null)
            {
                this.Suspended(this);
            }
        }

        /// <summary>
        /// This method is called to update the component.
        /// </summary>
        public abstract void Update(TimeSpan pDeltaTime);

        /// <summary>
        /// This method is called to resume the component.
        /// </summary>
        public void Resume()
        {
            this.State = TaskState.Running;
            if (this.Resumed != null)
            {
                this.Resumed(this);
            }
        }

        /// <summary>
        /// This method is called to abort the component.
        /// </summary>
        public void Abort()
        {
            if (this.State == TaskState.Running)
            {
                this.ComponentAborted();
                if (this.Aborted != null)
                {
                    this.Aborted(this);
                }
            }
        }

        /// <summary>
        /// This method is called to stop the component.
        /// </summary>
        public void Stop()
        {
            if (this.State != TaskState.Stopped)
            {
                this.State = TaskState.Stopped;
                this.ComponentStopped();
                if (this.Stopped != null)
                {
                    this.Stopped(this);
                }
            }
        }

        /// <summary>
        /// This method is called when the component is initialized.
        /// </summary>
        protected virtual void ComponentInitialized()
        {
            // Nothing to do.
        }

        /// <summary>
        /// This method is called when the component is started.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected virtual void ComponentStarted(string pPortName)
        {
            // Nothing to do.
        }

        /// <summary>
        /// This method is called when the component is stopped.
        /// </summary>
        protected virtual void ComponentStopped()
        {
            // Nothing to do.
        }

        /// <summary>
        /// This method is called when the component is aborted.
        /// </summary>
        protected virtual void ComponentAborted()
        {
            // Nothing to do.
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var lBuilder = new StringBuilder();

            lBuilder.AppendLine("********************************");
            lBuilder.AppendLine("--->" + this.Descriptor.Id);
            lBuilder.AppendLine("Inputs");
            foreach (var lInput in this.Descriptor.Inputs)
            {
                lBuilder.AppendLine(lInput.Name + " = " + this[lInput.Name]);
            }
            lBuilder.AppendLine("Outputs");
            foreach (var lOuput in this.Descriptor.Outputs)
            {
                lBuilder.AppendLine(lOuput.Name + " = " + this[lOuput.Name]);
            }
            return lBuilder.ToString();
        }

        /// <summary>
        /// Exécute les tâches définies par l'application associées à la libération ou à la redéfinition des ressources non managées.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Dispose()
        {
            // To implement by children if necessary  
        }

        #endregion // Methods.
    }
}