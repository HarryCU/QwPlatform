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

using System.Globalization;
using System.Threading;

namespace QwMicroKernel.Threading
{
    /// <summary>
    /// Support class used to handle threads
    /// </summary>
    /// <author>Marko Lahma (.NET)</author>
    public abstract class QwThread : IThreadRunnable
    {
        /// <summary>
        /// The instance of System.Threading.Thread
        /// </summary>
        private readonly Thread _thread;

        /// <summary>
        /// Initializes a new instance of the QThread class
        /// </summary>
        protected QwThread()
        {
            _thread = new Thread(Run);
        }

        /// <summary>
        /// Initializes a new instance of the Thread class.
        /// </summary>
        /// <param name="name">The name of the thread</param>
        protected QwThread(string name)
        {
            _thread = new Thread(Run);
            Name = name;
        }

        /// <summary>
        /// This method has no functionality unless the method is overridden
        /// </summary>
        public virtual void Run()
        {
        }

        /// <summary>
        /// Causes the operating system to change the state of the current thread instance to ThreadState.Running
        /// </summary>
        public void Start()
        {
            _thread.Start();
        }

        /// <summary>
        /// Interrupts a thread that is in the WaitSleepJoin thread state
        /// </summary>
        protected void Interrupt()
        {
            _thread.Interrupt();
        }

        /// <summary>
        /// Gets or sets the name of the thread
        /// </summary>
        public string Name
        {
            get { return _thread.Name; }
            protected set
            {
                _thread.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of a thread
        /// </summary>
        protected ThreadPriority Priority
        {
            get { return _thread.Priority; }
            set { _thread.Priority = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a thread is a background thread.
        /// </summary>
        protected bool IsBackground
        {
            set { _thread.IsBackground = value; }
        }

        /// <summary>
        /// Blocks the calling thread until a thread terminates
        /// </summary>
        public void Join()
        {
            _thread.Join();
        }

        public void Stop()
        {
            _thread.Abort();
        }

        /// <summary>
        /// Obtain a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Thread[{0},{1},]", Name, Priority);
        }
    }
}
