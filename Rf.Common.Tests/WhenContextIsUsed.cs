using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Playground;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

namespace Rf.Common.Tests
{
  [TestFixture]
  public class WhenContextIsUsed
  {
    [Test]
    public void ContextAcceptsOneOfAKind()
    {
      var ctx = new StandardContext();
      ctx.Add("Hello");
      ctx.Get<string>().ShouldBeEqualTo("Hello");
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void AddingTwiceMakesContextChoke()
    {
      var ctx = new StandardContext();
      ctx.Add("Hello");
      ctx.Add("World");
    }

    [Test]
    public void PrimitivePolymorphismIsThere()
    {
      var ctx = new StandardContext();
      var reader = new BinaryReader(new MemoryStream());
      ctx.Add<IDisposable>(reader);
      ctx.Get<IDisposable>().ShouldBeTheSameAs(reader);
    }

    [Test]
    public void PartnersCanBeChanged()
    {
      var ctx = new StandardContext();
      ctx.Add("Hotrod");
      ctx.Replace("Douchebag");
      ctx.Get<string>().ShouldBeEqualTo("Douchebag");
    }

    [Test]
    public void ContextLovesExtensions()
    {
      var extension = MockRepository
        .GenerateMock<IContextExtension<StandardContext>>();
      var ctx = new StandardContext();
      ctx.AddExtension(extension);
      extension.AssertWasCalled(
        e => e.Attach(null),
        o => o.Constraints(Is.Equal(ctx)));
    }

    [Test]
    public void ContextEnsuresLastDateWithExtension()
    {
      var extension = MockRepository
        .GenerateMock<IContextExtension<StandardContext>>();
      var ctx = new StandardContext();
      ctx.AddExtension(extension);
      ctx.Remove(extension);
      extension.AssertWasCalled(
        e => e.Remove(null),
        o => o.Constraints(Is.Equal(ctx)));
    }

    [Test]
    public void ContextDisposesOfDisposableThings()
    {
      var disp = MockRepository.GenerateMock<IDisposable>();
      using (var ctx = new StandardContext())
        ctx.Add(disp);
      disp.AssertWasCalled(d => d.Dispose());
    }

    [Test]
    public void ContextLeavesDisposableThingsAloneIfYouWant()
    {
      var disp = MockRepository.GenerateMock<IDisposable>();
      using (var ctx = new StandardContext())
        ctx.Add(disp, false);
      disp.AssertWasNotCalled(d => d.Dispose());
    }

    [Test]
    public void ContextIsCloneable()
    {
      StandardContext ctx = new StandardContext();
      ctx.Add("string");
      var ctx2 = ctx.CloneContext();
      ctx2.Get<string>().ShouldBeTheSameAs(ctx.Get<string>());
    }

    [Test]
    public void ContextCloneCanBeModifiedWithoutAffectingTheOriginal()
    {
      StandardContext ctx = new StandardContext();
      ctx.Add("horse");
      var ctx2 = ctx.CloneContext();
      ctx2.Replace("dog");
      ctx2.Add(new StringBuilder());
      ctx.Get<string>().ShouldBeTheSameAs("horse");
      ctx.Get<StringBuilder>().ShouldBeNull();
    }

    [Test]
    public void AnObjectAddedTwiceIsOnlyDisposedOnce()
    {
      var mock = MockRepository.GenerateMock<IDisposable>();
      mock.Expect(d => d.Dispose()).Repeat.Once();
      var ctx = new StandardContext();
      ctx.Add(mock);
      ctx.Add((object)mock);

      ctx.Dispose();
      mock.VerifyAllExpectations();
    }
  }
}