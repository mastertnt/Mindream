using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using XSystem;

namespace Mindream
{
    /// <summary>
    /// This class represents a simulation.
    /// </summary>
    public class ComponentDescriptorRegistry
    {
        #region Fields

        /// <summary>
        /// This field stores the dynamic type in progress.
        /// </summary>
        private readonly HashSet<Type> mInProgress = new HashSet<Type>();

        #endregion // Fields.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComponentDescriptorRegistry" /> class.
        /// </summary>
        public ComponentDescriptorRegistry()
        {
            this.Descriptors = new ObservableCollection<IComponentDescriptor>();
        }

        #endregion // Constructors.

        #region Properties

        /// <summary>
        ///     Gets or sets the component types.
        /// </summary>
        /// <value>
        ///     The component types.
        /// </value>
        public ObservableCollection<IComponentDescriptor> Descriptors
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to know if the dynamic discovering is enabled or not.
        /// </summary>
        public bool ExposeDynamic
        {
            get;
            private set;
        }

        #endregion // Properties.

        #region Methods

        /// <summary>
        /// Find all IComponent based types to get their corresponding component descriptors in cache.
        /// </summary>
        public void FindAllDescriptors(bool pExposeDynamic = false)
        {
            this.ExposeDynamic = pExposeDynamic;
            IEnumerable<Type> lDescriptorTypes = typeof(IComponent).GetInheritedTypes();
            foreach ( Type lDescriptorType in lDescriptorTypes )
            {
                this.FindAllDescriptors( lDescriptorType );
            }
        }

        /// <summary>
        /// This method can be used to find all component descriptors in a given assembly.
        /// </summary>
        public void FindAllDescriptors(Type pType)
        {
            object[] lAttributes = pType.GetCustomAttributes(typeof (FunctionComponentAttribute), false);
            if ( lAttributes.Length > 0 )
            {
                this.Descriptors.Add( new ComponentDescriptor( pType, this ) );
            }

            lAttributes = pType.GetCustomAttributes(typeof(VariableComponentAttribute), false);
            if (lAttributes.Length > 0)
            {
                this.Descriptors.Add(new VariableComponentDescriptor(pType, this));
            }
        }

        /// <summary>
        /// Finds all static descriptors in a given type.
        /// </summary>
        /// <param name="pType">Type of the explore.</param>
        public void FindStaticDescriptor(Type pType)
        {
            MethodInfo[] lMethods = pType.GetMethods();
            int lMethodCount = lMethods.Length;
            for (int lCurr = 0; lCurr < lMethodCount; lCurr++)
            {
                MethodInfo lMethod = lMethods[lCurr];
                object[] lAttributes =  lMethod.GetCustomAttributes(typeof(StaticMethodComponentAttribute), false);
                if (lAttributes.Length > 0)
                {
                    this.Descriptors.Add(new StaticMethodComponentDescriptor(lMethod));
                }
            }
        }

        /// <summary>
        /// This method can be used to exponent a dynamic type used by a component.
        /// </summary>
        public void ExposeDynamicType(Type pType)
        {
            if (this.ExposeDynamic)
            {
                if (this.Descriptors.FirstOrDefault(pDesc => pDesc.Id == pType.Name) == null)
                {
                    this.mInProgress.Add(pType);
                    this.Descriptors.Add(new DynamicComponentDescriptor(pType, this));
                }
            }
        }

        #endregion // Methods.
    }
}