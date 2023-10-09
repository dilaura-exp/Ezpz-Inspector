using Godot;
using System;

namespace Calcatz.EzpzInspector {

    public class UpperDescriptionAttribute : ExportFieldPropertyAttribute {

        public string _info;

        public UpperDescriptionAttribute(string info) { 
            this._info = info;
        }

    }

}
