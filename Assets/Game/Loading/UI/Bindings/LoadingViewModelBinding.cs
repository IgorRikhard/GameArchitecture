using Game.Loading.UI.ViewModels;
using Game.UI.App;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Loading.UI.Bindings
{
    public class LoadingViewModelBinding : BaseUIView<LoadingViewModel>
    {
        [SerializeField]
        private Slider _progressBar;

        [SerializeField]
        private Text _tipText;

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            _viewModel.progress.AsObservable().Subscribe(x => _progressBar.value = x).AddTo(bindableDisposables);
            _viewModel.progressTitle.AsObservable().Subscribe(x => _tipText.text = x).AddTo(bindableDisposables);
        }
    }
}