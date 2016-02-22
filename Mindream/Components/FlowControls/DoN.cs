﻿using Mindream.Attributes;
using Mindream.Descriptors;

namespace Mindream.Components.FlowControls
{
    [FunctionComponent("Flow control")]
    public class DoN : AMethodComponent
    {
        #region Events

        /// <summary>
        ///     This event is raised when the loop must be evaluated.
        /// </summary>
        public event ComponentReturnDelegate DoLoop;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStarted()
        {
            this.LoopIndex = 0;
            for (this.LoopIndex = 0; this.LoopIndex < this.Count; this.LoopIndex++)
            {
                if (this.DoLoop != null)
                {
                    this.DoLoop();
                }
            }

            this.Stop();
        }

        #endregion // Methods

        #region Inputs

        [In]
        public int Count
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
    }
}