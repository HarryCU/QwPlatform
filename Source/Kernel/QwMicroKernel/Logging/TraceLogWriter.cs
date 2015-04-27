// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;

namespace QwMicroKernel.Logging
{
    public sealed class TraceLogWriter : EmptyLogWriter
    {
        private readonly TraceSource _source;

        public TraceLogWriter(TraceSource source)
            : base(LoggingLevel.FromSourceLevels(source.Switch.Level))
        {
            _source = source;
        }

        protected override void LogInternal(LoggingLevel level, object obj, Exception exception)
        {
            string message = obj == null ? string.Empty : obj.ToString();

            if (exception == null)
                _source.TraceEvent(level.TraceEventType, 0, message);
            else
                _source.TraceData(level.TraceEventType, 0, (object)message, (object)exception);
        }
    }
}