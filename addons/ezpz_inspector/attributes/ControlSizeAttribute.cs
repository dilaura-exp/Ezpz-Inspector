using Godot;
using System;

namespace Calcatz.EzpzInspector {

    /// <summary>
    /// Overrides the size of inspector control.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ControlSizeAttribute : Attribute {

        public float _width;
        public float _height;

        /// <summary>
        /// Set the size of the control.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height">Value less than 0 means that the default value will be used.</param>
        public ControlSizeAttribute(float width, float height = -1) {
            _width = width;
            _height = height;
        }

    }

}
