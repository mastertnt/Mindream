using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This class is used to describe a view model for an output parameter.
    /// </summary>
    public class OutputParameterViewModel : PortViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputParameterViewModel"/> class.
        /// </summary>
        /// <param name="pParameter">The parameter.</param>
        public OutputParameterViewModel(IComponentMemberInfo pParameter)
        {
            this.DisplayString = string.IsNullOrWhiteSpace(pParameter.Name) ? "result" : pParameter.Name;
            this.Direction = PortDirection.Output;
        }
    }
}
