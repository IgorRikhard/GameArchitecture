using Game.UI.App;
using UnityEngine;

namespace Game.UI.Api
{
    public interface IUIServiceInitialize
    {
        void InitializeUIBuilder(UIBuilder uiBuilder, Transform uiRoot, bool enablePooling, int maxPoolSize);
    }
}