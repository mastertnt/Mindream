namespace Mindream.CallGraph
{
    /// <summary>
    /// This class represents an execution connection.
    /// </summary>
    public class ExecutionCall
    {
        /// <summary>
        /// Gets or sets the start port.
        /// </summary>
        /// <value>
        /// The start port.
        /// </value>
        public string StartPort
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the node to call.
        /// </summary>
        /// <value>
        /// The node to call.
        /// </value>
        public CallNode NextNode
        {
            get;
            set;
        }
    }
}
