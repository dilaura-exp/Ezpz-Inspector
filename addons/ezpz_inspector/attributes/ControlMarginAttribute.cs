using Godot;
using System;

namespace Calcatz.EzpzInspector {

    /// <summary>
    /// Adds margin to an inspector control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class ControlMarginAttribute : Attribute {

        public int _marginLeft;
        public int _marginTop;
        public int _marginRight;
        public int _marginBottom;

        public ControlMarginAttribute(int marginLeft, int marginTop, int marginRight, int marginBottom) {
            _marginLeft = marginLeft;
            _marginTop = marginTop;
            _marginRight = marginRight;
            _marginBottom = marginBottom;
        }

    }

}
