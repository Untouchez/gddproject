using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDDProject.Dialogues
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] Vector2 newNodeOffsetFromParent = new Vector2(50, 0);

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        // Awake() for a scriptable object will be called when that scriptable object is loaded/created in Unity in edit time.
        private void Awake()
        {
        }
#endif

        // Callback called by Unity when some value of this scriptable object is changed in the
        // inspector (in this case, whenever the nodes list is changed, we want to rebuild our lookup table).
        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach (var node in GetAllNodes())
            {
                nodeLookup.Add(node.name, node);
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (var childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                     yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);

            // register actual creation of a node
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            // register change in this Dialogue (before any changes to its state are done)
            Undo.RecordObject(this, "Added Dialogue Node");

            AddNode(newNode);
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            // instantiate a scriptable object
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = System.Guid.NewGuid().ToString();

            if (parent != null)
            {
                parent.AddChild(newNode.name);
                // alternate the new node's speaker between the player and AI
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                // create the new node next to its parent node to make it easier to find
                newNode.SetRectPosition(parent.GetRect().position + newNodeOffsetFromParent);
            }

            return newNode;
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            // register change in this dialogue (before any changes to its state are done)
            Undo.RecordObject(this, "Deleted Dialogue Node");

            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanChildren(nodeToDelete);

            // register actual deletion of a node
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        // Callback when we save
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (nodes.Count == 0)
            {
                DialogueNode rootNode = MakeNode(null);
                AddNode(rootNode);
            }

            // we are about to save this Dialogue object file
            // go through all the nodes and save them as new asset files inside the asset database
            if (AssetDatabase.GetAssetPath(this).Length != 0)
            {
                // an asset file for this Dialogue file exists!
                // note that it only exists after Awake() has finished calling
                foreach (DialogueNode node in GetAllNodes())
                {
                    // check if a given Dialogue Node has already been added to asset database (ie already has an asset file)
                    if (AssetDatabase.GetAssetPath(node).Length == 0)
                    {
                        // no asset file for this node, create one!
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
