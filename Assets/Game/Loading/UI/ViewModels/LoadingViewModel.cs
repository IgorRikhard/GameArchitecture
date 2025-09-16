using Game.UI.App;
using R3;

namespace Game.Loading.UI.ViewModels
{
    public class LoadingViewModel : BaseViewModel
    {
        public Observable<float> progress { get; }
        public Observable<string> progressTitle { get; }

        private readonly CompositeDisposable _disposables = new();

        public LoadingViewModel(Observable<float> progressObservable, Observable<string> progressTitleObservable)
        {
            progress = progressObservable;
            progressTitle = progressTitleObservable;
        }

        protected override void OnDispose()
        {
            _disposables.Dispose();
            base.OnDispose();
        }
    }
}