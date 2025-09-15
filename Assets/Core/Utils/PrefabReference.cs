using UnityEngine;

namespace Core.Utils
{
    [CreateAssetMenu(menuName = "Config/PrefabReference")]
    public class PrefabReference : ScriptableObject
    {
        [SerializeField] private LayerType layerType;
        
        public LayerType LayerType => layerType;
    }
}
