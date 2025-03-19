using Cysharp.Threading.Tasks;
using NOPE_Examples.Scripts.Error;
using NOPE_Examples.Scripts.Presenter;
using NOPE.Runtime.Core.Result;
using UnityEngine;
using UnityEngine.Networking;

namespace NOPE_Examples.Scripts.UseCase
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ImageFetchUseCase : IImageFetchUseCase
    {
        public async UniTask<Result<Texture2D, SampleError>> FetchImage(string url)
        {
            return await Result.Of(async () => await FetchImageInternal(url),
                    exception => new SampleError(SampleErrorType.Fatal, exception.Message))
                .Ensure(texture => texture != null,
                    new SampleError(SampleErrorType.Ignorable, "Image is null."));
        }

        private async UniTask<Texture2D> FetchImageInternal(string url)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();
            
            return ((DownloadHandlerTexture) request.downloadHandler).texture;
        }
    }
}