using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mindream;
using XGraph.ViewModels;

namespace DemoApplication.GraphViewModels
{
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
