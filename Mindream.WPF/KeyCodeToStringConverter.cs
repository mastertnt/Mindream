using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Mindream.WPF
{
    public class KeyCodeToStringConverter : TypeConverter
    {
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        /// <summary>
        /// Determines whether this instance [can convert from] the specified p context.
        /// </summary>
        /// <param name="pContext">The context.</param>
        /// <param name="pSourceType">Type of the source.</param>
        /// <returns>True if conversion can occured, false otherwise</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext pContext, Type pSourceType)
        {
            if (pSourceType == typeof(Key))
            {
                return true;
            }
            return base.CanConvertFrom(pContext, pSourceType);
        }

        // Overrides the ConvertFrom method of TypeConverter.
        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="pContext">The context.</param>
        /// <param name="pCulture">The culture.</param>
        /// <param name="pValue">The value.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext pContext, CultureInfo pCulture, object pValue)
        {
            if (pValue is Key)
            {
                var lV = KeyConverter.
                return new Point(int.Parse(lV[0]), int.Parse(lV[1]));
            }
            return base.ConvertFrom(pContext, pCulture, pValue);
        }

        // Overrides the ConvertTo method of TypeConverter.
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <param name="pContext">The context.</param>
        /// <param name="pCulture">The culture.</param>
        /// <param name="pValue">The value.</param>
        /// <param name="pDestinationType">Type of the destination.</param>
        /// <returns>The converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext pContext, CultureInfo pCulture, object pValue, Type pDestinationType)
        {
            if (pDestinationType == typeof (string))
            {
                return ((Point) pValue).X + "," + ((Point) pValue).Y;
            }
            return base.ConvertTo(pContext, pCulture, pValue, pDestinationType);
        }
    }
}