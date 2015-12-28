using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Mindream.Attributes;
using Mindream.Descriptors;
using XSystem;

namespace Mindream
{
    /// <summary>
    /// This class represents a simulation.
    /// </summary>
    public class ComponentDescriptorRegistry
    {
        #region Properties

        /// <summary>
        /// Gets or sets the component types.
        /// </summary>
        /// <value>
        /// The component types.
        /// </value>
        public ObservableCollection<IComponentDescriptor> Descriptors
        {
            get;
            set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptorRegistry"/> class.
        /// </summary>
        public ComponentDescriptorRegistry()
        {
            this.Descriptors = new ObservableCollection<IComponentDescriptor>();
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// This method can be used to find all component descriptors for all loaded assemblies 
        /// </summary>
        public void FindAllDescriptors()
        {
            Assembly[] lAssembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly lLoadedAssembly in lAssembliesLoaded)
            {
                this.FindAllDescriptors(lLoadedAssembly);
            }
        }

        /// <summary>
        /// This method can be used to find all component descriptors in a given assembly.
        /// </summary>
        public void FindAllDescriptors(Assembly pAssembly)
        {
            Type[] lExportedTypes = pAssembly.GetExportedTypes();
            foreach (var lExportedType in lExportedTypes)
            {
               // Try to locate [StaticMethodComponentAttribute] on methods. 
                this.FindAllDescriptors(lExportedType);
            }
        }

        /// <summary>
        /// This method can be used to find all component descriptors in a given assembly.
        /// </summary>
        public void FindAllDescriptors(Type pType)
        {
            object[] lAttributes = pType.GetCustomAttributes(typeof(FunctionComponentAttribute), false);
            if (lAttributes.Any())
            {
                this.Descriptors.Add(new FunctionComponentDescriptor(pType));
            }

            foreach (var lMethod in pType.GetMethods())
            {
                lAttributes = lMethod.GetCustomAttributes(typeof(StaticMethodComponentAttribute), false);
                if (lAttributes.Any())
                {
                    this.Descriptors.Add(new StaticMethodComponentDescriptor(lMethod));
                }
            }
        }

        #endregion // Methods.
    }
}
