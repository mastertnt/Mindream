using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mindream.Reflection;
using XSystem;
using IComponent = Mindream.Components.IComponent;

namespace Mindream.CallGraph
{
    /// <summary>
    /// This state stores the running state of a call node.
    /// </summary>
    enum CallNodeState
    {
        /// <summary>
        /// The started
        /// </summary>
        Started,

        /// <summary>
        /// The call node is broken when it starts.
        /// </summary>
        BreakStart,

        /// <summary>
        /// The pending start
        /// </summary>
        WaitingForStart,

        /// <summary>
        /// The call node is broken when it ends.
        /// </summary>
        BreakEnd,

        /// <summary>
        /// The undefined state
        /// </summary>
        Undefined,
    }


    /// <summary>
    /// This class represents a call node.
    /// </summary>
    /// <!-- NBY -->
    public class CallNode : IDisposable
    {

        #region Fields


        /// <summary>
        /// This field stores the current simulation step.
        /// </summary>
        private int mSimulationStep;

        /// <summary>
        /// This field stores the last simulation step when the node started.
        /// </summary>
        private int mLastStartStep;

        /// <summary>
        /// Gets or sets the initial values.
        /// </summary>
        /// <value>
        /// The initial values.
        /// </value>
        private readonly Dictionary<IComponentMemberInfo, object> mInitialValues;

        /// <summary>
        /// Gets or sets the initial values.
        /// </summary>
        /// <value>
        /// The initial values.
        /// </value>
        private readonly Dictionary<IComponentMemberInfo, object> mDefaultValues;

        /// <summary>
        /// This field stores the port start
        /// </summary>
        private List<string> mPendingStartPorts;

        /// <summary>
        /// The component owned by this CallNode.
        /// </summary>
        private IComponent mComponent;

        #endregion // Fields.

        #region Events
        /// <summary>
        /// Notify an output change of the Component of this CallNode, an event is raised for each connexion that implies this output. It gives the starting CallNode (this one), it's output id, the receiving CallNode and it's input id. 
        /// </summary>
        public event Action<CallNode, string, CallNode, string> OutputChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the instance (for component based on instance).
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public IComponent Component
        {
            get
            {
                return this.mComponent;
            }
            set
            {
                if (this.mComponent != null)
                {
                    this.mComponent.PropertyChanged -= this.OnOutputChanged;
                }

                this.mComponent = value;

                if (this.mComponent != null)
                {
                    this.mComponent.PropertyChanged += this.OnOutputChanged;
                    this.StoreDefaultValues();
                }
            }
        }

        /// <summary>
        /// Wait the start of the call node.
        /// </summary>
        public bool IsBreakInput
        {
            get;
            set;
        }

        /// <summary>
        /// Wait the end of the call node.
        /// </summary>
        public bool IsBreakOutput
        {
            get;
            set;
        }

        /// <summary>
        /// Called on an property change in the component of this CallNode.
        /// </summary>
        /// <param name="sender">The component owned by this CallNode.</param>
        /// <param name="e">The event argument.</param>
        private void OnOutputChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyOutputChanged(e.PropertyName);
        }

        /// <summary>
        /// Called on a property change of the component owned by this CallNode, notify if this implies a change in another node by connexion with an input.
        /// </summary>
        /// <param name="pPropertyName">The name of the property that changed.</param>
        private void NotifyOutputChanged(string pPropertyName)
        {
            if (this.OutputChanged != null 
                && String.IsNullOrEmpty(pPropertyName) == false)
            {
                // If the modified property is "Set" then we search all connexions between Set and the imputs of the others connected CallNode, and, for each of them, we send an event OutputChanged. 
                IEnumerable<CallNode> lOutputNodes = this.NodeParameters.Where(pElem => pElem.Value.Keys.Contains(pPropertyName)).Select(pElem => pElem.Key);
                if (lOutputNodes != null && lOutputNodes.Any())
                {
                    foreach (CallNode lOutputNode in lOutputNodes)
                    {
                        foreach (string targetParam in this.NodeParameters[lOutputNode][pPropertyName])
                        {
                            this.OutputChanged(this, pPropertyName, lOutputNode, targetParam);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the call nodes.
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public Dictionary<string, List<ExecutionCall>> NodeToCall
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the previous call nodes (connected to start).
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public Dictionary<CallNode, int> PreviousCallNodes
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the previous call nodes (connected to a parameter).
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public Dictionary<CallNode, int> PreviousParameterNodes
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the call nodes.
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public Dictionary<CallNode, Dictionary<string, List<string>>> NodeParameters
        {
            get;
            internal set;
        }

        /// <summary>
        /// This field stores the state of the call node.
        /// </summary>
        internal CallNodeState State
        {
            get;
            private set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNode" /> class.
        /// </summary>
        public CallNode()
        {
            this.NodeToCall = new Dictionary<string, List<ExecutionCall>>();
            this.NodeParameters = new Dictionary<CallNode, Dictionary<string, List<string>>>();
            this.PreviousParameterNodes = new Dictionary<CallNode, int>();
            this.PreviousCallNodes = new Dictionary<CallNode, int>();
            this.mInitialValues = new Dictionary<IComponentMemberInfo, object>();
            this.mDefaultValues = new Dictionary<IComponentMemberInfo, object>();
            this.mPendingStartPorts = new List<string>();
            this.mLastStartStep = -1;
            this.State = CallNodeState.Undefined;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        /// <param name="pSimulationStep">The simulation step.</param>
        public void Start(string pPortName, int pSimulationStep)
        {
            if (this.IsBreakInput == false)
            {
                if (this.State == CallNodeState.Undefined && this.mLastStartStep != pSimulationStep)
                {
                    this.mSimulationStep = pSimulationStep;

                    // Pull data from operator components (no end raised).
                    foreach (var lPreviousNode in this.PreviousParameterNodes.Keys)
                    {
                        if (lPreviousNode.Component.Descriptor.IsOperator)
                        {
                            lPreviousNode.Start(String.Empty, pSimulationStep);
                        }
                        lPreviousNode.TransferParameters(this);
                    }

                    this.mLastStartStep = pSimulationStep;
                    if (this.Component.Descriptor.IsOperator == false)
                    {
                        this.State = CallNodeState.Started;
                        this.Component.Started += this.OnComponentStarted;
                        this.Component.Start(pPortName);
                    }
                    else
                    {
                        this.Component.Start();
                    }
                    if (this.mPendingStartPorts.Count != 0)
                    {
                        this.State = CallNodeState.WaitingForStart;
                    }
                }
                else
                {
                    this.State = CallNodeState.WaitingForStart;
                    if (this.mPendingStartPorts.Contains(pPortName) == false)
                    {
                        this.mPendingStartPorts.Add(pPortName);
                    }
                }
            }
            else
            {
                this.State = CallNodeState.BreakStart;
            }
        }

        /// <summary>
        /// Starts the pending.
        /// </summary>
        /// <param name="pSimulationStep">The p simulation step.</param>
        internal void StartPending(int pSimulationStep)
        {
            if (pSimulationStep != this.mLastStartStep)
            {
                this.State = CallNodeState.Undefined;
                string lPendingStartPort = this.mPendingStartPorts.FirstOrDefault();
                this.mPendingStartPorts.Remove(lPendingStartPort);
                this.State = CallNodeState.Undefined;
                this.Start(lPendingStartPort, pSimulationStep);
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="pDeltaTime">The delta time.</param>
        /// <param name="pSimulationStep">The simulation step.</param>
        public void Update(TimeSpan pDeltaTime, int pSimulationStep)
        {
            // Pull data from data only components.
            foreach (var lPreviousNode in this.PreviousParameterNodes.Keys)
            {
                lPreviousNode.TransferParameters(this);
            }
            this.Component.Update(pDeltaTime);
        }

        /// <summary>
        /// Suspends this instance.
        /// </summary>
        public void Suspend()
        {
            this.Component.Suspend();
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Resume()
        {
            this.Component.Resume();
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.Component.Stop();
        }

        /// <summary>
        /// Aborts this instance.
        /// </summary>
        public void Abort()
        {
            this.Component.Abort();
        }

        /// <summary>
        /// Determines whether if the start port is connected.
        /// </summary>
        /// <param name="pStartName">Start port name.</param>
        /// <returns>
        /// <c>true</c> if the port is connected; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStartConnected(string pStartName)
        {
            foreach (var lPreviousCallNode in this.PreviousCallNodes.Keys)
            {
                foreach (var lExecutionCalls in lPreviousCallNode.NodeToCall)
                {
                    foreach (var lExecutionCall in lExecutionCalls.Value)
                    {
                        if (lExecutionCall.NextNode == this && lExecutionCall.StartPort == pStartName)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether if the start port is connected.
        /// </summary>
        /// <param name="pEndName">End port name.</param>
        /// <returns>
        ///   <c>true</c> if the port is connected; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEndConnected(string pEndName)
        {
            if (this.NodeToCall.ContainsKey(pEndName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether if the start port is connected.
        /// </summary>
        /// <param name="pParameterName">Input port name.</param>
        /// <returns>
        /// <c>true</c> if the port is connected; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInputConnected(string pParameterName)
        {
            foreach (var lPreviousCallNode in this.PreviousParameterNodes.Keys)
            {
                if (lPreviousCallNode.NodeParameters.ContainsKey(this))
                {
                    Dictionary<string, List<string>> lParameters = lPreviousCallNode.NodeParameters[this];
                    foreach (var lInputParameters in lParameters)
                    {
                        if (lInputParameters.Value.Contains(pParameterName))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether if the start port is connected.
        /// </summary>
        /// <param name="pParameterName">Output port name.</param>
        /// <returns>
        /// <c>true</c> if the port is connected; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOutputConnected(string pParameterName)
        {
            foreach (var lParameters in this.NodeParameters)
            {
                if (lParameters.Value.ContainsKey(pParameterName))
                {
                    if (lParameters.Value.FirstOrDefault(pElt => pElt.Key == pParameterName).Value.Count != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Called when [component started].
        /// </summary>
        /// <param name="pComponent">The started component.</param>
        /// <param name="pPortName">The name of the execution port to start</param>
        private void OnComponentStarted(IComponent pComponent, string pPortName)
        {
            this.Component.Started -= this.OnComponentStarted;
            this.Component.Stopped += this.OnComponentStopped;
            this.Component.Returned += this.OnComponentReturned;
            this.Component.Aborted += this.OnComponentStopped;
        }

        /// <summary>
        /// Called when [component stopped].
        /// </summary>
        /// <param name="pComponent">The p component.</param>
        private void OnComponentStopped(IComponent pComponent)
        {
            this.Component.Stopped -= this.OnComponentStopped;
            this.Component.Returned -= this.OnComponentReturned;
        }

        /// <summary>
        /// Called when [component succeed].
        /// </summary>
        /// <param name="pComponent">The component succeed.</param>
        /// <param name="pResultName">Id of the result.</param>
        private void OnComponentReturned(IComponent pComponent, string pResultName)
        {
            this.Component.Returned -= this.OnComponentReturned;
            if (this.NodeToCall.ContainsKey(pResultName))
            {
                foreach (var lExecutionCall in this.NodeToCall[pResultName])
                {
                    if (this.NodeParameters.ContainsKey(lExecutionCall.NextNode))
                    {
                        this.TransferParameters(lExecutionCall.NextNode);
                    }

                    // Pull data from data only components.
                    foreach (var lPreviousNode in lExecutionCall.NextNode.PreviousParameterNodes.Keys)
                    {
                        if (lPreviousNode.Component.Descriptor.IsOperator)
                        {
                            lPreviousNode.Start(String.Empty, this.mSimulationStep);
                        }
                        lPreviousNode.TransferParameters(lExecutionCall.NextNode);
                    }

                    lExecutionCall.NextNode.Start(lExecutionCall.StartPort, this.mSimulationStep);
                }
            }

            if (this.Component.IsUpdatable)
            {
                // Pull data from data only components.
                foreach (var lPreviousNode in this.PreviousParameterNodes.Keys)
                {
                    lPreviousNode.TransferParameters(this);
                }
            }
            else
            {
                // Restore all datas stored at the start.
                foreach (var lInput in this.mInitialValues)
                {
                    lInput.Key.SetValue(this.Component, lInput.Value);
                }
            }

            if (this.State != CallNodeState.WaitingForStart)
            {
                this.State = CallNodeState.Undefined;
            }
        }

        /// <summary>
        /// Transfers the linked parameters from a node to another.
        /// </summary>
        /// <param name="pNodeToCall">The node to call.</param>
        public void TransferParameters(CallNode pNodeToCall)
        {
            foreach (var lLinkedParameters in this.NodeParameters[pNodeToCall])
            {
                var lSourceInfo = this.Component.Descriptor.Outputs.First(pMember => pMember.Name == lLinkedParameters.Key);
                foreach (var lParameter in lLinkedParameters.Value)
                {
                    this.TransferParameter(lSourceInfo, pNodeToCall, lParameter);
                }
            }
        }

        /// <summary>
        /// Transfers the parameter.
        /// </summary>
        /// <param name="pSourceParameterName">Name of the source paramter.</param>
        /// <param name="pNodeToCall">The node to call.</param>
        /// <param name="pTargetParameterName">Name of the target parameter.</param>
        public void TransferParameter(string pSourceParameterName, CallNode pNodeToCall, string pTargetParameterName)
        {
            var lSourceInfo = this.Component.Descriptor.Outputs.First(pMember => pMember.Name == pSourceParameterName);
            this.TransferParameter(lSourceInfo, pNodeToCall, pTargetParameterName);
        }

        /// <summary>
        /// Transfers the parameter.
        /// </summary>
        /// <param name="pNodeToCall">The node to call.</param>
        /// <param name="pTargetParameterName">Name of the target parameter.</param>
        public void ClearParameter(CallNode pNodeToCall, string pTargetParameterName)
        {
            var lTargetInfo = pNodeToCall.Component.Descriptor.Inputs.First(pMember => pMember.Name == pTargetParameterName);
            pNodeToCall.Component[pTargetParameterName] = lTargetInfo.Type.DefaultValue();
        }


        /// <summary>
        /// Transfers the parameter.
        /// </summary>
        /// <param name="pSourceInfo">The source information.</param>
        /// <param name="pNodeToCall">The node to call.</param>
        /// <param name="pTargetParameterName">Name of the target parameter.</param>
        private void TransferParameter(IComponentMemberInfo pSourceInfo, CallNode pNodeToCall, string pTargetParameterName)
        {
            var lTargetInfo = pNodeToCall.Component.Descriptor.Inputs.First(pMember => pMember.Name == pTargetParameterName);
            switch (CanBeAssignedTo(pSourceInfo.Type, lTargetInfo.Type))
            {
                case AssignationType.SourceToTarget_Direct:
                    {
                        TypeConverter lConverter = TypeDescriptor.GetConverter(pSourceInfo.Type);
                        try
                        {
                            var lConvertedValue = lConverter.ConvertTo(this.Component[pSourceInfo.Name], lTargetInfo.Type);
                            pNodeToCall.Component[pTargetParameterName] = lConvertedValue;
                        }
                        catch (Exception)
                        {
                            var lConvertedValue = lTargetInfo.Type.DefaultValue();
                            pNodeToCall.Component[pTargetParameterName] = lConvertedValue;
                        }
                    }
                    break;

                case AssignationType.TargetToSource_Back:
                    {
                        TypeConverter lConverter = TypeDescriptor.GetConverter(lTargetInfo.Type);
                        try
                        {
                            var lConvertedValue = lConverter.ConvertFrom(null, null, this.Component[pSourceInfo.Name]);
                            pNodeToCall.Component[pTargetParameterName] = lConvertedValue;
                        }
                        catch (Exception)
                        {
                            var lConvertedValue = lTargetInfo.Type.DefaultValue();
                            pNodeToCall.Component[pTargetParameterName] = lConvertedValue;
                        }
                    }
                    break;

                case AssignationType.Assignable:
                    {
                        pNodeToCall.Component[pTargetParameterName] = this.Component[pSourceInfo.Name];
                    }
                    break;
            }
        }

        /// <summary>
        /// Determines whether this instance can be assigned to the specified target type.
        /// </summary>
        /// <param name="pSourceType">The type to assign.</param>
        /// <param name="pTargetType">The type that has to be assigned.</param>
        /// <returns>The type of assignation which can be done.</returns>
        public static AssignationType CanBeAssignedTo(Type pSourceType, Type pTargetType)
        {
            if (pTargetType.IsAssignableFrom(pSourceType) == false)
            {
                var lConverter = TypeDescriptor.GetConverter(pSourceType);
                if (lConverter.CanConvertTo(pTargetType))
                {
                    return AssignationType.SourceToTarget_Direct;
                }

                TypeConverter lBackConverter = TypeDescriptor.GetConverter(pTargetType);

                if (lBackConverter.CanConvertFrom(pSourceType))
                {
                    return AssignationType.TargetToSource_Back;
                }
            }
            else
            {
                return AssignationType.Assignable;
            }

            return AssignationType.NotAssignable;
        }

        /// <summary>
        /// Resets the input.
        /// </summary>
        /// <param name="pTargetParameterName">Name of the target parameter.</param>
        public void ResetParameter(string pTargetParameterName)
        {
            if (this.Component != null)
            {
                var lInput = this.Component.Descriptor.Inputs.FirstOrDefault(pMemberInfo => pMemberInfo.Name == pTargetParameterName);
                if (lInput != null)
                {
                    if (this.mInitialValues.ContainsKey(lInput))
                    {
                        lInput.SetValue(this.Component, this.mInitialValues[lInput]);
                    }
                    else if (this.mDefaultValues.ContainsKey(lInput))
                    {
                        lInput.SetValue(this.Component, this.mDefaultValues[lInput]);
                    }                    
                    else
                    {
                        this.Component[pTargetParameterName] = lInput.Type.DefaultValue();
                    }
                }
            }
        }

        /// <summary>
        /// Stores the values.
        /// </summary>
        internal void StoreValues()
        {
            // Clear dictionary before
            this.mInitialValues.Clear();

            // Store all datas before the pulling of previous node.
            foreach (var lInput in this.Component.Descriptor.Inputs)
            {
                this.mInitialValues.Add(lInput, lInput.GetValue(this.Component));
            }
        }

        /// <summary>
        /// Stores the values.
        /// </summary>
        internal void StoreDefaultValues()
        {
            // Clear dictionary before
            this.mDefaultValues.Clear();

            // Store all datas before the pulling of previous node.
            foreach (var lInput in this.Component.Descriptor.Inputs)
            {
                this.mDefaultValues.Add(lInput, lInput.GetValue(this.Component));
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        internal void RestoreValues()
        {
            // Restore all datas stored at the start.
            foreach (var lInput in this.mInitialValues)
            {
                lInput.Key.SetValue(this.Component, lInput.Value);
            }
            this.mInitialValues.Clear();
            this.State = CallNodeState.Undefined;
            this.mLastStartStep = -1;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Component != null)
            {
                this.Component.Dispose();
            }
        }

        #endregion // Methods.
    }
}