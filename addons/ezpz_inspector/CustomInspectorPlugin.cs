#if TOOLS
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Calcatz.EzpzInspector {

    public partial class CustomInspectorPlugin : EditorInspectorPlugin {

        #region CACHE
        private Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        private Dictionary<Type, MethodInfo[]> methodCache = new Dictionary<Type, MethodInfo[]>();
        private Dictionary<Type, Dictionary<string, FieldInfo>> fieldCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        private Dictionary<Type, Dictionary<string, PropertyInfo>> propertyCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        internal void Init() {
            typeCache = new Dictionary<string, Type>();
            methodCache = new Dictionary<Type, MethodInfo[]>();
            fieldCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            propertyCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        }

        internal void Clear() {
            typeCache.Clear();
            methodCache.Clear();
            fieldCache.Clear();
            propertyCache.Clear();
            typeCache = null;
            methodCache = null;
            fieldCache = null;
            propertyCache = null;
        }
        #endregion CACHE

        #region ATTRIBUTE-GETTERS
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
                if (type2 == typeof(Node) || type2 == typeof(Node2D) || type2 == typeof(Node3D) || type2 == null) {
                    break;
                }
                typeInfo = type2.GetTypeInfo();
            }

            return methods.ToArray();
        }

        private FieldInfo[] GetFieldsWithAttribute(Type type) {
            var fields = new List<FieldInfo>();
            var typeInfo = type.GetTypeInfo();

            while (true) {
                foreach (var f in typeInfo.DeclaredFields) {
                    var customAttribute = f.GetCustomAttribute<ExportFieldPropertyAttribute>();
                    if (customAttribute != null) {
                        fields.Add(f);
                    }
                }

                Type type2 = typeInfo.BaseType;
                if (type2 == typeof(Node) || type2 == typeof(Node2D) || type2 == typeof(Node3D) || type2 == null) {
                    break;
                }
                typeInfo = type2.GetTypeInfo();
            }

            return fields.ToArray();
        }

        private PropertyInfo[] GetPropertiesWithAttribute(Type type) {
            var properties = new List<PropertyInfo>();
            var typeInfo = type.GetTypeInfo();

            while (true) {
                foreach (var p in typeInfo.DeclaredProperties) {
                    var customAttribute = p.GetCustomAttribute<ExportFieldPropertyAttribute>();
                    if (customAttribute != null) {
                        properties.Add(p);
                    }
                }

                Type type2 = typeInfo.BaseType;
                if (type2 == typeof(Node) || type2 == typeof(Node2D) || type2 == typeof(Node3D) || type2 == null) {
                    break;
                }
                typeInfo = type2.GetTypeInfo();
            }

            return properties.ToArray();
        }
        #endregion

        private Type currentType;

        public override bool _CanHandle(GodotObject @object) {
            currentType = null;
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

                    var fields = GetFieldsWithAttribute(type);
                    if (fields.Length > 0) {
                        if (!fieldCache.TryGetValue(type, out var fieldDict)) {
                            fieldDict = new Dictionary<string, FieldInfo>();
                            foreach (var field in fields) {
                                if (!fieldDict.ContainsKey(field.Name)) {
                                    fieldDict.Add(field.Name, field);
                                }
                            }
                        }
                        fieldCache.Add(type, fieldDict);
                    }

                    var properties = GetPropertiesWithAttribute(type);
                    if (properties.Length > 0) {
                        if (!propertyCache.TryGetValue(type, out var propertyDict)) {
                            propertyDict = new Dictionary<string, PropertyInfo>();
                            foreach (var property in properties) {
                                if (!propertyDict.ContainsKey(property.Name)) {
                                    propertyDict.Add(property.Name, property);
                                }
                            }
                        }
                        propertyCache.Add(type, propertyDict);
                    }
                }
                currentType = type;
                return (methodCache.ContainsKey(type) && methodCache[type].Length > 0) ||
                    (fieldCache.ContainsKey(type) && fieldCache[type].Count > 0) ||
                    (propertyCache.ContainsKey(type) && propertyCache[type].Count > 0);
            }
            return false;
        }

        public override void _ParseCategory(GodotObject @object, string category) {
            if (currentType == null) return;
            if (currentType.Name != category) return;
            if (methodCache.TryGetValue(currentType, out var methods)) {
                foreach (var method in methods) {
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

            if (method.IsStatic) {
                button.Pressed += () => {
                    method.Invoke(null, new object[0]);
                };
            }
            else {
                button.Pressed += () => {
                    if (@object.HasMethod(methodName)) {
                        @object.Call(methodName);
                    }
                };
            }

            if (marginContainer == null) {
                AddCustomControl(button);
            }
            else {
                AddCustomControl(marginContainer);
            }
        }

        public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide) {
            if (currentType == null) return false;
            UpperDescriptionAttribute upperDescriptionAttribute = null;
            ControlMarginAttribute marginAttribute = null;
            ControlSizeAttribute sizeAttribute = null;
            ControlModulateColorAttribute modulateColorAttribute = null;
            if (fieldCache.TryGetValue(currentType, out var fields)) {
                if (fields.TryGetValue(name, out var field)) {
                    upperDescriptionAttribute = field.GetCustomAttribute<UpperDescriptionAttribute>();
                    marginAttribute = field.GetCustomAttribute<ControlMarginAttribute>();
                    sizeAttribute = field.GetCustomAttribute<ControlSizeAttribute>();
                    modulateColorAttribute = field.GetCustomAttribute<ControlModulateColorAttribute>();
                }
            }
            else if (propertyCache.TryGetValue(currentType, out var properties)) {
                if (properties.TryGetValue(name, out var property)) {
                    upperDescriptionAttribute = property.GetCustomAttribute<UpperDescriptionAttribute>();
                    marginAttribute = property.GetCustomAttribute<ControlMarginAttribute>();
                    sizeAttribute = property.GetCustomAttribute<ControlSizeAttribute>();
                    modulateColorAttribute = property.GetCustomAttribute<ControlModulateColorAttribute>();
                }
            }

            MarginContainer marginContainer = null;
            if (marginAttribute != null) {
                marginContainer = new MarginContainer();
                marginContainer.AddThemeConstantOverride("margin_left", marginAttribute._marginLeft);
                marginContainer.AddThemeConstantOverride("margin_top", marginAttribute._marginTop);
                marginContainer.AddThemeConstantOverride("margin_right", marginAttribute._marginRight);
                marginContainer.AddThemeConstantOverride("margin_bottom", marginAttribute._marginBottom);
            }

            if (upperDescriptionAttribute != null) {
                var label = new Label();
                label.Text = upperDescriptionAttribute._info;

                if (modulateColorAttribute != null) {
                    label.Modulate = modulateColorAttribute.GetColor();
                }

                if (marginContainer == null) {
                    AddCustomControl(label);
                    if (sizeAttribute != null) {
                        label.Size = new Vector2(sizeAttribute._width, sizeAttribute._height);
                    }
                }
                else {
                    if (sizeAttribute != null) {
                        label.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
                        if (sizeAttribute._height >= 0) {
                            label.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
                        }
                        label.CustomMinimumSize = new Vector2(sizeAttribute._width, sizeAttribute._height);
                    }
                    marginContainer.AddChild(label);
                    AddCustomControl(marginContainer);
                }
            }
            return false;
        }

    }

}
#endif