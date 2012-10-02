﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueParser.cs" company="The original author or authors.">
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

#region Using Directives
using System;
using System.Xml;
using Spring.Messaging.Amqp.Core;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;
#endregion

namespace Spring.Messaging.Amqp.Rabbit.Config
{
    /// <summary>
    /// A queue parser.
    /// </summary>
    public class QueueParser : AbstractSingleObjectDefinitionParser
    {
        private static readonly string ARGUMENTS_ELEMENT = "queue-arguments";

        private static readonly string ARGUMENTS_PROPERTY = "Arguments";

        private static readonly string DURABLE_ATTRIBUTE = "durable";

        private static readonly string EXCLUSIVE_ATTRIBUTE = "exclusive";

        private static readonly string AUTO_DELETE_ATTRIBUTE = "auto-delete";

        /// <summary>Gets a value indicating whether should generate id as fallback.</summary>
        protected override bool ShouldGenerateIdAsFallback { get { return true; } }

        /// <summary>The get object type.</summary>
        /// <param name="element">The element.</param>
        /// <returns>The System.Type.</returns>
        protected override Type GetObjectType(XmlElement element)
        {
            if (NamespaceUtils.IsAttributeDefined(element, "name"))
            {
                return typeof(Queue);
            }
            else
            {
                return typeof(AnonymousQueue);
            }
        }

        /// <summary>The do parse.</summary>
        /// <param name="element">The element.</param>
        /// <param name="parserContext">The parser context.</param>
        /// <param name="builder">The builder.</param>
        protected override void DoParse(XmlElement element, ParserContext parserContext, ObjectDefinitionBuilder builder)
        {
            if (!NamespaceUtils.IsAttributeDefined(element, "name") && !NamespaceUtils.IsAttributeDefined(element, ID_ATTRIBUTE))
            {
                parserContext.ReaderContext.ReportFatalException(element, "Queue must have either id or name (or both)");
            }

            var success = NamespaceUtils.AddConstructorArgValueIfAttributeDefined(builder, element, "name");

            if (!NamespaceUtils.IsAttributeDefined(element, "name"))
            {
                if (this.AttributeHasIllegalOverride(element, DURABLE_ATTRIBUTE, "false")
                    || this.AttributeHasIllegalOverride(element, EXCLUSIVE_ATTRIBUTE, "true")
                    || this.AttributeHasIllegalOverride(element, AUTO_DELETE_ATTRIBUTE, "true"))
                {
                    parserContext.ReaderContext.ReportFatalException(element, "Anonymous queue cannot specify durable='true', exclusive='false' or auto-delete='false'");
                    return;
                }
            }
            else
            {
                NamespaceUtils.AddConstructorArgBooleanValueIfAttributeDefined(builder, element, DURABLE_ATTRIBUTE, false);
                NamespaceUtils.AddConstructorArgBooleanValueIfAttributeDefined(builder, element, EXCLUSIVE_ATTRIBUTE, false);
                NamespaceUtils.AddConstructorArgBooleanValueIfAttributeDefined(builder, element, AUTO_DELETE_ATTRIBUTE, false);
            }

            var argumentsElement = element.GetElementsByTagName(ARGUMENTS_ELEMENT, element.NamespaceURI);

            if (argumentsElement.Count == 1)
            {
                var parser = new ObjectDefinitionParserHelper(parserContext);
                var map = parser.ParseMapElement(argumentsElement[0] as XmlElement, builder.RawObjectDefinition);

                var convertedMap = parser.ConvertToManagedDictionary<string, object>(map);

                builder.AddConstructorArg(convertedMap);
            }
        }

        private bool AttributeHasIllegalOverride(XmlElement element, string name, string allowed)
        {
            var result = element.GetAttributeNode(name) != null && element.GetAttributeNode(name).Specified && !allowed.Equals(element.GetAttribute(name));
            return result;
        }
    }
}
