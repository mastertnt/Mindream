using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
{
    public class OutputParameterViewModel : PortViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputParameterViewModel"/> class.
        /// </summary>
        /// <param name="pParameter">The p parameter.</param>
        public OutputParameterViewModel(ParameterInfo pParameter)
        {
            this.DisplayString = pParameter.Name;
            this.Direction = PortDirection.Output;
        }
    }
}
