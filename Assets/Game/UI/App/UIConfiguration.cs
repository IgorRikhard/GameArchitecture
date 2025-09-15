using UnityEngine;

namespace Game.UI.App
{
    [CreateAssetMenu(menuName = "Config/UIConfiguration")]
    public class UIConfiguration : ScriptableObject
    {
        [Header("Pooling Settings")]
        [SerializeField] private bool _enablePooling = true;
        [SerializeField] private int _maxPoolSize = 10;
        
        [Header("UI Root")]
        [SerializeField] private Transform _uiRoot;
        
        public bool EnablePooling => _enablePooling;
        public int MaxPoolSize => _maxPoolSize;
        public Transform UIRoot => _uiRoot;
        
        public UIBuilder CreateUIBuilder(Transform fallbackRoot = null)
        {
            var root = _uiRoot != null ? _uiRoot : fallbackRoot;
            if (root == null)
            {
                throw new System.ArgumentNullException(nameof(fallbackRoot), "UI Root must be provided either in configuration or as fallback");
            }
            
            return new UIBuilder(root, _enablePooling, _maxPoolSize);
        }
    }
}
