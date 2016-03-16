using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Mindream.Descriptors;
using XSerialization.Attributes;

namespace Mindream.Components
{
    /// <summary>
    ///     This class is the base class for all components.
    /// </summary>
    public abstract class AComponent : IComponent
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AComponent" /> class.
        /// </summary>
        protected AComponent()
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
                if (this.Inputs.ContainsKey(pParameterName))
                {
                    return this.Inputs[pParameterName];
                }

                if (this.Outputs.ContainsKey(pParameterName))
                {
                    return this.Outputs[pParameterName];
                }
                return null;
            }
            set
            {
                if (this.Descriptor.Inputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName) != null)
                {
                    if (this.Inputs.ContainsKey(pParameterName))
                    {
                        this.Inputs[pParameterName] = value;
                    }
                    else
                    {
                        this.Inputs.Add(pParameterName, value);
                    }
                }

                if (this.Descriptor.Outputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName) != null)
                {
                    if (this.Outputs.ContainsKey(pParameterName))
                    {
                        this.Outputs[pParameterName] = value;
                    }
                    else
                    {
                        this.Outputs.Add(pParameterName, value);
                    }
                }
            }
        }

        #endregion // Indexers.

        #region Properties

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
        public IComponentDescriptor Descriptor
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        /// <value>
        ///     The parameters.
        /// </value>
        public Dictionary<string, object> Inputs
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the results.
        /// </summary>
        /// <value>
        ///     The results.
        /// </value>
        [Browsable(false)]
        public Dictionary<string, object> Outputs
        {
            get;
            private set;
        }

        #endregion // Properties.

        #region Events

        /// <summary>
        ///     This event is raised when the component is initialized.
        /// </summary>
        public event Action<IComponent> Initialized;

        /// <summary>
        ///     This event is raised when the component is started.
        /// </summary>
        public event Action<IComponent> Started;

        /// <summary>
        ///     This event is raised when the component is stopped.
        /// </summary>
        public event Action<IComponent> Stopped;

        /// <summary>
        ///     This event is raised when the component has returned.
        /// </summary>
        public event Action<IComponent, string> Returned;

        /// <summary>
        ///     This event is raised when the component failed.
        /// </summary>
        public event Action<IComponent> Failed;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     Initializes the specified p descriptor.
        /// </summary>
        /// <param name="pDescriptor">The p descriptor.</param>
        public void Initialize(IComponentDescriptor pDescriptor)
        {
            this.Descriptor = pDescriptor;
            this.Inputs = new Dictionary<string, object>();
            this.Outputs = new Dictionary<string, object>();

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
            this.ComponentInitilialized();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        public void Start()
        {
            if (this.Started != null)
            {
                this.Started(this);
            }
            try
            {
                this.ComponentStarted();
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
        ///     This method is called to stop the component.
        /// </summary>
        public void Stop()
        {
            this.ComponentStopped();
            if (this.Stopped != null)
            {
                this.Stopped(this);
            }
        }

        /// <summary>
        ///     This method is called when the component is initialized.
        /// </summary>
        protected virtual void ComponentInitilialized()
        {
            // Nothing to do.
        }

        /// <summary>
        ///     This method is called when the component is started.
        /// </summary>
        protected virtual void ComponentStarted()
        {
            // Nothing to do.
        }

        /// <summary>
        ///     This method is called when the component is stopped.
        /// </summary>
        protected virtual void ComponentStopped()
        {
            // Nothing to do.
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
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

        #endregion // Methods.
    }
}