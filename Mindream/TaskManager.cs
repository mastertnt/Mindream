using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Task = Mindream.CallGraph.Task;

namespace Mindream
{
    // Summary:
    //     Represents the current stage in the lifecycle of a Mindream.Task.
    public enum TaskState
    {
        // Summary:
        //     The task has been created but has not yet been started.
        Created = 0,
        //
        // Summary:
        //     The task is running but has not yet completed.
        Running = 1,
        //
        // Summary:
        //     The task is suspended (waiting for a resume or stop).
        Suspended = 2,
        //
        // Summary:
        //     The task is implicitly waiting for attached child tasks to complete.
        WaitingForChildrenToComplete = 3,
        //
        // Summary:
        //     The task completed execution successfully.
        Terminated = 4,
        //
        // Summary:
        //     The task has been stopped explicitly by the manager.
        Stopped = 6,
        //
        // Summary:
        //     The task stopped due to an error.
        Faulted = 7,
    }

    /// <summary>
    /// This class is used internally to the manager.
    /// </summary>
    /// <seealso cref="Mindream.CallGraph.Task"/>
    internal class ManagedTask : Task
    {
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public TaskState State
        {
            get;
            set;
        }
    }

    /// <summary>
    /// This class represent a manager which manages creation, destruction, debug of tasks.
    /// </summary>
    public class TaskManager
    {
        #region Fields

        /// <summary>
        /// The container of all tasks.
        /// </summary>
        private readonly HashSet<ManagedTask> mTasks;

        /// <summary>
        /// The ms instance
        /// </summary>
        private static readonly TaskManager INSTANCE;

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets the tasks.
        /// </summary>
        /// <value>
        /// The tasks.
        /// </value>
        public IEnumerable<Task> Tasks
        {
            get
            {
                return this.mTasks;
            }
        }

        /// <summary>
        /// Gets the unique instance of the class.
        /// </summary>
        /// <value>
        /// The instance of the class.
        /// </value>
        public static TaskManager Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="TaskManager"/> class.
        /// </summary>
        static TaskManager()
        {
            INSTANCE = new TaskManager();            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManager"/> class.
        /// </summary>
        internal TaskManager()
        {
            this.mTasks = new HashSet<ManagedTask>();
        }

        #endregion // Constructors.

        #region Events

        /// <summary>
        /// Occurs when a new task is created.
        /// </summary>
        public event Action<TaskManager, Task> TaskCreated;

        /// <summary>
        /// Occurs when a task is started.
        /// </summary>
        public event Action<TaskManager, Task, bool> TaskStarted;

        /// <summary>
        /// Occurs when a new task is stopped.
        /// </summary>
        public event Action<TaskManager, Task> TaskStopped;

        /// <summary>
        /// Occurs when a new task is suspended.
        /// </summary>
        public event Action<TaskManager, Task> TaskSuspended;

        /// <summary>
        /// Occurs when a new task is destroyed.
        /// </summary>
        public event Action<TaskManager, Task> TaskDestroyed;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// Creates the task.
        /// </summary>
        /// <returns>The new created task</returns>
        public Task CreateTask()
        {
            ManagedTask lTask = new ManagedTask();
            this.mTasks.Add(lTask);
            if (this.TaskCreated != null)
            {
                this.TaskCreated(this, lTask);
            }
            return lTask;
        }

        /// <summary>
        /// Destroys a task.
        /// </summary>
        /// <param name="pTask">The task to destroy.</param>
        /// <returns>True if the destruction occured, false otherwise.</returns>
        public bool DestroyTask(Task pTask)
        {
            ManagedTask lTask = pTask as ManagedTask;
            if (this.mTasks.Contains(lTask))
            {
                this.mTasks.Remove(lTask);
                if (this.TaskDestroyed != null)
                {
                    this.TaskDestroyed(this, pTask);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all tasks from the manager.
        /// </summary>
        /// <returns>The number of removed tasks.</returns>
        public int ClearAll()
        {
            int lTaskCount = this.mTasks.Count;
            for (int lIndex = 0; lIndex < lTaskCount; lIndex++)
            {
                this.DestroyTask(this.mTasks.First());
            }
            return lTaskCount;
        }

        /// <summary>
        /// Restarts all tasks (stop and start)
        /// </summary>
        public void RestartAll()
        {
            this.StopAll();
            this.StartAll();
        }

        /// <summary>
        /// Starts all running tasks.
        /// </summary>
        public void StartAll()
        {
            List<ManagedTask> lNotRunningTasks = this.mTasks.Where(pTask => pTask.State != TaskState.Running).ToList();
            foreach (var lTask in lNotRunningTasks)
            {
                this.StartTask(lTask);
            }
        }

        /// <summary>
        /// Stops all running tasks.
        /// </summary>
        public void StopAll()
        {
            List<ManagedTask> lRunningTasks = this.mTasks.Where(pTask => pTask.State == TaskState.Running).ToList();
            foreach (var lTask in lRunningTasks)
            {
                this.StopTask(lTask);
            }
        }

        /// <summary>
        /// Starts a task.
        /// </summary>
        /// <returns></returns>
        public bool StartTask(Task pTask)
        {
            ManagedTask lTask = pTask as ManagedTask;
            if (this.mTasks.Contains(lTask) && lTask != null && lTask.State != TaskState.Running)
            {
                // Look for the first node.
                lTask.EntryNode.Start();
                if (this.TaskStarted != null)
                {
                    this.TaskStarted(this, lTask, lTask.State != TaskState.Suspended);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stop a task.
        /// </summary>
        /// <returns></returns>
        public bool StopTask(Task pTask)
        {
            ManagedTask lTask = pTask as ManagedTask;
            if (this.mTasks.Contains(lTask) && lTask != null && lTask.State == TaskState.Running)
            {
                // TODO.
                if (this.TaskStopped != null)
                {
                    this.TaskStopped(this, lTask);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Stop a task.
        /// </summary>
        /// <returns></returns>
        public bool SuspendTask(Task pTask)
        {
            ManagedTask lTask = pTask as ManagedTask;
            if (this.mTasks.Contains(lTask) && lTask != null && lTask.State == TaskState.Running)
            {
                // TODO.
                if (this.TaskSuspended != null)
                {
                    this.TaskSuspended(this, lTask);
                }

                return true;
            }
            return false;
        }

        

        #endregion // Methods.
    }
}
