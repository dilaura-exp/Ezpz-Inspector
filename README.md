# Ezpz-Inspector

![EzpzIcon](https://github.com/dilaura-exp/Ezpz-Inspector/assets/21215083/b1aef590-77f1-4ae6-8c87-f944748d989d)

Custom Inspector helper for Godot C# script.

Easily create a button in Inspector using `[ExportButton]` attribute.

Simple Button:
```csharp
    [ExportButton]
    private void Increment() {
        _currentValue++;
    }
```

Advanced Styled Button:
```csharp
    [ExportButton("Styled Button")]
    [ControlMargin(20, 20, 20, 20)]
    [ControlSize(200, 40)]
    [ControlModulateColor(0.9f, 0.1f, 0.1f, 1f)]
    private void StyledButton() {
        _currentValue++;
    }
```

![ezpzinspector-v1 1 0](https://github.com/dilaura-exp/Ezpz-Inspector/assets/21215083/b90eaaa6-1286-4795-ba0a-e7406d90a0aa)

# How to Install

Download the project and copy the addon folder into your Godot project.
Go to Project -> Project Settings.. > Plugins, and enable Ezpz Inspector.
If an error occured, make sure to compile the C# project first, and then try enabling again.

# How to Use

- Add `using Calcatz.EzpzInspector;` on top of your script.
- Add `[Tool]` attribute on top of your class declaration. This will enable your C# script to be instantiated, thus, modifiable during edit mode.
- Add `[ExportButton]` attribute on top of your method declaration. Please note that the button currently will not pass arguments.
- Since `[Tool]` attribute is used, other Godot's built-in methods will also be executed during edit mode. In this case, make sure to use `Engine.IsEditorHint()` to prevent your certain code from being executed during edit mode.

Here are optional attributes that you can add to do more advanced stylings:
- Add `[ControlMargin]` on top of your method to add margins to the button.
- Add `[ControlSize]` on top of your method to change the size of the button.
- Add `[ControlModulateColor]` on top of your method to modulate the color of the button. This will modulate based on the button style of your theme.
Note: Make sure the method also has `[ExportButton]` attribute.

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

    [ExportButton("Styled Button")]
    [ControlMargin(20, 20, 20, 20)]
    [ControlSize(200, 40)]
    [ControlModulateColor(0.9f, 0.1f, 0.1f, 1f)]
    private void StyledButton() {
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
