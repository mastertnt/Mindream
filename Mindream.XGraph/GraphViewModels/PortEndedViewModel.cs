using Mindream.Reflection;
using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This port view model represents the succeed event as output.
    /// </summary>
    public class PortEndedViewModel : PortViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PortEndedViewModel" /> class.
        /// </summary>
        /// <param name="pResult">The result.</param>
        public PortEndedViewModel(IComponentReturnInfo pResult)
        {
            this.Direction = PortDirection.Output;
            this.DisplayString = pResult.Name;
        }

        #endregion // Constructors.
    }
}
