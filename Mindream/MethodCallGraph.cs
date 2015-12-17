using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Mindream
{
    public class MethodCallGraph
    {
        /// <summary>
        /// Gets or sets the call nodes.
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public ObservableCollection<CallNode> CallNodes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first node.
        /// </summary>
        /// <value>
        /// The first node.
        /// </value>
        public CallNode FirstNode
        {
            get; set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallGraph"/> class.
        /// </summary>
        public MethodCallGraph()
        {
            this.CallNodes = new ObservableCollection<CallNode>();
        }
    }
}
