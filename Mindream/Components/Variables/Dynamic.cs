using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindream.Attributes;
using XSystem;

namespace Mindream.Components.Variables
{
    [FunctionComponent]
    public class Dynamic : AMethodComponent
    {
        #region Inputs

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [InOut]
        public object Value { get; set; }

        #endregion // Inputs

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStarted()
        {
            this.Stop();
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified p parameter.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="pParameterName">The parameter name.</param>
        /// <returns></returns>
        public override object this[string pParameterName]
        {
            set
            {
                if (pParameterName == "Value")
                {
                    this.Value = value;
                    return;
                }
                IComponentMemberInfo lComponentMemberInfo = this.Descriptor.Inputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    this.Value.SetPropertyValue(lComponentMemberInfo.Name, value);
                }
            }
            get
            {
                if (pParameterName == "Value")
                {
                    return this.Value;
                }

                IComponentMemberInfo lComponentMemberInfo = this.Descriptor.Outputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    return this.Value.GetPropertyValue(lComponentMemberInfo.Name);
                }
                return null;
            }
        }

        #endregion // Methods
    }
}
