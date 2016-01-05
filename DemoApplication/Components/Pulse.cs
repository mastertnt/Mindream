using System;
using System.Timers;
using Mindream;
using Mindream.Attributes;
using Mindream.Components;

namespace DemoApplication.Components
{
    /// <summary>
    /// 
    /// </summary>
    [FunctionComponent]
    public class Pulse : AFunctionComponent
    {
        #region Fields

        /// <summary>
        /// The m start time
        /// </summary>
        private DateTime mStartTime;

        #endregion // Fields.

        #region Inputs

        /// <summary>
        /// Gets or sets the update period.
        /// </summary>
        /// <value>
        /// The update period.
        /// </value>
        [In]
        public TimeSpan UpdatePeriod { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        [In]
        public TimeSpan Duration { get; set; }

        #endregion // Inputs

        #region Events

        /// <summary>
        /// Occurs when [ticked].
        /// </summary>
        public event ComponentReturnDelegate Ticked;

        /// <summary>
        /// Occurs when [ended].
        /// </summary>
        public event ComponentReturnDelegate Ended;

        #endregion // Events.


        /// <summary>
        /// This method is called when the component is started.
        /// </summary>
        protected override void ComponentStarted()
        {
            this.mStartTime = DateTime.Now;
            Timer lTimer = new Timer { Interval = this.UpdatePeriod.TotalMilliseconds };
            lTimer.Elapsed += this.OnTimerElapsed;
            lTimer.Enabled = true;
            lTimer.AutoReset = false;
        }

        /// <summary>
        /// Called when [timer elapsed].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnTimerElapsed(object pSender, ElapsedEventArgs pEventArgs)
        {
            TimeSpan lDiff = DateTime.Now - this.mStartTime;
            if (lDiff.TotalMilliseconds > this.Duration.TotalMilliseconds)
            {
                Timer lTimer = (Timer)pSender;
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
                Timer lTimer = (Timer)pSender;
                lTimer.Enabled = true;
            }
        }

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            if (this.Ended != null)
            {
                this.Ended();
            }
        }
    }
}
