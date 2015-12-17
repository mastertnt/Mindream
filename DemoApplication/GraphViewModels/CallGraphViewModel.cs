using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindream;
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
