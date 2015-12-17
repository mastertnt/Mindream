using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
{
    /// <summary>
    /// This port view model represents the succeed event as output.
    /// </summary>
    public class PortSucceedViewModel : PortViewModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PortSucceedViewModel"/> class.
        /// </summary>
        public PortSucceedViewModel()
        {
            this.Direction = PortDirection.Output;
            this.DisplayString = "Ended";
        }
    }
}
