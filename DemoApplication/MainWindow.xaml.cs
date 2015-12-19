using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private Point mDropPoint;

        private MethodCallGraph mCallGraph;

        private CallGraphViewModel mGraphViewModel = null;

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

            this.mCallGraph = new MethodCallGraph();
            this.mGraphViewModel = new CallGraphViewModel(this.mCallGraph);
            this.mGraphViewModel.ConnectionAdded += OnConnectionAdded;
            this.mGraph.SelectionChanged += OnSelectionChanged;
            this.mGraph.DataContext = this.mGraphViewModel;
        }

        /// <summary>
        /// Called when [drop].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSelectionChanged(object pSender, SelectionChangedEventArgs pEventArgs)
        {
            if (pEventArgs.AddedItems.Count != 0)
            {
                if (pEventArgs.AddedItems[0] is CallNodeViewModel)
                {
                    CallNodeViewModel lViewModel = pEventArgs.AddedItems[0] as CallNodeViewModel;
                    this.mPropertyEditor.SelectedObject = lViewModel.Node.Component;
                }
                
            }
            else
            {
                this.mPropertyEditor.SelectedObject = null;
            }
        }

        /// <summary>
        /// Called when [drop].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        private void OnDrop(object pSender, DragEventArgs pEventArgs)
        {
            if (pEventArgs.Data.GetDataPresent("ComponentDescriptor"))
            {
                IComponentDescriptor lDescriptor = pEventArgs.Data.GetData("ComponentDescriptor") as IComponentDescriptor;
                if (lDescriptor != null && pSender is IInputElement)
                {
                    mDropPoint = pEventArgs.GetPosition((IInputElement)pSender);
                    this.mGraphViewModel.NodeAdded += this.OnNodeAdded;
                    this.mCallGraph.CallNodes.Add(new CallNode {Component = lDescriptor.Create()});
                }
            }
        }

        /// <summary>
        /// Called when [node added].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The event arguments.</param>
        private void OnNodeAdded(GraphViewModel pSender, NodeViewModel pEventArgs)
        {
            pEventArgs.X = this.mDropPoint.X;
            pEventArgs.Y = this.mDropPoint.Y;
            this.mGraphViewModel.NodeAdded -= this.OnNodeAdded;
        }

        /// <summary>
        /// Called when [connection added].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The p event arguments.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnConnectionAdded(GraphViewModel pSender, ConnectionViewModel pEventArgs)
        {
            // Look for the input node.
            CallNodeViewModel lSourceNode = pSender.Nodes.FirstOrDefault(pNode => pNode.Ports.Contains(pEventArgs.Input)) as CallNodeViewModel;

            // CallNodeViewModel for the ouput node.
            CallNodeViewModel lDestinationNode = pSender.Nodes.FirstOrDefault(pNode => pNode.Ports.Contains(pEventArgs.Output)) as CallNodeViewModel;

            if (lSourceNode != null && lDestinationNode != null)
            {
                
            }
        }

        /// <summary>
        /// Called when [library preview mouse left button down].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnLibraryPreviewMouseLeftButtonDown(object pSender, MouseButtonEventArgs pEventArgs)
        {
            // Store the mouse position
            this.mStartPoint = pEventArgs.GetPosition(null);
        }

        /// <summary>
        /// Called when [library mouse move].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void OnLibraryMouseMove(object pSender, MouseEventArgs pEventArgs)
        {
            // Get the pCurrent mouse position
            Point lMousePos = pEventArgs.GetPosition(null);
            Vector lDiff = this.mStartPoint - lMousePos;

            if (pEventArgs.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(lDiff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(lDiff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                TreeListView lListView = pSender as TreeListView;
                TreeListViewItem lListViewItem = FindAnchestor<TreeListViewItem>((DependencyObject)pEventArgs.OriginalSource);
                if (lListViewItem == null)
                {
                    return;
                }

                // Initialize the drag & drop operation
                DataObject lDragData = new DataObject("ComponentDescriptor", lListViewItem.ViewModel.UntypedOwnedObject);
                DragDrop.DoDragDrop(lListViewItem, lDragData, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Finds the anchestor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pCurrent">The p current.</param>
        /// <returns></returns>
        private static T FindAnchestor<T>(DependencyObject pCurrent) where T : DependencyObject
        {
            do
            {
                if (pCurrent is T)
                {
                    return (T)pCurrent;
                }
                pCurrent = VisualTreeHelper.GetParent(pCurrent);
            }
            while (pCurrent != null);
            return null;
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object pSender, RoutedEventArgs pEventArgs)
        {
            if (this.mCallGraph.FirstNode != null)
            {
                this.mCallGraph.FirstNode.Component.Start();
            }
        }
 
    }
}
