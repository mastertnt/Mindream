using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Reflection;
using IComponent = Mindream.Components.IComponent;

namespace Mindream.Descriptors
{
    /// <summary>
    ///     This class represents a static method descritor.
    ///     For example, double Math.Sin(double)
    /// </summary>
    public class StaticMethodComponentDescriptor : ABaseComponentDescriptor
    {
        #region Constants

        /// <summary>
        /// The constant which defines the result name.
        /// </summary>
        public const string RESULT_NAME = "result";

        #endregion // Constants.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StaticMethodComponent" /> class.
        /// </summary>
        /// <param name="pMethod">The method to introspect.</param>
        public StaticMethodComponentDescriptor(MethodInfo pMethod)
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
                this.mOutputs.Add(new UserComponentMemberInfo(RESULT_NAME, this.Method.ReturnType));
            }
            else
            {
                this.Results.Add(new MethodReturnInfo("End"));
            }
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
        [Browsable(false)]
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

        /// <summary>
        ///     Gets or sets the component type.
        /// </summary>
        /// <value>
        ///     The component type (can be null)
        /// </value>
        public override Type ComponentType
        {
            get
            {
                return typeof (MethodInfo);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an operator.
        /// An operator has no start and end.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an operator; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public override bool IsOperator
        {
            get
            {
                return true;
            }
        }

        #endregion // Events.
    }
}