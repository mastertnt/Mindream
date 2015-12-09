using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This interface describes a component.
    /// </summary>
    interface IComponent
    {
        /// <summary>
        /// This event is raised when the component is started.
        /// </summary>
        event Action Started;

        /// <summary>
        /// This event is raised when the component succeed.
        /// </summary>
        event Action Succeed;

        /// <summary>
        /// This event is raised when the component failed.
        /// </summary>
        event Action Failed;

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        void Tick(DateTime pimulationDate, TimeSpan pRelativeTime, TimeSpan pDeltaTime);
    }
}
