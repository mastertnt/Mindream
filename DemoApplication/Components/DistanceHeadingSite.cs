using DemoApplication.Models;
using Mindream;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;

namespace DemoApplication.Components
{
    /// <summary>
    ///     This component computes the distance, heading and site between two geographical positions.
    /// </summary>
    /// <seealso cref="Mindream.Components.AComponent" />
    [FunctionComponent("Geographic")]
    public class DistanceHeadingSite : AComponent
    {
        #region Events

        /// <summary>
        ///     This event is raised when a false is computed.
        /// </summary>
        public event ComponentReturnDelegate End;

        #endregion // Events.

        #region Properties

        /// <summary>
        ///     Gets or sets the first.
        /// </summary>
        /// <value>
        ///     The first.
        /// </value>
        [In]
        public GeoPosition First
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the second.
        /// </summary>
        /// <value>
        ///     The second.
        /// </value>
        [In]
        public GeoPosition Second
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the distance in meters.
        /// </summary>
        /// <value>
        ///     The distance in meters.
        /// </value>
        [Out]
        public double Distance
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the heading in degrees
        /// </summary>
        /// <value>
        ///     The heading in degrees.
        /// </value>
        [Out]
        public double Heading
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the site in degrees
        /// </summary>
        /// <value>
        ///     The site in degrees.
        /// </value>
        [Out]
        public double Site
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [second start].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [second start]; otherwise, <c>false</c>.
        /// </value>
        [Start("SecondStart")]
        public bool SecondStart
        {
            get;
            set;
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.Heading = this.First.InitialBearingTo(this.Second);
            this.Distance = this.First.Distance2DTo(this.Second);
            this.Stop();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            if (this.End != null)
            {
                this.End();
            }
        }

        #endregion // Methods
    }
}