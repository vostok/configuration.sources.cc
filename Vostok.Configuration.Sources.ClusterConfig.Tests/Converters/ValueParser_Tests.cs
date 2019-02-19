using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.ClusterConfig.SettingsNodeConverters;
using Vostok.Configuration.Sources.ClusterConfig.Tests.Helpers;

namespace Vostok.Configuration.Sources.ClusterConfig.Tests.Converters
{
    [TestFixture]
    internal class ValueParser_Tests : TreeConstructionSet
    {
        private Func<string, ISettingsNode> parser;
        private ValueParser converter;

        [SetUp]
        public void SetUp()
        {
            parser = Substitute.For<Func<string, ISettingsNode>>();
            converter = new ValueParser(parser);
        }
        
        [Test]
        public void Should_parse_ObjectNode_with_single_child_when_parser_is_not_null()
        {
            var parsedValue = Value("parsed");

            parser.Invoke("value").Returns(parsedValue);
            
            converter.Convert(Object(Value("value")))
                .Should()
                .Be(parsedValue);
        }

        [Test]
        public void Should_throw_for_ArrayNode_when_parser_is_not_null()
        {
            new Action(() => converter.Convert(Array(Value("value"))))
                .Should()
                .Throw<Exception>();
        }
        
        [Test]
        public void Should_throw_for_ValueNode_when_parser_is_not_null()
        {
            new Action(() => converter.Convert(Value("value")))
                .Should()
                .Throw<Exception>();
        }
    }
}