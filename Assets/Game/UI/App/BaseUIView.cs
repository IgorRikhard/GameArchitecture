using System;
using R3;
using UnityEngine;

namespace Game.UI.App
{
    public abstract class BaseUIView<TViewModel> : MonoBehaviour, IUIView, IViewModelContainer where TViewModel : class
    {
        [SerializeField] protected bool startVisible = false;
        
        protected TViewModel _viewModel;
        protected bool _isVisible;
        protected CompositeDisposable bindableDisposables = new();
        
        public bool IsVisible => _isVisible;
        
        public event Action OnShow;
        public event Action OnHide;
        
        protected virtual void Awake()
        {
            if (!startVisible)
            {
                gameObject.SetActive(false);
            }
            _isVisible = startVisible;
        }
        
        public virtual void SetViewModel(TViewModel viewModel)
        {
            _viewModel = viewModel;
            OnViewModelSet();
        }
        
        // IViewModelContainer implementation
        void IViewModelContainer.SetViewModel(BaseViewModel viewModel)
        {
            if (viewModel is TViewModel typedViewModel)
            {
                SetViewModel(typedViewModel);
            }
            else
            {
                Debug.LogWarning($"Cannot set viewmodel of type {viewModel?.GetType().Name} on UI view expecting {typeof(TViewModel).Name}");
            }
        }
        
        protected virtual void OnViewModelSet()
        {
            // Override in derived classes to react to viewmodel changes
        }
        
        public virtual void Show()
        {
            if (_isVisible) return;
            
            gameObject.SetActive(true);
            _isVisible = true;
            OnShow?.Invoke();
            OnShowInternal();
        }
        
        public virtual void Hide()
        {
            if (!_isVisible) return;
            
            gameObject.SetActive(false);
            _isVisible = false;
            OnHide?.Invoke();
            OnHideInternal();
        }
        
        public virtual void SetActive(bool active)
        {
            if (active)
                Show();
            else
                Hide();
        }
        
        protected virtual void OnShowInternal()
        {
            // Override in derived classes for custom show logic
        }
        
        protected virtual void OnHideInternal()
        {
            // Override in derived classes for custom hide logic
        }
        
        protected virtual void OnDestroy()
        {
            OnShow = null;
            OnHide = null;
            bindableDisposables.Dispose();
        }
    }
}
