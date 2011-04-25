using System;
using NUnit.Framework;

namespace Rf.Common.Tests
{
    [TestFixture]
    public class TinyIocTests
    {
        private TinyIoc _ioc;

        [TestFixtureSetUp]
        public void Given()
        {
            _ioc = new TinyIoc();
            _ioc.Configure(c =>
                               {
                                   c.Add<IFoo>(ctx => new Bar());
                                   c.Add<IFoo>("baz", ctx => new Baz());
                                   c.AddSingleton(ctx=> new Qux());

                                   c.Add<IDependable>(ctx => new Depending(ctx.Get<IFoo>()));
                                   c.Add<IDependable>("b", ctx => new Depending(ctx.Get<IFoo>("baz")));
                               });
        }

        [Test]
        public void missing_registration_throws()
        {
            Assert.Throws<ArgumentException>(() => _ioc.Get<IDisposable>());
        }

        [Test]
        public void unknown_key_throws()
        {
            Assert.Throws<ArgumentException>(() => _ioc.Get<IFoo>("bang"));
        }

        [Test]
        public void gets_bar_as_default()
        {
            var instance = _ioc.Get<IFoo>();
            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<Bar>();
        }

        [Test]
        public void get_the_keyed_instance()
        {
            var instance = _ioc.Get<IFoo>("baz");
            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<Baz>();
        }

        [Test]
        public void registered_singleton_works()
        {
            var instance = _ioc.Get<Qux>();
            var instance2 = _ioc.Get<Qux>();
            ReferenceEquals(instance, instance2).ShouldBeTrue();
        }

        [Test]
        public void default_is_new_object()
        {
            var instance = _ioc.Get<IFoo>();
            var instance2 = _ioc.Get<IFoo>();
            ReferenceEquals(instance, instance2).ShouldBeFalse();
        }

        [Test]
        public void resolve_works()
        {
            var dep = GetAndAssertTopObject("default");
            dep.Foo.ShouldBeOfType<Bar>();
        }

        [Test]
        public void keyed_resolve_works()
        {
            var dep = GetAndAssertTopObject("b");
            dep.Foo.ShouldBeOfType<Baz>();
        }

        private Depending GetAndAssertTopObject(string key)
        {
            var instance = _ioc.Get<IDependable>(key);
            instance.ShouldBeOfType<Depending>();
            return (Depending)instance;
        }
    }


    interface IFoo { }
    interface IDependable { }
    class Bar : IFoo { }
    class Baz : IFoo { }

    class Depending : IDependable
    {
        public readonly IFoo Foo;

        public Depending(IFoo foo)
        {
            Foo = foo;
        }
    }

    class Qux { }
}