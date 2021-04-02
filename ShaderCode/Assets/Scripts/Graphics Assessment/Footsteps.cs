using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [System.Serializable]
    public struct FootStep
    {
        [HideInInspector] public RaycastHit ray;
        public Transform transform;
        public bool stepping;
        public float nextStep;
    }

    public FootStep[] feet;

    public VisualFXSystem.VisualFX fx;

    public void Step(int f)
    {
        fx.Begin(feet[f].transform);
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < feet.Length; i++)
        {
            FootStep foot = feet[i];

            bool stepping = Physics.Raycast(foot.transform.position, -foot.transform.TransformDirection(Vector3.up), out foot.ray, 1, ~(1 << 8));

            if (!foot.stepping && stepping)
            {
                if (foot.nextStep < Time.realtimeSinceStartup)
                {
                    Step(i);
                    foot.nextStep = Time.realtimeSinceStartup + 10;
                }

                foot.stepping = true;
            }
            else if (foot.stepping && !stepping)
            {
                foot.stepping = false;
            }

            feet[i] = foot;
        }
    }
}
