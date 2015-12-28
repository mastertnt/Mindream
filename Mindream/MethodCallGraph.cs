using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// 
    /// </summary>
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
        /// <param name="pSource">The source.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pResultName">Name of the result.</param>
        public void ConnectCall(CallNode pSource, CallNode pTarget, string pResultName)
        {
            if (pSource.NextNodes.ContainsKey(pResultName) == false)
            {
                pSource.NextNodes[pResultName] = new List<CallNode>();
            }
            pSource.NextNodes[pResultName].Add(pTarget);
        }

        /// <summary>
        /// Disconnects the call.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pTarget">The target.</param>
        public void DisconnectCall(CallNode pSource, CallNode pTarget)
        {
            //pSource.NextNodes.Remove(pTarget);
        }
        public void ConnectParameter(CallNode pSource, CallNode pTarget, string pSourceName, string pTargetName)
        {
            
        }

        #endregion // Methods.
    }
}
