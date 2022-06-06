using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GDDProject.Dialogues.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        Vector2 scrollPosition;

        [NonSerialized]
        GUIStyle nodeStyle;

        [NonSerialized]
        GUIStyle playerNodeStyle;

        [NonSerialized]
        DialogueNode draggingNode = null;

        [NonSerialized]
        Vector2 draggingOffset;

        [NonSerialized]
        DialogueNode creatingNode = null;

        [NonSerialized]
        DialogueNode deletingNode = null;

        [NonSerialized]
        DialogueNode linkingParentNode = null;

        [NonSerialized]
        bool draggingCanvas = false;

        [NonSerialized]
        Vector2 draggingCanvasOffset;

        const float canvasSize = 4000;
        const float backgroundSize = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }
        
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }
 
        private void OnEnable()
        {
            // This is a callback that triggers whenever an asset is selected/unselected.
            Selection.selectionChanged += OnSelectionChanged;

            // set up our dialogue node style
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChanged()
        {
            // show the Dialogue scriptable object in the inspector
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();  // updates the editor window UI; involves calling OnGUI() to change the label field in editor window
            }
        }

        // Callback triggered when we move the cursor over editor window.
        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                // create a scroll view in editor window
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                // reserve an auto layout space to occupy our scroll view
                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);

                // load png to be used for editor window background
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                
                // apply the png with appropriate scaling
                Rect textureCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    // create a new node, branched off from the parent node creatingNode
                    selectedDialogue.CreateNode(creatingNode);

                    creatingNode = null;
                }
                if (deletingNode != null)
                {
                    // delete the node
                    selectedDialogue.DeleteNode(deletingNode);

                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            // handle Mouse click & drag events
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    // select this node to show in inspector
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    // record scroll view drag offset
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    // unselect this node
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                // prevent node from snapping its top left corner to mouse position
                draggingNode.SetRectPosition(Event.current.mousePosition + draggingOffset);
                
                // refresh GUI to reflect the change in node's position
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                // update scroll view position
                // this updated scrollPosition is used to create a new scroll view in OnGUI(), which shows
                // the scrolled changes
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            // make sure that we are dragging the top node if multiple nodes overlap each other
            DialogueNode topNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                // check if a node's rect surrounds the cursor position
                if (node.GetRect().Contains(point))
                {
                    // the top node is the final node in the list
                    topNode = node;
                }
            }
            return topNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }

            // create a dialogue node UI
            GUILayout.BeginArea(node.GetRect(), style);

            // start noting any changes in fields during this OnGUI() call
            EditorGUI.BeginChangeCheck();

            string newText = EditorGUILayout.TextField(node.GetText());

            // stop noting field changes in this OnGUI() call and return whether a change is detected
            if (EditorGUI.EndChangeCheck())
            {
                // enable undoing in editor
                node.SetText(newText);
            }

            GUILayout.BeginHorizontal();

            // add a button to our node so we can click on it to create new node
            if (GUILayout.Button("+"))
            {
                // if the button is clicked, create a new node
                creatingNode = node;
            }

            // add a button to delete this node
            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }

            HandleLinkButton(node);

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void HandleLinkButton(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                // add a button to link to other nodes
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else
            {
                if (node == linkingParentNode)
                {
                    if (GUILayout.Button("cancel"))
                    {
                        // cancel linking
                        linkingParentNode = null;
                    }
                }
                else
                {
                    if (linkingParentNode.GetChildren().Contains(node.name))
                    {
                        if (GUILayout.Button("unlink"))
                        {
                            // unlink this child node
                            linkingParentNode.RemoveChild(node.name);
                            linkingParentNode = null;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("child"))
                        {
                            // link this child node
                            linkingParentNode.AddChild(node.name);
                            linkingParentNode = null;
                        }
                    }
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector3(node.GetRect().xMax, node.GetRect().center.y, 0);
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {                
                Vector3 endPosition = new Vector3(childNode.GetRect().xMin, childNode.GetRect().center.y, 0);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                // draw the Bezier curve between related nodes
                Handles.DrawBezier(startPosition, endPosition, 
                    startPosition + controlPointOffset, 
                    endPosition - controlPointOffset,
                    Color.white, null, 4.0f);
            }
        }
    }
}
