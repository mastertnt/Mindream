using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This port view model represents the start as input.
    /// </summary>
    public class PortStartViewModel : PortViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortStartViewModel"/> class.
        /// </summary>
        public PortStartViewModel(string pId, string pDisplayString)
        {
            this.Direction = PortDirection.Input;
            this.DisplayString = pDisplayString;
            this.Id = pId;
        }

        #endregion // Constructors.
    }
}
