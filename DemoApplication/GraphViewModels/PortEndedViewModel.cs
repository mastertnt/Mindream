using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindream;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
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
        public PortEndedViewModel(MethodEnd pResult)
        {
            this.Direction = PortDirection.Output;
            //this.DisplayString = pResult.ResultName;
        }

        #endregion // Constructors.
    }
}
