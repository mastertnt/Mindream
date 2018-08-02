using Mindream.Attributes;
using Mindream.Descriptors;
using System;
using System.ComponentModel;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    /// Do N times method component class definition.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class DoN : AMethodComponent
    {
        #region Inputs

        /// <summary>
        /// Gets or sets the do loop count.
        /// </summary>
        [In]
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current loop index.
        /// </summary>
        [Out]
        [Browsable(false)]
        public int LoopIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is updatable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is updatable; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public override bool IsUpdatable
        {
            get
            {
                return true;
            }
        }

        #endregion // Inputs.

        #region Events

        /// <summary>
        /// This event is raised when the loop must be evaluated.
        /// </summary>
        public event ComponentReturnDelegate DoLoop;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.LoopIndex = -1;
        }

        /// <summary>
        /// This method is called to update the component.
        /// </summary>
        /// <param name="pDeltaTime">The delta time.</param>
        public override void Update(TimeSpan pDeltaTime)
        {
            base.Update(pDeltaTime);
            this.TryDoLoop();
        }

        /// <summary>
        /// Tries to evaluate the loop.
        /// </summary>
        private void TryDoLoop()
        {
            if (this.State != TaskState.Stopped)
            {
                this.LoopIndex++;
                if (this.LoopIndex < this.Count)
                {
                    if (this.DoLoop != null)
                    {
                        this.DoLoop();
                    }
                }
                else
                {
                    this.Stop();
                    this.LoopIndex = -1;
                }
            }
        }

        #endregion // Methods.
    }
}