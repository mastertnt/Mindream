using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Mindream.CallGraph
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
        /// <param name="pResultName">Id of the result.</param>
        public void ConnectCall(CallNode pSource, CallNode pTarget, string pResultName)
        {
            if (pSource.NodeToCall.ContainsKey(pResultName) == false)
            {
                pSource.NodeToCall[pResultName] = new List<CallNode>();
            }
            pSource.NodeToCall[pResultName].Add(pTarget);
        }

        /// <summary>
        /// Disconnects the call.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pTarget">The target.</param>
        public void DisconnectCall(CallNode pSource, CallNode pTarget)
        {
            //pSource.NodeToCall.Remove(pTarget);
        }

        /// <summary>
        /// Connects an output of the source to the input of the target.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pSourceName">The name of the output.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pTargetName">The name of the input.</param>
        public void ConnectParameter(CallNode pSource, string pSourceName, CallNode pTarget, string pTargetName)
        {
            if (pSource.NodeParameters.ContainsKey(pTarget) == false)
            {
                pSource.NodeParameters[pTarget] = new Dictionary<string, List<string>>();            
            }
            if (pSource.NodeParameters[pTarget].ContainsKey(pSourceName) == false)
            {
                pSource.NodeParameters[pTarget][pSourceName] = new List<string>();
            }
            pSource.NodeParameters[pTarget][pSourceName].Add(pTargetName);
        }

        #endregion // Methods.
    }
}
