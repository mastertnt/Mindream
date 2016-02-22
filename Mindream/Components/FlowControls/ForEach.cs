using System.Collections;
using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.FlowControls
{
    /// <summary>
    ///     This class provides a way to iterate on IEnumerable.
    /// </summary>
    [FunctionComponent("Flow control")]
    public class ForEach : AFunctionComponent
    {
        #region Inputs

        /// <summary>
        ///     Gets or sets the enumerable.
        /// </summary>
        /// <value>
        ///     The enumerable.
        /// </value>
        [In]
        public IEnumerable Array
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the element.
        /// </summary>
        /// <value>
        ///     The element.
        /// </value>
        [Out]
        public object Element
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
            var lEnumerator = this.Array.GetEnumerator();
            while (lEnumerator.MoveNext())
            {
                this.Element = lEnumerator.Current;
                if (this.DoLoop != null)
                {
                    this.DoLoop();
                }
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