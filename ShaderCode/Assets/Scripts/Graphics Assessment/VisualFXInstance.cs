using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualFXSystem
{
    public class VisualFXInstance : MonoBehaviour
    {
        float countdown;
        public bool countingDown;

        public void Init(VisualFX fx, bool autoStop)
        {
            countingDown = autoStop;
            countdown = fx.duration;
        }

        public void Update()
        {
            if (!countingDown) return;

            countdown -= Time.deltaTime;

            if (countdown < 0)
            {
                Destroy(gameObject);
            }
        }

        public bool isFinished() { return countdown <= 0; }
    }
}