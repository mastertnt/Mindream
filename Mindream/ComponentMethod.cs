using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This class represents a component on a static method.
    /// </summary>
    public class ComponentMethod : IComponent
    {
        #region Fields

        /// <summary>
        /// THis fields stores the inner method info.
        /// </summary>
        private readonly MethodInfo mMethodInfo;

        #endregion //  Fields.

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

        /// <summary>
        /// Gets the inputs.
        /// </summary>
        /// <value>
        /// The inputs.
        /// </value>
        public List<ParameterInfo> Inputs
        {
            get
            {
                return this.mMethodInfo.GetParameters().Where(pParemeter => pParemeter.IsIn).ToList();
            }
        }

        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>
        /// The outputs.
        /// </value>
        public List<ParameterInfo> Outputs
        {
            get
            {
                List<ParameterInfo> lOutputs = this.mMethodInfo.GetParameters().Where(pParemeter => pParemeter.IsOut).ToList();
                lOutputs.Add(this.mMethodInfo.ReturnParameter);
                return lOutputs;
            }
        }

        #endregion // Events.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentMethod"/> class.
        /// </summary>
        public ComponentMethod(MethodInfo pMethod)
        {
            this.mMethodInfo = pMethod;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pSimulationDate">The absolute simulation date.</param>
        /// <param name="pRelativeTime">The relative simulation time.</param>
        /// <param name="pDeltaTime">The delta time since the last tick.</param>
        public void Tick(DateTime pSimulationDate, TimeSpan pRelativeTime, TimeSpan pDeltaTime)
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
