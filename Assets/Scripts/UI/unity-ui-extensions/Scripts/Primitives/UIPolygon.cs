/// Credit CiaccoDavide
/// Sourced from - http://ciaccodavi.de/unity/UIPolygon


namespace UnityEngine.UI.Extensions
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;
    [AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
    [System.Serializable]
    public class UIPolygon : UIPrimitiveBase
    {
        public List<Vector2> positions;
        public bool fill = true;
        public float thickness = 5;
        [Range(3, 360)]
        public int sides = 3;
        [Range(0, 360)]
        public float rotation = 0;
        [Range(0, 1)]
        public float[] VerticesDistances = new float[3];
        private float size = 0;

        bool finished;

        protected override void Start ()
        {
            positions = new List<Vector2>();
        }

        public void DrawPolygon(int _sides)
        {
            sides = _sides;
            VerticesDistances = new float[_sides + 1];
            for (int i = 0; i < _sides; i++) VerticesDistances[i] = 1; ;
            rotation = 0;
            SetAllDirty();
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = 0;
            SetAllDirty();
        }
        public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
        {
            sides = _sides;
            VerticesDistances = _VerticesDistances;
            rotation = _rotation;
            SetAllDirty();
        }
        void Update()
        {

            size = rectTransform.rect.width;
            if (rectTransform.rect.width > rectTransform.rect.height)
                size = rectTransform.rect.height;
            else
                size = rectTransform.rect.width;
            thickness = (float)Mathf.Clamp(thickness, 0, size / 2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;
            float degrees = 360f / sides;
            int vertices = sides + 1;


            //positions.AddRange();

          

            if (VerticesDistances.Length != vertices)
            {
                VerticesDistances = new float[vertices];
                for (int i = 0; i < vertices - 1; i++) VerticesDistances[i] = 1;
            }


            // last vertex is also the first!
            VerticesDistances[vertices - 1] = VerticesDistances[0];

            if (!finished)
            {
                for (int i = 0; i < vertices * 2; i++) positions.Add(new Vector2(0, 0));
                finished = true;
            }


            for (int i = 0; i < vertices; i++)
            {
                float outer = -rectTransform.pivot.x * size * VerticesDistances[i];
                float inner = -rectTransform.pivot.x * size * VerticesDistances[i] + thickness;
                float rad = Mathf.Deg2Rad * (i * degrees + rotation);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);
                uv0 = new Vector2(0, 1);
                uv1 = new Vector2(1, 1);
                uv2 = new Vector2(1, 0);
                uv3 = new Vector2(0, 0);
                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);
                if (fill)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                }
                prevX = pos1;
                prevY = pos2;
                //positions = new[] { pos0, pos1, pos2, pos3 };
                //positions.Add(pos0)s
                vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));

                //positions[i+1] = pos1;
                //positions[i+2] = pos2;
                //positions[i+3] = pos3;

                if (positions.Count < 4)
                    continue ;
                if(pos0!= new Vector2(0,0)) positions[i] = pos0;
                if(pos1!= new Vector2(0,0)) positions[i+1] = pos1;
                if(pos2!= new Vector2(0,0)) positions[i+2] = pos2;
                if(pos3!= new Vector2(0,0)) positions[i+3] = pos3;

                //Debug.Log(positions);
            }

           

        }
    }
}