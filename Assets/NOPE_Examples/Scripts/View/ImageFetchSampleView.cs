using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NOPE_Examples.Scripts.View
{
    public class ImageFetchSampleView : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        [SerializeField] private TMP_InputField urlInput;
        [SerializeField] private Button fetchButton;

        public IUniTaskAsyncEnumerable<AsyncUnit> FetchButtonClicked => fetchButton.OnClickAsAsyncEnumerable();
        public IUniTaskAsyncEnumerable<string> UrlInputChanged => urlInput.OnValueChangedAsAsyncEnumerable();

        public void SetImage(Texture2D texture)
        {
            image.texture = texture;
        }
    }
}
