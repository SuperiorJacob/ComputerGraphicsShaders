using Unity.Burst;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PixelUI
{
    [System.Serializable]
    public struct Pixel
    {
        // We hide these because they will get CREATED later on.
        [HideInInspector] public GameObject gameObject;
        [HideInInspector] public RectTransform transform;
        [HideInInspector] public Image render;

        // Setting up our inspector edits.
        [SerializeField] private Vector2 size;
        [SerializeField] private Vector2 position;
        [SerializeField] private Color color;

        // We don't want anything below in the inspector. Ill make an editor for it later, but for now it's just because it's easier.
        public Vector2 Size
        {
            get { return size; }
            set { size = value; if (transform != null) transform.sizeDelta = size; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; if (transform != null) transform.localPosition = position; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; if (render != null) render.color = value; }
        }

        // Remove this later
        public float endLife;
    }

    public class PixelObj : MonoBehaviour
    {
        public Transform pixels;
        public GameObject pixelPrefab;

        public Vector2 gravity;
        public Vector2 sizeMult;

        public float particleSpawnSpeed = 0.1f;
        public float deathTime = 5f;
        public Canvas myCanvas; // TEst

        public bool runParticles = false;

        [SerializeField]
        public List<Pixel> pixelList;

        IEnumerator CreateParticles()
        {
            while(true)
            {
                if (runParticles)
                    CreateParticle();
                
                yield return new WaitForSeconds(particleSpawnSpeed);
            }
        }

        void CreateParticle()
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, Input.mousePosition, myCanvas.worldCamera, out mousePos);

            Pixel newPixel = new Pixel();
            newPixel.Color = Random.ColorHSV();
            newPixel.Position = mousePos;
            newPixel.Size = sizeMult;
            newPixel.endLife = Time.realtimeSinceStartup + deathTime;
            pixelList.Add(newPixel);

            UpdatePixels();
        }

        void Awake()
        {
            StartCoroutine("CreateParticles");
        }

        void Start()
        {
            //for (int i = 0; i < 100; i++)
            //{
            //    Pixel newPixel = new Pixel();
            //    newPixel.Color = Random.ColorHSV();
            //    newPixel.Position = new Vector2(0, 0);
            //    newPixel.Size = new Vector2(Random.Range(5, sizeMult.x), Random.Range(5, sizeMult.y));

            //    pixelList.Add(newPixel);
            //}

            //UpdatePixels();
        }

        void Update()
        {
            for (int i = 0; i < pixelList.Count; i++)
            {
                Pixel pixel = pixelList[i];
                if (pixel.endLife < Time.realtimeSinceStartup)
                {
                    Destroy(pixel.gameObject);
                    pixelList.RemoveAt(i);
                }
                else
                {
                    pixel.Position += gravity * Time.deltaTime;
                    pixelList[i] = pixel;
                }
            }
        }

        void UpdatePixels()
        {
            for (int i = 0; i < pixelList.Count; i++)
            {
                Pixel pixel = pixelList[i];

                if (pixel.gameObject == null)
                {
                    pixel.gameObject = Instantiate(pixelPrefab); // Should be the canvas.

                    pixel.transform = pixel.gameObject.GetComponent<RectTransform>();
                    pixel.render = pixel.gameObject.GetComponent<Image>();

                    pixel.gameObject.transform.SetParent(pixels, false);
                }

                pixel.transform.sizeDelta = pixel.Size;
                pixel.transform.localPosition = pixel.Position;
                pixel.render.color = pixel.Color;

                pixelList[i] = pixel;
            }
        }
    }
}