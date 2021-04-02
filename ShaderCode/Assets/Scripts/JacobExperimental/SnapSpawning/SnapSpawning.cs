using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JacobExperimental.Snapping
{
    [ExecuteAlways]
    public class SnapSpawning : MonoBehaviour
    {
        private RaycastHit snapCast;
        private Vector3 transformAxis;

        public Transform snapObject;
        public SnapAxis axis;
        public bool invert = false;
        public bool snapping = false;

        // Update is called once per frame
        void Snapping()
        {
            Debug.Log(transformAxis);

            Debug.Log(Physics.defaultPhysicsScene.Raycast(transform.position, transformAxis, 100));

            if (Physics.Raycast(transform.position, transformAxis, out snapCast, 100))
            {
                snapObject.transform.position = snapCast.point;
                snapObject.transform.rotation = Quaternion.FromToRotation(transformAxis, snapCast.normal);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, snapCast.point);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transformAxis * 100);
            }

            Debug.Log(snapCast.point);
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR

            if (!Application.isPlaying && snapping)
            {
                transformAxis = (invert ? -1 : 1) * (axis == SnapAxis.X ? Vector3.right : (axis == SnapAxis.Z ? Vector3.forward : Vector3.up));

                // Performance be like oof
                Vector3 point = transform.position;

                Collider thisCollider = GetComponent<Collider>();
                foreach (Collider c in FindObjectsOfType(typeof(Collider)))
                {
                    if (c == thisCollider) continue;

                    int rayLength = 10;

                    for (int i = 0; i < (rayLength * rayLength) + 1; i++)
                    {
                        Vector3 check = point + (transformAxis * i * 0.01f);

                        if (c.bounds.Contains(check))
                        {
                            Debug.Log("WE HIT A COLLIDER: " + c.gameObject);

                            Debug.Log(check);

                            snapObject.transform.position = check;
                            snapping = false;

                            break;
                        }
                    }
                }

                //Snapping();
                UnityEditor.SceneView.RepaintAll();
            }
#endif
        }
    }

}