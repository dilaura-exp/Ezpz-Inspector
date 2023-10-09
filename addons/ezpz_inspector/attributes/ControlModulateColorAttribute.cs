using Godot;
using System;

namespace Calcatz.EzpzInspector {

    /// <summary>
    /// Changes the color modulation to inspector control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class ControlModulateColorAttribute : Attribute {

        public float _r;
        public float _g;
        public float _b;
        public float _a;

        public ControlModulateColorAttribute(float r, float g, float b, float a = 1) {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public Color GetColor() {
            return new Color(_r, _g, _b, _a);
        }

    }

}
