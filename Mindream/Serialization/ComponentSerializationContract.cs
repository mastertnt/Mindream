using System.Linq;
using System.Xml.Linq;
using Mindream.Components;
using XSerialization;
using XSerialization.Defaults;

namespace Mindream.Serialization
{
    /// <summary>
    ///     This class defines a serialization contract for TimeSpan.
    /// </summary>
    public class ComponentSerializationContract : AObjectSerializationContract<IComponent>
    {
        /// <summary>
        /// The component regisry to use for deserilization.
        /// </summary>
        public static ComponentDescriptorRegistry msRegistry;

        /// <summary>
        ///     Creates the specified element.
        /// </summary>
        /// <param name="pElement">The element.</param>
        /// <param name="pSerializationContext">The serialization context.</param>
        /// <returns></returns>
        public override object Create(XElement pElement, IXSerializationContext pSerializationContext)
        {
            var lDescriptorAttribute = pElement.Attribute("descriptor");
            if (lDescriptorAttribute != null)
            {
                if (msRegistry == null)
                {
                    msRegistry = new ComponentDescriptorRegistry();
                    msRegistry.FindAllDescriptors();
                }

                var lDescriptor = msRegistry.Descriptors.FirstOrDefault(pDescriptor => pDescriptor.Id == lDescriptorAttribute.Value);
                return lDescriptor.Create();
            }
            return null;
        }

        /// <summary>
        ///     This method writes the specified object.
        /// </summary>
        /// <param name="pObject">The object to serialize.</param>
        /// <param name="pParentElement">The parent element.</param>
        /// <param name="pSerializationContext">The serialization context.</param>
        /// <returns>The modified parent element</returns>
        public override XElement Write(object pObject, XElement pParentElement, IXSerializationContext pSerializationContext)
        {
            var lComponent = pObject as ABaseComponent;
            pParentElement.SetAttributeValue("descriptor", lComponent.Descriptor.Id);
            return base.Write(pObject, pParentElement, pSerializationContext);
        }
    }
}