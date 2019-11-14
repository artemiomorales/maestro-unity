﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Modify Settings")]
    public class ModifySettings : ScriptableObject
    {
        [Header("Modify Settings")]
        [Required]
        public TextFamily defaultTextFamily;

        [Required]
        public TextFamily activeTextFamily;

        [Required]
        public LayoutConfig _defaultLayoutConfig;

        [Required]
        public LayoutConfig _activeLayoutConfig;
    }
}