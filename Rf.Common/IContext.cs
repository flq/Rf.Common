namespace Rf.Common
{
  public interface IContext
  {
    void Add<T>(T @object);
    void Remove<T>();
    T Get<T>();
    string WhatDoIHave { get; }
  }

  public interface IContext<TARGET> : IContext
  {
    void AddExtension<T>(T extension) where T : IContextExtension<TARGET>;    
    void RemoveExtension<T>() where T : IContextExtension<TARGET>;
    TARGET Replace<T>(T @object);
    TARGET CloneContext();
  }

  public interface IContextExtension<T>
  {
    void Attach(T target);
    void Remove(T target);
  }
}