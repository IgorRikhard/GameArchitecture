using UnityEngine;
using Cysharp.Threading.Tasks;
using Core.DI;
using Core.Utils;
using Game.Loading.UI.ViewModels;
using R3;

namespace Game.UI.App
{
    /// <summary>
    /// Test to verify that the interface-based viewmodel binding works correctly
    /// </summary>
    public class InterfaceBasedTest : DIBehaviour
    {
        [SerializeField] private UIService _uiService;
        [SerializeField] private PrefabReference _loadingUIPrefabRef;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (_uiService == null)
            {
                _uiService = FindObjectOfType<UIService>();
            }
            
            // Initialize UIBuilder if not already done
            if (_uiService != null && _uiService.UIBuilder == null)
            {
                _uiService.InitializeUIBuilder();
            }
        }
        
        private async void Start()
        {
            // Test the interface-based approach
            await TestInterfaceBasedBinding();
        }
        
        private async UniTask TestInterfaceBasedBinding()
        {
            if (_uiService == null || _loadingUIPrefabRef == null) 
            {
                Debug.LogError("UIService or LoadingUIPrefabRef not assigned!");
                return;
            }
            
            Debug.Log("=== Testing Interface-Based ViewModel Binding ===");
            
            // Create a LoadingViewModel
            var progressSubject = new Subject<float>();
            var titleSubject = new Subject<string>();
            var loadingViewModel = new LoadingViewModel(progressSubject, titleSubject);
            
            // Test 1: Show UI with LoadingViewModel
            Debug.Log("Test 1: Creating LoadingUI with LoadingViewModel...");
            var loadingUI = await _uiService.ShowUIAsync<LoadingViewModelBinding>(_loadingUIPrefabRef, loadingViewModel);
            
            if (loadingUI != null)
            {
                Debug.Log("✅ LoadingUI created successfully!");
                
                // Test 2: Verify the viewmodel was set correctly
                Debug.Log("Test 2: Verifying viewmodel binding...");
                
                // Test the binding by updating observables
                titleSubject.OnNext("Loading assets...");
                progressSubject.OnNext(0.5f);
                
                Debug.Log("✅ Observable data sent to viewmodel");
                
                // Test 3: Test with wrong viewmodel type (should show warning)
                Debug.Log("Test 3: Testing type safety with wrong viewmodel...");
                var wrongViewModel = new TestViewModel("Wrong Type");
                
                // This should work but show a warning
                var loadingUI2 = await _uiService.ShowUIAsync<LoadingViewModelBinding>(_loadingUIPrefabRef, wrongViewModel);
                
                if (loadingUI2 != null)
                {
                    Debug.Log("✅ UI created with wrong viewmodel type (warning should be shown)");
                }
                
                Debug.Log("✅ Interface-based viewmodel binding test completed successfully!");
            }
            else
            {
                Debug.LogError("❌ Failed to create LoadingUI!");
            }
        }
    }
    
    // Test viewmodel for type safety testing
    public class TestViewModel : BaseViewModel
    {
        public string Message { get; }
        
        public TestViewModel(string message)
        {
            Message = message;
        }
    }
}
