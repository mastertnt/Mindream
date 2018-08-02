using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Mindream.CallGraph
{
    /// <summary>
    /// </summary>
    public class Task : IDisposable
    {
        #region Fields

        /// <summary>
        /// This field stores the call nodes.
        /// </summary>
        private ObservableCollection<CallNode> mCallNodes;

        #endregion // Fields.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Task" /> class.
        /// </summary>
        internal Task()
        {
            this.CallNodes = new ObservableCollection<CallNode>();
        }

        #endregion // Constructors

        #region Events

        /// <summary>
        /// Notify a property change.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify the adding of a CallNode to this Task.
        /// </summary>
        public event Action<CallNode> CallNodeAdded;

        /// <summary>
        /// Notify the removing of a CallNode from this Task.
        /// </summary>
        public event Action<CallNode> CallNodeRemoved;

        /// <summary>
        /// Occurs when [call connected].
        /// </summary>
        public event Action<Task, CallNode, string, CallNode, string> CallConnected;

        /// <summary>
        /// Occurs when [call disconnected].
        /// </summary>
        public event Action<Task, CallNode, string, CallNode, string> CallDisconnected;

        /// <summary>
        /// Occurs when [parameter connected].
        /// </summary>
        public event Action<Task, CallNode, string, CallNode, string> ParameterConnected;

        /// <summary>
        /// Occurs when [parameter disconnected].
        /// </summary>
        public event Action<Task, CallNode, string, CallNode, string> ParameterDisconnected;


        #endregion // Events.

        #region Properties

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public string Id
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets or sets the call nodes.
        /// </summary>
        /// <value>
        ///     The call nodes.
        /// </value>
        public ObservableCollection<CallNode> CallNodes
        {
            get
            {
                return this.mCallNodes;
            }
            set
            {
                if (this.mCallNodes != null)
                {
                    this.mCallNodes.CollectionChanged -= this.OnCollectionChanged;
                }

                this.mCallNodes = value;

                if (this.mCallNodes != null)
                {
                    this.mCallNodes.CollectionChanged += this.OnCollectionChanged;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the entry nodes.
        /// </summary>
        /// <value>
        ///     The entry nodes.
        /// </value>
        public List<CallNode> EntryNodes
        {
            get
            {
                List<CallNode> lResult = new List<CallNode>();
                foreach (CallNode lCallNode in this.CallNodes)
                {
                    if (lCallNode.PreviousCallNodes.Count == 0 && lCallNode.Component.Descriptor.IsOperator == false)
                    {
                        lResult.Add(lCallNode);
                    }
                }
                return lResult;
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update(TimeSpan pDeltaTime, int pSimulationStep)
        {
            foreach (var lCallNode in this.CallNodes)
            {
                if (lCallNode.Component.IsUpdatable && lCallNode.Component.State == TaskState.Running)
                {
                    lCallNode.Update(pDeltaTime, pSimulationStep);
                }
                else if (lCallNode.State == CallNodeState.WaitingForStart)
                {
                    lCallNode.StartPending(pSimulationStep);
                }
            }
        }

        /// <summary>
        ///     Connects the call.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pResultName">Id of the result coming from the source.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pStartName">Id of the start coming from the target.</param>
        public void ConnectCall(CallNode pSource, string pResultName, CallNode pTarget, string pStartName)
        {
            // If the execution call does not exist, connect it.
            if (this.IsConnected(pSource, pResultName, pTarget, pStartName) == null)
            {
                if (pSource.NodeToCall.ContainsKey(pResultName) == false)
                {
                    pSource.NodeToCall[pResultName] = new List<ExecutionCall>();
                }

                pSource.NodeToCall[pResultName].Add(new ExecutionCall() {NextNode = pTarget, StartPort = pStartName});

                // Update the previous call nodes.
                if (pTarget.PreviousCallNodes.ContainsKey(pSource) == false)
                {
                    pTarget.PreviousCallNodes.Add(pSource, 1);
                }
                else
                {
                    pTarget.PreviousCallNodes[pSource] += 1;
                }

                if (this.CallConnected != null)
                {
                    this.CallConnected(this, pSource, pResultName, pTarget, pStartName);
                }
            }
        }

        /// <summary>
        ///     Disconnects the call.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pResultName">Id of the result.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pStartName">Id of the start coming from the target.</param>
        public void DisconnectCall(CallNode pSource, string pResultName, CallNode pTarget, string pStartName)
        {
            ExecutionCall lCall = this.IsConnected(pSource, pResultName, pTarget, pStartName);
            if (lCall != null)
            {
                pSource.NodeToCall[pResultName].Remove(lCall);

                // Update the previous call nodes.
                if (pTarget.PreviousCallNodes.ContainsKey(pSource))
                {
                    pTarget.PreviousCallNodes[pSource] -= 1;
                    if (pTarget.PreviousCallNodes[pSource] == 0)
                    {
                        pTarget.PreviousCallNodes.Remove(pSource); 
                    }
                }

                if (this.CallDisconnected != null)
                {
                    this.CallDisconnected(this, pSource, pResultName, pTarget, pStartName);
                }
            }
        }

        /// <summary>
        /// Disconnects the call.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pTarget">The target.</param>
        public void DisconnectCalls(CallNode pSource, CallNode pTarget)
        {
            List<Tuple<string, string>> lConnectedResult = new List<Tuple<string, string>>();
            foreach (var lConnectionByResult in pSource.NodeToCall)
            {
                foreach (var lKey in lConnectionByResult.Value)
                {
                    foreach (var lExecutionCall in lConnectionByResult.Value)
                    {
                        if (lExecutionCall.NextNode == pTarget)
                        {
                            lConnectedResult.Add(new Tuple<string, string>(lConnectionByResult.Key, lExecutionCall.StartPort));
                        }
                    }
                }
            }
            foreach (var lResult in lConnectedResult)
            {
                this.DisconnectCall(pSource, lResult.Item1, pTarget, lResult.Item2);
            }
        }

        /// <summary>
        /// Determines whether the specified source is connected or not to the target (Result->StartName).
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pResultName">Id of the result.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pStartName">Id of the start coming from the target.</param>
        /// <returns>
        ///   <c>true</c> if the specified source is connected; otherwise, <c>false</c>.
        /// </returns>
        private ExecutionCall IsConnected(CallNode pSource, string pResultName, CallNode pTarget, string pStartName)
        {
            if (pSource.NodeToCall.ContainsKey(pResultName))
            {
                foreach (var lExecutionCall in pSource.NodeToCall[pResultName])
                {
                    if (lExecutionCall.NextNode == pTarget && lExecutionCall.StartPort == pStartName)
                    {
                        return lExecutionCall;
                    }
                }
            }
            return null;
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

            if (pSource.NodeParameters[pTarget][pSourceName].Contains(pTargetName) == false)
            {
                pSource.NodeParameters[pTarget][pSourceName].Add(pTargetName);

                // Update the previous call nodes.
                if (pTarget.PreviousParameterNodes.ContainsKey(pSource) == false)
                {
                    pTarget.PreviousParameterNodes.Add(pSource, 1);
                }
                else
                {
                    pTarget.PreviousParameterNodes[pSource] += 1;
                }

                // Reset the value in the target.
                pTarget.ResetParameter(pTargetName);

                if (this.ParameterConnected != null)
                {
                    this.ParameterConnected(this, pSource, pSourceName, pTarget, pTargetName);
                }
            }
        }

        /// <summary>
        ///     Disconnects an output of the source to the input of the target.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pSourceName">The name of the output.</param>
        /// <param name="pTarget">The target.</param>
        /// <param name="pTargetName">The name of the input.</param>
        public void DisconnectParameter(CallNode pSource, string pSourceName, CallNode pTarget, string pTargetName)
        {
            if (pSource.NodeParameters.ContainsKey(pTarget)
                && pSource.NodeParameters[pTarget].ContainsKey(pSourceName)
                && pSource.NodeParameters[pTarget][pSourceName].Contains(pTargetName)
                )
            {
                // Update the next call nodes.
                pSource.NodeParameters[pTarget][pSourceName].Remove(pTargetName);

                // Update the previous call nodes.
                if (pTarget.PreviousParameterNodes.ContainsKey(pSource))
                {
                    pTarget.PreviousParameterNodes[pSource] -= 1;
                    if (pTarget.PreviousParameterNodes[pSource] == 0)
                    {
                        pTarget.PreviousParameterNodes.Remove(pSource);
                    }
                }

                // Reset the value in the target.
                pTarget.ResetParameter(pTargetName);

                if (this.ParameterDisconnected != null)
                {
                    this.ParameterDisconnected(this, pSource, pSourceName, pTarget, pTargetName);
                }
            }
        }

        /// <summary>
        ///     Disconnects an output of the source to the input of the target.
        /// </summary>
        /// <param name="pSource">The source.</param>
        /// <param name="pTarget">The target.</param>
        public void DisconnectParameters(CallNode pSource, CallNode pTarget)
        {
            if (pSource.NodeParameters.ContainsKey(pTarget))
            {
                List<KeyValuePair<string, string>> lConnections = new List<KeyValuePair<string, string>>();
                Dictionary<string, List<string>> lConnected = pSource.NodeParameters[pTarget];
                foreach (KeyValuePair<string, List<string>> lKeyValuePair in lConnected)
                {
                    foreach (var lValue in lKeyValuePair.Value)
                    {
                        lConnections.Add(new KeyValuePair<string, string>(lKeyValuePair.Key, lValue));
                    }
                }
                foreach (var lConnection in lConnections)
                {
                    this.DisconnectParameter(pSource, lConnection.Key, pTarget, lConnection.Value);
                }

                // Remove all references to Target from the source
                if (pSource.NodeParameters.ContainsKey(pTarget))
                {
                    pSource.NodeParameters.Remove(pTarget);
                }
            }
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="pSender">The sender.</param>
        /// <param name="pEventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCollectionChanged(object pSender, NotifyCollectionChangedEventArgs pEventArgs)
        {
            if (pEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var lOldItem in pEventArgs.OldItems)
                {
                    CallNode lCallNode = lOldItem as CallNode;

                    // Disconnect all calls.
                    CallNode[] lPreviousCalls = lCallNode.PreviousCallNodes.Keys.ToArray();
                    foreach (CallNode lPreviousCall in lPreviousCalls)
                    {
                        this.DisconnectCalls(lPreviousCall, lCallNode);
                    }
                    foreach (var lPotentialNext in this.CallNodes)
                    {
                        this.DisconnectCalls(lCallNode, lPotentialNext);
                    }

                    CallNode[] lPreviousParameterNodes = lCallNode.PreviousParameterNodes.Keys.ToArray();
                    foreach (CallNode lPreviousParameter in lPreviousParameterNodes)
                    {
                        this.DisconnectParameters(lPreviousParameter, lCallNode);
                    }
                    foreach (var lPotentialNext in this.CallNodes)
                    {
                        this.DisconnectParameters(lCallNode, lPotentialNext);
                    }

                    lCallNode.Dispose();
                }
            }

            if (pEventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var lOldItem in pEventArgs.OldItems)
                {
                    CallNode lCallNode = lOldItem as CallNode;
                    this.NotifyCallNodeRemoved(lCallNode);
                }
            }

            if (pEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var lNewItem in pEventArgs.NewItems)
                {
                    CallNode lCallNode = lNewItem as CallNode;
                    this.NotifyCallNodeAdded(lCallNode);
                }
            }

        }

        /// <summary>
        /// Notify the listeners of the adding of a CallNode to this Task.
        /// </summary>
        /// <param name="pCallNodeAdded">The node that was added.</param>
        public void NotifyCallNodeAdded(CallNode pCallNodeAdded)
        {
            if (this.CallNodeAdded != null)
            {
                this.CallNodeAdded(pCallNodeAdded);
            }
        }

        /// <summary>
        /// Notify the listeners of the removing of a CallNode from this Task.
        /// </summary>
        /// <param name="pCallNodeRemoved"></param>
        public void NotifyCallNodeRemoved(CallNode pCallNodeRemoved)
        {
            if (this.CallNodeRemoved != null)
            {
                this.CallNodeRemoved(pCallNodeRemoved);
            }
        }

        /// <summary>
        /// Clean all ressources
        /// </summary>
        public void Dispose()
        {
            if (this.mCallNodes != null)
            {
                this.mCallNodes.CollectionChanged -= this.OnCollectionChanged;
                foreach (CallNode lNode in this.mCallNodes.ToList())
                {
                    lNode.Dispose();
                }
                this.mCallNodes.Clear();
            }
            this.mCallNodes = null;
        }

       #endregion // Methods.
    }
}