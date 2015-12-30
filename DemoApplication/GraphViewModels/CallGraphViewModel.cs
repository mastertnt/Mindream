using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Mindream;
using Mindream.CallGraph;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
{
    /// <summary>
    /// This view model represents a call graph.
    /// </summary>
    public class CallGraphViewModel : GraphViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallGraphViewModel"/> class.
        /// </summary>
        /// <param name="pGraph">The p graph.</param>
        public CallGraphViewModel(MethodCallGraph pGraph)
        {
            pGraph.CallNodes.CollectionChanged += this.OnCallNodesChanged;

            // Create call nodes.
            foreach (var lCallNode in pGraph.CallNodes)
            {
                CallNodeViewModel lNodeViewModel = new CallNodeViewModel(lCallNode);
                this.AddNode(lNodeViewModel);
            }

            // Create connections for next nodes.
            foreach (var lSourceNode in pGraph.CallNodes)
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
                                    PortViewModel lTargetPortViewModel = lSourceViewModel.Ports.FirstOrDefault(pPort => pPort is PortStartViewModel);
                                    if (lTargetPortViewModel != null)
                                    {
                                        ConnectionViewModel lConnection = new ConnectionViewModel();
                                        lConnection.Output = lSourcePortViewModel;
                                        lConnection.Input = lTargetPortViewModel;
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
