using System.Linq;
using Mindream.Descriptors;

namespace Mindream.Components
{
    /// <summary>
    /// This class represents a component on a static method.
    /// </summary>
    public class StaticMethodComponent : AComponent
    {
        #region Fields

        /// <summary>
        /// This fields stores the parameters.
        /// </summary>
        private object[] mParameters;

        /// <summary>
        /// This fields stores the last result.
        /// </summary>
        private object mLastResult;

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets or sets the typed descriptor.
        /// </summary>
        /// <value>
        /// The typed descriptor.
        /// </value>
        protected StaticMethodComponentDescriptor TypedDescriptor
        {
            get
            {
                return (StaticMethodComponentDescriptor) this.Descriptor;
            }
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticMethodComponent"/> class.
        /// </summary>
        public StaticMethodComponent()
        {
            
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// This method is called when the component is initialized.
        /// </summary>
        protected override void ComponentInitilialized()
        {
            int lHasResult = 0; //this.Descriptor.Outputs.Count(pParameter => pParameter.Position == -1);
            int lRefCount = 0; //this.Descriptor.Outputs.Count(pParameter => pParameter.IsOut == false && pParameter.ParameterType.IsByRef);

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


        /// <summary>
        /// This method is called when the component is started.
        /// </summary>
        protected override void ComponentStarted()
        {
            if (this.Descriptor != null)
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

        #endregion // Methods.
    }
}
