# Ezpz-Inspector

![EzpzIcon](https://github.com/dilaura-exp/Ezpz-Inspector/assets/21215083/b1aef590-77f1-4ae6-8c87-f944748d989d)

Custom Inspector helper for Godot C# script.

Easily create a button in Inspector using `[ExportButton]` attribute.

```csharp
    [ExportButton]
    private void Increment() {
        _currentValue++;
    }
```

![ezpzinspector](https://github.com/dilaura-exp/Ezpz-Inspector/assets/21215083/27fc9220-cc6b-4c3a-a598-c9d96baae757)

# How to Install

Download the project and copy the addon folder into your Godot project.
Go to Project -> Project Settings.. > Plugins, and enable Ezpz Inspector.
If an error occured, make sure to compile the C# project first, and then try enabling again.

# How to Use

- Add `using Calcatz.EzpzInspector;` on top of your script.
- Add `[Tool]` attribute on top of your class declaration. This will enable your C# script to be instantiated, thus, modifiable during edit mode.
- Add `[ExportButton]` attribute on top of your method declaration. Please note that the button currently will not pass arguments.
- Since `[Tool]` attribute is used, other Godot's built-in methods will also be executed during edit mode. In this case, make sure to use `Engine.IsEditorHint()` to prevent your certain code from being executed during edit mode.

# Example

Here is an example where we can increment an integer by clicking a button.

```csharp
using Godot;
using Calcatz.EzpzInspector;

[Tool]
public partial class IncrementValue : Node {

    [Export]
    private int _currentValue = 0;

    [ExportButton]
    private void Increment() {
        _currentValue++;
    }

    // [Tool] attribute is required, but it also makes our code executed in editor.
    // In that case, if you don't want your code to be executed in editor, use Engine.IsEditorHint()

    public override void _Ready() {
        if (Engine.IsEditorHint()) return;
        // Write execution here ...
    }

    public override void _Process(double delta) {
        if (Engine.IsEditorHint()) return;
        // Write execution here ...
    }

}
```
