using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Mindream;
using Mindream.Attributes;
using Mindream.Components;

namespace DemoApplication.Components
{
    [FunctionComponent]
    public class Pulse : AFunctionComponent
    {
        #region Fields

        private DateTime mStartTime;

        #endregion // Fields.

        #region Inputs

        [In]
        public TimeSpan UpdatePeriod { get; set; }

        [In]
        public TimeSpan Duration { get; set; }

        #endregion // Inputs

        #region Events

        public event ComponentReturnDelegate Ticked;

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
        ///     This method is called to start the component.
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
