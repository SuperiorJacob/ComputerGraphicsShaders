using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JacobExperimental.PixelUI.V2
{

    [RequireComponent(typeof(Pixels))]
    public class PixelParticles : MonoBehaviour
    {
        private Pixels pixel;
        private float nextSpawn = 0.0f;

        public bool runParticles = false;
        public float particleSpawnSpeed = 0.1f;
        public Canvas myCanvas;

        void Start()
        {
            pixel = GetComponent<Pixels>();
        }

        void CreateParticle()
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out mousePos);

            Pixel newPixel = new Pixel();
            newPixel.color = Color.white;
            newPixel.position = Vector2.zero;
            newPixel.size = new Vector2(100, 100);
            newPixel.constantVelocity = new Vector2(-10, -18);
            newPixel.endLife = Time.realtimeSinceStartup + 1000f;

            pixel.AddParticle(newPixel);
        }

        void Update()
        {
            if (runParticles && Time.realtimeSinceStartup > nextSpawn)
            {
                nextSpawn = Time.realtimeSinceStartup + particleSpawnSpeed;

                CreateParticle();
            }

            for (int i = 0; i < pixel.pixelRenderBuffer.Count; i++)
            {
                Pixel p = pixel.pixelRenderBuffer[i];
                if (p.endLife < Time.realtimeSinceStartup)
                {
                    pixel.pixelRenderBuffer.RemoveAt(i);
                }
                else
                {
                    p.position += p.constantVelocity * Time.deltaTime;
                    pixel.pixelRenderBuffer[i] = p;
                }
            }
        }
    }
}