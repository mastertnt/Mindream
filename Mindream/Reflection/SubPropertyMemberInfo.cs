using System.Reflection;
using Mindream.Components;
using XSystem;

namespace Mindream.Reflection
{
    /// <summary>
    ///     This class is used to reflect a property member info on a sub-property of the compobebt
    /// </summary>
    public class SubPropertyMemberInfo : PropertyMemberInfo
    {
        #region Fields

        /// <summary>
        ///     This field stores the instance property name
        /// </summary>
        private readonly string mInstancePropertyName;

        #endregion // Fields.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SubPropertyMemberInfo" /> class.
        /// </summary>
        /// <param name="pInstanceName">Name of the instance.</param>
        /// <param name="pUnderlyingInfo">The underlying information.</param>
        public SubPropertyMemberInfo(string pInstanceName, PropertyInfo pUnderlyingInfo)
            : base(pUnderlyingInfo)
        {
            this.mInstancePropertyName = pInstanceName;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        ///     Sets the value.
        /// </summary>
        /// <param name="pInstance">The instance to modify.</param>
        /// <param name="pNewValue">The value of component member info.</param>
        public override void SetValue(IComponent pInstance, object pNewValue)
        {
            object lInstance = pInstance.GetPropertyValue(this.mInstancePropertyName);
            if (lInstance != null)
            {
                lInstance.SetPropertyValue(this.Name, pNewValue);
            }
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <returns>The value of the component member info.</returns>
        public override object GetValue(IComponent pInstance)
        {
            object lInstance = pInstance.GetPropertyValue(this.mInstancePropertyName);
            if (lInstance != null)
            {
                return lInstance.GetPropertyValue(this.Name);
            }
            return null;
        }

        #endregion // Methods.
    }
}