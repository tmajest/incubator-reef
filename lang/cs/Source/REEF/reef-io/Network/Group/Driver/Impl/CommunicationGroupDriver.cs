﻿/**
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

using Org.Apache.Reef.IO.Network.Group.Config;
using Org.Apache.Reef.IO.Network.Group.Operators.Impl;
using Org.Apache.Reef.IO.Network.Group.Topology;
using Org.Apache.Reef.IO.Network.Utilities;
using Org.Apache.Reef.Utilities.Logging;
using Org.Apache.Reef.Tang.Exceptions;
using Org.Apache.Reef.Tang.Formats;
using Org.Apache.Reef.Tang.Implementations;
using Org.Apache.Reef.Tang.Interface;
using Org.Apache.Reef.Tang.Util;
using System.Collections.Generic;
using System.Reflection;

namespace Org.Apache.Reef.IO.Network.Group.Driver.Impl
{
    /// <summary>
    /// Used to configure MPI operators in Reef driver.
    /// All operators in the same Communication Group run on the the 
    /// same set of tasks.
    /// </summary>
    public class CommunicationGroupDriver : ICommunicationGroupDriver
    {
        private static readonly Logger LOGGER = Logger.GetLogger(typeof(CommunicationGroupDriver));

        private string _groupName;
        private string _driverId;
        private int _numTasks;
        private int _tasksAdded;
        private bool _finalized;

        private AvroConfigurationSerializer _confSerializer;

        private object _topologyLock;
        private Dictionary<string, object> _operatorSpecs;
        private Dictionary<string, object> _topologies;

        /// <summary>
        /// Create a new CommunicationGroupDriver.
        /// </summary>
        /// <param name="groupName">The communication group name</param>
        /// <param name="driverId">Identifier of the Reef driver</param>
        /// <param name="numTasks">The number of tasks each operator will use</param>
        /// <param name="confSerializer">Used to serialize task configuration</param>
        public CommunicationGroupDriver(
            string groupName,
            string driverId, 
            int numTasks,
            AvroConfigurationSerializer confSerializer)
        {
            _confSerializer = confSerializer;
            _groupName = groupName;
            _driverId = driverId;
            _numTasks = numTasks;
            _tasksAdded = 0;
            _finalized = false;

            _topologyLock = new object();

            _operatorSpecs = new Dictionary<string, object>();
            _topologies = new Dictionary<string, object>();
            TaskIds = new List<string>();
        }

        /// <summary>
        /// Returns the list of task ids that belong to this Communication Group
        /// </summary>
        public List<string> TaskIds { get; private set; } 

        /// <summary>
        /// Adds the Broadcast MPI operator to the communication group.
        /// </summary>
        /// <typeparam name="T">The type of messages that operators will send</typeparam>
        /// <param name="operatorName">The name of the broadcast operator</param>
        /// <param name="spec">The specification that defines the Broadcast operator</param>
        /// <returns>The same CommunicationGroupDriver with the added Broadcast operator info</returns>
        public ICommunicationGroupDriver AddBroadcast<T>(
            string operatorName, 
            BroadcastOperatorSpec<T> spec)
        {
            if (_finalized)
            {
                throw new IllegalStateException("Can't add operators once the spec has been built.");
            }

            ITopology<T> topology = new FlatTopology<T>(operatorName, _groupName, spec.SenderId, _driverId, spec);
            _topologies[operatorName] = topology;
            _operatorSpecs[operatorName] = spec;

            return this;
        }

        /// <summary>
        /// Adds the Reduce MPI operator to the communication group.
        /// </summary>
        /// <typeparam name="T">The type of messages that operators will send</typeparam>
        /// <param name="operatorName">The name of the reduce operator</param>
        /// <param name="spec">The specification that defines the Reduce operator</param>
        /// <returns>The same CommunicationGroupDriver with the added Reduce operator info</returns>
        public ICommunicationGroupDriver AddReduce<T>(
            string operatorName, 
            ReduceOperatorSpec<T> spec)
        {
            if (_finalized)
            {
                throw new IllegalStateException("Can't add operators once the spec has been built.");
            }

            ITopology<T> topology = new FlatTopology<T>(operatorName, _groupName, spec.ReceiverId, _driverId, spec);
            _topologies[operatorName] = topology;
            _operatorSpecs[operatorName] = spec;

            return this;
        }

        /// <summary>
        /// Adds the Scatter MPI operator to the communication group.
        /// </summary>
        /// <typeparam name="T">The type of messages that operators will send</typeparam>
        /// <param name="operatorName">The name of the scatter operator</param>
        /// <param name="spec">The specification that defines the Scatter operator</param>
        /// <returns>The same CommunicationGroupDriver with the added Scatter operator info</returns>
        public ICommunicationGroupDriver AddScatter<T>(string operatorName, ScatterOperatorSpec<T> spec)
        {
            if (_finalized)
            {
                throw new IllegalStateException("Can't add operators once the spec has been built.");
            }

            ITopology<T> topology = new FlatTopology<T>(operatorName, _groupName, spec.SenderId, _driverId, spec);
            _topologies[operatorName] = topology;
            _operatorSpecs[operatorName] = spec;

            return this;
        }

        /// <summary>
        /// Finalizes the CommunicationGroupDriver.
        /// After the CommunicationGroupDriver has been finalized, no more operators may
        /// be added to the group.
        /// </summary>
        /// <returns>The same finalized CommunicationGroupDriver</returns>
        public ICommunicationGroupDriver Build()
        {
            _finalized = true;
            return this;
        }

        /// <summary>
        /// Add a task to the communication group.
        /// The CommunicationGroupDriver must have called Build() before adding tasks to the group.
        /// </summary>
        /// <param name="taskId">The id of the task to add</param>
        public void AddTask(string taskId)
        {
            if (!_finalized)
            {
                throw new IllegalStateException("CommunicationGroupDriver must call Build() before adding tasks to the group.");
            }

            lock (_topologyLock)
            {
                _tasksAdded++;
                if (_tasksAdded > _numTasks)
                {
                    throw new IllegalStateException("Added too many tasks to Communication Group, expected: " + _numTasks);
                }

                TaskIds.Add(taskId);
                foreach (string operatorName in _operatorSpecs.Keys)
                {
                    AddTask(operatorName, taskId);
                }
            }
        }

        /// <summary>
        /// Get the Task Configuration for this communication group. 
        /// Must be called only after all tasks have been added to the CommunicationGroupDriver.
        /// </summary>
        /// <param name="taskId">The task id of the task that belongs to this Communication Group</param>
        /// <returns>The Task Configuration for this communication group</returns>
        public IConfiguration GetGroupTaskConfiguration(string taskId)
        {
            if (!TaskIds.Contains(taskId))
            {
                return null;
            }

            // Make sure all tasks have been added to communication group before generating config
            lock (_topologyLock)
            {
                if (_tasksAdded != _numTasks)
                {
                    throw new IllegalStateException(
                        "Must add all tasks to communication group before fetching configuration");
                }
            }

            var confBuilder = TangFactory.GetTang().NewConfigurationBuilder()
                .BindNamedParameter<MpiConfigurationOptions.DriverId, string>(
                    GenericType<MpiConfigurationOptions.DriverId>.Class,
                    _driverId)
                .BindNamedParameter<MpiConfigurationOptions.CommunicationGroupName, string>(
                    GenericType<MpiConfigurationOptions.CommunicationGroupName>.Class,
                    _groupName);

            foreach (var operatorName in _topologies.Keys)
            {
                var innerConf = TangFactory.GetTang().NewConfigurationBuilder(GetOperatorConfiguration(operatorName, taskId))
                    .BindNamedParameter<MpiConfigurationOptions.DriverId, string>(
                        GenericType<MpiConfigurationOptions.DriverId>.Class,
                        _driverId)
                    .BindNamedParameter<MpiConfigurationOptions.OperatorName, string>(
                        GenericType<MpiConfigurationOptions.OperatorName>.Class,
                        operatorName)
                    .Build();

                confBuilder.BindSetEntry<MpiConfigurationOptions.SerializedOperatorConfigs, string>(
                    GenericType<MpiConfigurationOptions.SerializedOperatorConfigs>.Class,
                    _confSerializer.ToString(innerConf));
            }

            return confBuilder.Build();
        }

        private void AddTask(string operatorName, string taskId)
        {
            var topology = _topologies[operatorName];
            MethodInfo info = topology.GetType().GetMethod("AddTask");
            info.Invoke(topology, new[] { (object) taskId });
        }

        private IConfiguration GetOperatorConfiguration(string operatorName, string taskId)
        {
            var topology = _topologies[operatorName];
            MethodInfo info = topology.GetType().GetMethod("GetTaskConfiguration");
            return (IConfiguration) info.Invoke(topology, new[] { (object) taskId });
        }
    }
}