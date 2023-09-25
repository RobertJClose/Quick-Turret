using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace QuickTurret.Waypoints
{
    [CustomEditor(typeof(Waypoints))]
    public class WaypointsEditor : Editor
    {
        SerializedProperty pointsProp;
        ReorderableList list;
        Rect waypointGUIWindowRect;
        Vector2 inspectorGUIScroll;

        const int GUIVERTSPACING = 5;
        const int GUIHORZSPACING = 5;
        const float UNSELECTEDSIZE = 0.33f;
        const float SELECTEDSIZE = 0.5f;
        const float LINETHICKNESS = 3.5f;
        const float CONESIZE = 0.25f;
        readonly Color UNSELECTEDCOLOR = new Color(0.5f, 0.5f, 1f, 1f);
        readonly Color SELECTEDCOLOR = new Color(0f, 0f, 0.5f, 1f);
        readonly Color ADDINGCONNECTIONCOLOR = new Color(0.5f, 0f, 0f, 1f);
        readonly Color REMOVINGCONNECTIONCOLOR = new Color(0f, 0.5f, 0f, 1f);

        Waypoint SelectedWaypoint { get; set; }
        bool ExpandWindow { get; set; }
        bool HasSelection { get { return SelectedWaypoint != null; } }
        bool IsEditingConnectionsInScene { get; set; }
        bool IsAppendingToSelectedWaypoint { get; set; }
        bool WillShowAllNames { get; set; }
        bool WillFocusOnSelect { get; set; }
        bool WillSelectNewAfterAppending { get; set; }

        private void OnEnable()
        {
            waypointGUIWindowRect = new Rect(50, 2, 0, 0);
            inspectorGUIScroll = Vector2.zero;
            ExpandWindow = false;
            IsEditingConnectionsInScene = false;
            IsAppendingToSelectedWaypoint = false;
            WillFocusOnSelect = true;
            WillSelectNewAfterAppending = true;
            pointsProp = serializedObject.FindProperty("points");

            list = new ReorderableList(serializedObject, pointsProp, false, true, true, true);
            list.drawHeaderCallback = DrawListHeader;
            list.drawElementCallback = DrawListElement;
            list.elementHeight = 2f * (EditorGUIUtility.singleLineHeight + GUIVERTSPACING);
            list.onAddCallback = AddListElement;
            list.onRemoveCallback = RemoveListElement;
            list.onSelectCallback = ListItemSelected;

            SelectedWaypoint = null;
        }

        #region Custom scene view Behaviour

        public void OnSceneGUI()
        {
            Waypoints waypoints = target as Waypoints;

            // Poll the shortcut for appending to the selected waypoint (A).
            if (HasSelection && EditorWindow.mouseOverWindow == SceneView.currentDrawingSceneView && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A)
                ToggleAppendingMode();

            // Poll the shortcut for editing connections in the scene view (S).
            if (HasSelection && EditorWindow.mouseOverWindow == SceneView.currentDrawingSceneView && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.S)
                IsEditingConnectionsInScene = !IsEditingConnectionsInScene;

            // Poll the shortcut for deleting the selected waypoint (D).
            if (HasSelection && EditorWindow.mouseOverWindow == SceneView.currentDrawingSceneView && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D)
                DeleteSelectedWaypoint();

            if (IsAppendingToSelectedWaypoint)
                DrawAppendSphere();

            for (int index = 0; index < waypoints.points.Count; index++)
            {
                Waypoint waypoint = waypoints.points[index];
                DrawConnections(waypoint);
                DrawWaypoint(waypoint, index);
                DrawName(waypoint);
            }

            Handles.BeginGUI();
            waypointGUIWindowRect = GUILayout.Window(0, waypointGUIWindowRect, WaypointsWindow, "Waypoints");
            Handles.EndGUI();
        }

        void DrawAppendSphere()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

            if (HandleUtility.PlaceObject(Event.current.mousePosition, out Vector3 position, out Vector3 normal))
            {
                if (Handles.Button(position, Quaternion.identity, 1.25f * SELECTEDSIZE, 1.35f * SELECTEDSIZE, Handles.SphereHandleCap))
                {
                    AppendToSelectedWaypoint(position);
                }
            }
        }

        void DrawConnections(Waypoint waypoint)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            foreach (WaypointConnection connection in waypoint.Connections)
            {
                Waypoint source = waypoint;
                Waypoint destination = connection.destination;

                Vector3 toDestination = (connection.destination.Position - waypoint.Position).normalized;
                Vector3 midpoint = (connection.destination.Position + waypoint.Position) / 2f;

                // Line
                Handles.color = new Color(1f, 1f, 1f, 0.33f);
                Handles.DrawLine(waypoint.Position, connection.destination.Position - UNSELECTEDSIZE * toDestination, LINETHICKNESS);

                // Cone
                Vector3 conePosition = connection.destination.Position - UNSELECTEDSIZE * toDestination;
                Handles.color = Color.black;
                Handles.ConeHandleCap(0, conePosition, Quaternion.LookRotation(toDestination), CONESIZE, EventType.Repaint);

                // Split Connection handle
                if (!IsEditingConnectionsInScene && HasSelection && (source == SelectedWaypoint || destination == SelectedWaypoint))
                {
                    Handles.color = REMOVINGCONNECTIONCOLOR;
                    if (Handles.Button(midpoint, Quaternion.LookRotation(toDestination), UNSELECTEDSIZE, 1.1f * UNSELECTEDSIZE, Handles.SphereHandleCap))
                    {
                        SplitConnection(connection, midpoint);

                        // If we don't return now, the next iteration of the foreach loop 
                        // will throw an exception.
                        return;
                    }
                }
            }
        }

        void DrawWaypoint(Waypoint waypoint, int index)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

            bool isSelectedWaypoint = ReferenceEquals(waypoint, SelectedWaypoint);
            bool doesSelectedPointHere = HasSelection && SelectedWaypoint.HasConnection(waypoint);
            bool HerePointsToSelectedWaypoint = HasSelection && waypoint.HasConnection(SelectedWaypoint);

            float handleSize = isSelectedWaypoint ? SELECTEDSIZE : UNSELECTEDSIZE;

            // Check if this is the currently selected waypoint, and if so, draw the movement handles and return.
            if (isSelectedWaypoint)
            {
                Handles.color = SELECTEDCOLOR;

                if (!IsAppendingToSelectedWaypoint)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 position = Handles.PositionHandle(waypoint.Position, Quaternion.identity);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, $"Move Waypoint: {SelectedWaypoint.Name}");
                        SelectedWaypoint.Position = position;
                    }
                }

                Handles.SphereHandleCap(0, waypoint.Position, Quaternion.identity, handleSize, EventType.Repaint);
                return;
            }

            // This is not the selected waypoint.
            if (IsEditingConnectionsInScene)
            {
                Handles.color = doesSelectedPointHere ? REMOVINGCONNECTIONCOLOR : ADDINGCONNECTIONCOLOR;

                if (Handles.Button(waypoint.Position, Quaternion.identity, handleSize, 1.5f * handleSize, Handles.SphereHandleCap))
                {
                    if (doesSelectedPointHere)
                    {
                        Undo.RecordObject(target, $"Removed Waypoint Connection: {SelectedWaypoint.Name} To {waypoint.Name}");
                        SelectedWaypoint.RemoveConnection(waypoint);
                    }
                    else
                    {
                        Undo.RecordObject(target, $"Added Waypoint Connection: {SelectedWaypoint.Name} To {waypoint.Name}");
                        SelectedWaypoint.AddConnection(waypoint);
                    }
                }
            }
            else
            {
                Handles.color = UNSELECTEDCOLOR;

                if (Handles.Button(waypoint.Position, Quaternion.identity, handleSize, 1.5f * handleSize, Handles.SphereHandleCap))
                {
                    SelectWaypoint(waypoint);
                    list.Select(index);
                    Repaint();
                }
            }
        }

        void DrawName(Waypoint waypoint)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            bool isSelectedWaypoint = ReferenceEquals(waypoint, SelectedWaypoint);
            bool doesSelectedPointHere = HasSelection && SelectedWaypoint.HasConnection(waypoint);
            bool HerePointsToSelectedWaypoint = HasSelection && waypoint.HasConnection(SelectedWaypoint);

            Handles.BeginGUI();

            Vector2 waypointGUIPos = HandleUtility.WorldToGUIPoint(waypoint.Position);
            if (WillShowAllNames || isSelectedWaypoint || doesSelectedPointHere || HerePointsToSelectedWaypoint)
            {
                Rect nameRect = new Rect(0f, 0f, 90f, 20f);
                nameRect.x = waypointGUIPos.x + 15f;
                nameRect.y = waypointGUIPos.y - 10f;
                GUIStyle centreAlignedLabel = new GUIStyle(GUI.skin.label);
                centreAlignedLabel.alignment = TextAnchor.MiddleCenter;

                GUI.Label(nameRect, waypoint.Name, centreAlignedLabel);
            }

            Handles.EndGUI();
        }

        void WaypointsWindow(int windowID)
        {
            // Initialisation of GUIStyles.
            GUIStyle centreAlignedLabel = new GUIStyle(GUI.skin.label);
            centreAlignedLabel.alignment = TextAnchor.MiddleCenter;
            GUIStyle centreAlignedTextField = new GUIStyle(GUI.skin.textField);
            centreAlignedTextField.alignment = TextAnchor.MiddleCenter;

            // Expand/hide this window
            string expandOrHideText = ExpandWindow ? "Hide" : "Expand";
            if (GUILayout.Button(expandOrHideText, GUILayout.MinWidth(120f)))
            { 
                ExpandWindow = !ExpandWindow;
                waypointGUIWindowRect.width = 0;
                waypointGUIWindowRect.height = 0;
            }

            if (!ExpandWindow)
                return;

            // Simple actions and settings
            if (GUILayout.Button("Create New"))
                CreateUnconnectedWaypoint();

            if (GUILayout.Button("Delete All"))
                DeleteAllWaypoints(viaSerializedObject: false);

            WillShowAllNames = EditorGUILayout.ToggleLeft("Show All Names", WillShowAllNames);
            WillFocusOnSelect = EditorGUILayout.ToggleLeft("Focus On Select", WillFocusOnSelect);
            WillSelectNewAfterAppending = EditorGUILayout.ToggleLeft("Select After Appending", WillSelectNewAfterAppending);

            GUILayout.Space(3);

            // This part of the window shows the current selection.
            if (IsAppendingToSelectedWaypoint)
            {
                GUILayout.Label($"Appending To:\n{SelectedWaypoint.Name}...", centreAlignedLabel);
                if (GUILayout.Button("Cancel"))
                    IsAppendingToSelectedWaypoint = false;
            }
            else if (HasSelection)
            {
                GUILayout.Label("Selection:", centreAlignedLabel);

                EditorGUI.BeginChangeCheck();
                string name = EditorGUILayout.TextField(SelectedWaypoint.Name, centreAlignedTextField);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, $"Rename Waypoint: {SelectedWaypoint.Name}");
                    SelectedWaypoint.Name = name;
                }

                EditorGUI.BeginChangeCheck();
                Vector3 position = EditorGUILayout.Vector3Field("", SelectedWaypoint.Position);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, $"Move Waypoint: {SelectedWaypoint.Name}");
                    SelectedWaypoint.Position = position;
                }

                if (GUILayout.Button("Append (A)"))
                    ToggleAppendingMode();

                if (GUILayout.Button("Focus"))
                    SceneView.currentDrawingSceneView.LookAt(SelectedWaypoint.Position);

                if (GUILayout.Button("Connect In Scene (S)"))
                    IsEditingConnectionsInScene = !IsEditingConnectionsInScene;

                if (EditorGUILayout.DropdownButton(new GUIContent("View Connections"), FocusType.Passive, GUI.skin.button))
                    CreateWaypointsDropdownMenu(SelectedWaypoint);

                if (GUILayout.Button("Deselect"))
                    DeselectWaypoint();

                if (GUILayout.Button("Delete (D)"))
                    DeleteSelectedWaypoint();
            }
            else
                GUILayout.Label("No Waypoint Selected", centreAlignedLabel);

            GUI.DragWindow();
        }
         
        #endregion

        #region Custom inspector behaviour

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("Delete All Waypoints"))
            {
                Undo.SetCurrentGroupName("Delete All Waypoints");
                DeleteAllWaypoints(viaSerializedObject: true);
            }

            inspectorGUIScroll = EditorGUILayout.BeginScrollView(inspectorGUIScroll, GUILayout.Height(100));

            list.DoLayoutList();

            EditorGUILayout.EndScrollView();

            if (GUI.changed)
                SceneView.RepaintAll();

            serializedObject.ApplyModifiedProperties();
            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
        }

        private void DrawListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Waypoints");
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            serializedObject.Update();

            // Initialisation
            SerializedProperty listWaypointProp = pointsProp.GetArrayElementAtIndex(index);
            Waypoint listWaypoint = listWaypointProp.managedReferenceValue as Waypoint;

            bool loneWaypoint = list.count <= 1;

            float topLeftBlankWidth = 15f;
            float bottomRightBlankWidth = 10f;
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float widthSelect = 65f;
            float widthConnections = loneWaypoint ? -GUIHORZSPACING : 85f;
            float widthName = rect.width - topLeftBlankWidth - widthSelect - widthConnections - 3 * GUIHORZSPACING;
            float widthDelete = 50f;
            float widthPosition = rect.width - widthDelete - bottomRightBlankWidth;
            float yLineOne = rect.y + GUIVERTSPACING / 2f;
            float yLineTwo = rect.y + EditorGUIUtility.singleLineHeight + GUIVERTSPACING;
            float xName = rect.x + topLeftBlankWidth;
            float xSelect = rect.x + topLeftBlankWidth + widthName + GUIHORZSPACING;
            float xConnections = rect.x + topLeftBlankWidth + widthName + widthSelect + 2 * GUIHORZSPACING;
            float xPosition = rect.x;
            float xDelete = rect.x + widthPosition + GUIHORZSPACING;

            Rect rectName = new Rect(xName, yLineOne, widthName, singleLineHeight);
            Rect rectSelect = new Rect(xSelect, yLineOne, widthSelect, singleLineHeight);
            Rect rectConnections = new Rect(xConnections, yLineOne, widthConnections, singleLineHeight);
            Rect rectPosition = new Rect(xPosition, yLineTwo, widthPosition, singleLineHeight);
            Rect rectDelete = new Rect(xDelete, yLineTwo, widthDelete, singleLineHeight);

            // Name field
            EditorGUI.BeginChangeCheck();
            string name = EditorGUI.TextField(rectName, listWaypoint.Name);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"Rename Waypoint {listWaypoint.Name}");
                listWaypoint.Name = name;
            }

            // Select/Deselect Button
            bool isSelected = ReferenceEquals(listWaypoint, SelectedWaypoint);
            if (!isSelected && GUI.Button(rectSelect, "Select"))
            {
                SelectWaypoint(listWaypoint);
                list.Select(index);
            }
            else if (isSelected && GUI.Button(rectSelect, "Deselect"))
            {
                DeselectWaypoint();
                list.Deselect(index);
            }

            // Connections button - disappears if there's only one waypoint.
            // Clicking a waypoint in the dropdown menu add/removes a connection to it.
            if (!loneWaypoint && EditorGUI.DropdownButton(rectConnections, new GUIContent("Connections"), FocusType.Passive, GUI.skin.button))
                CreateWaypointsDropdownMenu(listWaypoint);

            // Position Vector3 field
            EditorGUI.BeginChangeCheck();
            Vector3 position = EditorGUI.Vector3Field(rectPosition, GUIContent.none, listWaypoint.Position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, $"Move Waypoint: {listWaypoint.Name}");
                listWaypoint.Position = position;
            }

            // Delete button
            if (GUI.Button(rectDelete, "Delete"))
            {
                // We must redraw the reorderable list here or face possible errors.
                Undo.SetCurrentGroupName($"Delete Waypoint: {listWaypoint.Name}");
                RemoveWaypoint(listWaypoint);
                list.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddListElement(ReorderableList list)
        {
            serializedObject.Update();

            SerializedProperty nextWaypointCount = serializedObject.FindProperty("totalAddedCounter");
            Undo.SetCurrentGroupName($"Create Waypoint: Waypoint {nextWaypointCount.intValue:000}");

            int newElementIndex = pointsProp.arraySize;
            pointsProp.arraySize += 1;
            SerializedProperty newElement = pointsProp.GetArrayElementAtIndex(newElementIndex);
            Waypoint newWaypoint = new Waypoint($"Waypoint {nextWaypointCount.intValue:000}", Vector3.up);
            nextWaypointCount.intValue += 1;
            newElement.managedReferenceValue = newWaypoint;

            SelectWaypoint(newWaypoint);
            list.Select(newElementIndex);

            serializedObject.ApplyModifiedProperties();

            SceneView.lastActiveSceneView.LookAt(Vector3.zero);
        }

        private void RemoveListElement(ReorderableList list)
        {
            serializedObject.Update();

            Waypoint dyingWaypoint = pointsProp.GetArrayElementAtIndex(list.index).managedReferenceValue as Waypoint;
            Undo.SetCurrentGroupName($"Delete Waypoint: {dyingWaypoint.Name}");

            // Remove all connections to this waypoint
            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                Waypoint loopWaypoint = pointsProp.GetArrayElementAtIndex(i).managedReferenceValue as Waypoint;
                if (loopWaypoint.HasConnection(dyingWaypoint))
                    loopWaypoint.RemoveConnection(dyingWaypoint);
            }
            // Bring the changes to connections on the live Waypoints object onto serializedObject.
            serializedObject.Update();

            // Remove the waypoint from the list
            pointsProp.DeleteArrayElementAtIndex(list.index);
            serializedObject.ApplyModifiedProperties();
            DeselectWaypoint();

            // Repaint the scene view
            SceneView.RepaintAll();
        }

        private void ListItemSelected(ReorderableList list)
        {
            SerializedProperty waypointsProperty = list.serializedProperty;
            Waypoint waypoint = waypointsProperty.GetArrayElementAtIndex(list.index).managedReferenceValue as Waypoint;
            SelectWaypoint(waypoint);
            SceneView.RepaintAll();
        }

        #endregion

        private void AppendToSelectedWaypoint(Vector3 position)
        {
            Waypoints waypoints = target as Waypoints;

            Undo.RecordObject(target, $"Appended Waypoint: {waypoints.NextWaypointName} to {SelectedWaypoint.Name}");

            Waypoint newWaypoint = waypoints.Add(position);
            SelectedWaypoint.AddConnection(newWaypoint);
            IsAppendingToSelectedWaypoint = false;

            if (WillSelectNewAfterAppending)
                SelectWaypoint(newWaypoint);

            Repaint();
        }

        private void CreateUnconnectedWaypoint()
        {
            Waypoints waypoints = target as Waypoints;

            Undo.RecordObject(target, $"Create Waypoint: {waypoints.NextWaypointName}");

            Waypoint newWaypoint = waypoints.Add(waypoints.transform.position);
            SceneView.currentDrawingSceneView.LookAt(newWaypoint.Position);
            SelectWaypoint(newWaypoint);

            Repaint();
        }

        private void CreateWaypointsDropdownMenu(Waypoint hostWaypoint)
        {
            serializedObject.Update();

            GenericMenu menu = new GenericMenu();
            menu.allowDuplicateNames = true;

            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                SerializedProperty loopProperty = pointsProp.GetArrayElementAtIndex(i);
                Waypoint loopWaypoint = loopProperty.managedReferenceValue as Waypoint;

                if (ReferenceEquals(loopWaypoint, hostWaypoint))
                    continue;

                CreateWaypointMenuItem(menu, hostWaypoint, loopWaypoint);
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Close"), false, () => { });

            EditorGUI.BeginChangeCheck();

            menu.ShowAsContext();

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateWaypointMenuItem(GenericMenu menu, Waypoint hostWaypoint, Waypoint menuWaypoint)
        {
            bool hasOutgoingConnection = hostWaypoint.HasConnection(menuWaypoint);
            bool hasIncomingConnection = menuWaypoint.HasConnection(hostWaypoint);

            GenericMenu.MenuFunction RemoveOutgoingConnection = () =>
            {
                Undo.RecordObject(target, $"Removed Waypoint Connection: {hostWaypoint.Name} To {menuWaypoint.Name}");

                hostWaypoint.RemoveConnection(menuWaypoint);
            };

            GenericMenu.MenuFunction AddOutgoingConnection = () =>
            {
                Undo.RecordObject(target, $"Added Waypoint Connection: {hostWaypoint.Name} To {menuWaypoint.Name}");

                hostWaypoint.AddConnection(menuWaypoint);
            };

            GenericMenu.MenuFunction RemoveIncomingConnection = () =>
            {
                Undo.RecordObject(target, $"Removed Waypoint Connection: {menuWaypoint.Name} To {hostWaypoint.Name}");

                menuWaypoint.RemoveConnection(hostWaypoint);
            };

            GenericMenu.MenuFunction AddIncomingConnection = () =>
            {
                Undo.RecordObject(target, $"Added Waypoint Connection: {menuWaypoint.Name} To {hostWaypoint.Name}");

                menuWaypoint.AddConnection(hostWaypoint);
            };

            if (hasOutgoingConnection)
                menu.AddItem(new GUIContent($"Outgoing/Connected/{menuWaypoint.Name}"), true, RemoveOutgoingConnection);
            else
                menu.AddItem(new GUIContent($"Outgoing/Not Connected/{menuWaypoint.Name}"), false, AddOutgoingConnection);

            if (hasIncomingConnection)
                menu.AddItem(new GUIContent($"Incoming/Connected/{menuWaypoint.Name}"), true, RemoveIncomingConnection);
            else
                menu.AddItem(new GUIContent($"Incoming/Not Connected/{menuWaypoint.Name}"), false, AddIncomingConnection);
        }

        private void DebugProperty(SerializedProperty property)
        {
            string arraySizeString = property.isArray ? $"Array size: {property.arraySize}" : "";
            Debug.Log($"Property name: {property.displayName}. Path: {property.propertyPath}. Type: {property.propertyType}. Is array: {property.isArray}. {arraySizeString}");
        }

        private void DeselectWaypoint()
        {
            SelectedWaypoint = null;
            IsEditingConnectionsInScene = false;
            waypointGUIWindowRect.height = 0;

            Repaint();
        }

        private void DeleteSelectedWaypoint()
        {
            Undo.RecordObject(target, $"Delete Waypoint: {SelectedWaypoint.Name}");

            Waypoints waypoints = target as Waypoints;
            waypoints.Remove(SelectedWaypoint);
            DeselectWaypoint();
            Repaint();
        }

        private void DeleteAllWaypoints(bool viaSerializedObject)
        {
            Undo.RecordObject(target, "Delete All Waypoints");

            if (viaSerializedObject)
            {
                pointsProp.ClearArray();
                serializedObject.FindProperty("totalAddedCounter").intValue = 0;
                DeselectWaypoint();
            }
            else
            {
                Waypoints waypoints = target as Waypoints;
                waypoints.DeleteAll();
                DeselectWaypoint();
            }
        }

        private void RemoveWaypoint(Waypoint waypoint)
        {
            // Remove all connections to this waypoint.
            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                Waypoint loopWaypoint = pointsProp.GetArrayElementAtIndex(i).managedReferenceValue as Waypoint;
                if (loopWaypoint.HasConnection(waypoint))
                    loopWaypoint.RemoveConnection(waypoint);
            }
            // Bring the changes on the live Waypoints object onto serializedObject.
            serializedObject.Update();

            // Remove the waypoint from the list
            pointsProp.DeleteArrayElementAtIndex(list.index);
            serializedObject.ApplyModifiedProperties();

            // If the currently selected waypoint is being deleted, unselect it.
            if (ReferenceEquals(waypoint, SelectedWaypoint))
                DeselectWaypoint();

            // Repaint the scene view.
            SceneView.RepaintAll();
        }

        private void SelectWaypoint(Waypoint waypoint)
        {
            SelectedWaypoint = waypoint;

            if (WillFocusOnSelect)
                SceneView.lastActiveSceneView.LookAt(waypoint.Position);
        }

        private void SplitConnection(WaypointConnection connection, Vector3 position)
        {
            Waypoint source = connection.source;
            Waypoint destination = connection.destination;

            Undo.RecordObject(target, $"Split Waypoint Connection: {source.Name} To {destination.Name}");

            source.RemoveConnection(connection);
            Waypoint newWaypoint = (target as Waypoints).Add(position);
            source.AddConnection(newWaypoint);
            newWaypoint.AddConnection(destination);
            SelectWaypoint(newWaypoint);
        }

        private void ToggleAppendingMode()
        {
            IsAppendingToSelectedWaypoint = !IsAppendingToSelectedWaypoint;
            waypointGUIWindowRect.height = 0;
        }
    }
}
