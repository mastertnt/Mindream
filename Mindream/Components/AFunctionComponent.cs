using System.Linq;

namespace Mindream.Components
{
    /// <summary>
    ///     This class can be used to create a component of type function like branch.
    /// </summary>
    public abstract class AFunctionComponent : AComponent
    {
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
                var lComponentMemberInfo = this.Descriptor.Inputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    lComponentMemberInfo.SetValue(this, value);
                }
            }
            get
            {
                var lComponentMemberInfo = this.Descriptor.Outputs.FirstOrDefault(pParameter => pParameter.Name == pParameterName);
                if (lComponentMemberInfo != null)
                {
                    return lComponentMemberInfo.GetValue(this);
                }
                return null;
            }
        }
    }
}