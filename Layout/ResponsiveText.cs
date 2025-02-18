﻿using UnityEngine;
using TMPro;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveText : ResponsiveLayoutElement
    {
        protected TMP_Text textMeshPro;

        protected override void OnEnable()
        {
            base.OnEnable();
            StoreText();
        }

        private void StoreText()
        {
            if (textMeshPro == null) {
                textMeshPro = GetComponent<TextMeshPro>();
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreText();
        }
    }
}