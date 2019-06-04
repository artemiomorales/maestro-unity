﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class InteractableRectTransform : MonoBehaviour
    {
        protected RectTransform rectTransform;

        protected virtual void Start()
        {
            GetComponents();
        }

        protected virtual void GetComponents()
        {
            GetRectTransform();
        }

        void GetRectTransform()
        {
            if (rectTransform == null) {
                rectTransform = GetComponent<RectTransform>();
            }
        }

    }
}