using XTreeListView.ViewModel.Generic;

namespace Mindream.XGraph.LibraryViewModels
{
    /// <summary>
    /// This class is the root view model for the component descriptor registry.
    /// </summary>
    public class ComponentDescriptorRegistryViewModel : ARootHierarchicalItemViewModel<ComponentDescriptorRegistry>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptorRegistryViewModel"/> class.
        /// </summary>
        /// <param name="pRegistry">The registry.</param>
        public ComponentDescriptorRegistryViewModel(ComponentDescriptorRegistry pRegistry)
        {
            this.Model = pRegistry;
            this.BindChildren("Descriptors", typeof(ComponentDescriptorViewModel));
        }

        #endregion // Constructors.
    }
}
