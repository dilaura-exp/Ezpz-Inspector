using Godot;
using System;

namespace Calcatz.EzpzInspector {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExportFieldPropertyAttribute : Attribute {
    }

}
