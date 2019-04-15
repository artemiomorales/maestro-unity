﻿using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;

public class DuplicateTimeline
{
    [MenuItem("Timeline/Duplicate With Bindings", true)]
    public static bool DuplicateWithBindingsValidate()
    {
        if (UnityEditor.Selection.activeGameObject == null)
            return false;

        var playableDirector = UnityEditor.Selection.activeGameObject.GetComponent<PlayableDirector>();
        if (playableDirector == null)
            return false;

        var playableAsset = playableDirector.playableAsset;
        if (playableAsset == null)
            return false;

        var path = AssetDatabase.GetAssetPath(playableAsset);
        if (string.IsNullOrEmpty(path))
            return false;

        return true;
    }

    [MenuItem("Timeline/Duplicate With Bindings")]
    public static void DuplicateWithBindings()
    {
        if (UnityEditor.Selection.activeGameObject == null)
            return;

        var playableDirector = UnityEditor.Selection.activeGameObject.GetComponent<PlayableDirector>();
        if (playableDirector == null)
            return;

        var playableAsset = playableDirector.playableAsset;
        if (playableAsset == null)
            return;

        var path = AssetDatabase.GetAssetPath(playableAsset);
        if (string.IsNullOrEmpty(path))
            return;

        string newPath = path.Replace(".", "(Clone).");
        if (!AssetDatabase.CopyAsset(path, newPath)) {
            Debug.LogError("Couldn't Clone Asset");
            return;
        }

        var newPlayableAsset = AssetDatabase.LoadMainAssetAtPath(newPath) as PlayableAsset;
        var gameObject = Selection.activeGameObject;

        GameObject newGo;

        GameObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(gameObject) as GameObject;
        if (prefabRoot != null) {
            newGo = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
            PrefabUtility.SetPropertyModifications(newGo, PrefabUtility.GetPropertyModifications(gameObject));
        }
        else {
            newGo = GameObject.Instantiate(gameObject);
        }

        var newPlayableDirector = gameObject.GetComponent<PlayableDirector>();
        newPlayableDirector.playableAsset = newPlayableAsset;

        var oldBindings = playableAsset.outputs.GetEnumerator();
        var newBindings = newPlayableAsset.outputs.GetEnumerator();


        while (oldBindings.MoveNext()) {
            var oldBindings_sourceObject = oldBindings.Current.sourceObject;

            newBindings.MoveNext();

            var newBindings_sourceObject = newBindings.Current.sourceObject;


            newPlayableDirector.SetGenericBinding(
                newBindings_sourceObject,
                playableDirector.GetGenericBinding(oldBindings_sourceObject)
            );
        }
    }
}
