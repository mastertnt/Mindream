using System;
using Mindream.Components;

namespace Mindream.Reflection
{
    /// <summary>
    ///     This is a base class for the compoment member info.
    /// </summary>
    /// <seealso cref="Mindream.Reflection.IComponentMemberInfo" />
    public class UserComponentMemberInfo : IComponentMemberInfo
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserComponentMemberInfo" /> class.
        /// </summary>
        /// <param name="pName">Id of the component info.</param>
        /// <param name="pType">Type of the component info.</param>
        public UserComponentMemberInfo(string pName, Type pType)
        {
            this.Name = pName;
            this.Type = pType;
        }

        #endregion // Constructors.

        #region Properties

        /// <summary>
        ///     Gets or sets the name of the component member info.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets the position of the component member info.
        /// </summary>
        /// <value>
        ///     The position.
        /// </value>
        public int Position
        {
            get
            {
                return -1;
            }
        }

        /// <summary>
        ///     Gets or sets the type of the component member info.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        public Type Type
        {
            get;
            private set;
        }

        #endregion // Properties.

        #region Methods

        /// <summary>
        ///     Sets the value.
        /// </summary>
        /// <param name="pInstance">The instance to modify.</param>
        /// <param name="pNewValue">The value of component member info.</param>
        public void SetValue(IComponent pInstance, object pNewValue)
        {
            if (pInstance is AComponent)
            {
                var lComponent = pInstance as AComponent;
                if (pInstance.Descriptor.Inputs.Contains(this))
                {
                    lComponent.Inputs.SetValue(this.Name, pNewValue);
                }
                if (pInstance.Descriptor.Outputs.Contains(this))
                {
                    lComponent.Outputs.SetValue(this.Name, pNewValue);
                }
            }
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <returns>The value of the component member info.</returns>
        public object GetValue(IComponent pInstance)
        {
            if (pInstance is AComponent)
            {
                var lComponent = pInstance as AComponent;
                if (pInstance.Descriptor.Inputs.Contains(this))
                {
                    return lComponent.Inputs.GetValue(this.Name);
                }
                if (pInstance.Descriptor.Outputs.Contains(this))
                {
                    return lComponent.Outputs.GetValue(this.Name);
                }
            }
            return null;
        }

        #endregion // Methods.
    }
}