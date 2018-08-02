using Mindream.CallGraph;

namespace Mindream.XGraph.Model
{
    /// <summary>
    /// A class used to have a position on a node.
    /// </summary>
    /// <seealso cref="Mindream.CallGraph.CallNode" />
    public class LocatableCallNode : CallNode
    {
        #region Properties

        /// <summary>
        /// Gets or sets a flag to know if the node is removable from the graph by the user.
        /// </summary>
        public bool IsRemovable
        {
            get;
            set;
        }

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

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocatableCallNode"/> class.
        /// </summary>
        public LocatableCallNode()
        {
            this.IsRemovable = true;
        }

        #endregion // Constructors.
    }
}
