using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindream;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
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

        #endregion // Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNodeViewModel"/> class.
        /// </summary>
        public CallNodeViewModel(CallNode pNode)
        {
            this.Node = pNode;
            this.Ports.Add(new PortStartViewModel());
            this.Ports.Add(new PortSucceedViewModel());

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
                return this.Node.Component.Descriptor.Name;
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
                return this.Node.Component.Descriptor.Name;
            }
            set
            {
                // Cannot be modified.
            }
        }
    }
}
