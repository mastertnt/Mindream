using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mindream.Attributes;
using Mindream.Reflection;
using IComponent = Mindream.Components.IComponent;

namespace Mindream.Descriptors
{
    /// <summary>
    ///     This class represents the base class for all component descriptors.
    /// </summary>
    public abstract class ABaseComponentDescriptor : IComponentDescriptor
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ABaseComponentDescriptor" /> class.
        /// </summary>
        protected ABaseComponentDescriptor()
        {
            // ReSharper disable once VirtualMemberCallInContructor
            this.AdditionalStartPorts = new List<string>();
            this.Results = new List<IComponentReturnInfo>();
        }

        #endregion // Constructors.

        #region Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Browsable(false)]
        public virtual string Id
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the additional start ports.
        /// </summary>
        /// <value>
        ///     The additional start port.
        /// </value>
        [Browsable(false)]
        public List<string> AdditionalStartPorts
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the inputs.
        /// </summary>
        /// <value>
        ///     The inputs.
        /// </value>
        [Browsable(false)]
        public virtual List<IComponentMemberInfo> Inputs
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the outputs.
        /// </summary>
        /// <value>
        ///     The outputs.
        /// </value>
        [Browsable(false)]
        public virtual List<IComponentMemberInfo> Outputs
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the outputs.
        /// </summary>
        /// <value>
        ///     The outputs.
        /// </value>
        [Browsable(false)]
        public virtual List<IComponentReturnInfo> Results
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the component attribute.
        /// </summary>
        /// <value>
        ///     The component attribute.
        /// </value>
        [Browsable(false)]
        public abstract AComponentAttribute ComponentAttribute
        {
            get;
        }

        /// <summary>
        ///     Gets or sets the component type.
        /// </summary>
        /// <value>
        ///     The component type (can be null)
        /// </value>
        [Browsable(false)]
        public abstract Type ComponentType
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an operator.
        /// An operator has no start and end.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an operator; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public virtual bool IsOperator
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance needs to instanciate an internal object.
        /// Useful for variable components.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance needs to create an internal object; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        public virtual bool NeedCreate
        {
            get
            {
                return false;
            }
        }

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     Creates an instance.
        /// </summary>
        /// <returns>
        ///     The created instance of the component.
        /// </returns>
        public abstract IComponent Create();

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var lBuilder = new StringBuilder();
            lBuilder.AppendLine(this.Id);
            lBuilder.AppendLine("in:");
            foreach (var lInput in this.Inputs)
            {
                lBuilder.AppendLine("(" + lInput.Type + ") " + lInput.Name);
            }
            lBuilder.AppendLine("out:");
            foreach (var lOuput in this.Outputs)
            {
                lBuilder.AppendLine("(" + lOuput.Type + ") " + lOuput.Name);
            }
            return lBuilder.ToString();
        }

        #endregion // Methods.
    }
}