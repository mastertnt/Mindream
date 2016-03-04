using Mindream.Reflection;
using XGraph.ViewModels;

namespace Mindream.XGraph.GraphViewModels
{
    /// <summary>
    /// This class is used to describe a view model for an output parameter.
    /// </summary>
    public class OutputParameterViewModel : PortViewModel
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputParameterViewModel"/> class.
        /// </summary>
        /// <param name="pParameter">The parameter.</param>
        public OutputParameterViewModel(IComponentMemberInfo pParameter)
        {
            this.DisplayString = string.IsNullOrWhiteSpace(pParameter.Name) ? "result" : pParameter.Name;
            this.Direction = PortDirection.Output;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Determines whether this source port can be connected to the specified p port view model.
        /// </summary>
        /// <param name="pTargetPortViewModel">The target port view model.</param>
        /// <returns>True if the connection can be done, false otherwise.</returns>
        public override bool CanBeConnectedTo(PortViewModel pTargetPortViewModel)
        {
            return (pTargetPortViewModel is InputParameterViewModel);
        }

        #endregion // Methods.
    }
}
