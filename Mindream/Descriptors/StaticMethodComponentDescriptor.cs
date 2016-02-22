using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Reflection;

namespace Mindream.Descriptors
{
    /// <summary>
    ///     This class represents a static method descritor.
    ///     For example, double Math.Sin(double)
    /// </summary>
    public class StaticMethodComponentDescriptor : ABaseComponentDescriptor
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StaticMethodComponent" /> class.
        /// </summary>
        /// <param name="pMethod">The method to introspect.</param>
        /// <param name="pRegistry">The descriptor registry for dynamic type.</param>
        public StaticMethodComponentDescriptor(MethodInfo pMethod, ComponentDescriptorRegistry pRegistry)
        {
            this.Method = pMethod;

            var lInputParameters = this.Method.GetParameters().Where(pParemeter => pParemeter.IsOut == false);
            foreach (var lInputParameter in lInputParameters)
            {
                this.mInputs.Add(new ParameterMemberInfo(lInputParameter));
            }


            var lOutputParameters = this.Method.GetParameters().Where(pParameter => pParameter.IsOut || (pParameter.IsOut == false && pParameter.ParameterType.IsByRef));
            foreach (var lOutputParameter in lOutputParameters)
            {
                this.mOutputs.Add(new ParameterMemberInfo(lOutputParameter));
            }

            if (this.Method.ReturnType != typeof (void))
            {
                this.mOutputs.Add(new BaseComponentMemberInfo("return", this.Method.ReturnType));
            }

            this.Results.Add(new GenericReturnInfo("End"));
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        ///     Creates an instance of a component.
        /// </summary>
        /// <returns>
        ///     The created instance of the component.
        /// </returns>
        public override IComponent Create()
        {
            var lComponent = new StaticMethodComponent();
            lComponent.Initialize(this);
            return lComponent;
        }

        #endregion // Methods.

        #region Fields

        /// <summary>
        ///     This field stores the inputs.
        /// </summary>
        private readonly List<IComponentMemberInfo> mInputs = new List<IComponentMemberInfo>();

        /// <summary>
        ///     This field stores the outputs.
        /// </summary>
        private readonly List<IComponentMemberInfo> mOutputs = new List<IComponentMemberInfo>();

        #endregion // Fields.

        #region Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public override string Id
        {
            get
            {
                return this.Method.Name;
            }
        }

        /// <summary>
        ///     Gets the inputs.
        /// </summary>
        /// <value>
        ///     The inputs.
        /// </value>
        public override List<IComponentMemberInfo> Inputs
        {
            get
            {
                return this.mInputs;
            }
        }

        /// <summary>
        ///     Gets the outputs.
        /// </summary>
        /// <value>
        ///     The outputs.
        /// </value>
        public override List<IComponentMemberInfo> Outputs
        {
            get
            {
                return this.mOutputs;
            }
        }

        /// <summary>
        ///     Gets or sets the method.
        /// </summary>
        /// <value>
        ///     The method.
        /// </value>
        public MethodInfo Method
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the component attribute.
        /// </summary>
        /// <value>
        ///     The component attribute.
        /// </value>
        public override AComponentAttribute ComponentAttribute
        {
            get
            {
                return this.Method.GetCustomAttributes(typeof (AComponentAttribute), true).FirstOrDefault() as AComponentAttribute;
            }
        }

        #endregion // Events.
    }
}