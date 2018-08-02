using System.Collections;
using Mindream.Attributes;
using Mindream.Descriptors;
using System.ComponentModel;
using System;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    /// This class provides a way to iterate on IEnumerable.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class ForEach : AComponent
    {
        #region Fields

        /// <summary>
        /// Stores the array enumerator.
        /// </summary>
        private IEnumerator mEnumerator;

        #endregion // Fields.

        #region Inputs

        /// <summary>
        /// Gets or sets the enumerable.
        /// </summary>
        /// <value>
        /// The enumerable.
        /// </value>
        [In]
        public IEnumerable Array
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        [Out]
        public object Element
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
            if (this.Array != null)
            {
                this.mEnumerator = this.Array.GetEnumerator();
            }
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
                if (this.mEnumerator != null && this.mEnumerator.MoveNext())
                {
                    this.Element = this.mEnumerator.Current;
                    if (this.DoLoop != null)
                    {
                        this.DoLoop();
                    }
                }
                else
                {
                    if (this.Array != null)
                    {
                        this.mEnumerator = this.Array.GetEnumerator();
                    }

                    this.Stop();
                }
            }
        }

        /// <summary>
        /// This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            this.mEnumerator = null;
            if (this.Ended != null)
            {
                this.Ended();
            }
        }

        #endregion // Methods.
    }
}