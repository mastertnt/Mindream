using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    ///     This class provides a way to iterate on an array.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class WhileLoop : AFunctionComponent
    {
        #region Inputs

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Branch" /> is condition.
        /// </summary>
        /// <value>
        ///     <c>true</c> if condition; otherwise, <c>false</c>.
        /// </value>
        [In]
        public bool Condition
        {
            get;
            set;
        }

        [Out]
        public int LoopIndex
        {
            get;
            set;
        }

        #endregion // Inputs

        #region Events

        /// <summary>
        ///     This event is raised when the loop must be evaluated.
        /// </summary>
        public event ComponentReturnDelegate DoLoop;

        /// <summary>
        ///     This event is raised when the loop is ended.
        /// </summary>
        public event ComponentReturnDelegate Ended;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStarted()
        {
            this.LoopIndex = 0;
            while (this.Condition)
            {
                if (this.DoLoop != null)
                {
                    this.DoLoop();
                }
                this.LoopIndex++;
            }

            this.Stop();
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

        #endregion // Methods
    }
}