using System;
using System.Timers;
using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.Timers
{
    /// <summary>
    ///     This enumeration enumerates all the states of the simulation.
    /// </summary>
    public enum SimulationState
    {
        Running,
        Freeze,
        Stopped
    }

    /// <summary>
    ///     This enumeration enumerates all the states of the simulation.
    /// </summary>
    internal enum SimulationAction
    {
        None,
        AskPlay,
        AskPause,
        AskStop,
        AskResume
    }

    /// <summary>
    ///     This component is a singleton to behave like a simulation engine which can be started/paused/resumed/stopped.
    /// </summary>
    /// <seealso cref="Mindream.Components.AFunctionComponent" />
    [FunctionComponent("Timing")]
    public class Simulation : ASingletonFunctionComponent
    {
        #region Fields

        /// <summary>
        /// The next action to execute.
        /// </summary>
        private SimulationAction mAction;

        #endregion // Fields.

        #region Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Simulation Instance
        {
            get;
            protected set;
        }

        /// <summary>
        ///     Gets or sets the update period.
        /// </summary>
        /// <value>
        ///     The update period.
        /// </value>
        [In]
        public TimeSpan UpdatePeriod
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the initial date.
        /// </summary>
        /// <value>
        ///     The initial date.
        /// </value>
        [In]
        public DateTime InitialDate
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the start date.
        /// </summary>
        /// <value>
        ///     The start date.
        /// </value>
        [Out]
        public DateTime AbsoluteDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time elapsed.
        /// </summary>
        /// <value>
        /// The time elapsed.
        /// </value>
        [Out]
        public TimeSpan TimeElapsed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delta time.
        /// </summary>
        /// <value>
        /// The delta time.
        /// </value>
        [Out]
        public TimeSpan DeltaTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the tick.
        /// </summary>
        /// <value>
        /// The index of the tick.
        /// </value>
        [Out]
        public int TickIndex
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        [Out]
        public SimulationState State
        {
            get;
            set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="Simulation" /> class.
        /// </summary>
        static Simulation()
        {
            Instance = new Simulation();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Simulation" /> class.
        /// </summary>
        protected Simulation()
        {
            this.UpdatePeriod = new TimeSpan(0, 0, 1);
        }

        #endregion // Constructors.

        #region Events

        /// <summary>
        ///     Occurs when [ticked].
        /// </summary>
        public event ComponentReturnDelegate Ticked;

        /// <summary>
        ///     Occurs when [finalized].
        /// </summary>
        public event ComponentReturnDelegate Ended;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     Plays the simulation
        /// </summary>
        public void StopSim()
        {
            this.mAction = SimulationAction.AskStop;
        }

        /// <summary>
        ///     This method is called when the component is initialized.
        /// </summary>
        protected override void ComponentInitilialized()
        {
            base.ComponentInitilialized();
            this.InitialDate = DateTime.Now;
        }

        /// <summary>
        ///     This method is called when the component is started.
        /// </summary>
        protected override void ComponentStarted()
        {
            base.ComponentStarted();
            this.mAction = SimulationAction.None;
            this.State = SimulationState.Running;
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
            if (this.mAction == SimulationAction.AskStop)
            {
                this.mAction = SimulationAction.None;
                var lTimer = (Timer) pSender;
                lTimer.Enabled = false;
                lTimer.Elapsed -= this.OnTimerElapsed;
                this.Stop();
            }
            else
            {
                this.DeltaTime = this.UpdatePeriod;
                this.TickIndex++;
                double lTotalMilliseconds = this.TickIndex*this.UpdatePeriod.TotalMilliseconds;
                this.AbsoluteDate = this.InitialDate.AddMilliseconds(lTotalMilliseconds);
                this.TimeElapsed = new TimeSpan(0, 0, 0, 0, (int)lTotalMilliseconds);
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

        #endregion // Methods.
    }
}