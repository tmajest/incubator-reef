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
package org.apache.reef.common;

import org.apache.reef.io.naming.Identifiable;
import org.apache.reef.util.Optional;

/**
 * Common interface for all error messages in REEF.
 * Most of its functionality is generic and implemented in the AbstractFailure class.
 */
public interface Failure extends Identifiable {

  /**
   * @return One-line error message. Should never be null.
   */
  String getMessage();

  /**
   * @return Optional long error description.
   */
  Optional<String> getDescription();

  /**
   * @return Java Exception that caused the error, if any.
   */
  Optional<Throwable> getReason();


  /**
   * @return Optional serialized version of the error message.
   */
  Optional<byte[]> getData();

  /**
   * Return the original Java Exception, or generate a new one if it does not exists.
   * ALWAYS returns an exception.
   *
   * @return A java exception. Never null.
   */
  Throwable asError();
}
