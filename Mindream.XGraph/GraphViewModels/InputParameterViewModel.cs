using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This class is used to describe a view model for an input parameter.
    /// </summary>
    public class InputParameterViewModel : PortViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputParameterViewModel"/> class.
        /// </summary>
        /// <param name="pParameter">The p parameter.</param>
        public InputParameterViewModel(IComponentMemberInfo pParameter)
        {
            this.DisplayString = pParameter.Name;
            this.Direction = PortDirection.Input;
        }
    }
}
