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
            this.Id = pResult.Name;
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
            return (pTargetPortViewModel is PortStartViewModel);
        }

        #endregion // Methods.
    }
}
