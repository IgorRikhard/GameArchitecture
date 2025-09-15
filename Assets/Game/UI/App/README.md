# UI System Documentation

## Overview

This UI system provides a comprehensive solution for managing UI elements with the following features:

- **Layer Management**: Organize UI elements by layers (Base, AppLoading, etc.)
- **Object Pooling**: Reuse UI objects to improve performance
- **ViewModel Binding**: Generic viewmodel binding system
- **Dependency Injection**: Integration with the existing DI system
- **Async Loading**: Support for async UI loading with cancellation tokens

## Core Components

### 1. IUIView Interface
```csharp
public interface IUIView
{
    void Show();
    void Hide();
    void SetActive(bool active);
    bool IsVisible { get; }
    event Action OnShow;
    event Action OnHide;
}
```

### 2. BaseUIView<TViewModel>
Base class for all UI views with generic viewmodel binding:
```csharp
public abstract class BaseUIView<TViewModel> : MonoBehaviour, IUIView where TViewModel : class
{
    protected TViewModel _viewModel;
    
    public virtual void SetViewModel(TViewModel viewModel)
    public virtual void Show()
    public virtual void Hide()
    // ... other methods
}
```

### 3. BaseViewModel
Base class for all viewmodels:
```csharp
public abstract class BaseViewModel : IDisposable
{
    public event Action OnDisposed;
    // ... disposal logic
}
```

### 4. UIBuilder
Pure C# library class for UI management with pooling and layer support:
```csharp
public class UIBuilder
{
    // Dictionary<LayerType, List<IUIView>> for active UI management
    // Dictionary<Type, Queue<GameObject>> for object pooling
    
    public UIBuilder(Transform uiRoot, bool enablePooling = true, int maxPoolSize = 10)
    public async UniTask<T> ShowUI<T>(PrefabReference prefabRef, LayerType layerType, BaseViewModel viewModel = null)
    public void HideUI<T>(LayerType layerType)
    public void HideAllUI(LayerType layerType)
    public void Dispose()
    // ... other methods
}
```

### 5. UIService
Service wrapper that integrates with the DI system:
```csharp
public class UIService : IDisposable
{
    [SerializeField] private UIConfiguration _uiConfiguration;
    
    public void InitializeUIBuilder(Transform uiRoot = null, bool enablePooling = true, int maxPoolSize = 10)
    public async UniTask<T> ShowUI<T>(PrefabReference prefabRef, LayerType layerType, BaseViewModel viewModel = null)
    public void HideUI<T>(LayerType layerType)
    // ... other methods
}
```

### 6. UIConfiguration
ScriptableObject for configuring UI settings:
```csharp
[CreateAssetMenu(menuName = "Config/UIConfiguration")]
public class UIConfiguration : ScriptableObject
{
    public bool EnablePooling { get; }
    public int MaxPoolSize { get; }
    public Transform UIRoot { get; }
    public UIBuilder CreateUIBuilder(Transform fallbackRoot = null)
}
```

### 7. UIBuilderFactory
Static factory for creating UIBuilder instances:
```csharp
public static class UIBuilderFactory
{
    public static UIBuilder CreateUIBuilder(Transform uiRoot, bool enablePooling = true, int maxPoolSize = 10)
    public static UIBuilder CreateUIBuilderWithDefaultSettings(Transform uiRoot)
    public static UIBuilder CreateUIBuilderWithoutPooling(Transform uiRoot)
}
```

## Usage Examples

### 1. Creating a UI View

```csharp
public class MyUIView : BaseUIView<MyViewModel>
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Text _titleText;
    
    protected override void OnViewModelSet()
    {
        base.OnViewModelSet();
        if (_viewModel != null)
        {
            _titleText.text = _viewModel.Title;
        }
    }
    
    private void OnCloseButtonClicked()
    {
        Hide();
    }
}
```

### 2. Creating a ViewModel

```csharp
public class MyViewModel : BaseViewModel
{
    public string Title { get; set; }
    public string Content { get; set; }
    
    public MyViewModel(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
```

### 3. Using the UI System

```csharp
public class MyController : DIBehaviour
{
    [SerializeField] private UIService _uiService;
    [SerializeField] private PrefabReference _myUIPrefabRef;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Initialize UIBuilder if needed
        if (_uiService.UIBuilder == null)
        {
            _uiService.InitializeUIBuilder();
        }
    }
    
    public async void ShowMyUI()
    {
        var viewModel = new MyViewModel("Hello", "World");
        // Now requires PrefabReference as first parameter
        await _uiService.ShowUI<MyUIView>(_myUIPrefabRef, LayerType.Base, viewModel);
    }
    
    public void HideMyUI()
    {
        _uiService.HideUI<MyUIView>(LayerType.Base);
    }
}
```

### 4. Using UIBuilder Directly (Library Mode)

```csharp
public class MyUIManager
{
    private UIBuilder _uiBuilder;
    
    public MyUIManager(Transform uiRoot)
    {
        _uiBuilder = new UIBuilder(uiRoot, enablePooling: true, maxPoolSize: 10);
    }
    
    public async UniTask ShowUI<T>(PrefabReference prefabRef, LayerType layerType, BaseViewModel viewModel = null) 
        where T : Component, IUIView
    {
        await _uiBuilder.ShowUI<T>(prefabRef, layerType, viewModel);
    }
    
    public void Dispose()
    {
        _uiBuilder?.Dispose();
    }
}
```

### 5. Using UIConfiguration

```csharp
public class MyController : DIBehaviour
{
    [SerializeField] private UIConfiguration _uiConfig;
    [SerializeField] private UIService _uiService;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Initialize with configuration
        _uiService.InitializeUIBuilder();
    }
}
```

## Setup Instructions

1. **Create UI Prefabs**: Create prefabs for your UI views with the appropriate components
2. **Create PrefabReferences**: Create PrefabReference ScriptableObjects for each UI prefab
3. **Create UIConfiguration** (Optional): Create a UIConfiguration ScriptableObject for UI settings
4. **Setup Layers**: Ensure your LayerType enum includes all necessary layers
5. **Initialize UIBuilder**: Call InitializeUIBuilder() on UIService or use UIBuilderFactory
6. **Pass PrefabReferences**: Always pass PrefabReference as parameter when showing UI

## Configuration

### UIBuilder Settings
- `uiRoot`: Root transform for all UI elements (required in constructor)
- `enablePooling`: Enable/disable object pooling (default: true)
- `maxPoolSize`: Maximum number of objects to keep in pool per type (default: 10)

### UIConfiguration ScriptableObject
- Create a UIConfiguration asset to centralize UI settings
- Can specify UI root, pooling settings, and other configuration
- UIService will automatically use configuration if provided

### Layer Management
The system automatically creates layer containers for each LayerType enum value. UI elements are organized under these containers.

## Performance Features

- **Object Pooling**: Reuses UI objects instead of creating/destroying them
- **Async Loading**: Non-blocking UI loading with cancellation support
- **Memory Management**: Automatic cleanup and disposal of viewmodels
- **Event System**: Efficient event handling for UI state changes

## Best Practices

1. Always inherit from `BaseUIView<TViewModel>` for your UI views
2. Use the generic viewmodel binding system for data management
3. Pass PrefabReference as parameter when showing UI (no more registration needed)
4. Use appropriate layer types for UI organization
5. Implement proper disposal in viewmodels and UIBuilder instances
6. Use async/await for UI operations when possible
7. Use UIConfiguration for centralized settings management
8. Consider using UIBuilderFactory for consistent UIBuilder creation
9. Always call Dispose() on UIBuilder when no longer needed
10. Keep PrefabReferences as SerializeField for easy assignment in Inspector
