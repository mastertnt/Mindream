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
    class ComponentDescriptorViewModel : AHierarchicalItemViewModel<IComponentDescriptor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptorViewModel"/> class.
        /// </summary>
        /// <param name="pComponentDescriptor">The p component descriptor.</param>
        public ComponentDescriptorViewModel(IComponentDescriptor pComponentDescriptor)
        :base(pComponentDescriptor)
        {
            
        }

        /// <summary>
        /// Gets or sets the display string.
        /// </summary>
        /// <value>
        /// The display string.
        /// </value>
        public override string DisplayString
        {
            get
            {
                return this.OwnedObject.Name;
            }
            set
            {
                // Nothing to do.
            }
        }

        /// <summary>
        /// Gets the icon source.
        /// </summary>
        /// <value>
        /// The icon source.
        /// </value>
        public override System.Windows.Media.ImageSource IconSource
        {
            get
            {
                return null;
            }
        }
    }
}
