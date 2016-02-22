using XTreeListView.ViewModel.Generic;

namespace Mindream.XTreeListView.LibraryViewModels
{
    /// <summary>
    /// This class represents the category of component as string.
    /// </summary>
    public class ComponentCategoryViewModel : AHierarchicalItemViewModel<string>
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
                    return this.OwnedObject;
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
            /// Initializes a new instance of the <see cref="ComponentCategoryViewModel"/> class.
            /// </summary>
            /// <param name="pComponentCategory">The component category.</param>
            public ComponentCategoryViewModel(string pComponentCategory)
                : base(pComponentCategory)
            {

            }

            #endregion // Constructors.
    }
}
