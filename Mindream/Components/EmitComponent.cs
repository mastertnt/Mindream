using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindream.Descriptors;

namespace Mindream.Components
{
    public class EmitComponent : AComponent
    {
        #region Events

        /// <summary>
        ///     This event is raised when a false is computed.
        /// </summary>
        public event ComponentReturnDelegate End;

        #endregion // Events.
        
        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start.</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.Stop();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            this.End?.Invoke();
        }

    }
}
