using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mindream.Reflection
{
    /// <summary>
    /// A component member info based on a parameter info.
    /// </summary>
    public class ParameterMemberInfo : IComponentMemberInfo
    {
        #region Fields

        /// <summary>
        /// The fields stores the underlying information.
        /// </summary>
        private readonly ParameterInfo mUnderlyingInfo;

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets or sets the name of the component member info.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return this.mUnderlyingInfo.Name;
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
                return this.mUnderlyingInfo.Position;
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
                return this.mUnderlyingInfo.ParameterType;
            }
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMemberInfo" /> class.
        /// </summary>
        /// <param name="pUnderlyingInfo">The parameter information.</param>
        public ParameterMemberInfo(ParameterInfo pUnderlyingInfo)
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
            pInstance[this.mUnderlyingInfo.Name] = pNewValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>The value of the component member info.</returns>
        public object GetValue(IComponent pInstance)
        {
            return pInstance[this.mUnderlyingInfo.Name];
        }

        #endregion // Methods.
    }
}
