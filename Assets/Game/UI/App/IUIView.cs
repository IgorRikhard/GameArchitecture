using System;

namespace Game.UI.App
{
    public interface IUIView
    {
        void Show();
        void Hide();
        void SetActive(bool active);
        bool IsVisible { get; }
        event Action OnShow;
        event Action OnHide;
    }
}
