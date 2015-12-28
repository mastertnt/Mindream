using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This class represents a component on a static method.
    /// </summary>
    public class StaticMethodComponent : IComponent
    {
        #region Fields

        /// <summary>
        /// This fields stores the parameters.
        /// </summary>
        private readonly object[] mParameters;

        /// <summary>
        /// This fields stores the last result.
        /// </summary>
        private object mLastResult;

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets the descriptor.
        /// </summary>
        /// <value>
        /// The descriptor.
        /// </value>
        [Browsable(false)]
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
        protected StaticMethodComponentDescriptor TypedDescriptor
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
        //[Editor(typeof(LastNameUserControlEditor), typeof(LastNameUserControlEditor))]
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
        //[Editor(typeof(LastNameUserControlEditor), typeof(LastNameUserControlEditor))]
        [Browsable(false)]
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
            int lHasResult = this.Descriptor.Outputs.Count(pParameter => pParameter.Position == -1);
            int lRefCount = this.Descriptor.Outputs.Count(pParameter => pParameter.IsOut == false && pParameter.ParameterType.IsByRef);

            // Create a parameter array for method invocation.
            this.mParameters = new object[this.Descriptor.Inputs.Count + this.Descriptor.Outputs.Count - lHasResult - lRefCount];

            // Initialize the input parameters.
            foreach (var lInput in this.Descriptor.Inputs)
            {
                this.Inputs.Add(lInput.Name, null);
            }

            // Initialize the output parameters.
            foreach (var lOutput in this.Descriptor.Outputs)
            {
                if (lOutput.Position != -1)
                {
                    this.Outputs.Add(lOutput.Name, null);
                }
                else
                {
                    this.Outputs.Add("result", null);
                }
            }
        }

        #endregion // Constructors.

        #region Events

        /// <summary>
        /// This event is raised when the component is started.
        /// </summary>
        public event Action<IComponent> Started;

        /// <summary>
        /// This event is raised when the component ended.
        /// </summary>
        public event Action<IComponent, string> Ended;

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
                    // Copy the input parameters.
                    foreach (var lInputParameter in this.Descriptor.Inputs)
                    {
                        this.mParameters[lInputParameter.Position] = this.Inputs[lInputParameter.Name];
                    }

                    // Copy the ouput parameters.
                    foreach (var lOuputParameter in this.Descriptor.Outputs)
                    {
                        if (lOuputParameter.Position != -1)
                        {
                            this.mParameters[lOuputParameter.Position] = this.Outputs[lOuputParameter.Name];
                        }
                    }

                    this.mLastResult = this.TypedDescriptor.Method.Invoke(null, this.mParameters);

                    // Copy the output.
                    foreach (var lParameter in this.Descriptor.Outputs)
                    {
                        if (lParameter.Position != -1)
                        {
                            this.Outputs[lParameter.Name] = this.mParameters[lParameter.Position];
                        }
                        else
                        {
                            this.Outputs["result"] = this.mLastResult;
                        }
                    }
                    this.Stop();
                }
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
        /// This method is called to stop the component.
        /// </summary>
        public void Stop()
        {
            if (this.Ended != null)
            {
                this.Ended(this, "Ended");
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder lBuilder = new StringBuilder();

            foreach (var lOuput in this.Outputs)
            {
                lBuilder.AppendLine(lOuput.Key + " = " + lOuput.Value);
            }

            return lBuilder.ToString();
        }

        #endregion // Methods.
    }
}
