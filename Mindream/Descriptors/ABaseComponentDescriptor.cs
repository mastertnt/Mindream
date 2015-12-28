using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mindream.Descriptors
{
    /// <summary>
    /// This class represents the base class for all component descriptors.
    /// </summary>
    public abstract class ABaseComponentDescriptor : IComponentDescriptor
    {
        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the inputs.
        /// </summary>
        /// <value>
        /// The inputs.
        /// </value>
        public virtual List<IComponentMemberInfo> Inputs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>
        /// The outputs.
        /// </value>
        public virtual List<IComponentMemberInfo> Outputs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>
        /// The outputs.
        /// </value>
        public virtual List<MethodEnd> Results
        {
            get;
            set;
        }

        #endregion // Events.

        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="ABaseComponentDescriptor"/> class.
        /// </summary>
        protected ABaseComponentDescriptor()
        {
            this.Results = new List<MethodEnd>();
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <returns>
        /// The created instance of the component.
        /// </returns>
        public abstract IComponent Create();
        
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine(this.Name);
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
