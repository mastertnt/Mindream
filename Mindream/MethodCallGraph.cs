using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Mindream
{
    public class MethodCallGraph
    {
        #region Properties

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

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallGraph"/> class.
        /// </summary>
        public MethodCallGraph()
        {
            this.CallNodes = new ObservableCollection<CallNode>();
        }

        #endregion // Constructors

        #region Methods

        /// <summary>
        /// Connects the call.
        /// </summary>
        /// <param name="pSource">The p source.</param>
        /// <param name="pTarget">The p target.</param>
        public void ConnectCall(CallNode pSource, CallNode pTarget)
        {
            pSource.NextNodes.Add(pTarget);
        }

        /// <summary>
        /// Disconnects the call.
        /// </summary>
        /// <param name="pSource">The p source.</param>
        /// <param name="pTarget">The p target.</param>
        public void DisconnectCall(CallNode pSource, CallNode pTarget)
        {
            pSource.NextNodes.Remove(pTarget);
        }
        public void ConnectParameter(CallNode pSource, CallNode pTarget, string pSourceName, string pTargetName)
        {
            
        }

        #endregion // Methods.
    }
}
