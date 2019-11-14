﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using AltSalt.Maestro.Animation;

namespace AltSalt.Maestro
{
    public static class EditorDuplicateCommand
    {
        [MenuItem("Edit/AltSaltDuplicate #d", false, 0)]
        static void DuplicateSelection()
        {
            List<UnityEngine.Object> newSelection = new List<UnityEngine.Object>();
            for (int i = 0; i < Selection.gameObjects.Length; i++) {
        
                GameObject sourceObject = Selection.gameObjects[i];
                GameObject newGo = Utils.DuplicateObject(sourceObject);

                newGo.transform.parent = sourceObject.transform.parent;
                newGo.transform.SetSiblingIndex(sourceObject.transform.GetSiblingIndex() + 1);
                Undo.RegisterCreatedObjectUndo(newGo, "Duplicate");

                newSelection.Add(newGo);
            }

            if(TimelineEditor.inspectedAsset != null) {
                newSelection.AddRange(TrackPlacement.DuplicateTracks(TimelineEditor.inspectedAsset, TimelineEditor.inspectedDirector, Selection.objects));
                TimelineUtils.RefreshTimelineContentsAddedOrRemoved();
            }

            Selection.objects = newSelection.ToArray();
        }
    }
}