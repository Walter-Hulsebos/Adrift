using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "ProtoTexture Collection")]
public class ProtoTexture : ScriptableObject
{
    static Shader _defaultShader;
    public static Shader DefaultShader
    {
        get
        {
            if ((_defaultShader == null) || (_defaultShader.isSupported))
            {
                _defaultShader = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null ? UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.defaultShader : Shader.Find("Standard");
            }

            return _defaultShader;
        }
    }

    [System.Serializable]
    public class TextureDefinition
    {
        public StringProtoProperty name = new StringProtoProperty("ProtoTexture");
        public Vector2Int size;
        public StringProtoProperty subTitle = new StringProtoProperty("1m x 1m");

        [Header("Character")]
        public Font font;
        public int fontSize = 0;
        public Color textColor = Color.white;

        [Header("Background")]
        public Color backgroundColor = new Color(1f, 0.5f, 0.2f, 1f);
        public ColorProtoProperty checkerboardColor = new ColorProtoProperty(new Color(0f, 0f, 0f, 0.3f));
        public ColorProtoProperty gridColor = new ColorProtoProperty(new Color(1f, 1f, 1f, 0.3f));
        public ColorProtoProperty outlineColor = new ColorProtoProperty(Color.white);

        [SerializeField]
        Texture2D _texture = null;
        public Texture2D Texture { get { return _texture; } }

        [SerializeField]
        Material _material = null;
        public Material Material { get { return _material; } }

        readonly float subTitleScale = 1.5f;

#if UNITY_EDITOR
        static Material _protoMaterial;
        public static Material ProtoMaterial
        {
            get
            {
                if (!_protoMaterial)
                {
                    _protoMaterial = new Material(Shader.Find("Hidden/ProtoTexture-Colored")) { hideFlags = HideFlags.HideAndDontSave };

                    _protoMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _protoMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _protoMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                    _protoMaterial.SetInt("_ZWrite", 0);
                }

                return _protoMaterial;
            }
        }

        static Material _protoTextMaterial;
        public static Material ProtoTextMaterial
        {
            get
            {
                if (!_protoTextMaterial)
                    _protoTextMaterial = new Material(Shader.Find("Hidden/ProtoTextureFont")) { hideFlags = HideFlags.HideAndDontSave };

                return _protoTextMaterial;
            }
        }

        public void BuildIfEmpty(ProtoTexture protoTexture)
        {
            if (_texture == null)
                Build(protoTexture);
        }

        public void Build(ProtoTexture protoTexture)
        {
            if (font == null)
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            string path = AssetDatabase.GetAssetPath(protoTexture);

            bool reload = false;

            if (_texture == null)
            {
                _texture = new Texture2D(size.x, size.y, TextureFormat.ARGB32, true);
                AssetDatabase.AddObjectToAsset(_texture, path);
                AssetDatabase.SetMainObject(protoTexture, path);
                reload = true;
            }

            if ((_texture != null) && ((_texture.width != size.x) || (_texture.height != size.y)))
                _texture.Resize(size.x, size.y, TextureFormat.ARGB32, true);

            RenderTexture rt = RenderTexture.GetTemporary(size.x, size.y, 32, RenderTextureFormat.ARGB32);
            rt.Create();

            RenderTexture.active = rt;

            // initialize matrix
            GL.PushMatrix();
            GL.LoadPixelMatrix(0f, size.x, 0f, size.y);

            // draw background
            GL.Clear(true, true, backgroundColor);
            ProtoMaterial.SetPass(0);

            GL.Begin(GL.QUADS);

            // draw checkerboard
            if (checkerboardColor.Valid)
                DrawCheckerboard(checkerboardColor);

            // draw grid
            if (gridColor.Valid)
                DrawGrid(gridColor);

            // draw grid
            if (outlineColor.Valid)
                DrawOutline(2f, outlineColor);

            GL.End();

            float subTitleOffset = 0f;
            Vector3 min = Vector3.zero, max = Vector3.zero;
            bool hasText = false;
            float underlineWidth = 2f;
            float topLine = 2f;

            // setup title
            if (name.Valid)
            {
                var titleGen = GetGenerationSettings(size);
                titleGen.font = font;

                // draw text
                ProtoTextMaterial.mainTexture = font.material.mainTexture;
                ProtoTextMaterial.SetPass(0);

                TextGenerator gen = new TextGenerator();
                gen.Populate(name, titleGen);

                var verts = gen.verts;

                GL.Begin(GL.QUADS);
                GL.Color(textColor);

                underlineWidth = gen.lines[gen.lineCount - 1].height;
                topLine = gen.lines[gen.lineCount - 1].topY;
                subTitleOffset = topLine - (underlineWidth * 1.4f);

                Vector2 offset = new Vector2(Mathf.Round(underlineWidth * 0.46f), -Mathf.Round(underlineWidth * 0.3f));
                Vector2 down = new Vector2(offset.x, offset.y - Mathf.Ceil(underlineWidth * 0.3f));

                min = (Vector2)verts[0].position + down;
                max = (Vector2)verts[0].position + down;
                hasText = true;

                for (int i = 0; i < verts.Count; ++i)
                {
                    GL.TexCoord(verts[i].uv0);
                    GL.Vertex((Vector2)verts[i].position + offset);

                    min = Vector3.Min(min, (Vector2)verts[i].position + down);
                    max = Vector3.Max(max, (Vector2)verts[i].position + down);
                }

                GL.End();
            }

            // setup sub-title
            if (subTitle.Valid)
            {
                var titleGen = GetGenerationSettings(size, subTitleScale);
                titleGen.font = font;

                TextGenerator gen = new TextGenerator();
                gen.Populate(subTitle, titleGen);
                var verts = gen.verts;

                // draw text
                ProtoTextMaterial.mainTexture = font.material.mainTexture;
                ProtoTextMaterial.SetPass(0);

                GL.Begin(GL.QUADS);
                GL.Color(textColor);

                var lineHeight = gen.lines[gen.lineCount - 1].height;
                Vector2 offset = new Vector2(Mathf.Round(lineHeight * 0.46f), -Mathf.Round(lineHeight * 0.3f));

                if (name.Valid)
                {
                    offset.x = Mathf.Round(underlineWidth * 0.46f);
                    offset.y = (topLine - underlineWidth * 1.6f) - size.y;
                }

                for (int i = 0; i < verts.Count; ++i)
                {
                    GL.TexCoord(verts[i].uv0);
                    GL.Vertex((Vector2)verts[i].position + offset);

                    min = Vector3.Min(min, (Vector2)verts[i].position + offset);
                    max = Vector3.Max(max, (Vector2)verts[i].position + offset);
                }

                GL.End();
            }

            if (hasText && subTitle.Valid)
            {
                // draw underline
                ProtoMaterial.SetPass(0);
                GL.Begin(GL.QUADS);
                Rect extents = Rect.MinMaxRect(min.x, subTitleOffset, max.x, subTitleOffset);
                DrawLine(Rect.NormalizedToPoint(extents, Vector2.zero), Rect.NormalizedToPoint(extents, Vector2.right), Mathf.Max(2f, underlineWidth / 16f));
                GL.End();
            }

            GL.PopMatrix();

            _texture.ReadPixels(new Rect(0f, 0f, size.x, size.y), 0, 0, true);
            _texture.Apply();

            RenderTexture.ReleaseTemporary(rt);

            if (_texture.name != name)
            {
                _texture.name = name;
                reload = true;
            }

            if (ValidateMaterial(protoTexture) || reload)
                AssetDatabase.ImportAsset(path); // asset has changed
        }

        public bool ValidateMaterial(ProtoTexture protoTexture)
        {
            bool changed = false;

            if (_material == null)
            {
                string path = AssetDatabase.GetAssetPath(protoTexture);

                _material = new Material(DefaultShader);
                AssetDatabase.AddObjectToAsset(_material, path);
                AssetDatabase.SetMainObject(protoTexture, path);
                changed = true;
            }

            if (_material.mainTexture != _texture)
                _material.mainTexture = _texture;

            if (_material.name != name)
            {
                _material.name = name;
                changed = true;
            }

            return changed;
        }

        void DrawCheckerboard(Color color)
        {
            for (var gridStep = 16; gridStep < Mathf.Min(size.x, size.y); gridStep *= 8)
            {
                var oddX = false;
                var oddY = false;

                color.a *= 0.5f;

                for (var x = 0; x < size.x; x += gridStep)
                {
                    oddY = oddX;

                    for (var y = 0; y < size.y; y += gridStep)
                    {
                        if (oddY)
                            DrawRect(new Rect(x, y, gridStep, gridStep), color);

                        oddY = !oddY;
                    }

                    oddX = !oddX;
                }
            }
        }

        void DrawGrid(Color color)
        {
            // draw grid
            GL.Color(color);
            var width = 1f;

            for (var gridStep = 16; gridStep < Mathf.Min(size.x, size.y); gridStep *= 8)
            {
                for (var x = 0.5f; x < size.x; x += gridStep)
                    DrawLine(new Vector2(x, 0), new Vector2(x, size.y), width, color);

                for (var y = 0.5f; y < size.y; y += gridStep)
                    DrawLine(new Vector2(0, y), new Vector2(size.x, y), width, color);
            }
        }

        void DrawOutline(float width, Color color)
        {
            DrawRect(new Rect(0f, 0f, size.x, size.y), width, color);
        }

        void DrawRect(Rect rect, Color color)
        {
            var min = rect.min;
            var max = rect.max;

            GL.Color(color);

            GL.Vertex3(min.x, min.y, 0f);
            GL.Vertex3(max.x, min.y, 0f);
            GL.Vertex3(max.x, max.y, 0f);
            GL.Vertex3(min.x, max.y, 0f);
        }

        void DrawRect(Rect rect, float width, Color color)
        {
            var min = rect.min;
            var max = rect.max;

            GL.Color(color);

            var p1 = new Vector3(min.x, min.y, 0f);
            var p2 = new Vector3(max.x, min.y, 0f);
            var p3 = new Vector3(max.x, max.y, 0f);
            var p4 = new Vector3(min.x, max.y, 0f);

            DrawLine(p1, p2, width);
            DrawLine(p2, p3, width);
            DrawLine(p3, p4, width);
            DrawLine(p4, p1, width);
        }

        void DrawLine(Vector2 start, Vector2 end, float width, Color color)
        {
            GL.Color(color);
            DrawLine(start, end, width);
        }

        void DrawLine(Vector2 start, Vector2 end, float width)
        {
            var vec = (end - start).normalized * width * 0.5f;
            var temp = vec.x;
            vec.x = -vec.y;
            vec.y = temp;

            GL.Vertex(start - vec);
            GL.Vertex(start + vec);

            GL.Vertex(end + vec);
            GL.Vertex(end - vec);
        }

        public TextGenerationSettings GetGenerationSettings(Vector2 extents, float scale = 1f)
        {
            TextGenerationSettings result = default(TextGenerationSettings);
            result.generationExtents = extents;

            int size = Mathf.CeilToInt((fontSize > 0 ? fontSize : font.fontSize) * scale);

            if ((font != null) && font.dynamic)
            {
                result.fontSize = size;
                result.resizeTextMinSize = size;
                result.resizeTextMaxSize = size;
            }

            result.textAnchor = TextAnchor.UpperLeft;
            result.alignByGeometry = false;
            result.scaleFactor = 1f;
            result.color = textColor;
            result.font = font;
            result.pivot = Vector2.zero;
            result.richText = true;
            result.lineSpacing = 0;
            result.fontStyle = FontStyle.Normal;
            result.resizeTextForBestFit = false;
            result.updateBounds = false;
            result.horizontalOverflow = HorizontalWrapMode.Overflow;
            result.verticalOverflow = VerticalWrapMode.Overflow;

            return result;
        }

        public void Clear()
        {
            DestroyImmediate(Texture, true);
            DestroyImmediate(Material, true);
        }
#endif
    }

    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("_definitions")]
    List<TextureDefinition> _textureDefinitions;

    public TextureDefinition GetTextureDefinition(int index)
    {
        return _textureDefinitions[index];
    }

#if UNITY_EDITOR
    public void RemoveTexture(int index)
    {
        _textureDefinitions[index].Clear();
        _textureDefinitions.RemoveAt(index);

        AssetDatabase.Refresh(); // removed a texture definition
    }

    public TextureDefinition AddTexture()
    {
        var def = new TextureDefinition()
        {
            name = new StringProtoProperty("ProtoTexture") { enabled = true },
            subTitle = new StringProtoProperty("1m x 1m") { enabled = true },
            size = new Vector2Int(512, 512)
        };

        if (_textureDefinitions == null)
            _textureDefinitions = new List<TextureDefinition>();

        _textureDefinitions.Add(def);
        def.Build(this);

        return def;
    }

    [ContextMenu("Rebuild All")]
    void BuildAll()
    {
        _textureDefinitions.ForEach(d => d.Build(this));
    }

    public void Build(int index)
    {
        _textureDefinitions[index].Build(this);
    }

    public bool HasTexture(Texture2D texture)
    {
        return _textureDefinitions.Find(t => t.Texture == texture) != null;
    }

    void OnValidate()
    {
        _textureDefinitions.ForEach(d => d.BuildIfEmpty(this));

        bool reload = false;
        for (int i = 0; i < _textureDefinitions.Count; ++i)
            reload |= _textureDefinitions[i].ValidateMaterial(this);

        if (reload)
            AssetDatabase.Refresh();
    }
#endif
}

public abstract class ProtoProperty { }

public abstract class ProtoProperty<TVariableType, TProperty> : ProtoProperty where TProperty : ProtoProperty<TVariableType, TProperty>
{
    public bool enabled = true;

    protected ProtoProperty(TVariableType value)
    {
        Value = value;
        enabled = true;
    }

    public bool Valid { get { return enabled && TestValueValidity(); } }
    public abstract TVariableType Value { get; set; }

    public abstract bool TestValueValidity();

    public static implicit operator TVariableType(ProtoProperty<TVariableType, TProperty> variable)
    {
        return variable.Value;
    }
}

[System.Serializable]
public class StringProtoProperty : ProtoProperty<string, StringProtoProperty>
{
    [SerializeField, Delayed]
    [UnityEngine.Serialization.FormerlySerializedAs("value")]
    string _value;

    public StringProtoProperty(string value) : base(value) { }

    public override string Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public override bool TestValueValidity()
    {
        return !string.IsNullOrEmpty(Value);
    }
}

[System.Serializable]
public class ColorProtoProperty : ProtoProperty<Color, ColorProtoProperty>
{
    [SerializeField]
    [UnityEngine.Serialization.FormerlySerializedAs("value")]
    Color _value;

    public ColorProtoProperty(Color value) : base(value) { }

    public override Color Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public override bool TestValueValidity()
    {
        return Value.a > 0f;
    }
}
