﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueuedSync.cs" company="The original author or authors.">
//   Copyright 2002-2012 the original author or authors.
//   
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
//   the License. You may obtain a copy of the License at
//   
//   http://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
//   an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
//   specific language governing permissions and limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Spring.Threading.Helpers
{
    /// <summary>
    /// Used by <see cref="WaitNode"/>.
    /// NOTE: this class is NOT present in java.util.concurrent.
    /// </summary>
    internal interface IQueuedSync
    {
        // was WaitQueue.QueuedSync in BACKPORT_3_1
        // invoked with sync on wait node, (atomically) just before enqueuing
        /// <summary>The recheck.</summary>
        /// <param name="node">The node.</param>
        /// <returns>The System.Boolean.</returns>
        bool Recheck(WaitNode node);

        // invoked with sync on wait node, (atomically) just before signalling
        /// <summary>The take over.</summary>
        /// <param name="node">The node.</param>
        void TakeOver(WaitNode node);
    }
}
