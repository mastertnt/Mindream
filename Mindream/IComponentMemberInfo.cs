using System;

namespace Mindream
{
    /// <summary>
    /// This interface represents a component member info.
    /// </summary>
    public interface IComponentMemberInfo
    {
        /// <summary>
        /// Gets or sets the name of the component member info.
        /// </summary>
        /// <value>
        /// The return.
        /// </value>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets the position of the component member info.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        int Position
        {
            get;
        }

        /// <summary>
        /// Gets or sets the type of the component member info.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type 
        {  
            get;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="pInstance">The instance to modify.</param>
        /// <param name="pNewValue">The value of component member info.</param>
        void SetValue(IComponent pInstance, object pNewValue);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="pInstance">The instance to check.</param>
        /// <returns>The value of the component member info.</returns>
        object GetValue(IComponent pInstance);

    }
}
