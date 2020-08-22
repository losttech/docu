using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Docu.Documentation.Comments;
using Docu.Events;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    public abstract class BaseDocumentModelGeneratorFixture : BaseFixture
    {
        protected ICommentParser StubParser;
        public IEventAggregator StubEventAggregator;
        private IContainer container;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            container = ContainerBootstrapper.BootstrapStructureMap();
        }

        [SetUp]
        public void CreateStubs()
        {
            StubEventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            StubParser = MockRepository.GenerateStub<ICommentParser>();
            StubParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());
        }

        public ICommentParser RealParser { get { return container.GetInstance<ICommentParser>(); } }

        protected DocumentedType Type<T>(string xml)
        {
            return new DocumentedType(Identifier.FromType(typeof(T)), xml.ToNode(), typeof(T));
        }

        protected DocumentedType Type(Type type, string xml)
        {
            return new DocumentedType(Identifier.FromType(type), xml.ToNode(), type);
        }

        protected DocumentedMethod Method<T>(string xml, Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return new DocumentedMethod(Identifier.FromMethod(method, typeof(T)), xml.ToNode(), method, typeof(T));
        }

        protected DocumentedProperty Property<T>(string xml, Expression<Func<T, object>> propertyAction)
        {
            var property = ((MemberExpression)propertyAction.Body).Member as PropertyInfo;

            return new DocumentedProperty(Identifier.FromProperty(property, typeof(T)), xml.ToNode(), property, typeof(T));
        }

        protected DocumentedField Field<T>(string xml, Expression<Func<T, object>> fieldAction)
        {
            var field = ((MemberExpression)fieldAction.Body).Member as FieldInfo;

            return new DocumentedField(Identifier.FromField(field, typeof(T)), xml.ToNode(), field, typeof(T));
        }

        protected DocumentedEvent Event<T>(string xml, string eventName)
        {
            var ev = typeof(T).GetEvent(eventName);
            return new DocumentedEvent(Identifier.FromEvent(ev, typeof(T)), xml.ToNode(), ev, typeof(T));
        }
    }
}