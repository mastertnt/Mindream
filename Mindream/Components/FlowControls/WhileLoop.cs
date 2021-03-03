using Mindream.Attributes;
using Mindream.Descriptors;
using System;
using System.ComponentModel;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    /// This class provides a way to iterate on an array.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class WhileLoop : AComponent
    {
        #region Inputs

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Branch" /> is condition.
        /// </summary>
        /// <value>
        /// <c>true</c> if condition; otherwise, <c>false</c>.
        /// </value>
        [In]
        public bool Condition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the loop.
        /// </summary>
        /// <value>
        /// The index of the loop.
        /// </value>
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

        #endregion // Inputs

        #region Events

        /// <summary>
        /// This event is raised when the loop must be evaluated.
        /// </summary>
        public event ComponentReturnDelegate DoLoop;

        /// <summary>
        /// This event is raised when the loop is ended.
        /// </summary>
        public event ComponentReturnDelegate Ended;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.LoopIndex = 0;
            this.TryDoLoop(true);
        }

        /// <summary>
        /// This method is called to update the component.
        /// </summary>
        /// <param name="pDeltaTime">The delta time.</param>
        public override void Update(TimeSpan pDeltaTime)
        {
            base.Update(pDeltaTime);
            this.TryDoLoop(false);
        }

        /// <summary>
        /// Tries to evaluate the loop.
        /// </summary>
        /// <param name="pStarting">Flag indicating if the component is starting or not.</param>
        private void TryDoLoop(bool pStarting)
        {
            if (this.Condition)
            {
                if (pStarting == false)
                {
                    this.LoopIndex++;
                }

                if (this.DoLoop != null)
                {
                    this.DoLoop();
                }
            }
            else
            {
                this.Stop();
                this.LoopIndex = 0;
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

        #endregion // Methods
    }
}