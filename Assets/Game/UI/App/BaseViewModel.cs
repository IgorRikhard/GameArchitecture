using System;

namespace Game.UI.App
{
    public abstract class BaseViewModel : IDisposable
    {
        public event Action OnDisposed;
        
        protected bool _isDisposed;
        
        public bool IsDisposed => _isDisposed;
        
        protected virtual void OnDispose()
        {
            // Override in derived classes for custom disposal logic
        }
        
        public void Dispose()
        {
            if (_isDisposed) return;
            
            OnDispose();
            OnDisposed?.Invoke();
            _isDisposed = true;
        }
    }
}
