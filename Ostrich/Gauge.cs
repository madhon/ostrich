/*
 *  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */
namespace Ostrich
{
    using System;
    using System.Reflection;
    using Ostrich.Logging;
    using Ostrich.Util;

    public class Gauge
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly Func<double> func;

        public Gauge(Func<double> func)
        {
            Guard.AgainstNullArgument("func", func);
            this.func = func;
        }

        public double Value
        {
            get
            {
                try
                {
                    return func();
                }
                catch (Exception e)
                {
                    Logger.Warn(e.Message);
                }

                return 0;
            }
        }
    }
}
