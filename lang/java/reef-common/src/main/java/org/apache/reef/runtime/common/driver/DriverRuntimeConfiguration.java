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
package org.apache.reef.runtime.common.driver;

import org.apache.reef.annotations.audience.ClientSide;
import org.apache.reef.annotations.audience.Private;
import org.apache.reef.driver.catalog.ResourceCatalog;
import org.apache.reef.driver.client.JobMessageObserver;
import org.apache.reef.driver.evaluator.EvaluatorRequestor;
import org.apache.reef.driver.parameters.DriverIdleSources;
import org.apache.reef.runtime.common.driver.api.RuntimeParameters;
import org.apache.reef.runtime.common.driver.catalog.ResourceCatalogImpl;
import org.apache.reef.runtime.common.driver.client.ClientManager;
import org.apache.reef.runtime.common.driver.client.JobMessageObserverImpl;
import org.apache.reef.runtime.common.driver.idle.ClockIdlenessSource;
import org.apache.reef.runtime.common.driver.idle.EventHandlerIdlenessSource;
import org.apache.reef.runtime.common.driver.resourcemanager.NodeDescriptorHandler;
import org.apache.reef.runtime.common.driver.resourcemanager.ResourceAllocationHandler;
import org.apache.reef.runtime.common.driver.resourcemanager.ResourceManagerStatus;
import org.apache.reef.runtime.common.driver.resourcemanager.ResourceStatusHandler;
import org.apache.reef.tang.formats.ConfigurationModule;
import org.apache.reef.tang.formats.ConfigurationModuleBuilder;
import org.apache.reef.wake.time.Clock;

@Private
@ClientSide
public final class DriverRuntimeConfiguration extends ConfigurationModuleBuilder {

  public static final ConfigurationModule CONF = new DriverRuntimeConfiguration()
      // Resource Catalog
      .bindImplementation(ResourceCatalog.class, ResourceCatalogImpl.class)

          // JobMessageObserver
      .bindImplementation(EvaluatorRequestor.class, EvaluatorRequestorImpl.class) // requesting evaluators
      .bindImplementation(JobMessageObserver.class, JobMessageObserverImpl.class) // sending message to job client

          // Client manager
      .bindNamedParameter(DriverRuntimeConfigurationOptions.JobControlHandler.class, ClientManager.class)

          // Bind the resourcemanager parameters
      .bindNamedParameter(RuntimeParameters.NodeDescriptorHandler.class, NodeDescriptorHandler.class)
      .bindNamedParameter(RuntimeParameters.ResourceAllocationHandler.class, ResourceAllocationHandler.class)
      .bindNamedParameter(RuntimeParameters.ResourceStatusHandler.class, ResourceStatusHandler.class)
      .bindNamedParameter(RuntimeParameters.RuntimeStatusHandler.class, ResourceManagerStatus.class)

          // Bind to the Clock
      .bindSetEntry(Clock.RuntimeStartHandler.class, DriverRuntimeStartHandler.class)
      .bindSetEntry(Clock.RuntimeStopHandler.class, DriverRuntimeStopHandler.class)

          // Bind the idle handlers
      .bindSetEntry(DriverIdleSources.class, ClockIdlenessSource.class)
      .bindSetEntry(DriverIdleSources.class, EventHandlerIdlenessSource.class)
      .bindSetEntry(DriverIdleSources.class, ResourceManagerStatus.class)
      .bindSetEntry(Clock.IdleHandler.class, ClockIdlenessSource.class)

      .build();
}
