using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace GDDProject.Dialogues.Editor
{
    public class DialogueModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        // called right before asset movement/rename
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            // modify Dialogue scriptable object's name to resolve a bug in Unity

            // retrieve the asset at sourcePath, which may not be a Dialogue scriptable object (this callback
            // is called everytime we move/rename an asset!)
            Dialogue dialogue = AssetDatabase.LoadMainAssetAtPath(sourcePath) as Dialogue;

            // check if the asset that we renamed is indeed a Dialogue scriptable object
            if (dialogue == null)
            {
                return AssetMoveResult.DidNotMove;
            }

            // check if what we have done is indeed a rename and not an asset move between directories
            if (Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath))
            {
                return AssetMoveResult.DidNotMove;
            }

            // if we reached here, then we know for sure that we have renamed a Dialogue scriptable object
            // modify the name!
            dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);

            // we are not handling the asset movement logic here, so we will let Unity handle it by telling
            // it that we did not move the asset
            return AssetMoveResult.DidNotMove;
        }

        private static bool MovingDirectory(string sourcePath, string destinationPath)
        {
            return Path.GetDirectoryName(sourcePath) != Path.GetDirectoryName(destinationPath);
        }
    }
}
