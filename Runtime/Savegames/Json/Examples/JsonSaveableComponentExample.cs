using UnityEngine;

namespace GlitchedPolygons.SavegameFramework.Json.Examples
{
    /// <summary>
    /// Example implementation of the <see cref="JsonSaveableComponent"/> (for the JSON implementation of the <see cref="SavegameFramework"/>).
    /// </summary>
    public class JsonSaveableComponentExample : JsonSaveableComponent
    {
        [SerializeField]
        private int testInt = 42;

        [SerializeField]
        private bool testBool = true;

        [SerializeField]
        private Vector2 testVec2 = Vector2.right;

        [SerializeField]
        private string testStr = "Test \n multi line \n strings \n with special characters sauce ayyyy *%ç*/\\   \\ \\\\ // / /// .. ,, ../ ..\\ @ @@ *** !!! ç%/";

        [SerializeField]
        private Vector3 savedCoordinates;

        [SerializeField]
        private Quaternion savedOrientation;

        public override void BeforeSaving()
        {
            Transform thisTransform = transform;

            savedCoordinates = thisTransform.position;
            savedOrientation = thisTransform.rotation;
        }

        public override void AfterLoading()
        {
            Transform thisTransform = transform;

            thisTransform.position = savedCoordinates;
            thisTransform.rotation = savedOrientation;
        }
    }
}

// Copyright (C) Raphael Beck, 2022 | https://glitchedpolygons.com