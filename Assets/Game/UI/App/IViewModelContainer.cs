using Game.UI.App;

namespace Game.UI.App
{
    /// <summary>
    /// Interface for UI views that can contain and manage viewmodels
    /// </summary>
    public interface IViewModelContainer
    {
        /// <summary>
        /// Sets the viewmodel for this UI view
        /// </summary>
        /// <param name="viewModel">The viewmodel to set</param>
        void SetViewModel(BaseViewModel viewModel);
    }
}

