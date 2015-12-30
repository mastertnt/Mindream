using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using DemoApplication.GraphViewModels;
using DemoApplication.LibraryViewModels;
using Microsoft.Win32;
using Mindream;
using Mindream.CallGraph;
using XGraph.ViewModels;
using XSerialization;
using XTreeListView.Gui;

namespace DemoApplication
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Fields

        /// <summary>
        /// This field stores the position where a drag is started.
        /// </summary>
        private Point mStartPoint;

        /// <summary>
        /// This field stores the position where a drop is done.
        /// </summary>
        private Point mDropPoint;

        /// <summary>
        /// This field stores the call graph.
        /// </summary>
        private MethodCallGraph mCallGraph;

        /// <summary>
        /// This field stores the graph view model.
        /// </summary>
        private CallGraphViewModel mGraphViewModel;

        /// <summary>
        /// This field stores the selected view model
        /// </summary>
        private CallNodeViewModel mSelectedViewModel;

        #endregion // Fields.

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            
            InitializeComponent();

            ComponentDescriptorRegistry lComponentDescriptorRegistry = new ComponentDescriptorRegistry();
            lComponentDescriptorRegistry.FindAllDescriptors();
            this.mComponentDescriptorLibrary.ViewModel = new ComponentDescriptorRegistryViewModel(lComponentDescriptorRegistry);

            this.NewProject(null);
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
                    this.mSelectedViewModel = (CallNodeViewModel) pEventArgs.AddedItems[0];
                    this.mPropertyEditor.SelectedObject = this.mSelectedViewModel.Node.Component;
                }
            }
            else
            {
                this.mPropertyEditor.SelectedObject = null;
                this.mSelectedViewModel = null;
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
            CallNodeViewModel lInputViewModel = this.mGraphViewModel.Nodes.FirstOrDefault(pNode => pNode.Ports.Contains(pEventArgs.Input)) as CallNodeViewModel;

            // Look for the ouput node.
            CallNodeViewModel lOutputViewModel = this.mGraphViewModel.Nodes.FirstOrDefault(pNode => pNode.Ports.Contains(pEventArgs.Output)) as CallNodeViewModel;

            // Create the connection in call graph.
            if (lInputViewModel != null && lOutputViewModel != null && pEventArgs.Input is PortEndedViewModel && pEventArgs.Output is PortStartViewModel)
            {
                this.mCallGraph.ConnectCall(lInputViewModel.Node, lOutputViewModel.Node, pEventArgs.Input.DisplayString);
            }

            if (lInputViewModel != null && lOutputViewModel != null && pEventArgs.Input is OutputParameterViewModel && pEventArgs.Output is InputParameterViewModel)
            {
                this.mCallGraph.ConnectParameter(lInputViewModel.Node, pEventArgs.Input.DisplayString, lOutputViewModel.Node, pEventArgs.Output.DisplayString);
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

            if (pEventArgs.LeftButton == MouseButtonState.Pressed && (Math.Abs(lDiff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(lDiff.Y) > SystemParameters.MinimumVerticalDragDistance))
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
        /// Handles the Click event of the start button.
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void StartClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            this.mOutput.Text = string.Empty;
            foreach (var lNode in this.mCallGraph.CallNodes)
            {
                lNode.Component.Returned += this.OnComponentReturned;
            }

            if (this.mSelectedViewModel != null)
            {
                this.mSelectedViewModel.Node.Start();
            }
        }

        /// <summary>
        /// Handles the Click event of the start button.
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void NewClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            this.NewProject(null);
        }

        /// <summary>
        /// Removes the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveClicked(object pSender, RoutedEventArgs pEventArgs)
        {
        }

        /// <summary>
        /// Loads the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LoadClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            OpenFileDialog lDialog = new OpenFileDialog()
            {
                Filter = "Task Files(*.task.xml)|*.task.xml|All(*.*)|*"
            };

            if (lDialog.ShowDialog() == true)
            {
                XSerializer lSerializer = new XSerializer();
                MethodCallGraph lCallGraph = lSerializer.Deserialize(lDialog.FileName) as MethodCallGraph;
                this.NewProject(lCallGraph);
            }
        }

        /// <summary>
        /// Saves the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            SaveFileDialog lDialog = new SaveFileDialog()
            {
                Filter = "Task Files(*.task.xml)|*.task.xml|All(*.*)|*"
            };

            if (lDialog.ShowDialog() == true)
            {
                XSerializer lSerializer = new XSerializer();
                lSerializer.Serialize(this.mCallGraph, lDialog.FileName);
            }
        }

        /// <summary>
        /// Called when [component succeed].
        /// </summary>
        /// <param name="pComponent">The component.</param>
        /// <param name="pResult">The result.</param>
        private void OnComponentReturned(IComponent pComponent, string pResult)
        {
            this.mOutput.Text += pComponent.Descriptor.Id;
            this.mOutput.Text += Environment.NewLine;
            this.mOutput.Text += pResult;
            this.mOutput.Text += Environment.NewLine;
            this.mOutput.Text += pComponent.ToString();
            this.mOutput.Text += Environment.NewLine;
        }

        /// <summary>
        /// News the project.
        /// </summary>
        private void NewProject(MethodCallGraph pCallGraph)
        {
            if (pCallGraph == null)
            {
                this.mCallGraph = new MethodCallGraph();
            }
            else
            {
                this.mCallGraph = pCallGraph;
            }
            
            this.mGraphViewModel = new CallGraphViewModel(this.mCallGraph);
            this.mGraphViewModel.ConnectionAdded += OnConnectionAdded;
            this.mGraph.SelectionChanged += OnSelectionChanged;
            this.mGraph.DataContext = this.mGraphViewModel;
            this.mSelectedViewModel = null;
        }
    }
}
