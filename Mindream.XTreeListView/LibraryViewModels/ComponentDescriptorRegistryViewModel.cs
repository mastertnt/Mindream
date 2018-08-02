using System.Linq;
using Mindream.Attributes;
using XTreeListView.ViewModel;
using XTreeListView.ViewModel.Generic;

namespace Mindream.XTreeListView.LibraryViewModels
{
    /// <summary>
    /// This class is the root view model for the component descriptor registry.
    /// </summary>
    public sealed class ComponentDescriptorRegistryViewModel : ARootHierarchicalItemViewModel<ComponentDescriptorRegistry>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptorRegistryViewModel"/> class.
        /// </summary>
        /// <param name="pRegistry">The registry.</param>
        public ComponentDescriptorRegistryViewModel(ComponentDescriptorRegistry pRegistry)
        {
            this.Model = pRegistry;
            foreach (var lModel in this.Model.Descriptors)
            {
                string lCategory = "Others";
                AComponentAttribute lAttribute = lModel.ComponentAttribute;
                if (lAttribute != null)
                {
                    lCategory = lAttribute.Category;
                }

                // Retrieve the parent.
                IHierarchicalItemViewModel lParent = this.Children.FirstOrDefault(pChild => pChild.UntypedOwnedObject.Equals(lCategory));
                if (lParent == null)
                {
                    lParent = new ComponentCategoryViewModel(lCategory);
                    this.AddChild(lParent);    
                }

                lParent.AddChild(new ComponentDescriptorViewModel(lModel));
            }
        }

        #endregion // Constructors.
    }
}
