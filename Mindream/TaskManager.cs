using Mindream.CallGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using XSerialization.Attributes;

namespace Mindream
{
    /// <summary>
    ///     Represents the current stage in the lifecycle of a Mindream.Task.
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        ///     The task has been created but has not yet been started.
        /// </summary>
        Created = 0,

        /// <summary>
        ///     The task is running but has not yet completed.
        /// </summary>
        Running = 1,

        /// <summary>
        ///     The task is suspended (waiting for a resume or stop).
        /// </summary>
        Suspended = 2,

        /// <summary>
        ///     The task is implicitly waiting for attached child tasks to complete.
        /// </summary>
        WaitingForChildrenToComplete = 3,

        /// <summary>
        ///     The task completed execution successfully.
        /// </summary>
        Terminated = 4,

        /// <summary>
        ///     The task has been stopped explicitly by the manager.
        /// </summary>
        Stopped = 6,

        /// <summary>
        ///     The task stopped due to an error.
        /// </summary>
        Faulted = 7
    }

    /// <summary>
    ///     This class is used internally to the manager.
    /// </summary>
    /// <seealso cref="Mindream.CallGraph.Task" />
    internal class ManagedTask : Task
    {
        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>
        ///     The state.
        /// </value>
        [SkipXSerialization]
        public TaskState State
        {
            get;
            set;
        }
    }

    /// <summary>
    ///     This class represent a manager which manages creation, destruction, debug of tasks.
    /// </summary>
    public class TaskManager
    {
        #region Fields

        /// <summary>
        ///     The container of all tasks.
        /// </summary>
        private readonly Dictionary<string, ManagedTask> mTasks;

        /// <summary>
        ///     The ms instance
        /// </summary>
        private static readonly TaskManager INSTANCE;

        /// <summary>
        /// This field stores the current simulation step.
        /// </summary>
        private int mSimulationStep;

        #endregion // Fields.

        #region Properties

        /// <summary>
        ///     Gets the tasks.
        /// </summary>
        /// <value>
        ///     The tasks.
        /// </value>
        public IEnumerable<Task> Tasks
        {
            get
            {
                return this.mTasks.Values;
            }
        }

        /// <summary>
        ///     Gets the unique instance of the class.
        /// </summary>
        /// <value>
        ///     The instance of the class.
        /// </value>
        public static TaskManager Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the task having the given id.
        /// </summary>
        /// <param name="pId">The task id to look for.</param>
        /// <returns>The found task, null otherwise.</returns>
        public Task this[string pId]
        {
            get
            {
                return this.GetTask( pId );
            }
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="TaskManager" /> class.
        /// </summary>
        static TaskManager()
        {
            INSTANCE = new TaskManager();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskManager" /> class.
        /// </summary>
        private TaskManager()
        {
            this.mTasks = new Dictionary<string, ManagedTask>();
        }

        #endregion // Constructors.

        #region Events

        /// <summary>
        ///     Occurs when a new task is created.
        /// </summary>
        public event Action<TaskManager, Task> TaskCreated;

        /// <summary>
        ///     Occurs when a new task is renamed.
        /// </summary>
        public event Action<TaskManager, Task> TaskRenamed;

        /// <summary>
        ///     Occurs when a task is started.
        /// </summary>
        public event Action<TaskManager, Task, bool> TaskStarted;

        /// <summary>
        ///     Occurs when a new task is stopped.
        /// </summary>
        public event Action<TaskManager, Task> TaskStopped;

        /// <summary>
        ///     Occurs when a new task is suspended.
        /// </summary>
        public event Action<TaskManager, Task> TaskSuspended;

        /// <summary>
        ///     Occurs when a new task is destroyed.
        /// </summary>
        public event Action<TaskManager, Task> TaskDestroyed;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// Gets the task having the given id.
        /// </summary>
        /// <param name="pId">The task id</param>
        /// <returns>The task if found, null otherwise.</returns>
        public Task GetTask(string pId)
        {
            if ( string.IsNullOrEmpty( pId ) )
            {
                return null;
            }

            ManagedTask lToReturn;
            if ( this.mTasks.TryGetValue( pId, out lToReturn ) )
            {
                return lToReturn;
            }

            return null;
        }

        /// <summary>
        ///     Creates the task.
        /// </summary>
        /// <returns>The new created task</returns>
        public Task CreateTask(string pId)
        {
            ManagedTask lTask;
            if ( this.mTasks.TryGetValue( pId, out lTask ) )
            {
                // Already exist, Edit it?? Implicit get.
                return lTask;
            }

            // Not existing, create it.
            lTask = new ManagedTask() { Id = pId };
            this.mTasks.Add( pId, lTask);
            if (this.TaskCreated != null)
            {
                this.TaskCreated(this, lTask);
            }

            return lTask;
        }

        /// <summary>
        ///     Destroys a task.
        /// </summary>
        /// <param name="pTask">The task to destroy.</param>
        /// <returns>True if the destruction occured, false otherwise.</returns>
        public bool DestroyTask(Task pTask)
        {
            ManagedTask lTask = pTask as ManagedTask;
            if ( lTask != null )
            {
                if ( lTask.State == TaskState.Running )
                {
                    this.InternalStop( lTask );
                }

                if ( this.mTasks.Remove( lTask.Id ) )
                {
                    if (this.TaskDestroyed != null)
                    {
                        this.TaskDestroyed(this, pTask);
                    }

                    pTask.Dispose();
                    return true;
                }
            }

            pTask.Dispose();
            return false;
        }

        /// <summary>
        ///     Clears all tasks from the manager.
        /// </summary>
        /// <returns>The number of removed tasks.</returns>
        public int ClearAll()
        {
            this.StopAll();
            var lTaskCount = this.mTasks.Count;
            for (var lIndex = 0; lIndex < lTaskCount; lIndex++)
            {
                this.DestroyTask( this.mTasks.Values.First() );
            }
            return lTaskCount;
        }

        /// <summary>
        ///     Restarts all tasks (stop and start)
        /// </summary>
        public void RestartAll()
        {
            this.StopAll();
            this.StartAll();
        }

        /// <summary>
        ///     Starts all tasks.
        /// </summary>
        /// <param name="pOnlyNotRunningTask"></param>
        public void StartAll(bool pOnlyNotRunningTask = false)
        {
            if (pOnlyNotRunningTask == false && this.IsRunning == false)
            {
                this.mSimulationStep = 0;
            }
            this.IsRunning = true;
            var lNotRunningTasks = this.mTasks.Values.Where(pTask => pTask.State != TaskState.Running).ToList();
            foreach (var lTask in lNotRunningTasks)
            {
                this.StartTask(lTask);
            }
        }

        /// <summary>
        ///     Updates all running tasks.
        /// </summary>
        public void UpdateAll(TimeSpan pDeltaTime)
        {
            var lRunningTasks = this.mTasks.Values.Where(pTask => pTask.State == TaskState.Running).ToList();
            foreach (var lTask in lRunningTasks)
            {
                lTask.Update(pDeltaTime, this.mSimulationStep);
            }
            this.mSimulationStep++;
        }

        /// <summary>
        ///     Stops all running tasks.
        /// </summary>
        public void StopAll()
        {
            var lRunningTasks = this.mTasks.Values.Where(pTask => pTask.State == TaskState.Running).ToList();
            foreach (var lTask in lRunningTasks)
            {
                foreach (var lCallNode in lTask.CallNodes)
                {
                    lCallNode.RestoreValues();
                }

                this.StopTask(lTask);
            }

            this.IsRunning = false;
            this.mSimulationStep = 0;
        }

        /// <summary>
        /// Stores all values before execution.
        /// </summary>
        public void StoreAllValues()
        {           
            var lNotRunningTasks = this.mTasks.Values.Where(pTask => pTask.State != TaskState.Running).ToList();
            foreach (var lTask in lNotRunningTasks)
            {
                foreach (var lCallNode in lTask.CallNodes)
                {
                    lCallNode.StoreValues();
                }
            }
        }

        /// <summary>
        /// Renames a task if the id is not used.
        /// </summary>
        /// <param name="pTask">The task to rename.</param>
        /// <param name="pNewId">The new identifier.</param>
        /// <returns>True if the renaming succeed, false otherwise.</returns>
        public bool RenameTask(Task pTask, string pNewId)
        {
            ManagedTask lTask;
            if (this.mTasks.TryGetValue(pNewId, out lTask))
            {
                return false;
            }

            if (this.mTasks.TryGetValue(pTask.Id, out lTask))
            {
                this.mTasks.Remove(pTask.Id);
                this.mTasks.Add(pNewId, lTask);
                if (this.TaskRenamed != null)
                {
                    this.TaskRenamed(this, lTask);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Starts a task.
        /// </summary>
        /// <returns></returns>
        public bool StartTask(Task pTask)
        {
            var lTask = pTask as ManagedTask;
            if ( lTask != null && 
                 this.mTasks.ContainsKey( lTask.Id ) && 
                 lTask.State != TaskState.Running)
            {
                // Look for the first node.
                foreach (var lEntryNode in lTask.EntryNodes)
                {
                    lEntryNode.Start(string.Empty, this.mSimulationStep);
                }
                
                lTask.State = TaskState.Running;
                if (this.TaskStarted != null)
                {
                    this.TaskStarted(this, lTask, lTask.State != TaskState.Suspended);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Stop a task.
        /// </summary>
        /// <returns></returns>
        public bool StopTask(Task pTask)
        {
            ManagedTask lTask = pTask as ManagedTask;
            if ( lTask != null && 
                 this.mTasks.ContainsKey( lTask.Id ) && 
                 lTask.State == TaskState.Running )
            {
                this.InternalStop( lTask );

                return true;
            }

            return false;
        }

        /// <summary>
        /// Internal task stop
        /// Note: This method assumes task is not null.
        /// </summary>
        /// <param name="pTask">The task to stop.</param>
        private void InternalStop(ManagedTask pTask)
        {
            foreach ( var lCallNode in pTask.CallNodes )
            {
                lCallNode.Abort();
            }

            pTask.State = TaskState.Stopped;

            if (this.TaskStopped != null)
            {
                this.TaskStopped( this, pTask);
            }
        }

        /// <summary>
        ///     Stop a task.
        /// </summary>
        /// <returns></returns>
        public bool SuspendTask(Task pTask)
        {
            var lTask = pTask as ManagedTask;
            if ( lTask != null && 
                 this.mTasks.ContainsKey( lTask.Id ) && 
                 lTask.State == TaskState.Running )
            {
                lTask.State = TaskState.Suspended;
                // Do something on call node.
                if (this.TaskSuspended != null)
                {
                    this.TaskSuspended(this, lTask);
                }

                return true;
            }
            return false;
        }

        /**
         * This method is called when a node is frozen by a breakpoint.
         */
        internal void Break(CallNode pBreakNode)
        {
            Task lBrokenTask = this.Tasks.FirstOrDefault(pTask => pTask.CallNodes.Contains(pBreakNode));
            if (lBrokenTask != null)
            {
                foreach (var lCallNode in lBrokenTask.CallNodes)
                {
                    lCallNode.IsBroken = true;
                }
            }
        }

        /**
        * This method is called when a task must continue after a break.
        */
        public void Continue(Task pTask)
        {
            foreach (var lCallNode in pTask.CallNodes)
            {
                lCallNode.IsBroken = false;
            }

            // Restart all nodes with the state "BreakEnd"
            foreach (var lCallNode in pTask.CallNodes)
            {
                if (lCallNode.State == CallNodeState.BreakEnd)
                {
                    lCallNode.Continue = true;
                    lCallNode.State = CallNodeState.Undefined;
                    lCallNode.OnComponentReturned(null, lCallNode.BreakInfo);
                    lCallNode.Continue = false;
                }
            }

            // Restart all nodes with the state "FreezeByBreak" and "BreakStart"
            foreach (var lCallNode in pTask.CallNodes)
            {
                if (lCallNode.State == CallNodeState.BreakStart)
                {
                    lCallNode.Continue = true;
                    lCallNode.State = CallNodeState.Undefined;
                    lCallNode.Start(lCallNode.BreakInfo, this.mSimulationStep);
                    lCallNode.Continue = false;
                }

                if (lCallNode.State == CallNodeState.FreezeByBreak)
                {
                    lCallNode.State = CallNodeState.Undefined;
                    lCallNode.Start(lCallNode.BreakInfo, this.mSimulationStep);
                }
            }
        }

        #endregion // Methods.
    }
}