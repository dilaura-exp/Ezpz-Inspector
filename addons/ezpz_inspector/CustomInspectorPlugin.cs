#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Calcatz.EzpzInspector {

    public partial class CustomInspectorPlugin : EditorInspectorPlugin {

        private Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        private Dictionary<Type, MethodInfo[]> methodCache = new Dictionary<Type, MethodInfo[]>();

        internal void Init() {
            typeCache = new Dictionary<string, Type>();
            methodCache = new Dictionary<Type, MethodInfo[]>();
        }

        internal void Clear() {
            typeCache.Clear();
            methodCache.Clear();
            typeCache = null;
            methodCache = null;
        }

        private MethodInfo[] GetExportedButtonMethods(Type type) {
            var methods = new List<MethodInfo>();
            var typeInfo = type.GetTypeInfo();

            while (true) {
                foreach (var m in typeInfo.DeclaredMethods) {
                    var exportButton = m.GetCustomAttribute<ExportButtonAttribute>();
                    if (exportButton != null) {
                        methods.Add(m);
                    }
                }

                Type type2 = typeInfo.BaseType;
                if (type2 == typeof(Node) || type2 == null) {
                    break;
                }
                typeInfo = type2.GetTypeInfo();
            }

            return methods.ToArray();
        }

        public override bool _CanHandle(GodotObject @object) {
            var script = @object.GetScript().As<CSharpScript>();
            if (script != null && !string.IsNullOrEmpty(script.ResourcePath)) {
                if (!typeCache.TryGetValue(script.ResourcePath, out var type)) {
                    var temp = script.New().AsGodotObject();
                    type = temp.GetType();
                    temp.Free();
                    typeCache.Add(script.ResourcePath, type);

                    var methods = GetExportedButtonMethods(type);
                    if (methods.Length > 0) {
                        methodCache.Add(type, methods);
                    }
                }
                return methodCache.ContainsKey(type) && methodCache[type].Length > 0;
            }
            return false;
        }

        public override void _ParseCategory(GodotObject @object, string category) {
            foreach (var kvp in methodCache) {
                if (category != kvp.Key.Name) continue;

                foreach (var method in kvp.Value) {
                    HandleMethod(@object, method);
                }
            }
        }

        private void HandleMethod(GodotObject @object, MethodInfo method) {
            var button = new Button();
            string methodName = method.Name;

            var buttonAttribute = method.GetCustomAttribute<ExportButtonAttribute>();
            if (buttonAttribute._text == null) {
                button.Text = methodName;
            }
            else {
                button.Text = buttonAttribute._text;
            }


            MarginContainer marginContainer = null;
            var marginAttribute = method.GetCustomAttribute<ControlMarginAttribute>();
            if (marginAttribute != null) {
                marginContainer = new MarginContainer();
                marginContainer.AddThemeConstantOverride("margin_left", marginAttribute._marginLeft);
                marginContainer.AddThemeConstantOverride("margin_top", marginAttribute._marginTop);
                marginContainer.AddThemeConstantOverride("margin_right", marginAttribute._marginRight);
                marginContainer.AddThemeConstantOverride("margin_bottom", marginAttribute._marginBottom);
                marginContainer.AddChild(button);
            }

            var sizeAttribute = method.GetCustomAttribute<ControlSizeAttribute>();
            if (sizeAttribute != null) {
                if (marginContainer == null) {
                    button.Size = new Vector2(sizeAttribute._width, sizeAttribute._height);
                }
                else {
                    button.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
                    if (sizeAttribute._height >= 0) {
                        button.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
                    }
                    button.CustomMinimumSize = new Vector2(sizeAttribute._width, sizeAttribute._height);
                }
            }

            var modulateColorAttribute = method.GetCustomAttribute<ControlModulateColorAttribute>();
            if (modulateColorAttribute != null) {
                button.Modulate = modulateColorAttribute.GetColor();
            }

            button.Pressed += () => {
                if (@object.HasMethod(methodName)) {
                    @object.Call(methodName);
                }
            };

            if (marginContainer == null) {
                AddCustomControl(button);
            }
            else {
                AddCustomControl(marginContainer);
            }
        }
    }

}
#endif