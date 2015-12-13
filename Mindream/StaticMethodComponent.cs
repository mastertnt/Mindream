using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindream
{
    /// <summary>
    /// This class represents a component on a static method.
    /// </summary>
    public class StaticMethodComponent : IComponent
    {
        #region Properties

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <value>
        /// The descriptor.
        /// </value>
        public IComponentDescriptor Descriptor
        {
            get { return this.TypedDescriptor; }
        }

        /// <summary>
        /// Gets or sets the typed descriptor.
        /// </summary>
        /// <value>
        /// The typed descriptor.
        /// </value>
        StaticMethodComponentDescriptor TypedDescriptor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public Dictionary<string, object> Inputs 
        { 
            get;
            private set;
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public Dictionary<string, object> Outputs
        {
            get;
            private set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticMethodComponent"/> class.
        /// </summary>
        public StaticMethodComponent(StaticMethodComponentDescriptor pDescriptor)
        {
            this.TypedDescriptor = pDescriptor;
            this.Inputs = new Dictionary<string, object>();
            this.Outputs = new Dictionary<string, object>();
        }

        #endregion // Constructors.

        #region Events

        /// <summary>
        /// This event is raised when the component is started.
        /// </summary>
        public event Action<IComponent> Started;

        /// <summary>
        /// This event is raised when the component succeed.
        /// </summary>
        public event Action<IComponent> Succeed;

        /// <summary>
        /// This event is raised when the component failed.
        /// </summary>
        public event Action<IComponent> Failed;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        public void Start()
        {
            if (this.Started != null)
            {
                this.Started(this);
            }

            try
            {
                if (this.TypedDescriptor != null)
                {
                    this.TypedDescriptor.Method.Invoke(null, new object[] {0.5});
                    this.Stop();
                }
            }
            catch
            {
                if (this.Failed != null)
                {
                    this.Failed(this);
                }
            }
        }

        /// <summary>
        /// This method is called to stop the component.
        /// </summary>
        public void Stop()
        {
            if (this.Succeed != null)
            {
                this.Succeed(this);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified p parameter.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="pParameterName">The parameter name.</param>
        /// <returns></returns>
        public object this[string pParameterName]
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


        #endregion // Methods.


        
    }
}
