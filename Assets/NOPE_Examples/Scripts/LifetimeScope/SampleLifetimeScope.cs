using NOPE_Examples.Scripts.UseCase;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace NOPE_Examples.Scripts.LifetimeScope
{
    public class SampleLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ImageFetchUseCase>(Lifetime.Singleton).AsImplementedInterfaces();

            // Register EntryPoint
            builder.RegisterEntryPoint<SampleEntryPoint>();
        }
    }

    public class SampleEntryPoint : IStartable
    {
        private readonly IObjectResolver _resolver;
        private readonly GameObject _prefab;
        
        public SampleEntryPoint(IObjectResolver resolver)
        {
            _resolver = resolver;
            _prefab = Resources.Load<GameObject>("Prefabs/ImageFetchPageView");
        }
        
        public void Start()
        {
            // Spawn the prefab on the scene under the Canvas
            var canvas = GameObject.Find("Canvas");
            var instance = _resolver.Instantiate(_prefab, canvas.transform);
        }
    }
}
