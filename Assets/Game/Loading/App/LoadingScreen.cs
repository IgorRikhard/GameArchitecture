using UnityEngine;
using UnityEngine.UI;

namespace Loading
{
	public class LoadingScreen : MonoBehaviour
	{
		[SerializeField]
		private Slider _progressBar;

		[SerializeField]
		private Text _tipText;

		public void SetProgress(float value)
		{
			if (_progressBar != null)
			{
				_progressBar.value = Mathf.Clamp01(value);
			}
		}

		public void SetTip(string text)
		{
			if (_tipText != null)
			{
				_tipText.text = text ?? string.Empty;
			}
		}
	}
}
