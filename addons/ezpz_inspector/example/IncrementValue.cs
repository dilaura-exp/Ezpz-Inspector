using Calcatz.EzpzInspector;
using Godot;
using System;

namespace Calcatz.Example {

    [Tool]
    public partial class IncrementValue : Node {

        [Export]
        [UpperDescription("This is our current value.")]
        [ControlModulateColor(0.1f, 0.9f, 0.9f, 1f)]
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

        [ExportButton]
        [ControlMargin(10, 10, 10, 10)]
        private static void StaticMethod() {
            GD.Print("Static Method is called.");
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

}