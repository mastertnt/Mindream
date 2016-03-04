using System.Collections.Specialized;
using System.Linq;
using Mindream.CallGraph;
using Mindream.XGraph.Model;
using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This view model represents a call graph.
    /// </summary>
    public class CallGraphViewModel : GraphViewModel
    {
        private MethodCallGraph mGraph;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallGraphViewModel"/> class.
        /// </summary>
        /// <param name="pGraph">The p graph.</param>
        public CallGraphViewModel(MethodCallGraph pGraph)
        {
            this.mGraph = pGraph;
            pGraph.CallNodes.CollectionChanged += this.OnCallNodesChanged;
            // Create call nodes.
            foreach (var lCallNode in this.mGraph.CallNodes)
            {
                CallNodeViewModel lNodeViewModel = new CallNodeViewModel(lCallNode);
                this.AddNode(lNodeViewModel);
                var lLocatableCallNode = lCallNode as LocatableCallNode;
                if (lLocatableCallNode != null)
                {
                    lNodeViewModel.X = lLocatableCallNode.X;
                    lNodeViewModel.Y = lLocatableCallNode.Y;
                }
            }

            // Create connections for next nodes.
            foreach (var lSourceNode in this.mGraph.CallNodes)
            {
                CallNodeViewModel lSourceViewModel = this.Nodes.Cast<CallNodeViewModel>().FirstOrDefault(pNode => pNode.Node == lSourceNode);
                if (lSourceViewModel != null)
                {
                    foreach (var lConnectedNode in lSourceNode.NodeToCall)
                    {
                        PortViewModel lSourcePortViewModel = lSourceViewModel.Ports.FirstOrDefault(pPort => pPort.DisplayString == lConnectedNode.Key && pPort.Direction == PortDirection.Output);
                        if (lSourcePortViewModel != null)
                        {
                            foreach (var lTargetNode in lConnectedNode.Value)
                            {
                                CallNodeViewModel lTargetViewModel = this.Nodes.Cast<CallNodeViewModel>().FirstOrDefault(pNode => pNode.Node == lTargetNode);
                                if (lTargetViewModel != null)
                                {
                                    PortViewModel lTargetPortViewModel = lTargetViewModel.Ports.FirstOrDefault(pPort => pPort is PortStartViewModel);
                                    if (lSourcePortViewModel != null)
                                    {
                                        ConnectionViewModel lConnection = new ConnectionViewModel { Output = lSourcePortViewModel, Input = lTargetPortViewModel };
                                        this.AddConnection(lConnection);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Create connections for parameters.
            foreach (var lSourceNode in this.mGraph.CallNodes)
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
                                PortViewModel lSourcePortViewModel = lSourceViewModel.Ports.FirstOrDefault(pPort => pPort.DisplayString == lConnections.Key && pPort.Direction == PortDirection.Output);
                                if (lSourcePortViewModel != null)
                                {
                                    foreach (var lTarget in lConnections.Value)
                                    {
                                        PortViewModel lTargetPortViewModel = lTargetViewModel.Ports.FirstOrDefault(pPort => pPort.DisplayString == lTarget && pPort.Direction == PortDirection.Input);
                                        if (lTargetPortViewModel != null)
                                        {
                                            ConnectionViewModel lConnection = new ConnectionViewModel { Output = lSourcePortViewModel, Input = lTargetPortViewModel };
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
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            
        }

        /// <summary>
        /// Called when [call nodes changed].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCallNodesChanged(object pSender, NotifyCollectionChangedEventArgs pEventArgs)
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

                }
                break;
            }
        }

    }
}
