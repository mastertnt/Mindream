using Mindream.CallGraph;
using Mindream.XGraph.Model;
using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This class represents a call node view model.
    /// </summary>
    public class CallNodeViewModel : NodeViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public CallNode Node
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        public override double X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
                var lNode = this.Node as LocatableCallNode;
                if (lNode != null)
                {
                    lNode.X = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        public override double Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
                var lNode = this.Node as LocatableCallNode;
                if (lNode != null)
                {
                    lNode.Y = value;
                }
            }
        }

        #endregion // Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNodeViewModel"/> class.
        /// </summary>
        public CallNodeViewModel(CallNode pNode)
        {
            this.Node = pNode;
            this.Ports.Add(new PortStartViewModel());

            foreach (var lResult in this.Node.Component.Descriptor.Results)
            {
                this.Ports.Add(new PortEndedViewModel(lResult));
            }

            foreach (var lInput in this.Node.Component.Descriptor.Inputs)
            {
                this.Ports.Add(new InputParameterViewModel(lInput));
            }

            foreach (var lOutput in this.Node.Component.Descriptor.Outputs)
            {
                this.Ports.Add(new OutputParameterViewModel(lOutput));
            }
        }

        /// <summary>
        /// Gets or sets the display string.
        /// </summary>
        /// <value>
        /// The display string.
        /// </value>
        public override string DisplayString
        {
            get
            {
                return this.Node.Component.Descriptor.Id;
            }
            set
            {
                // Cannot be modified.
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public override string Description
        {
            get
            {
                return this.Node.Component.Descriptor.Id;
            }
            set
            {
                // Cannot be modified.
            }
        }
    }
}
