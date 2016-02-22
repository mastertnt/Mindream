using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Reflection;
using XSystem;

namespace Mindream.Descriptors
{
    /// <summary>
    ///     This class represents a function component descriptor.
    /// </summary>
    public class FunctionComponentDescriptor : ABaseComponentDescriptor
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionComponentDescriptor" /> class.
        /// </summary>
        /// <param name="pType">The method to introspect.</param>
        /// <param name="pRegistry">The descriptor registry for dynamic type.</param>
        public FunctionComponentDescriptor(Type pType, ComponentDescriptorRegistry pRegistry)
        {
            this.Type = pType;

            // Look for inputs.
            foreach (var lPropertyInfo in this.Type.GetProperties())
            {
                var lAttribute = lPropertyInfo.GetCustomAttributes(typeof (ParameterAttribute), true).FirstOrDefault() as ParameterAttribute;
                if (lAttribute != null)
                {
                    if (lPropertyInfo.CanRead && lAttribute.IsOutput)
                    {
                        this.Outputs.Add(new PropertyMemberInfo(lPropertyInfo));

                        if (lPropertyInfo.PropertyType.IsSimple() == false && typeof (ICollection).IsAssignableFrom(lPropertyInfo.PropertyType) == false)
                        {
                            pRegistry.ExposeDynamicType(lPropertyInfo.PropertyType);
                        }
                    }

                    if (lPropertyInfo.GetSetMethod() != null && lAttribute.IsInput)
                    {
                        this.Inputs.Add(new PropertyMemberInfo(lPropertyInfo));

                        if (lPropertyInfo.PropertyType.IsSimple() == false && typeof (ICollection).IsAssignableFrom(lPropertyInfo.PropertyType) == false)
                        {
                            pRegistry.ExposeDynamicType(lPropertyInfo.PropertyType);
                        }
                    }
                }
            }

            // Look for events.
            foreach (var lEventInfo in this.Type.GetEvents())
            {
                if (lEventInfo.EventHandlerType == typeof (ComponentReturnDelegate))
                {
                    this.Results.Add(new EventReturnInfo(lEventInfo));
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
            AComponent lComponent;
            if (typeof(ASingletonFunctionComponent).IsAssignableFrom(this.Type))
            {
                PropertyInfo lPropertyInfo = this.Type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                lComponent = (AComponent) lPropertyInfo.GetValue(null, null);
            }
            else
            {
                lComponent = Activator.CreateInstance(this.Type) as AComponent;
            }
             
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
                return this.Type.Name;
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
        public Type Type
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
                return this.Type.GetCustomAttributes(typeof (AComponentAttribute), true).FirstOrDefault() as AComponentAttribute;
            }
        }

        #endregion // Events.
    }
}