/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using Org.Apache.REEF.Utilities.Logging;
using Org.Apache.REEF.Tang.Annotations;
using System;

namespace Org.Apache.REEF.Wake.Impl
{
    /// <summary>A logging event handler</summary>
    public class LoggingEventHandler<T> : IObserver<T>
    {
        private static readonly Logger LOGGER = Logger.GetLogger(typeof(T));

        [Inject]
        public LoggingEventHandler()
        {
        }

        /// <summary>Logs the event</summary>
        /// <param name="value">an event</param>
        public void OnNext(T value)
        {
            LOGGER.Log(Level.Verbose, "Event: " + DateTime.Now + value);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
