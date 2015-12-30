using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Mindream.CallGraph
{
    /// <summary>
    /// This class represents a call node.
    /// </summary>
    /// <!-- NBY -->
    public class CallNode
    {
        /// <summary>
        /// Gets or sets the instance (for component based on instance).
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public object Instance
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public IComponent Component
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the call nodes.
        /// </summary>
        /// <value>
        /// The call nodes.
        /// </value>
        public Dictionary<string, List<CallNode>> NodeToCall
        {
            get;
            set;
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
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallNode"/> class.
        /// </summary>
        public CallNode()
        {
            this.NodeToCall = new Dictionary<string, List<CallNode>>();
            this.NodeParameters = new Dictionary<CallNode, Dictionary<string, List<string>>>();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.Component.Returned += this.OnComponentReturned;
            this.Component.Start();
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
                foreach (var lNodeToCall in this.NodeToCall[pResultName])
                {
                    if (this.NodeParameters.ContainsKey(lNodeToCall))
                    {
                        foreach (var lLinkedParameters in this.NodeParameters[lNodeToCall])
                        {
                            IComponentMemberInfo lSourceInfo = this.Component.Descriptor.Outputs.First(pMember => pMember.Name == lLinkedParameters.Key);
                            foreach (var lParameter in lLinkedParameters.Value)
                            {
                                IComponentMemberInfo lTargetInfo = lNodeToCall.Component.Descriptor.Inputs.First(pMember => pMember.Name == lParameter);
                                if (lSourceInfo.Type != lTargetInfo.Type)
                                {
                                    object lConvertedValue = TypeDescriptor.GetConverter(lSourceInfo.Type).ConvertTo(this.Component[lLinkedParameters.Key], lTargetInfo.Type);
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
