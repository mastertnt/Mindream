using Mindream.CallGraph;
using Mindream.Components;
using Mindream.XGraph.Model;
using System.Windows.Media;
using XGraph.ViewModels;
using System.Linq;

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
        public CallNode Node
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        public override double X
        {
            get
            {
                LocatableCallNode lNode = this.Node as LocatableCallNode;
                if (lNode != null)
                {
                    return lNode.X;
                }

                return base.X;
            }
            set
            {
                LocatableCallNode lNode = this.Node as LocatableCallNode;
                if (lNode != null)
                {
                    lNode.X = value;
                }
                else
                {
                    base.X = value;
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
                LocatableCallNode lNode = this.Node as LocatableCallNode;
                if (lNode != null)
                {
                    return lNode.Y;
                }

                return base.Y;
            }
            set
            {
                LocatableCallNode lNode = this.Node as LocatableCallNode;
                if (lNode != null)
                {
                    lNode.Y = value;
                }
                else
                {
                    base.Y = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selection.
        /// </summary>
        public virtual object Selection
        {
            get
            {
                return this.Node.Component;
            }
        }

        /// <summary>
        /// Gets or sets the display string.
        /// </summary>
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

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNodeViewModel"/> class.
        /// </summary>
        public CallNodeViewModel(CallNode pNode)
        {
            this.Node = pNode;
            this.Node.Component.Started += this.OnComponentStarted;
            this.Node.Component.Stopped += this.OnComponentStopped;
            this.Node.Component.Aborted += this.OnComponentStopped;
            this.InitializePorts();
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Initializes the ports.
        /// </summary>
        protected virtual void InitializePorts()
        {
            if (this.Node.Component.Descriptor.IsOperator == false)
            {
                if (this.Node.Component.Descriptor.AdditionalStartPorts.Count != 0)
                {
                    foreach (string lPortName in this.Node.Component.Descriptor.AdditionalStartPorts)
                    {
                        this.Ports.Add(new PortStartViewModel(lPortName, lPortName));
                    }
                }
                else
                {
                    // Add the defaut port.
                    this.Ports.Add(new PortStartViewModel("Start", "Start"));
                }

            }

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
        /// Called when [component started].
        /// </summary>
        /// <param name="pComponent">The component.</param>
        /// <param name="pPortName">The name of the execution port to start</param>
        private void OnComponentStarted(IComponent pComponent, string pPortName)
        {
            this.IsActive = true;
            this.OnPropertyChanged("IsActive");
        }

        /// <summary>
        /// Called when [component stopped].
        /// </summary>
        /// <param name="pComponent">The component.</param>
        private void OnComponentStopped(IComponent pComponent)
        {
            this.IsActive = false;
            this.OnPropertyChanged("IsActive");
        }

        /// <summary>
        /// Callen when the call node must be validated (on connection or disconnection).
        /// </summary>
        protected internal virtual void Validate()
        {
        }

        #endregion // Methods.
    }
}
