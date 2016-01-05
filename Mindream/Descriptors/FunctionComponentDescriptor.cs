using System;
using System.Collections.Generic;
using System.Linq;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Reflection;

namespace Mindream.Descriptors
{
    /// <summary>
    /// This class represents a function component descriptor.
    /// For example, a branch or a flip/flop is considered as a function descriptor.
    /// </summary>
    public class FunctionComponentDescriptor : ABaseComponentDescriptor
    {
        #region Fields

        /// <summary>
        /// This field stores the inputs.
        /// </summary>
        private readonly List<IComponentMemberInfo> mInputs = new List<IComponentMemberInfo>();

        /// <summary>
        /// This field stores the outputs.
        /// </summary>
        private readonly List<IComponentMemberInfo> mOutputs = new List<IComponentMemberInfo>();

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Id
        {
            get
            {
                return this.Type.Name;
            }
        }

        /// <summary>
        /// Gets the inputs.
        /// </summary>
        /// <value>
        /// The inputs.
        /// </value>
        public override List<IComponentMemberInfo> Inputs
        {
            get
            {
                return this.mInputs;
            }
        }

        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>
        /// The outputs.
        /// </value>
        public override List<IComponentMemberInfo> Outputs
        {
            get
            {
                return this.mOutputs;
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type
        {
            get; private set;
        }

        #endregion // Events.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionComponentDescriptor"/> class.
        /// </summary>
        /// <param name="pType">Type of the p.</param>
        public FunctionComponentDescriptor(Type pType)
        {
            this.Type = pType;

            // Look for inputs.
            foreach (var lPropertyInfo in this.Type.GetProperties())
            {
                ParameterAttribute lAttribute = lPropertyInfo.GetCustomAttributes(typeof (ParameterAttribute), true).FirstOrDefault() as ParameterAttribute;
                if (lAttribute != null)
                {
                    if (lPropertyInfo.CanRead && lAttribute.IsOutput)
                    {
                        this.Outputs.Add(new PropertyMemberInfo(lPropertyInfo));
                    }

                    if (lPropertyInfo.GetSetMethod() != null && lAttribute.IsInput)
                    {
                        this.Inputs.Add(new PropertyMemberInfo(lPropertyInfo));
                    }
                }

                
            }

            // Look for inputs.
            foreach (var lEventInfo in this.Type.GetEvents())
            {
                if (lEventInfo.EventHandlerType == typeof(ComponentReturnDelegate))
                {
                    this.Results.Add(new EventReturnInfo(lEventInfo));
                }
            }
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Creates an instance of a component.
        /// </summary>
        /// <returns>
        /// The created instance of the component.
        /// </returns>
        public override IComponent Create()
        {
            AComponent lComponent = Activator.CreateInstance(this.Type) as AComponent;
            lComponent.Initialize(this);
            return lComponent;
        }

        #endregion // Methods.
    }
}
