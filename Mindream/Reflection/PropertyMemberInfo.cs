using System;
using System.Reflection;

namespace Mindream.Reflection
{
    /// <summary>
    /// A component member info based on a property info.
    /// </summary>
    public class PropertyMemberInfo : IComponentMemberInfo
    {
        #region Fields

        /// <summary>
        /// The fields stores the underlying information.
        /// </summary>
        private readonly PropertyInfo mUnderlyingInfo;

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets or sets the name of the component member info.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Name
        {
            get
            {
                return this.mUnderlyingInfo.Name;
            }
        }

        /// <summary>
        /// Gets or sets the type of the component member info.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type
        {
            get
            {
                return this.mUnderlyingInfo.PropertyType;
            }
        }

        /// <summary>
        /// Gets or sets the position of the component member info.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position
        {
            get
            {
                return 0;
            }
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMemberInfo"/> class.
        /// </summary>
        /// <param name="pUnderlyingInfo">The p property information.</param>
        public PropertyMemberInfo(PropertyInfo pUnderlyingInfo)
        {
            this.mUnderlyingInfo = pUnderlyingInfo;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="pInstance">The instance to modify.</param>
        /// <param name="pNewValue">The value of component member info.</param>
        public void SetValue(IComponent pInstance, object pNewValue)
        {
            this.mUnderlyingInfo.SetValue(pInstance, pNewValue, null);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value of the component member info.</returns>
        public object GetValue(IComponent pInstance)
        {
            return this.mUnderlyingInfo.GetValue(pInstance, null);
        }

        #endregion // Methods.
    }
}
