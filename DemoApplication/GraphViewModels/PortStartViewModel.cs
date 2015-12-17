using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
{
    /// <summary>
    /// This port view model represents the start as input.
    /// </summary>
    public class PortStartViewModel : PortViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortStartViewModel"/> class.
        /// </summary>
        public PortStartViewModel()
        {
            this.Direction = PortDirection.Input;
            this.DisplayString = "Stop";
        }
    }
}
