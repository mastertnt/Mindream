using System.Windows.Media;
using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// Class defining the connection view model. 
    /// </summary>
    public class ConnectionLinkViewModel : ConnectionViewModel
    {
        #region Fields

        /// <summary>
        /// The execution color link.
        /// </summary>
        public static readonly Color EXECUTION_LINK_COLOR = Color.FromRgb(34, 139, 34);

        /// <summary>
        /// The conversion link color.
        /// </summary>
        public static readonly Color CONVERSION_LINK_COLOR = Color.FromRgb(150, 131, 236);

        /// <summary>
        /// The active link color
        /// </summary>
        public static readonly Color ACTIVE_LINK_COLOR = Color.FromRgb(34, 139, 34);

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the brush used to color the connection.
        /// </summary>
        public override Brush Brush
        {
            get
            {
                if (this.IsActive)
                {
                    return (new SolidColorBrush(ConnectionLinkViewModel.ACTIVE_LINK_COLOR));
                }
                if (this.Input != null && this.Output != null)
                {
                    if (this.Input is PortStartViewModel && this.Output is PortEndedViewModel)
                    {
                        return (new SolidColorBrush(ConnectionLinkViewModel.EXECUTION_LINK_COLOR));
                    }
                    if (this.Input.PortType != this.Output.PortType)
                    {
                        return (new SolidColorBrush(ConnectionLinkViewModel.CONVERSION_LINK_COLOR));
                    }
                }

                return base.Brush;
            }
            set
            {
                base.Brush = value;
            }
        }

        #endregion //Properties.
    }
}
