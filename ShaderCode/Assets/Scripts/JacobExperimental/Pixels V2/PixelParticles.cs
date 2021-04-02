using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JacobExperimental.PixelUI.V2
{
    [System.Serializable]
    public struct EditorVector2ToVector2
    {
        public Vector2 start;
        public Vector2 end;
        public float speed;
    }

    [RequireComponent(typeof(Pixels))]
    public class PixelParticles : MonoBehaviour
    {
        private Pixels pixel;
        private float nextSpawn = 0.0f;

        public Vector2 particleConstantVelocity = Vector2.zero;
        public Vector2 particleSize = Vector2.one;
        public Color particleColor = Color.white;

        [Space()]

        public EditorVector2ToVector2 sizeOverTime;

        [Space()]

        public bool cullKillsParticles = false;
        public float particleEndLife = 100f;
        public float particleSpawnSpeed = 0.1f;

        [Space()]

        public bool runParticles = false;

        [Space()]

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
            newPixel.color = particleColor;
            newPixel.position = mousePos;
            newPixel.size = particleSize;
            newPixel.constantVelocity = particleConstantVelocity;
            newPixel.endLife = Time.realtimeSinceStartup + particleEndLife;
            newPixel.cullKill = cullKillsParticles;

            pixel.AddParticle(newPixel);
        }

        void Update()
        {
            if (runParticles && Time.realtimeSinceStartup > nextSpawn && Input.GetMouseButton(0))
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

                    if (i <= 0) pixel.SetVerticesDirty();
                }
                else
                {
                    if (p.shouldCull && p.cullKill)
                    {
                        pixel.pixelRenderBuffer.RemoveAt(i);
                        continue;
                    }

                    p.position += p.constantVelocity * Time.deltaTime;

                    pixel.SetVerticesDirty();

                    pixel.pixelRenderBuffer[i] = pixel.CheckCulling(p);
                }
            }
        }
    }
}