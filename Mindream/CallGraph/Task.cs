using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Mindream.CallGraph
{
    /// <summary>
    /// </summary>
    public class Task
    {
        #region Fields

        /// <summary>
        /// The list of entry nodes.
        /// </summary>
        private readonly List<CallNode> mEntryNodes = new List<CallNode>(); 

        #endregion // Fields.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Task" /> class.
        /// </summary>
        internal Task()
        {
            this.CallNodes = new ObservableCollection<CallNode>();
            this.CallNodes.CollectionChanged += this.OnCallNodesOnCollectionChanged;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        ///     Gets or sets the call nodes.
        /// </summary>
        /// <value>
        ///     The call nodes.
        /// </value>
        public ObservableCollection<CallNode> CallNodes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entry nodes.
        /// </summary>
        /// <value>
        /// The entry nodes.
        /// </value>
        internal CallNode EntryNode
        {
            get
            {
                return this.mEntryNodes.FirstOrDefault();
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        ///     Connects the call.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pResultName">Id of the result.</param>
        public void ConnectCall(CallNode pSource, CallNode pTarget, string pResultName)
        {
            if (pSource.NodeToCall.ContainsKey(pResultName) == false)
            {
                pSource.NodeToCall[pResultName] = new List<CallNode>();
            }
            pSource.NodeToCall[pResultName].Add(pTarget);
            this.mEntryNodes.Remove(pTarget);
        }

        /// <summary>
        ///     Disconnects the call.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pTarget">The target.</param>
        public void DisconnectCall(CallNode pSource, CallNode pTarget)
        {
            //pSource.NodeToCall.Remove(pTarget);
        }

        /// <summary>
        ///     Connects an output of the source to the input of the target.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pSourceName">The name of the output.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pTargetName">The name of the input.</param>
        public void ConnectParameter(CallNode pSource, string pSourceName, CallNode pTarget, string pTargetName)
        {
            if (pSource.NodeParameters.ContainsKey(pTarget) == false)
            {
                pSource.NodeParameters[pTarget] = new Dictionary<string, List<string>>();
            }
            if (pSource.NodeParameters[pTarget].ContainsKey(pSourceName) == false)
            {
                pSource.NodeParameters[pTarget][pSourceName] = new List<string>();
            }
            pSource.NodeParameters[pTarget][pSourceName].Add(pTargetName);
        }

        /// <summary>
        /// Called when the call nodes collection has been modified.
        /// </summary>
        /// <param name="pSender">The sender.</param>
        /// <param name="pNotifyCollectionChangedEventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCallNodesOnCollectionChanged(object pSender, NotifyCollectionChangedEventArgs pNotifyCollectionChangedEventArgs)
        {
            switch (pNotifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (var lNewItem in pNotifyCollectionChangedEventArgs.NewItems)
                    {
                        this.mEntryNodes.Add(lNewItem as CallNode);
                    }
                }
                break;

                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (var lOldItem in pNotifyCollectionChangedEventArgs.OldItems)
                    {
                        this.mEntryNodes.Remove(lOldItem as CallNode);
                    }
                }
                break;

                case NotifyCollectionChangedAction.Reset:
                {
                    this.mEntryNodes.Clear();
                }
                break;
            }
        }

        #endregion // Methods.
    }
}