using System.ComponentModel;
using System.Linq;
using Mindream.Attributes;
using XSystem;

namespace Mindream.Components.Variables
{
    /// <summary>
    /// A dynamic variable component.
    /// </summary>
    [VariableComponent("Types")]
    public class Dynamic<TType> : AMethodComponent where TType : class
    {
        #region Fields

        private TType mValue;

        #endregion // Fields.

        #region Inputs

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [Out]
        [Browsable(false)]
        public TType Get
        {
            get
            {
                return this.mValue;
            } 
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [In]
        public TType Set
        {
            get
            {
                return this.mValue;
            }
            set
            {
                this.mValue = value;
            }
        }

        #endregion // Inputs

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.Stop();
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.Object" /> with the specified p parameter.
        /// </summary>
        /// <value>
        ///     The <see cref="System.Object" />.
        /// </value>
        /// <param name="pParameterName">The parameter name.</param>
        /// <returns></returns>
        public override object this[string pParameterName]
        {
            set
            {
                if (pParameterName == "Value" || pParameterName == "Get" || pParameterName == "Set")
                {
                    this.Set = (TType) value;
                    return;
                }
                var lComponentMemberInfo = this.Descriptor.Inputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    this.Set.SetPropertyValue(lComponentMemberInfo.Name, value);
                }
            }
            get
            {
                if (pParameterName == "Value" || pParameterName == "Get" || pParameterName == "Set")
                {
                    return this.Get;
                }

                var lComponentMemberInfo = this.Descriptor.Outputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    return this.Get.GetPropertyValue(lComponentMemberInfo.Name);
                }
                return null;
            }
        }

        #endregion // Methods
    }
}