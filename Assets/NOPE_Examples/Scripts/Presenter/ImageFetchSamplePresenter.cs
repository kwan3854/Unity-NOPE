using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using NOPE_Examples.Scripts.Error;
using NOPE_Examples.Scripts.View;
using NOPE.Runtime.Core.Result;
using UnityEngine;
using VContainer;

namespace NOPE_Examples.Scripts.Presenter
{
    public class ImageFetchSamplePresenter : MonoBehaviour
    {
        [SerializeField] private ImageFetchSampleView view;
        
        private string _currentUrl;
        private IImageFetchUseCase _imageFetchUseCase;

        [Inject]
        public void Inject(IImageFetchUseCase useCase)
        {
            _imageFetchUseCase = useCase;
        }
        
        private void Start()
        {
            view.FetchButtonClicked
                .ForEachAwaitAsync(async _ =>
                {
                    await _imageFetchUseCase.FetchImage(_currentUrl)
                        .Finally(async finalResult => { await OnFetchImageClick(finalResult); });
                }, destroyCancellationToken)
                .Forget();

            view.UrlInputChanged
                .ForEachAwaitAsync(async url =>
                {
                    _currentUrl = url;
                    await UniTask.CompletedTask;
                }, destroyCancellationToken)
                .Forget();
        }

        private async UniTask OnFetchImageClick(Result<Texture2D, SampleError> finalResult)
        {
            if (finalResult.IsSuccess)
            {
                view.SetImage(finalResult.Value);
            }
            else
            {
                switch (finalResult.Error.Type)
                {
                    case SampleErrorType.Fatal:
                        Application.Quit();
                        Debug.LogError($"Fatal error: {finalResult.Error.Message}");
                        break;
                    case SampleErrorType.Retryable:
                        // Retry the operation
                        Debug.LogWarning($"Retryable error: {finalResult.Error.Message}");
                        break;
                    case SampleErrorType.Ignorable:
                        // Ignore the error
                        Debug.Log($"Ignorable error: {finalResult.Error.Message}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            await UniTask.CompletedTask;
        }
    }
}
