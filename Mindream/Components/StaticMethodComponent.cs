using Mindream.Descriptors;
using System.Linq;

namespace Mindream.Components
{
    /// <summary>
    ///     This class represents a component on a static method.
    /// </summary>
    public class StaticMethodComponent : AComponent
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StaticMethodComponent" /> class.
        /// </summary>
        public StaticMethodComponent()
        {
        }

        #endregion // Constructors.

        #region Properties

        /// <summary>
        ///     Gets or sets the typed descriptor.
        /// </summary>
        /// <value>
        ///     The typed descriptor.
        /// </value>
        protected StaticMethodComponentDescriptor TypedDescriptor
        {
            get
            {
                return (StaticMethodComponentDescriptor) this.Descriptor;
            }
        }

        #endregion // Properties.

        #region Fields

        /// <summary>
        ///     This fields stores the parameters.
        /// </summary>
        private object[] mParameters;

        /// <summary>
        ///     This fields stores the last result.
        /// </summary>
        private object mLastResult;

        #endregion // Fields.

        #region Methods

        /// <summary>
        ///     This method is called when the component is initialized.
        /// </summary>
        protected override void ComponentInitilialized()
        {
            var lHasResult = this.Descriptor.Outputs.Count(pParameter => pParameter.Position == -1);
            var lRefCount = 0;//this.Descriptor.Outputs.Count(pParameter => pParameter.IsOut == false && pParameter.ParameterType.IsByRef);

            // Create a parameter array for method invocation.
            this.mParameters = new object[this.Descriptor.Inputs.Count + this.Descriptor.Outputs.Count - lHasResult - lRefCount];

            // Initialize the input parameters.
            foreach (var lInput in this.Descriptor.Inputs)
            {
                this.Inputs.SetValue(lInput.Name, null);
            }

            // Initialize the output parameters.
            foreach (var lOutput in this.Descriptor.Outputs)
            {
                if (lOutput.Position != -1)
                {
                    this.Outputs.SetValue(lOutput.Name, null);
                }
                else
                {
                    this.Outputs.SetValue(StaticMethodComponentDescriptor.RESULT_NAME, null);
                }
            }
        }

        /// <summary>
        /// This method is called when the component is started.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
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
                        this.Outputs[StaticMethodComponentDescriptor.RESULT_NAME] = this.mLastResult;
                    }
                }
                this.Stop();
            }
        }

        #endregion // Methods.
    }
}