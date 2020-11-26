using System.Xml.Linq;
using Mindream.CallGraph;
using XSerialization;
using XSerialization.Defaults;

namespace Mindream.Serialization
{
    /// <summary>
    /// Task serialization contract class definition.
    /// </summary>
    public class TaskSerializationContract : AObjectSerializationContract<Task>
    {
        #region Methods

        /// <summary>
        /// Creates the specified element.
        /// </summary>
        /// <param name="pElement">The element.</param>
        /// <param name="pSerializationContext">The serialization context.</param>
        /// <returns>The created task.</returns>
        public override object Create(XElement pElement, IXSerializationContext pSerializationContext)
        {
            XElement lTaskId = pElement.Element("Id");
            if (lTaskId != null)
            {
                return TaskManager.Instance.CreateTask(lTaskId.Value);
            }

            return null;
        }       

       #endregion // Methods.
    }
}
