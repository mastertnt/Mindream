using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindream;
using XTreeListView.ViewModel;
using XTreeListView.ViewModel.Generic;

namespace DemoApplication
{
    public class ComponentDescriptorRegistryViewModel : ARootHierarchicalItemViewModel<ComponentDescriptorRegistry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptorRegistryViewModel"/> class.
        /// </summary>
        /// <param name="pRegistry">The registry.</param>
        public ComponentDescriptorRegistryViewModel(ComponentDescriptorRegistry pRegistry)
        {
            this.Model = pRegistry;
            this.BindChildren("Descriptors", typeof(ComponentDescriptorViewModel));
        }
    }
}
