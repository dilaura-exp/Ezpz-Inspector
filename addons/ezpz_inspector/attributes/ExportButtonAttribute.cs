using Godot;
using System;

namespace Calcatz.EzpzInspector {

    /// <summary>
    /// Draws a button in the inspector of this script in which will call the specified method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExportButtonAttribute : Attribute {

        public string _text = null;

        public ExportButtonAttribute() {

        }

        public ExportButtonAttribute(string text) {
            _text = text;
        }

    }

}
