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
package org.apache.reef.tests.fail.task;

import org.apache.reef.task.Task;
import org.apache.reef.task.events.TaskStart;
import org.apache.reef.tests.library.exceptions.SimulatedTaskFailure;
import org.apache.reef.wake.EventHandler;

import javax.inject.Inject;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * A basic task that just fails when we invoke it.
 */
public final class FailTaskStart implements Task, EventHandler<TaskStart> {

  private static final Logger LOG = Logger.getLogger(FailTaskStart.class.getName());

  private transient boolean isRunning = true;

  @Inject
  public FailTaskStart() {
    LOG.info("FailTaskStart created.");
  }

  @Override
  public byte[] call(final byte[] memento) {
    synchronized (this) {
      LOG.info("FailTaskStart.call() invoked. Waiting for the message.");
      while (this.isRunning) {
        try {
          this.wait();
        } catch (final InterruptedException ex) {
          LOG.log(Level.WARNING, "wait() interrupted.", ex);
        }
      }
    }
    return new byte[0];
  }

  @Override
  public void onNext(final TaskStart event) throws SimulatedTaskFailure {
    final SimulatedTaskFailure ex = new SimulatedTaskFailure("FailTaskStart.send() invoked.");
    LOG.log(Level.FINE, "FailTaskStart.onNext() invoked: {0}", ex);
    throw ex;
  }
}
