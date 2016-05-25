﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Mindream;
using Mindream.CallGraph;
using Mindream.Descriptors;
using Mindream.XGraph.GraphViewModels;
using Mindream.XGraph.Model;
using Mindream.XTreeListView.LibraryViewModels;
using XGraph.ViewModels;
using XSerialization;
using XTreeListView.Gui;

namespace DemoApplication
{
    /// <summary>
    ///     Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Fields

        /// <summary>
        ///     This field stores the position where a drag is started.
        /// </summary>
        private Point mStartPoint;

        /// <summary>
        ///     This field stores the position where a drop is done.
        /// </summary>
        private Point mDropPoint;

        /// <summary>
        ///     This field stores the call graph.
        /// </summary>
        private Task mCurrentTask;

        /// <summary>
        ///     This field stores the graph view model.
        /// </summary>
        private TaskViewModel mTaskViewModel;

        /// <summary>
        ///     This field stores the selected view model
        /// </summary>
        private CallNodeViewModel mSelectedViewModel;


        /// <summary>
        ///     Gets the instance.
        /// </summary>
        public static MainWindow Instance
        {
            get;
            private set;
        }

        #endregion // Fields.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            var lComponentDescriptorRegistry = new ComponentDescriptorRegistry();
            lComponentDescriptorRegistry.FindAllDescriptors();
            this.mComponentDescriptorLibrary.ViewModel = new ComponentDescriptorRegistryViewModel(lComponentDescriptorRegistry);
            this.mGraph.SelectionChanged += this.OnSelectionChanged;
            this.NewProject();
            Instance = this;
            Mindream.WPF.Components.Inputs.Keyboard.EventNotifier = this;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        ///     Called when [drop].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
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
        ///     Called when [drop].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="DragEventArgs" /> instance containing the event data.</param>
        private void OnDrop(object pSender, DragEventArgs pEventArgs)
        {
            if (pEventArgs.Data.GetDataPresent("ComponentDescriptor"))
            {
                var lDescriptor = pEventArgs.Data.GetData("ComponentDescriptor") as IComponentDescriptor;
                if (lDescriptor != null && pSender is IInputElement)
                {
                    this.mDropPoint = pEventArgs.GetPosition((IInputElement) pSender);
                    this.mTaskViewModel.NodeAdded += this.OnNodeAdded;
                    this.mCurrentTask.CallNodes.Add(new LocatableCallNode {Component = lDescriptor.Create()});
                }
            }
        }

        /// <summary>
        ///     Called when [node added].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The event arguments.</param>
        private void OnNodeAdded(GraphViewModel pSender, NodeViewModel pEventArgs)
        {
            Point lGraphPos;
            if (this.mGraph.MapToGraph(this.mDropPoint, out lGraphPos) == false)
            {
                // Drop point is not in the graph limits. Adding the node at (0,0).
                lGraphPos = new Point();
            }

            pEventArgs.X = lGraphPos.X;
            pEventArgs.Y = lGraphPos.Y;

            this.mTaskViewModel.NodeAdded -= this.OnNodeAdded;
        }

        /// <summary>
        ///     Called when [connection added].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The p event arguments.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnConnectionAdded(GraphViewModel pSender, ConnectionViewModel pEventArgs)
        {
            // Look for the input node.
            var lInputViewModel = this.mTaskViewModel.Nodes.FirstOrDefault(pNode => pNode.Ports.Contains(pEventArgs.Input)) as CallNodeViewModel;

            // Look for the ouput node.
            var lOutputViewModel = this.mTaskViewModel.Nodes.FirstOrDefault(pNode => pNode.Ports.Contains(pEventArgs.Output)) as CallNodeViewModel;

            // Create the connection in call graph.
            if (lInputViewModel != null && lOutputViewModel != null && pEventArgs.Input is PortStartViewModel && pEventArgs.Output is PortEndedViewModel)
            {
                this.mCurrentTask.ConnectCall(lOutputViewModel.Node, lInputViewModel.Node, pEventArgs.Output.DisplayString);
            }

            if (lInputViewModel != null && lOutputViewModel != null && pEventArgs.Input is InputParameterViewModel && pEventArgs.Output is OutputParameterViewModel)
            {
                this.mCurrentTask.ConnectParameter(lOutputViewModel.Node, pEventArgs.Output.DisplayString, lInputViewModel.Node, pEventArgs.Input.DisplayString);
            }
        }

        /// <summary>
        ///     Called when [library preview mouse left button down].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void OnLibraryPreviewMouseLeftButtonDown(object pSender, MouseButtonEventArgs pEventArgs)
        {
            // Store the mouse position
            this.mStartPoint = pEventArgs.GetPosition(null);
        }

        /// <summary>
        ///     Called when [library mouse move].
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void OnLibraryMouseMove(object pSender, MouseEventArgs pEventArgs)
        {
            // Get the pCurrent mouse position
            var lMousePos = pEventArgs.GetPosition(null);
            var lDiff = this.mStartPoint - lMousePos;

            if (pEventArgs.LeftButton == MouseButtonState.Pressed && (Math.Abs(lDiff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(lDiff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                var lListViewItem = FindAnchestor<TreeListViewItem>((DependencyObject) pEventArgs.OriginalSource);
                if (lListViewItem == null)
                {
                    return;
                }

                // Initialize the drag & drop operation
                var lDragData = new DataObject("ComponentDescriptor", lListViewItem.ViewModel.UntypedOwnedObject);
                DragDrop.DoDragDrop(lListViewItem, lDragData, DragDropEffects.Move);
            }
        }

        /// <summary>
        ///     Finds the anchestor.
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
                    return (T) pCurrent;
                }
                pCurrent = VisualTreeHelper.GetParent(pCurrent);
            }
            while (pCurrent != null);
            return null;
        }

        /// <summary>
        ///     Handles the click event on "Start".
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void StartClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            if (this.mSelectedViewModel != null)
            {
                this.mSelectedViewModel.Node.Start();
            }
        }

        /// <summary>
        ///     Handles the click event on "New project".
        /// </summary>
        /// <param name="pSender">The event sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void NewClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            this.NewProject();
        }

        /// <summary>
        ///     Removes the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void RemoveClicked(object pSender, RoutedEventArgs pEventArgs)
        {

        }

        /// <summary>
        ///     Loads the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void LoadClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            var lDialog = new OpenFileDialog
            {
                Filter = "Task Files(*.task.xml)|*.task.xml|All(*.*)|*"
            };

            if (lDialog.ShowDialog() == true)
            {
                var lSerializer = new XSerializer();
                var lCallGraph = lSerializer.Deserialize(lDialog.FileName) as Task;
                this.NewProject();
            }
        }

        /// <summary>
        ///     Saves the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SaveClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            var lDialog = new SaveFileDialog
            {
                Filter = "Task Files(*.task.xml)|*.task.xml|All(*.*)|*"
            };

            if (lDialog.ShowDialog() == true)
            {
                var lSerializer = new XSerializer();
                lSerializer.Serialize(this.mCurrentTask, lDialog.FileName);
            }
        }

        /// <summary>
        ///     Saves the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CloseClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            this.mTaskViewModel.ConnectionAdded -= this.OnConnectionAdded;
            this.mTaskViewModel = null;
            this.mGraph.DataContext = null;
            this.mCurrentTask = null;
        }

        /// <summary>
        ///     Saves the clicked.
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CollectClicked(object pSender, RoutedEventArgs pEventArgs)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete();
            GC.Collect();
        }

        /// <summary>
        ///     News the project.
        /// </summary>
        private void NewProject()
        {
            TaskManager.Instance.ClearAll();
            this.mCurrentTask = TaskManager.Instance.CreateTask();
            this.mTaskViewModel = new TaskViewModel(this.mCurrentTask);
            this.mGraph.DataContext = this.mTaskViewModel;
        }

        #endregion // Methods.
    }
}