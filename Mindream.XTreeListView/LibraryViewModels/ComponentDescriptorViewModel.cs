using Mindream.Descriptors;
using XTreeListView.ViewModel.Generic;

namespace Mindream.XTreeListView.LibraryViewModels
{
    /// <summary>
    /// This class is the view model for the component descriptor.
    /// </summary>
    public class ComponentDescriptorViewModel : AHierarchicalItemViewModel<IComponentDescriptor>
    {
        #region Properties

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
                return this.OwnedObject.Id + "(" + this.OwnedObject.GetType().Name + ")";
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

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentDescriptorViewModel"/> class.
        /// </summary>
        /// <param name="pComponentDescriptor">The p component descriptor.</param>
        public ComponentDescriptorViewModel(IComponentDescriptor pComponentDescriptor)
        :base(pComponentDescriptor)
        {
            
        }

        #endregion // Constructors.
    }
}
