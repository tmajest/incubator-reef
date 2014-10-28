/**
 * Copyright (C) 2014 Microsoft Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *         http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package org.apache.reef.io;

import org.apache.reef.annotations.Unstable;

// FIXME:
// This was another more type safe
// alternative to integer partitions
@Unstable
public class PartitionSpec {
  private final Type type;
  private final int id;

  public PartitionSpec(final Type type, final int id) {
    this.type = type;
    this.id = id;
  }

  public Type getType() {
    return type;
  }

  public int getId() {
    return id;
  }

  public enum Type {
    SINGLE,
    ALL,
    NONE
  }
}