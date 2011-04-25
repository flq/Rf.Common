using System;
using System.Collections.Concurrent;

namespace Rf.Common
{
    public interface Ioc
    {
        T Get<T>();
        T Get<T>(string key);
    }

    public interface IConfigureTinyIoc
    {
        void Add<T>(Func<Ioc, T> constructor);
        void Add<T>(string key, Func<Ioc, T> constructor);
        void AddSingleton<T>(Func<Ioc, T> constructor);
    }

    internal interface IBuilder
    {
        object Get(string key);
    }

    public class TinyIoc : IConfigureTinyIoc, Ioc
    {
        private readonly ConcurrentDictionary<Type, IBuilder> _builders = new ConcurrentDictionary<Type, IBuilder>();

        public void Configure(Action<IConfigureTinyIoc> configure)
        {
            configure(this);
        }

        public T Get<T>()
        {
            return Get<T>("default");
        }

        public T Get<T>(string key)
        {
            IBuilder builder;
            _builders.TryGetValue(typeof(T), out builder);
            try
            {
                if (builder != null)
                    return (T) builder.Get(key);
                throw new ArgumentException("Nothing registered for " + typeof (T).Name);
            }
            catch (ArgumentException x)
            {
                throw new ArgumentException("Nothing registered for " + key + ":" + typeof(T).Name, x);
            }
        }

        void IConfigureTinyIoc.Add<T>(Func<Ioc, T> constructor)
        {
            GetBuilder<T>().Add("default", constructor);
        }

        void IConfigureTinyIoc.Add<T>(string key, Func<Ioc, T> constructor)
        {
            GetBuilder<T>().Add(key, constructor);
        }

        void IConfigureTinyIoc.AddSingleton<T>(Func<Ioc, T> constructor)
        {
            GetSingletonBuilder<T>().Singleton(constructor);
        }

        private Builder GetBuilder<T>()
        {
            return (Builder)_builders.GetOrAdd(typeof(T), new Builder(this));
        }

        private SingletonBuilder GetSingletonBuilder<T>()
        {
            return (SingletonBuilder)_builders.GetOrAdd(typeof(T), new SingletonBuilder(this));
        }
    }

    internal class SingletonBuilder : IBuilder
    {
        private readonly Ioc _tinyIoc;
        private Func<Ioc, object> _constructor;
        private readonly object singletonBuildLock = new object();
        private object _theObj;

        public SingletonBuilder(Ioc tinyIoc)
        {
            _tinyIoc = tinyIoc;
        }

        public object Get(string key)
        {
            return _theObj ?? BuildObject();
        }

        private object BuildObject()
        {
            lock (singletonBuildLock)
                return _theObj ?? (_theObj = _constructor(_tinyIoc));
        }

        public void Singleton<T>(Func<Ioc, T> constructor)
        {
            _constructor = ctx => { var obj = constructor(ctx); return obj; };
        }
    }

    internal class Builder : IBuilder
    {
        private readonly Ioc _tinyIoc;
        private readonly ConcurrentDictionary<string, Func<Ioc, object>> _builders = new ConcurrentDictionary<string, Func<Ioc, object>>();

        public Builder(Ioc tinyIoc)
        {
            _tinyIoc = tinyIoc;
        }

        public void Add<T>(string key, Func<Ioc, T> constructor)
        {
            _builders.TryAdd(key, ctx => { var obj = constructor(ctx); return obj; });
        }

        public object Get(string key)
        {
            Func<Ioc, object> constructor;
            _builders.TryGetValue(key, out constructor);
            if (constructor != null)
                return constructor(_tinyIoc);
            throw new ArgumentException("Unknown instance", key);
        }
    }
}