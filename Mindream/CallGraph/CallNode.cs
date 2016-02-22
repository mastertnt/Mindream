using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IComponent = Mindream.Components.IComponent;

namespace Mindream.CallGraph
{
    /// <summary>
    ///     This class represents a call node.
    /// </summary>
    /// <!-- NBY -->
    public class CallNode
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CallNode" /> class.
        /// </summary>
        public CallNode()
        {
            this.NodeToCall = new Dictionary<string, List<CallNode>>();
            this.NodeParameters = new Dictionary<CallNode, Dictionary<string, List<string>>>();
        }

        /// <summary>
        ///     Gets or sets the instance (for component based on instance).
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public object Instance
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the component.
        /// </summary>
        /// <value>
        ///     The component.
        /// </value>
        public IComponent Component
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the call nodes.
        /// </summary>
        /// <value>
        ///     The call nodes.
        /// </value>
        public Dictionary<string, List<CallNode>> NodeToCall
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the call nodes.
        /// </summary>
        /// <value>
        ///     The call nodes.
        /// </value>
        public Dictionary<CallNode, Dictionary<string, List<string>>> NodeParameters
        {
            get;
            set;
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public void Start()
        {
            this.Component.Started += this.OnComponentStarted;
            this.Component.Start();
        }

        /// <summary>
        ///     Called when [component started].
        /// </summary>
        /// <param name="pComponent">The p component.</param>
        private void OnComponentStarted(IComponent pComponent)
        {
            this.Component.Started -= this.OnComponentStarted;
            this.Component.Stopped += this.OnComponentStopped;
            this.Component.Returned += this.OnComponentReturned;
        }

        /// <summary>
        ///     Called when [component stopped].
        /// </summary>
        /// <param name="pComponent">The p component.</param>
        private void OnComponentStopped(IComponent pComponent)
        {
            this.Component.Stopped -= this.OnComponentStopped;
            this.Component.Returned -= this.OnComponentReturned;
        }

        /// <summary>
        ///     Called when [component succeed].
        /// </summary>
        /// <param name="pComponent">The component succeed.</param>
        /// <param name="pResultName">Id of the result.</param>
        private void OnComponentReturned(IComponent pComponent, string pResultName)
        {
            if (this.NodeToCall.ContainsKey(pResultName))
            {
                foreach (var lNodeToCall in this.NodeToCall[pResultName])
                {
                    if (this.NodeParameters.ContainsKey(lNodeToCall))
                    {
                        foreach (var lLinkedParameters in this.NodeParameters[lNodeToCall])
                        {
                            var lSourceInfo = this.Component.Descriptor.Outputs.First(pMember => pMember.Name == lLinkedParameters.Key);
                            foreach (var lParameter in lLinkedParameters.Value)
                            {
                                var lTargetInfo = lNodeToCall.Component.Descriptor.Inputs.First(pMember => pMember.Name == lParameter);
                                if (lSourceInfo.Type != lTargetInfo.Type && lTargetInfo.Type.IsAssignableFrom(lSourceInfo.Type) == false)
                                {
                                    var lConvertedValue = TypeDescriptor.GetConverter(lSourceInfo.Type).ConvertTo(this.Component[lLinkedParameters.Key], lTargetInfo.Type);
                                    lNodeToCall.Component[lParameter] = lConvertedValue;
                                }
                                else
                                {
                                    lNodeToCall.Component[lParameter] = this.Component[lLinkedParameters.Key];
                                }
                            }
                        }
                    }
                    lNodeToCall.Start();
                }
            }
        }
    }
}