using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mindream
{
    /// <summary>
    /// This class represents a simulation.
    /// </summary>
    public class ComponentRegistry
    {
        #region Properties

        /// <summary>
        /// Gets or sets the component types.
        /// </summary>
        /// <value>
        /// The component types.
        /// </value>
        public HashSet<ComponentType> ComponentTypes
        {
            get;
            set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentRegistry"/> class.
        /// </summary>
        public ComponentRegistry()
        {
            this.ComponentTypes = new HashSet<ComponentType>();
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// This method registers a component.
        /// </summary>
        /// <param name="pType">Type of the component.</param>
        /// <returns>True if the registration succeed, false otherwise.</returns>
        public bool RegisterComponentType(Type pType)
        {
            ComponentType lComponent = new ComponentType(pType);
            return this.ComponentTypes.Add(lComponent);
        }

        /// <summary>
        /// This method unregisters a component.
        /// </summary>
        /// <param name="pType">Type of the component.</param>
        /// <returns>True if the unregistration succeed, false otherwise.</returns>
        public bool UnregisterComponentType(Type pType)
        {
            ComponentType lComponent = new ComponentType(pType);
            return this.ComponentTypes.Remove(lComponent);
        }

        /// <summary>
        /// This method ticks the simulation.
        /// </summary>
        /// <param name="pSimulationDate">The absolute simulation date.</param>
        /// <param name="pRelativeTime">The relative simulation time.</param>
        /// <param name="pDeltaTime">The delta time since the last tick.</param>
        public void Tick(DateTime pSimulationDate, TimeSpan pRelativeTime, TimeSpan pDeltaTime)
        {
            
        }

        #endregion // Methods.
    }
}
