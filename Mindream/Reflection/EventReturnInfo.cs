using System.Reflection;

namespace Mindream.Reflection
{
    /// <summary>
    /// This class represents a component return info based on an event.
    /// </summary>
    public class EventReturnInfo : IComponentReturnInfo
    {
        #region Fields

        /// <summary>
        /// The fields stores the underlying information.
        /// </summary>
        private readonly EventInfo mUnderlyingInfo;

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return this.mUnderlyingInfo.Name;
            }
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventReturnInfo"/> class.
        /// </summary>
        /// <param name="pUnderlyingInfo">The event information.</param>
        public EventReturnInfo(EventInfo pUnderlyingInfo)
        {
            this.mUnderlyingInfo = pUnderlyingInfo;
        }

        #endregion // Constructors.
    }
}
