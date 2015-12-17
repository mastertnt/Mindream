using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DemoApplication.GraphViewModels;
using Mindream;
using XGraph.ViewModels;
using XTreeListView.Gui;

namespace DemoApplication
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private ComponentDescriptorRegistry mComponentDescriptorRegistry;

        private Point mStartPoint;

        private CallGraph mCallGraph;

        #endregion // Fields.

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.mComponentDescriptorRegistry = new ComponentDescriptorRegistry();
            this.mComponentDescriptorRegistry.FindAllDescriptors(typeof(Samples));
            this.mComponentDescriptorLibrary.ViewModel =  new ComponentDescriptorRegistryViewModel(this.mComponentDescriptorRegistry);

            this.mCallGraph = new CallGraph();
            this.mGraph.DataContext = new CallGraphViewModel(this.mCallGraph);
        }

        /// <summary>
        /// Called when [drop].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void OnDrop(object pSender, DragEventArgs pEventArgs)
        {
            if (pEventArgs.Data.GetDataPresent("ComponentDescriptor"))
            {
                IComponentDescriptor lDescriptor = pEventArgs.Data.GetData("ComponentDescriptor") as IComponentDescriptor;
                this.mCallGraph.CallNodes.Add(lDescriptor.Create());
            }
        }

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            this.mStartPoint = e.GetPosition(null);
        }
        
        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = this.mStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                TreeListView listView = sender as TreeListView;
                TreeListViewItem listViewItem = FindAnchestor<TreeListViewItem>((DependencyObject)e.OriginalSource);
                if (listViewItem == null)
                    return;

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("ComponentDescriptor", listViewItem.ViewModel.UntypedOwnedObject);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }



        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
 
    }
}
