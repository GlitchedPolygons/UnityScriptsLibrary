using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

namespace GlitchedPolygons.SavegameFramework.Xml.Examples
{
    /// <summary>
    /// Example implementation of the <see cref="XmlSaveableComponent"/> (for the xml savegame framework).
    /// </summary>
    public class XmlSaveableComponentExample : XmlSaveableComponent
    {
        #region Constants

        /// <summary>
        /// Cached "true" string.
        /// </summary>
        private const string TRUE = "true";

        /// <summary>
        /// Cached "false" string.
        /// </summary>
        private const string FALSE = "false";

        /// <summary>
        /// Cache the entire root XElement to return from the <see cref="GetXml"/> method.<para> </para>
        ///
        /// If you do this on all <see cref="XmlSaveableComponent"/>s, this will tremendously decrease the GC Alloc when saving the game.<para> </para>
        ///
        /// Also: use the <c>nameof()</c> operator instead of magic strings to avoid any catastrophes when renaming one or more variables in your code.
        /// </summary>
        private readonly XElement XELEMENT = new("test",
            new XElement("position", null),
            new XElement("rotation", null),
            new XElement(nameof(testInt), null),
            new XElement(nameof(testString), null),
            new XElement(nameof(testFloat), null),
            new XElement(nameof(testBool), null)
        );

        #endregion

        #region Cached XElements

        private XElement xe_testInt = null;
        private XElement XE_TestInt => xe_testInt ??= XELEMENT.Element(nameof(testInt));

        private XElement xe_testString = null;
        private XElement XE_TestString => xe_testString ??= XELEMENT.Element(nameof(testString));

        private XElement xe_testFloat = null;
        private XElement XE_TestFloat => xe_testFloat ??= XELEMENT.Element(nameof(testFloat));

        private XElement xe_testBool = null;
        private XElement XE_TestBool => xe_testBool ??= XELEMENT.Element(nameof(testBool));

        private XElement xe_position = null;
        private XElement XE_Position => xe_position ??= XELEMENT.Element("position");

        private XElement xe_rotation = null;
        private XElement XE_Rotation => xe_rotation ??= XELEMENT.Element("rotation");

        #endregion

        // Here some test variables that need to persist via savegames:

        public int testInt = 7;
        public string testString = "SAUCE?";
        public float testFloat = 42.0f;
        public bool testBool = true;

        private Vector3 loadedPosition;
        private Quaternion loadedRotation;

        [ContextMenu("Randomize")]
        private void Randomize()
        {
            Debug.Log(this);

            testInt = UnityEngine.Random.Range(-9999, 9999);
            testString = Guid.NewGuid().ToString();
            testFloat = UnityEngine.Random.Range(-9999.9f, 9999.9f);
            testBool = UnityEngine.Random.value > 0.5f;
        }

        /// <inheritdoc/>
        public override XElement GetXml()
        {
            // This method will return an xml element that will be
            // embedded into the savegame xml tree. It should contain
            // all the data that is necessary to reconstruct this
            // component when the game is loaded. 

            // Set the cached XElements values and then return the entire thing.

            XE_TestInt.Value = testInt.ToString();
            XE_TestString.Value = testString;
            XE_TestFloat.Value = testFloat.ToString(CultureInfo.InvariantCulture);
            XE_TestBool.Value = testBool ? TRUE : FALSE;

            return XELEMENT;
        }

        /// <inheritdoc/>
        public override bool LoadXml(XElement xElement)
        {
            // Reconstruct the component's state based on the 
            // retrieved XElement (the one passed as method parameter).
            // Return true if everything was successful; return false if something went wrong...

            try
            {
                foreach (var subElement in xElement.Descendants())
                {
                    switch (subElement.Name.LocalName)
                    {
                        case nameof(testInt):
                            testInt = int.Parse(subElement.Value);
                            break;
                        case nameof(testFloat):
                            testFloat = float.Parse(subElement.Value);
                            break;
                        case nameof(testString):
                            testString = subElement.Value;
                            break;
                        case nameof(testBool):
                            testBool = string.CompareOrdinal(subElement.Value, TRUE) == 0;
                            break;
                        case "position":
                        {
                            string[] positionVectorComponents = subElement.Value.Split(';');

                            if (positionVectorComponents.Length == 3)
                            {
                                loadedPosition = new Vector3(float.Parse(positionVectorComponents[0]), float.Parse(positionVectorComponents[1]), float.Parse(positionVectorComponents[2]));
                            }

                            break;
                        }
                        case "rotation":
                        {
                            string[] rotationVectorComponents = subElement.Value.Split(';');

                            if (rotationVectorComponents.Length == 4)
                            {
                                loadedRotation = new Quaternion(float.Parse(rotationVectorComponents[0]), float.Parse(rotationVectorComponents[1]), float.Parse(rotationVectorComponents[2]), float.Parse(rotationVectorComponents[3]));
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"There was an error loading the savegame. Affected component: {this}... Exception: {e}");
                return false;
            }

            return true;
        }

        public override void BeforeSaving()
        {
            Transform thisTransform = transform;

            Vector3 position = thisTransform.position;
            Quaternion rotation = thisTransform.rotation;

            XE_Position.Value = $"{position.x};{position.y};{position.z}";
            XE_Rotation.Value = $"{rotation.x};{rotation.y};{rotation.z};{rotation.w}";
        }

        public override void AfterLoading()
        {
            Transform thisTransform = transform;

            thisTransform.position = loadedPosition;
            thisTransform.rotation = loadedRotation;
        }
    }
}

// Copyright (C) Raphael Beck, 2018 | https://glitchedpolygons.com