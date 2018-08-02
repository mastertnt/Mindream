using System.Collections.Specialized;
using System.Linq;
using Mindream.CallGraph;
using Mindream.XGraph.Model;
using XGraph.ViewModels;
using System.Windows.Media;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This view model represents a call graph.
    /// </summary>
    public class TaskViewModel : AGraphViewModel
    {
        #region Properties

        /// <summary>
        /// This field stores a graph node.
        /// </summary>
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        public Task Graph
        {
            get;
            private set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskViewModel"/> class.
        /// </summary>
        /// <param name="pGraph">The p graph.</param>
        public TaskViewModel(Task pGraph)
        {
            this.Graph = pGraph;
            pGraph.CallNodes.CollectionChanged += this.OnCallNodesChanged;
            pGraph.CallConnected += this.OnCallConnected;
            pGraph.CallDisconnected += this.OnCallDisconnected;
            pGraph.ParameterConnected += this.OnParameterConnected;
            pGraph.ParameterDisconnected += this.OnParameterDisconnected;

            this.InitializeNodes();
            this.InitializeCall();
            this.InitializeParameterConnections();
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Called when [call nodes changed].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCallNodesChanged(object pSender, NotifyCollectionChangedEventArgs pEventArgs)
        {
            switch (pEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var lNewObject in pEventArgs.NewItems)
                        {
                            CallNodeViewModel lNodeViewModel = new CallNodeViewModel(lNewObject as CallNode);
                            this.AddNode(lNodeViewModel);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var lOldObject in pEventArgs.OldItems)
                        {
                            CallNodeViewModel lNodeViewModel = this.Nodes.Cast<CallNodeViewModel>().FirstOrDefault(pNode => pNode.Node == lOldObject);
                            this.RemoveNode(lNodeViewModel);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Initializes the nodes.
        /// </summary>
        protected virtual void InitializeNodes()
        {
            // Create call nodes.
            foreach (var lCallNode in this.Graph.CallNodes)
            {
                CallNodeViewModel lNodeViewModel = new CallNodeViewModel(lCallNode);
                this.AddNode(lNodeViewModel);
                var lLocatableCallNode = lCallNode as LocatableCallNode;
                if (lLocatableCallNode != null)
                {
                    lNodeViewModel.X = lLocatableCallNode.X;
                    lNodeViewModel.Y = lLocatableCallNode.Y;
                }
                lNodeViewModel.Validate();
            }
        }

        /// <summary>
        /// Initializes the call.
        /// </summary>
        protected virtual void InitializeCall()
        {
            foreach (var lSourceNode in this.Graph.CallNodes)
            {
                CallNodeViewModel lSourceViewModel = this.Nodes.Cast<CallNodeViewModel>().FirstOrDefault(pNode => pNode.Node == lSourceNode);
                if (lSourceViewModel != null)
                {
                    foreach (var lConnectedNode in lSourceNode.NodeToCall)
                    {
                        PortViewModel lSourcePortViewModel = lSourceViewModel.Ports.FirstOrDefault(pPort => pPort.Id == lConnectedNode.Key && pPort.Direction == PortDirection.Output);
                        if (lSourcePortViewModel != null)
                        {
                            foreach (var lTargetNode in lConnectedNode.Value)
                            {
                                CallNodeViewModel lTargetViewModel = this.Nodes.Cast<CallNodeViewModel>().FirstOrDefault(pNode => pNode.Node == lTargetNode.NextNode);
                                if (lTargetViewModel != null)
                                {
                                    PortViewModel lTargetPortViewModel = lTargetViewModel.Ports.FirstOrDefault(pPort => pPort is PortStartViewModel && lTargetNode.StartPort == pPort.Id);
                                    if (lSourcePortViewModel != null)
                                    {
                                        ConnectionLinkViewModel lConnection = new ConnectionLinkViewModel { Output = lSourcePortViewModel, Input = lTargetPortViewModel };
                                        lTargetViewModel.OnPropertyChanged("IsActive");
                                        this.AddConnection(lConnection);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }

        /// <summary>
        /// Initializes the parameter connections.
        /// </summary>
        protected virtual void InitializeParameterConnections()
        {
            foreach (var lSourceNode in this.Graph.CallNodes)
            {
                CallNodeViewModel lSourceViewModel = this.Nodes.Cast<CallNodeViewModel>().FirstOrDefault(pNode => pNode.Node == lSourceNode);

                if (lSourceViewModel != null)
                {
                    foreach (var lConnectedParameters in lSourceNode.NodeParameters)
                    {
                        CallNodeViewModel lTargetViewModel = this.Nodes.Cast<CallNodeViewModel>().FirstOrDefault(pNode => pNode.Node == lConnectedParameters.Key);

                        if (lTargetViewModel != null)
                        {
                            foreach (var lConnections in lConnectedParameters.Value)
                            {
                                PortViewModel lSourcePortViewModel = lSourceViewModel.Ports.FirstOrDefault(pPort => pPort.Id == lConnections.Key && pPort.Direction == PortDirection.Output);
                                var lSourceInfo = lSourceViewModel.Node.Component.Descriptor.Outputs.FirstOrDefault(pPort => pPort.Name == lSourcePortViewModel.Id);
                                lSourcePortViewModel.PortType = lSourceInfo.Type.ToString();
                                if (lSourcePortViewModel != null)
                                {
                                    foreach (var lTarget in lConnections.Value)
                                    {
                                        PortViewModel lTargetPortViewModel = lTargetViewModel.Ports.FirstOrDefault(pPort => pPort.Id == lTarget && pPort.Direction == PortDirection.Input);
                                        var lTargetInfo = lTargetViewModel.Node.Component.Descriptor.Inputs.FirstOrDefault(pPort => pPort.Name == lTargetPortViewModel.Id);
                                        lTargetPortViewModel.PortType = lTargetInfo.Type.ToString();
                                        if (lTargetPortViewModel != null)
                                        {
                                            ConnectionLinkViewModel lConnection = new ConnectionLinkViewModel { Output = lSourcePortViewModel, Input = lTargetPortViewModel };
                                            lTargetViewModel.OnPropertyChanged("IsActive");
                                            this.AddConnection(lConnection);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fires the request connection creation.
        /// </summary>
        /// <param name="pOutput">The output.</param>
        /// <param name="pInput">The input.</param>
        protected override void RequestConnectionCreation(PortViewModel pOutput, PortViewModel pInput)
        {
            var lSourceNode = pOutput.ParentNode as CallNodeViewModel;
            var lTargetNode = pInput.ParentNode as CallNodeViewModel;

            // Create a call connection in the model.
            if (lSourceNode != null && lTargetNode != null && pInput is PortStartViewModel && pOutput is PortEndedViewModel)
            {
                if (lTargetNode.Node.PreviousCallNodes.Count < lTargetNode.Node.Component.MaxStartCount)
                {
                    this.Graph.ConnectCall(lSourceNode.Node, pOutput.Id, lTargetNode.Node, pInput.Id);
                }              
            }

            // Create a parameter connection in the model.
            if (lSourceNode != null && lTargetNode != null && pInput is InputParameterViewModel && pOutput is OutputParameterViewModel)
            {
                this.Graph.ConnectParameter(lSourceNode.Node, pOutput.Id, lTargetNode.Node, pInput.Id);
            }
        }

        /// <summary>
        /// Fires the request connection creation.
        /// </summary>
        /// <param name="pConnectionToRemove">The connection to remove.</param>
        protected override void RequestConnectionRemoval(ConnectionViewModel pConnectionToRemove)
        {
            var lSourceNode = pConnectionToRemove.Output.ParentNode as CallNodeViewModel;
            var lTargetNode = pConnectionToRemove.Input.ParentNode as CallNodeViewModel;

            // Remove a call connection from the model.
            if (lSourceNode != null && lTargetNode != null && pConnectionToRemove.Input is PortStartViewModel && pConnectionToRemove.Output is PortEndedViewModel)
            {
                this.Graph.DisconnectCall(lSourceNode.Node, pConnectionToRemove.Output.Id, lTargetNode.Node, pConnectionToRemove.Input.Id);
            }

            // Remove a parameter connection from the model.
            if (lSourceNode != null && lTargetNode != null && pConnectionToRemove.Input is InputParameterViewModel && pConnectionToRemove.Output is OutputParameterViewModel)
            {
                this.Graph.DisconnectParameter(lSourceNode.Node, pConnectionToRemove.Output.Id, lTargetNode.Node, pConnectionToRemove.Input.Id);
            }
        }

        /// <summary>
        /// Called when [parameter disconnected].
        /// </summary>
        /// <param name="pEventSender">The p event sender.</param>
        /// <param name="pSource">The p source.</param>
        /// <param name="pOutput">The p output.</param>
        /// <param name="pTarget">The p target.</param>
        /// <param name="pInput">The p input.</param>
        protected virtual void OnParameterDisconnected(Task pEventSender, CallNode pSource, string pOutput, CallNode pTarget, string pInput)
        {
            var lSourceNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pSource);
            CallNodeViewModel lTargetNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pTarget) as CallNodeViewModel;
            var lOuputPort = lSourceNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Output && pPort is OutputParameterViewModel && (pPort as OutputParameterViewModel).Id == pOutput);
            var lInputPort = lTargetNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Input && pPort is InputParameterViewModel && (pPort as InputParameterViewModel).Id == pInput);

            ConnectionViewModel lConnection = this.Connections.FirstOrDefault(pConnection => pConnection.Input == lInputPort && pConnection.Output == lOuputPort);
            this.RemoveConnection(lConnection);

            lTargetNode.OnPropertyChanged("IsActive");
            lTargetNode.Validate();
        }

        /// <summary>
        /// Called when [parameter connected].
        /// </summary>
        /// <param name="pEventSender">The p event sender.</param>
        /// <param name="pSource">The source.</param>
        /// <param name="pOutput">The output.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pInput">The input.</param>
        protected virtual void OnParameterConnected(Task pEventSender, CallNode pSource, string pOutput, CallNode pTarget, string pInput)
        {
            var lSourceNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pSource);
            CallNodeViewModel lTargetNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pTarget) as CallNodeViewModel;

            var lSourceInfo = (pSource.Component.Descriptor.Outputs.FirstOrDefault(pMember => pMember.Name.Equals(pOutput)));
            var lTargetInfo = (pTarget.Component.Descriptor.Inputs.FirstOrDefault(pMember => pMember.Name.Equals(pInput)));

            PortViewModel lOuputPort = lSourceNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Output && pPort is OutputParameterViewModel && (pPort as OutputParameterViewModel).Id == pOutput);
            PortViewModel lInputPort = lTargetNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Input && pPort is InputParameterViewModel && (pPort as InputParameterViewModel).Id == pInput);

            lOuputPort.PortType = lSourceInfo.Type.ToString();
            lInputPort.PortType = lTargetInfo.Type.ToString();

            ConnectionLinkViewModel lViewModel = new ConnectionLinkViewModel
            {
                Input = lInputPort,
                Output = lOuputPort
            };

            this.AddConnection(lViewModel);
            lTargetNode.OnPropertyChanged("IsActive");
            lTargetNode.Validate();
        }

        /// <summary>
        /// Called when [call disconnected].
        /// </summary>
        /// <param name="pEventSender">The event sender.</param>
        /// <param name="pSource">The source.</param>
        /// <param name="pOutput">The output.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pStartPort">The start port.</param>
        protected virtual void OnCallDisconnected(Task pEventSender, CallNode pSource, string pOutput, CallNode pTarget, string pStartPort)
        {
            var lSourceNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pSource);
            CallNodeViewModel lTargetNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pTarget) as CallNodeViewModel;
            var lOuputPort = lSourceNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Output && (pPort as PortEndedViewModel).Id == pOutput);
            var lInputPort = lTargetNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Input && (pPort as PortStartViewModel).Id == pStartPort);

            ConnectionViewModel lConnection = this.Connections.FirstOrDefault(pConnection => pConnection.Input == lInputPort && pConnection.Output == lOuputPort);
            this.RemoveConnection(lConnection);
            lTargetNode.OnPropertyChanged("IsActive");
            lTargetNode.Validate();
        }

        /// <summary>
        /// Called when [call connected].
        /// </summary>
        /// <param name="pEventSender">The event sender.</param>
        /// <param name="pSource">The source.</param>
        /// <param name="pOutput">The output.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pStartPort">The start port.</param>
        protected virtual void OnCallConnected(Task pEventSender, CallNode pSource, string pOutput, CallNode pTarget, string pStartPort)
        {
            var lSourceNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pSource);
            CallNodeViewModel lTargetNode = this.Nodes.FirstOrDefault(pNode => (pNode as CallNodeViewModel).Node == pTarget) as CallNodeViewModel;
            var lOuputPort = lSourceNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Output && (pPort as PortEndedViewModel).Id == pOutput);
            var lInputPort = lTargetNode.Ports.FirstOrDefault(pPort => pPort.Direction == PortDirection.Input && (pPort as PortStartViewModel).Id == pStartPort);

            ConnectionLinkViewModel lViewModel = new ConnectionLinkViewModel
            {
                Input = lInputPort,
                Output = lOuputPort,
            };
            
            this.AddConnection(lViewModel);
            lTargetNode.OnPropertyChanged("IsActive");
            lTargetNode.Validate();
        }

        /// <summary>
        /// Validate a node.
        /// </summary>
        /// <param name="pNodeViewModel"> Node to validate</param>
        protected void Validate(CallNodeViewModel pNodeViewModel)
        {
            pNodeViewModel.Validate();
        }

        #endregion // Methods.
    }
}
