using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    #region ingame script start

    abstract class Task
    {
        private static int lastId = 0;

        public int id;
        public int priority = -1; // auto
        public string Name;
        public bool IsEnqueued;

        public Task(string name = null)
        {
            Name = name;
            id = lastId;
            lastId = (lastId + 1) % ushort.MaxValue;
        }

        public abstract bool Run();
        public abstract string TypeCode();
        public string DisplayName()
        {
            return string.Format("*{0}* {1}-{2}", TypeCode(), id, Name ?? "-");
        }

        public abstract int InstructionsLimit();
    }

    abstract class Task<T> : Task
    {
        public Action<T> Done;
        public T result;

        public Task(string name = null) : base(name) { }

        abstract public bool DoWork();
        virtual public void EndWork() { }
        protected void DoDone()
        {
            EndWork();
            if (Done != null)
            {
                Done(result);
            }
        }
    }

    /* not used
    abstract class InterruptibleTask<T> : Task<T>
    {

        public InterruptibleTask(string name = null) : base(name) { }

        internal bool Timeout()
        {
            return Program.Current.Runtime.CurrentInstructionCount > (Program.Current.Runtime.MaxInstructionCount - Scheduler.TASK_END_LIMIT);
        }

        public override bool Run()
        {
            bool done = DoWork();
            if (done)
            {
                DoDone();
            }

            return done;
        }

        public override string TypeCode()
        {
            return "INT";
        }

        public override int InstructionsLimit()
        {
            return int.MaxValue;
        }
    }
    */


    abstract class FastTask<T> : Task<T>
    {
        public override bool Run()
        {
            DoWork();
            DoDone();
            return true;
        }
        public FastTask(string name = null) : base(name) { }

        public override string TypeCode()
        {
            return "FST";
        }

        public override int InstructionsLimit()
        {
            return 3000;
        }
    }

    /* not used
    abstract class ContinuousTask<T> : Task<T>
    {
        public ContinuousTask(string name = null) : base(name) { }

        public override bool Run()
        {
            DoWork();
            return false;
        }

        public override string TypeCode()
        {
            return "CNT";
        }

        public override int InstructionsLimit()
        {
            return 3000;
        }
    }
    */

    /* not used
    class SimpleTask<T> : FastTask<T>
    {
        Func<T> _task;

        public override bool DoWork()
        {
            result = _task();
            return true;
        }

        public override string TypeCode()
        {
            return "SMP";
        }

        public SimpleTask(Func<T> task, string name = null) : base(name)
        {
            _task = task;
        }

        public SimpleTask(Action task, string name = null) : base(name)
        {
            _task = () =>
            {
                task();
                return default(T);
            };
        }
    }
    */

    class Scheduler
    {
        public const string LOG_CAT = "sch";

        public const int PRIORITY_LOWEST = 4;
        public const int DEFAULT_PRIORITY = 2;
        public const int TASK_END_LIMIT = 7000;
        public const int SCHEDULLER_START_LIMIT = 10000;

        List<List<Task>> tasks;
        Task current = null;

        public Scheduler()
        {
            tasks = new List<List<Task>>();
            for(int i = 0; i <= PRIORITY_LOWEST; i++)
            {
                tasks.Add(new List<Task>());
            }
        }

        internal bool CanStartTask(int limit)
        {
            return limit < Program.Current.Runtime.MaxInstructionCount - Program.Current.Runtime.CurrentInstructionCount - TASK_END_LIMIT;
        }

        internal bool Timeout()
        {
            return Program.Current.Runtime.CurrentInstructionCount > (Program.Current.Runtime.MaxInstructionCount - SCHEDULLER_START_LIMIT);
        }

        public bool HasTasks()
        {
            return tasks.FirstOrDefault(x => x.Count > 0) != null;
        }

        int startInst = 0, endInst = 0, instLimit = 1;
        string taskName = "";

        IMyGridProgramRuntimeInfo Runtime()
        {
            return Program.Current.Runtime;
        }

        void ShowInstructionsAlertIfNeeded()
        {
            if ((endInst - startInst) > instLimit)
            {
                Log.WriteFormat(LOG_CAT, LogLevel.Error, "\uE056 Task \"{2}\" exceeded instructions limit ({0,5}/{1})!\n", endInst - startInst, instLimit, taskName);
                endInst = 0;
                startInst = 0;
                instLimit = 1;
            }
        }
         
        public void Run()
        {
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "instructions: {0,5}/{1}", Runtime().CurrentInstructionCount,
                Runtime().MaxInstructionCount);
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "tasks: {0}", string.Join("/", tasks.Select(x => x.Count)));

            Queue<Task> taskBuffer = new Queue<Task>();

            ShowInstructionsAlertIfNeeded();

            while (!Timeout() && HasTasks())
            {
                ShowInstructionsAlertIfNeeded();
                
                startInst = Runtime().CurrentInstructionCount;
                Task task = DequeTaskInt();
                current = task;

                Log.WriteFormat(LOG_CAT, LogLevel.Verbose, task.DisplayName());
                instLimit = task.InstructionsLimit();
                taskName = task.DisplayName();
                if (CanStartTask(instLimit))
                {
                    if (!task.Run())
                    {
                        taskBuffer.Enqueue(task);
                    }
                }
                else
                {
                    Log.Write(LOG_CAT, LogLevel.Verbose, "task requares more instructions then awailable, skipping");
                    taskBuffer.Enqueue(task);
                }
                current = null;

                endInst = Runtime().CurrentInstructionCount;
                Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "instructions: {0,5}/{1}", endInst,
                    Runtime().MaxInstructionCount);

            }

            while (taskBuffer.Count != 0)
            {
                EnqueueTaskInt(taskBuffer.Dequeue());
            }
        }

        public void EnqueueTask(Task task)
        {
            Log.WriteFormat(LOG_CAT, LogLevel.Verbose, "new {0} task: {1}", task.priority == -1 ? "pA" : "p" + task.priority.ToString(), task.DisplayName());
            EnqueueTaskInt(task);
        }

        public void CancelTask(Task task)
        {
            foreach (var queue in tasks)
            {
                queue.Remove(task);
            }
        }

        private Task DequeTaskInt()
        {
            var queue = tasks.FirstOrDefault(q => q.Count > 0);
            if (queue == null)
            {
                return null;
            }
            else
            {
                var task = queue[0];
                queue.RemoveAt(0);
                task.IsEnqueued = false;
                return task;
            }
        }

        private void EnqueueTaskInt(Task task)
        {
            if (task.priority == -1)
            {
                task.priority = current != null ? current.priority : DEFAULT_PRIORITY;
            }
            task.IsEnqueued = true;
            tasks[task.priority].Add(task);
        }
    }

    #endregion // ingame script end
}
