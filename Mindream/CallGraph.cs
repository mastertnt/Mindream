using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Mindream
{
    public class CallGraph
    {
        /// <summary>
        /// Gets or sets the call nodes.
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public ObservableCollection<IComponent> CallNodes
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallGraph"/> class.
        /// </summary>
        public CallGraph()
        {
            this.CallNodes = new ObservableCollection<IComponent>();
        }
    }
}
