// MIT License
// 
// Copyright (c) 2019 Sabresaurus
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in all
// 	copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// 	SOFTWARE.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PrefabExplorer : EditorWindow
{
    string filter = "";

    Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/Prefab Explorer")]
    static void CreateAndShow()
    {
        EditorWindow window = EditorWindow.GetWindow<PrefabExplorer>("Prefab Explorer");

        window.Show();
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    void OnGUI()
    {
        GameObject prefabRoot = PrefabUtility.FindPrefabRoot(Selection.activeGameObject);
        if (prefabRoot != null)
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
            GUIStyle cancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
            GUIStyle noCancelStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty");

            GUILayout.Space(10);
            filter = EditorGUILayout.TextField(filter, searchStyle);
            if (!string.IsNullOrEmpty(filter))
            {
                if (GUILayout.Button("", cancelStyle))
                {
                    filter = "";
                    GUIUtility.hotControl = 0;
                    EditorGUIUtility.editingTextField = false;
                }
            }
            else
            {
                GUILayout.Button("", noCancelStyle);
            }
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawName(prefabRoot.transform);
            RecurseChildren(prefabRoot.transform);
            EditorGUILayout.EndScrollView();
        }
    }

    void DrawName(Transform transform)
    {
        if (!string.IsNullOrEmpty(filter) && transform.name.IndexOf(filter, System.StringComparison.CurrentCultureIgnoreCase) == -1)
        {
            // Skip if a filter is applied and we don't match
            return;
        }
        GUIStyle style = EditorStyles.label;
        if (Selection.objects.Contains(transform.gameObject))
        {
            style = new GUIStyle(style);
            style.normal.textColor = Color.blue;
        }

        Rect rect = EditorGUILayout.GetControlRect();
        rect = EditorGUI.IndentedRect(rect);

        if (GUI.Button(rect, transform.name, style))
        {
            if (Event.current.shift)
            {
                List<Object> selection = new List<Object>(Selection.objects);
                selection.Add(transform.gameObject);
                Selection.objects = selection.ToArray();
            }
            else
            {
                Selection.activeTransform = transform;
            }
        }
    }

    void RecurseChildren(Transform parent)
    {
        EditorGUI.indentLevel++;
        foreach (Transform childTransform in parent)
        {
            DrawName(childTransform);

            if (childTransform.childCount > 0)
                RecurseChildren(childTransform);
        }
        EditorGUI.indentLevel--;
    }
}