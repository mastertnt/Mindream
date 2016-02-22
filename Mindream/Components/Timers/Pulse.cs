using System;
using System.Timers;
using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.Timers
{
    [FunctionComponent("Timing")]
    public class Pulse : AFunctionComponent
    {
        #region Fields

        private DateTime mStartTime;

        #endregion // Fields.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pulse" /> class.
        /// </summary>
        public Pulse()
        {
            this.Duration = new TimeSpan(0, 0, 10);
            this.UpdatePeriod = new TimeSpan(0, 0, 1);
        }

        #endregion // Constructors.

        /// <summary>
        ///     This method is called when the component is started.
        /// </summary>
        protected override void ComponentStarted()
        {
            this.mStartTime = DateTime.Now;
            var lTimer = new Timer {Interval = this.UpdatePeriod.TotalMilliseconds};
            lTimer.Elapsed += this.OnTimerElapsed;
            lTimer.Enabled = true;
            lTimer.AutoReset = false;
        }

        /// <summary>
        ///     Called when [timer elapsed].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        private void OnTimerElapsed(object pSender, ElapsedEventArgs pEventArgs)
        {
            var lDiff = DateTime.Now - this.mStartTime;
            if (lDiff.TotalMilliseconds > this.Duration.TotalMilliseconds)
            {
                var lTimer = (Timer) pSender;
                lTimer.Enabled = false;
                lTimer.Elapsed -= this.OnTimerElapsed;
                this.Stop();
            }
            else
            {
                if (this.Ticked != null)
                {
                    this.Ticked();
                }
                var lTimer = (Timer) pSender;
                lTimer.Enabled = true;
            }
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            if (this.Ended != null)
            {
                this.Ended();
            }
        }

        #region Inputs

        [In]
        public TimeSpan UpdatePeriod
        {
            get;
            set;
        }

        [In]
        public TimeSpan Duration
        {
            get;
            set;
        }

        #endregion // Inputs

        #region Events

        public event ComponentReturnDelegate Ticked;

        public event ComponentReturnDelegate Ended;

        #endregion // Events.
    }
}