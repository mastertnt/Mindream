using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Components.Variables;
using Mindream.Reflection;
using XSystem;
using IComponent = Mindream.Components.IComponent;

namespace Mindream.Descriptors
{
    /// <summary>
    ///     This class represents a dynamic component descriptor.
    /// </summary>
    /// <seealso cref="Mindream.Descriptors.ABaseComponentDescriptor" />
    public class DynamicComponentDescriptor : ABaseComponentDescriptor
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicComponentDescriptor" /> class.
        /// </summary>
        /// <param name="pType">The method to introspect.</param>
        /// <param name="pRegistry">The descriptor registry for dynamic type.</param>
        public DynamicComponentDescriptor(Type pType, ComponentDescriptorRegistry pRegistry)
        {
            this.mComponentType = pType;
            var lType = typeof (Dynamic);

            // Look for inputs in dynamic.
            foreach (var lPropertyInfo in lType.GetProperties())
            {
                var lAttribute = lPropertyInfo.GetCustomAttributes(typeof (ParameterAttribute), true).FirstOrDefault() as ParameterAttribute;
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
            foreach (var lPropertyInfo in this.ComponentType.GetProperties())
            {
                if (lPropertyInfo.CanRead)
                {
                    this.Outputs.Add(new PropertyMemberInfo(lPropertyInfo));

                    if (lPropertyInfo.PropertyType.IsSimple() == false && typeof (ICollection).IsAssignableFrom(lPropertyInfo.PropertyType) == false)
                    {
                        pRegistry.ExposeDynamicType(lPropertyInfo.PropertyType);
                    }
                }

                if (lPropertyInfo.GetSetMethod() != null)
                {
                    this.Inputs.Add(new PropertyMemberInfo(lPropertyInfo));

                    if (lPropertyInfo.PropertyType.IsSimple() == false && typeof (ICollection).IsAssignableFrom(lPropertyInfo.PropertyType) == false)
                    {
                        pRegistry.ExposeDynamicType(lPropertyInfo.PropertyType);
                    }
                }
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
            AComponent lComponent = new Dynamic();
            lComponent.Initialize(this);
            return lComponent;
        }

        #endregion // Methods.

        #region Fields

        /// <summary>
        ///     This field stores the component type.
        /// </summary>
        private readonly Type mComponentType;

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
                return this.ComponentType.Name;
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
        ///     Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        public override Type ComponentType
        {
            get
            {
                return this.mComponentType;
            }
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
                return null;
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