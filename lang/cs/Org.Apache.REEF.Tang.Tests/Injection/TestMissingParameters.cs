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

using System;
using Org.Apache.REEF.Tang.Annotations;
using Org.Apache.REEF.Tang.Exceptions;
using Org.Apache.REEF.Tang.Implementations;
using Org.Apache.REEF.Tang.Interface;
using Org.Apache.REEF.Tang.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Apache.REEF.Tang.Implementations.Tang;

namespace Org.Apache.REEF.Tang.Tests.Injection
{
    [TestClass]
    public class TestMissingParameters
    {
        [TestMethod]
        public void MultipleParameterTest()
        {
            ICsConfigurationBuilder cb = TangFactory.GetTang().NewConfigurationBuilder();
            cb.BindNamedParameter<MultiParameterConstructor.NamedString, string>(GenericType<MultiParameterConstructor.NamedString>.Class, "foo");
            cb.BindNamedParameter<MultiParameterConstructor.NamedInt, int>(GenericType<MultiParameterConstructor.NamedInt>.Class, "8");
            cb.BindNamedParameter<MultiParameterConstructor.NamedBool, bool>(GenericType<MultiParameterConstructor.NamedBool>.Class, "true");
            IInjector i = TangFactory.GetTang().NewInjector(cb.Build());
            var o = i.GetInstance<MultiParameterConstructor>();
            o.Verify("foo", 8, true);
        }

        [TestMethod]
        public void MissingAllParameterTest()
        {
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //missing arguments: [ 
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor+NamedBool, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor+NamedString, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor+NamedInt, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //]
            MultiParameterConstructor obj = null;
            try
            {
                ICsConfigurationBuilder cb = TangFactory.GetTang().NewConfigurationBuilder();
                IInjector i = TangFactory.GetTang().NewInjector(cb.Build());
                obj = i.GetInstance<MultiParameterConstructor>();
            }
            catch (InjectionException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            Assert.IsNull(obj);
        }

        [TestMethod]
        public void MissingTwoParameterTest()
        {
            //Cannot inject Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null: 
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //missing arguments: [ 
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor+NamedString, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor+NamedInt, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //]
            MultiParameterConstructor obj = null;
            try
            {
                ICsConfigurationBuilder cb = TangFactory.GetTang().NewConfigurationBuilder();
                cb.BindNamedParameter<MultiParameterConstructor.NamedBool, bool>(GenericType<MultiParameterConstructor.NamedBool>.Class, "true");
                IInjector i = TangFactory.GetTang().NewInjector(cb.Build());
                obj = i.GetInstance<MultiParameterConstructor>();
            }
            catch (InjectionException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            Assert.IsNull(obj);
        }

        [TestMethod]
        public void MissingOneParameterTest()
        {
            //Cannot inject Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null: 
            //Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null 
            //missing argument Org.Apache.REEF.Tang.Tests.Injection.MultiParameterConstructor+NamedInt, Org.Apache.REEF.Tang.Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
            MultiParameterConstructor obj = null;
            try
            {
                ICsConfigurationBuilder cb = TangFactory.GetTang().NewConfigurationBuilder();
                cb.BindNamedParameter<MultiParameterConstructor.NamedBool, bool>(GenericType<MultiParameterConstructor.NamedBool>.Class, "true");
                cb.BindNamedParameter<MultiParameterConstructor.NamedString, string>(GenericType<MultiParameterConstructor.NamedString>.Class, "foo");
                IInjector i = TangFactory.GetTang().NewInjector(cb.Build());
                obj = i.GetInstance<MultiParameterConstructor>();
            }
            catch (InjectionException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            Assert.IsNull(obj);
        }
    }

    public class MultiParameterConstructor
    {
        private readonly string str;
        private readonly int iVal;
        private readonly bool bVal;

        [Inject]
        public MultiParameterConstructor([Parameter(typeof(NamedBool))] bool b, [Parameter(typeof(NamedString))] string s, [Parameter(typeof(NamedInt))] int i)
        {
            this.str = s;
            this.iVal = i;
            this.bVal = b;
        }

        public void Verify(string s, int i, bool b)
        {
            Assert.AreEqual(str, s);
            Assert.AreEqual(iVal, i);
            Assert.AreEqual(bVal, b);
        }

        [NamedParameter]
        public class NamedString : Name<string>
        {
        }

        [NamedParameter]
        public class NamedInt : Name<int>
        {
        }

        [NamedParameter]
        public class NamedBool : Name<bool>
        {
        }
    }
}