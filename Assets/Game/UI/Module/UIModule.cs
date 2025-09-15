using Core.DI;
using Core.Modules;
using Game.UI.App;
using UnityEngine;

namespace Game.UI.UIModule
{
    [CreateAssetMenu(menuName = "Config/UIModule", fileName = "UIModules")]
    public class UIModule : GameModule
    {
        [SerializeField] private UIInitService.Settings _mainUIPrefabSettings;

        [SerializeField] private UIConfiguration _configuration;

        public override void Install(ServiceContainer container)
        {
            container.InstantiateAndBind<UIService>();
            container.InstantiateAndBind<UIInitService>(_mainUIPrefabSettings, _configuration);
        }
    }
}