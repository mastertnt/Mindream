using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindream.CallGraph;

namespace Mindream.XGraph.Model
{
    public class LocatableCallNode : CallNode
    {
        /// <summary>
        /// Gets or sets the X position of the node.
        /// </summary>
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y position of the node.
        /// </summary>
        public double Y
        {
            get;
            set;
        }
    }
}
