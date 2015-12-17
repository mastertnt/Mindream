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
        #region Fields

        /// <summary>
        /// The component
        /// </summary>
        private readonly IComponent mComponent;

        #endregion // Fields.

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNodeViewModel"/> class.
        /// </summary>
        public CallNodeViewModel(IComponent pComponent)
        {
            this.mComponent = pComponent;
            this.Ports.Add(new PortStartViewModel());
            this.Ports.Add(new PortSucceedViewModel());

            foreach (var lInput in pComponent.Descriptor.Inputs)
            {
                this.Ports.Add(new InputParameterViewModel(lInput));
            }

            foreach (var lOutput in pComponent.Descriptor.Outputs)
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
                return this.mComponent.Descriptor.Name;
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
                return this.mComponent.Descriptor.Name;
            }
            set
            {
                // Cannot be modified.
            }
        }
    }
}
