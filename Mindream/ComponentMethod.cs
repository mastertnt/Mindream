using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This class represents a component on a static method.
    /// </summary>
    public class ComponentMethod : IComponent
    {
        #region Events

        /// <summary>
        /// This event is raised when the component is started.
        /// </summary>
        public event Action Started;

        /// <summary>
        /// This event is raised when the component succeed.
        /// </summary>
        public event Action Succeed;

        /// <summary>
        /// This event is raised when the component failed.
        /// </summary>
        public event Action Failed;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pimulationDate"></param>
        /// <param name="pRelativeTime"></param>
        /// <param name="pDeltaTime"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Tick(DateTime pimulationDate, TimeSpan pRelativeTime, TimeSpan pDeltaTime)
        {
            if (this.Started != null)
            {
                this.Started();
            }

            try
            {
                // Call the method.


                if (this.Succeed != null)
                {
                    this.Succeed();
                }
            }
            catch
            {
                if (this.Failed != null)
                {
                    this.Failed();
                }
            }
        }

        #endregion // Methods.
    }
}
