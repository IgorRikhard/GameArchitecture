using Cysharp.Threading.Tasks;
using Game.Loading.UI.Bindings;
using Game.Loading.UI.ViewModels;

namespace Game.Loading.Api
{
    public interface ILoadingScreenUIService
    {
        UniTask<LoadingViewModelBinding> ShowLoadingScreen(LoadingViewModel viewModel);

        void HideLoadingScreen();
    }
}