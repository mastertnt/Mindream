using Mindream.Attributes;
using Mindream.Components;
using Mindream.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XSystem;

namespace Mindream.Descriptors
{
    /// <summary>
    ///     This class represents a function component descriptor.
    /// </summary>
    public class ComponentDescriptor : ABaseComponentDescriptor
    {
        #region Fields

        /// <summary>
        ///     This field stores the component type.
        /// </summary>
        private readonly Type mComponentType;

        #endregion // Fields.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptor" /> class.
        /// </summary>
        /// <param name="pComponentType">The method to introspect.</param>
        /// <param name="pRegistry">The descriptor registry for dynamic type.</param>
        public ComponentDescriptor(Type pComponentType, ComponentDescriptorRegistry pRegistry)
        {
            this.mComponentType = pComponentType;

            // Look for inputs.
            PropertyInfo[] lProperties = this.ComponentType.GetProperties();
            int lPropertyCount = lProperties.Length;
            for ( int lCurr = 0; lCurr < lPropertyCount; lCurr++ )
            {
                PropertyInfo lPropertyInfo = lProperties[ lCurr ];
                ParameterAttribute lAttribute = lPropertyInfo.GetCustomAttributes(typeof(ParameterAttribute), true).FirstOrDefault() as ParameterAttribute;
                if ( lAttribute != null )
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

                StartAttribute lStartAttribute = lPropertyInfo.GetCustomAttributes(typeof(StartAttribute), true).FirstOrDefault() as StartAttribute;
                if (lStartAttribute != null)
                {
                    if (lPropertyInfo.CanRead && lPropertyInfo.CanWrite && lPropertyInfo.PropertyType == typeof(bool))
                    {
                        this.AdditionalStartPorts.Add(lStartAttribute.PortName);
                    }
                }
            }

            // Look for events.
            EventInfo[] lEvents = this.ComponentType.GetEvents();
            int lEventCount = lEvents.Length;
            for ( int lCurr = 0; lCurr < lEventCount; lCurr++ )
            {
                EventInfo lEventInfo = lEvents[ lCurr ];
                if ( lEventInfo.EventHandlerType == typeof (ComponentReturnDelegate) )
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
            ABaseComponent lComponent;
            if (typeof (ASingletonFunctionComponent).IsAssignableFrom(this.ComponentType))
            {
                var lPropertyInfo = this.ComponentType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                lComponent = (AComponent) lPropertyInfo.GetValue(null, null);
            }
            else
            {
                lComponent = Activator.CreateInstance(this.ComponentType) as ABaseComponent;
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
        ///     Gets or sets the component type.
        /// </summary>
        /// <value>
        ///     The component type (can be null)
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
                return this.ComponentType.GetCustomAttributes(typeof (AComponentAttribute), true).FirstOrDefault() as AComponentAttribute;
            }
        }

        #endregion // Events.
    }
}