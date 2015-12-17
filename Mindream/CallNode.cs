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
        public ObservableCollection<CallNode> NextNodes
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNode"/> class.
        /// </summary>
        public CallNode()
        {
            this.NextNodes = new ObservableCollection<CallNode>();
        }
    }
}
