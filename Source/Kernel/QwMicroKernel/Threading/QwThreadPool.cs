#region License
/* 
 * All content copyright Terracotta, Inc., unless otherwise indicated. All rights reserved. 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at 
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0 
 *   
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations 
 * under the License.
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace QwMicroKernel.Threading
{
    /// <summary>
    /// This is class is a simple implementation of a thread pool, based on the
    /// <see cref="IThreadPool" /> interface.
    /// </summary>
    /// <remarks>
    /// <see cref="IThreadRunnable" /> objects are sent to the pool with the <see cref="RunInThread" />
    /// method, which blocks until a <see cref="Thread" /> becomes available.
    /// 
    /// The pool has a fixed number of <see cref="Thread" />s, and does not grow or
    /// shrink based on demand.
    /// </remarks>
    /// <author>James House</author>
    /// <author>Juergen Donnerstag</author>
    /// <author>Marko Lahma (.NET)</author>
    public class QwThreadPool : IThreadPool
    {
        private const int DefaultThreadPoolSize = 10;

        private readonly object _nextRunnableLock = new object();
        private readonly LinkedList<WorkerThread> _availWorkers = new LinkedList<WorkerThread>();
        private readonly LinkedList<WorkerThread> _busyWorkers = new LinkedList<WorkerThread>();

        private int _count = DefaultThreadPoolSize;
        private bool _handoffPending;
        private bool _isShutdown;
        private ThreadPriority _prio = ThreadPriority.Normal;
        private string _schedulerInstanceName;

        private List<WorkerThread> _workers;

        /// <summary> 
        /// Create a new (unconfigured) <see cref="QwThreadPool" />.
        /// </summary>
        public QwThreadPool()
        {
        }

        /// <summary>
        /// Create a new <see cref="QwThreadPool" /> with the specified number
        /// of <see cref="Thread" /> s that have the given priority.
        /// </summary>
        /// <param name="threadCount">
        /// the number of worker <see cref="Thread" />s in the pool, must
        /// be > 0.
        /// </param>
        /// <param name="threadPriority">
        /// the thread priority for the worker threads.
        /// 
        /// </param>
        public QwThreadPool(int threadCount, ThreadPriority threadPriority)
        {
            ThreadCount = threadCount;
            ThreadPriority = threadPriority;
        }

        /// <summary>
        /// Gets or sets the number of worker threads in the pool.
        /// Set  has no effect after <see cref="Initialize()" /> has been called.
        /// </summary>
        public int ThreadCount
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// Get or set the thread priority of worker threads in the pool.
        /// Set operation has no effect after <see cref="Initialize()" /> has been called.
        /// </summary>
        public ThreadPriority ThreadPriority
        {
            get { return _prio; }
            set { _prio = value; }
        }

        /// <summary>
        /// Gets or sets the thread name prefix.
        /// </summary>
        /// <value>The thread name prefix.</value>
        public virtual string ThreadNamePrefix { get; set; }

        /// <summary> 
        /// Gets or sets the value of makeThreadsDaemons.
        /// </summary>
        public virtual bool MakeThreadsDaemons { get; set; }

        /// <summary>
        /// Gets the size of the pool.
        /// </summary>
        /// <value>The size of the pool.</value>
        public virtual int PoolSize
        {
            get { return ThreadCount; }
        }

        /// <summary>
        /// Inform the <see cref="IThreadPool" /> of the Scheduler instance's Id, 
        /// prior to initialize being invoked.
        /// </summary>
        public virtual string InstanceId
        {
            set { }
        }

        /// <summary>
        /// Inform the <see cref="IThreadPool" /> of the Scheduler instance's name, 
        /// prior to initialize being invoked.
        /// </summary>
        public virtual string InstanceName
        {
            set { _schedulerInstanceName = value; }
        }

        /// <summary>
        /// Called by the QuartzScheduler before the <see cref="ThreadPool" /> is
        /// used, in order to give the it a chance to Initialize.
        /// </summary>
        public virtual void Initialize()
        {
            if (_workers != null && _workers.Count > 0)
            {
                // already initialized...
                return;
            }

            if (_count <= 0)
            {
                throw new Exception("Thread count must be > 0");
            }

            // create the worker threads and start them
            foreach (WorkerThread wt in CreateWorkerThreads(_count))
            {
                wt.Start();
                _availWorkers.AddLast(wt);
            }
        }

        /// <summary>
        /// Terminate any worker threads in this thread group.
        /// Jobs currently in progress will complete.
        /// </summary>
        public virtual void Shutdown(bool waitForJobsToComplete)
        {
            // Give waiting (wait(1000)) worker threads a chance to shut down.
            // Active worker threads will shut down after finishing their
            // current job.
            lock (_nextRunnableLock)
            {
                _isShutdown = true;

                if (_workers == null) // case where the pool wasn't even initialize()ed
                {
                    return;
                }

                // signal each worker thread to shut down
                foreach (WorkerThread thread in _workers)
                {
                    if (thread != null)
                    {
                        thread.Shutdown();
                    }
                }
                Monitor.PulseAll(_nextRunnableLock);


                if (waitForJobsToComplete)
                {
                    bool interrupted = false;
                    try
                    {
                        // wait for hand-off in runInThread to complete...
                        while (_handoffPending)
                        {
                            try
                            {
                                Monitor.Wait(_nextRunnableLock, 100);
                            }
                            catch (ThreadInterruptedException)
                            {
                                interrupted = true;
                            }
                        }

                        // Wait until all worker threads are shut down
                        while (_busyWorkers.Count > 0)
                        {
                            LinkedListNode<WorkerThread> wt = _busyWorkers.First;
                            try
                            {
                                // note: with waiting infinite time the
                                // application may appear to 'hang'.
                                Monitor.Wait(_nextRunnableLock, 2000);
                            }
                            catch (ThreadInterruptedException)
                            {
                                interrupted = true;
                            }
                        }

                        while (_workers.Count > 0)
                        {
                            int index = _workers.Count - 1;
                            WorkerThread wt = _workers[index];
                            try
                            {
                                wt.Join();
                                _workers.RemoveAt(index);
                            }
                            catch (ThreadInterruptedException)
                            {
                                interrupted = true;
                            }
                        }
                    }
                    finally
                    {
                        if (interrupted)
                        {
                            Thread.CurrentThread.Interrupt();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Run the given <see cref="IThreadRunnable" /> object in the next available
        /// <see cref="Thread" />. If while waiting the thread pool is asked to
        /// shut down, the Runnable is executed immediately within a new additional
        /// thread.
        /// </summary>
        /// <param name="runnable">The <see cref="IThreadRunnable" /> to be added.</param>
        public virtual bool RunInThread(IThreadRunnable runnable)
        {
            if (runnable == null)
            {
                return false;
            }

            lock (_nextRunnableLock)
            {
                _handoffPending = true;

                // Wait until a worker thread is available
                while ((_availWorkers.Count < 1) && !_isShutdown)
                {
                    try
                    {
                        Monitor.Wait(_nextRunnableLock, 500);
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                }

                if (!_isShutdown)
                {
                    WorkerThread wt = _availWorkers.First.Value;
                    _availWorkers.RemoveFirst();
                    _busyWorkers.AddLast(wt);
                    wt.Run(runnable);
                }
                else
                {
                    // If the thread pool is going down, execute the Runnable
                    // within a new additional worker thread (no thread from the pool).
                    WorkerThread wt = new WorkerThread(this, "WorkerThread-LastJob", _prio, MakeThreadsDaemons, runnable);
                    _busyWorkers.AddLast(wt);
                    _workers.Add(wt);
                    wt.Start();
                }
                Monitor.PulseAll(_nextRunnableLock);
                _handoffPending = false;
            }

            return true;
        }

        public int BlockForAvailableThreads()
        {
            lock (_nextRunnableLock)
            {
                while ((_availWorkers.Count < 1 || _handoffPending) && !_isShutdown)
                {
                    try
                    {
                        Monitor.Wait(_nextRunnableLock, 500);
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                }

                return _availWorkers.Count;
            }
        }

        protected void MakeAvailable(WorkerThread wt)
        {
            lock (_nextRunnableLock)
            {
                if (!_isShutdown)
                {
                    _availWorkers.AddLast(wt);
                }
                _busyWorkers.Remove(wt);
                Monitor.PulseAll(_nextRunnableLock);
            }
        }

        /// <summary>
        /// Creates the worker threads.
        /// </summary>
        /// <param name="threadCount">The thread count.</param>
        /// <returns></returns>
        protected virtual IList<WorkerThread> CreateWorkerThreads(int threadCount)
        {
            _workers = new List<WorkerThread>();
            for (int i = 1; i <= threadCount; ++i)
            {
                string threadPrefix = ThreadNamePrefix;
                if (threadPrefix == null)
                {
                    threadPrefix = _schedulerInstanceName + "_Worker";
                }

                var workerThread = new WorkerThread(
                    this,
                    string.Format(CultureInfo.InvariantCulture, "{0}-{1}", threadPrefix, i),
                    ThreadPriority,
                    MakeThreadsDaemons);

                _workers.Add(workerThread);
            }

            return _workers;
        }

        /// <summary>
        /// Terminate any worker threads in this thread group.
        /// Jobs currently in progress will complete.
        /// </summary>
        public virtual void Shutdown()
        {
            Shutdown(true);
        }

        protected virtual void ClearFromBusyWorkersList(WorkerThread wt)
        {
            lock (_nextRunnableLock)
            {
                _busyWorkers.Remove(wt);
                Monitor.PulseAll(_nextRunnableLock);
            }
        }

        /// <summary>
        /// A Worker loops, waiting to Execute tasks.
        /// </summary>
        protected class WorkerThread : QwThread
        {
            private readonly object _lockObject = new object();

            // A flag that signals the WorkerThread to terminate.
            private volatile bool _run = true;

            private IThreadRunnable _runnable;
            private readonly QwThreadPool _tp;
            private readonly bool _runOnce;

            /// <summary>
            /// Create a worker thread and start it. Waiting for the next Runnable,
            /// executing it, and waiting for the next Runnable, until the Shutdown
            /// flag is set.
            /// </summary>
            internal WorkerThread(QwThreadPool tp, string name,
                                  ThreadPriority prio, bool isDaemon)
                : this(tp, name, prio, isDaemon, null)
            {
            }

            /// <summary>
            /// Create a worker thread, start it, Execute the runnable and terminate
            /// the thread (one time execution).
            /// </summary>
            internal WorkerThread(QwThreadPool tp, string name,
                                  ThreadPriority prio, bool isDaemon, IThreadRunnable runnable)
                : base(name)
            {
                _tp = tp;
                _runnable = runnable;
                if (runnable != null)
                {
                    _runOnce = true;
                }
                Priority = prio;
                IsBackground = isDaemon;
            }

            /// <summary>
            /// Signal the thread that it should terminate.
            /// </summary>
            internal virtual void Shutdown()
            {
                _run = false;
            }

            public void Run(IThreadRunnable newRunnable)
            {
                lock (_lockObject)
                {
                    if (_runnable != null)
                    {
                        throw new ArgumentException("Already running a Runnable!");
                    }

                    _runnable = newRunnable;
                    Monitor.PulseAll(_lockObject);
                }
            }

            /// <summary>
            /// Loop, executing targets as they are received.
            /// </summary>
            public override void Run()
            {
                bool ran = false;

                while (_run)
                {
                    try
                    {
                        lock (_lockObject)
                        {
                            while (_runnable == null && _run)
                            {
                                Monitor.Wait(_lockObject, 500);
                            }
                            if (_runnable != null)
                            {
                                ran = true;
                                _runnable.Run();
                            }

                        }
                    }
                    catch (Exception exceptionInRunnable)
                    {
                        throw exceptionInRunnable;
                    }
                    finally
                    {
                        lock (_lockObject)
                        {
                            _runnable = null;
                        }
                        // repair the thread in case the runnable mucked it up...
                        Priority = _tp.ThreadPriority;

                        if (_runOnce)
                        {
                            _run = false;
                            _tp.ClearFromBusyWorkersList(this);
                        }
                        else if (ran)
                        {
                            ran = false;
                            _tp.MakeAvailable(this);
                        }
                    }
                }
            }
        }
    }
}
