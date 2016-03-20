using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream
{
    /// <summary>
    ///     This class represents a simulation.
    /// </summary>
    public class ComponentDescriptorRegistry
    {
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

        #endregion // Properties.

        #region Methods

        /// <summary>
        ///     This method can be used to find all component descriptors for all loaded assemblies
        /// </summary>
        public void FindAllDescriptors()
        {
            var lAssembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var lLoadedAssembly in lAssembliesLoaded)
            {
                this.FindAllDescriptors(lLoadedAssembly);
            }
        }

        /// <summary>
        ///     This method can be used to find all component descriptors in a given assembly.
        /// </summary>
        public void FindAllDescriptors(Assembly pAssembly)
        {
            var lExportedTypes = pAssembly.GetExportedTypes();
            foreach (var lExportedType in lExportedTypes)
            {
                // Try to locate [StaticMethodComponentAttribute] on methods. 
                this.FindAllDescriptors(lExportedType);
            }
        }

        /// <summary>
        ///     This method can be used to find all component descriptors in a given assembly.
        /// </summary>
        public void FindAllDescriptors(Type pType)
        {
            var lAttributes = pType.GetCustomAttributes(typeof (FunctionComponentAttribute), false);
            if (lAttributes.Any())
            {
                this.Descriptors.Add(new FunctionComponentDescriptor(pType, this));
            }

            foreach (var lMethod in pType.GetMethods())
            {
                lAttributes = lMethod.GetCustomAttributes(typeof (StaticMethodComponentAttribute), false);
                if (lAttributes.Any())
                {
                    this.Descriptors.Add(new StaticMethodComponentDescriptor(lMethod));
                }
            }
        }

        /// <summary>
        ///     This method can be used to exponent a dynamic type used by a component.
        /// </summary>
        public void ExposeDynamicType(Type pType)
        {
            if (this.Descriptors.FirstOrDefault(pDesc => pDesc.Id == pType.Name) == null)
            {
                this.Descriptors.Add(new DynamicComponentDescriptor(pType, this));
            }
        }

        #endregion // Methods.
    }
}