using UnityEngine;

namespace GlitchedPolygons.Utilities
{
    public class ColliderGizmo : MonoBehaviour
    {
        #if UNITY_EDITOR

        private BoxCollider boxCollider;
        private SphereCollider sphereCollider;
        
        [SerializeField]
        private Color triggerGizmoColor = new (1f, 0.5f, 0.0f, 0.3f);

        public void Refresh()
        {
            boxCollider = GetComponent<BoxCollider>();
            sphereCollider = GetComponent<SphereCollider>();
        }
        
        private void OnDrawGizmos()
        {
            Vector3 scale = transform.localScale;

            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider>();
            }

            if (boxCollider != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.color =  triggerGizmoColor * 1.2f;
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

                Gizmos.color = triggerGizmoColor;
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            }

            if (sphereCollider == null)
            {
                sphereCollider = GetComponent<SphereCollider>();
            }

            if (sphereCollider != null)
            {
                float radiusScale = Mathf.Max(scale.x, scale.y, scale.z);

                Vector3 center = sphereCollider.center;
                
                Vector3 position = transform.position + new Vector3(
                    center.x * scale.x,
                    center.y * scale.y,
                    center.z * scale.z);

                Gizmos.color = triggerGizmoColor * 1.2f;
                Gizmos.DrawWireSphere(position, sphereCollider.radius * radiusScale);

                Gizmos.color = triggerGizmoColor;
                Gizmos.DrawSphere(position, sphereCollider.radius * radiusScale);
            }
        }
#endif
    }
}

// Copyright (C) Raphael Beck, 2022 | https://glitchedpolygons.com