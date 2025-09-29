using System.Collections.Generic;
using DG.Tweening;
using Game.Simulation.Utils;
using R3;
using UnityEngine;

namespace Game.Simulation
{
    public class Simulate : MonoBehaviour
    {
        [SerializeField]
        private GameObject _mask;
        
        [SerializeField]
        private GameObject _rect;
        
        // Start is called before the first frame update
        void Start()
        {
            SimulateUtils.GetSpaceClickedObservable().Subscribe(x =>
            {
                Debug.Log("------SPACE CLICK------");
                _mask.transform.DOScale(_rect.transform.localScale, 2.6f).SetEase(Ease.OutCubic);
            });

            Dictionary<string, int> dic = new();
            dic.Add("a", 1);
            var c = "a"[0];
            int value;
            dic.TryGetValue("a", out value);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
