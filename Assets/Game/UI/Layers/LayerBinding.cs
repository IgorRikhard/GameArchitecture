using UnityEngine;

namespace Game.Layers
{
    public class LayerBinding : MonoBehaviour
    {
        [SerializeField] private LayerType _layerType;

        public LayerType LayerType => _layerType;
    }
}