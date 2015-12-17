using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
{
    /// <summary>
    /// This class is used to describe a parameter.
    /// </summary>
    public class InputParameterViewModel : PortViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputParameterViewModel"/> class.
        /// </summary>
        /// <param name="pParameter">The p parameter.</param>
        public InputParameterViewModel(ParameterInfo pParameter)
        {
            this.DisplayString = pParameter.Name;
            this.Direction = PortDirection.Input;
        }
    }
}
