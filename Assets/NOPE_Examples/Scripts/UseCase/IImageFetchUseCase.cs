using Cysharp.Threading.Tasks;
using NOPE_Examples.Scripts.Error;
using NOPE.Runtime.Core.Result;
using UnityEngine;

namespace NOPE_Examples.Scripts.Presenter
{
    public interface IImageFetchUseCase
    {
        public UniTask<Result<Texture2D, SampleError>> FetchImage(string url);
    }
}