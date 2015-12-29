using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This class represents a call node.
    /// </summary>
    /// <!-- NBY -->
    public class CallNode
    {
        /// <summary>
        /// Gets or sets the instance (for component based on instance).
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public IComponent Component
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the call nodes.
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public Dictionary<string, List<CallNode>> NextNodes
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNode"/> class.
        /// </summary>
        public CallNode()
        {
            this.NextNodes = new Dictionary<string, List<CallNode>>();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.Component.Returned += this.OnComponentReturned;
            this.Component.Start();
        }

        /// <summary>
        /// Called when [component succeed].
        /// </summary>
        /// <param name="pComponent">The component succeed.</param>
        /// <param name="pResultName">Name of the result.</param>
        private void OnComponentReturned(IComponent pComponent, string pResultName)
        {
            this.Component.Returned -= this.OnComponentReturned;
            if (this.NextNodes.ContainsKey(pResultName))
            {
                foreach (var lNode in this.NextNodes[pResultName])
                {
                    // Now, pass the current output to the inputs.

                    lNode.Start();
                }
            }
        }
    }
}
