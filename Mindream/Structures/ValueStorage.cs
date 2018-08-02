using System.Collections.Generic;

namespace Mindream.Structures
{
    /// <summary>
    ///     This structure stores some values.
    /// </summary>
    /// <seealso cref="object" />
    public class ValueStorage : Dictionary<string, object>
    {
        /// <summary>
        ///     Sets a value to a state vector.
        /// </summary>
        /// <param name="pName">Name of the variable.</param>
        /// <param name="pValue">The value.</param>
        public void SetValue(string pName, object pValue)
        {
            if (this.ContainsKey(pName))
            {
                this[pName] = pValue;
            }
            else
            {
                this.Add(pName, pValue);
            }
        }

        /// <summary>
        ///     Gets a value from a state vector.
        /// </summary>
        /// <param name="pName">Name of the variable.</param>
        /// <returns>The retrieved value or null if missing.</returns>
        public object GetValue(string pName)
        {
            if (this.ContainsKey(pName))
            {
                return this[pName];
            }
            return null;
        }
    }
}