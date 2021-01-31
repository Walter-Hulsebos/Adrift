#if URP
using UnityEngine.Rendering.Universal;
#endif

using UnityEditor.Rendering;
using UnityEditor;

namespace SCPE
{
#if URP
    [VolumeComponentEditor(typeof(CloudShadows))]
    sealed class CloudShadowsEditor : VolumeComponentEditor
    {
        SerializedDataParameter texture;
        SerializedDataParameter size;
        SerializedDataParameter density;
        SerializedDataParameter speed;
        SerializedDataParameter direction;

        private bool isSetup;

        public override void OnEnable()
        {
            base.OnEnable();

            var o = new PropertyFetcher<CloudShadows>(serializedObject);
            isSetup = AutoSetup.ValidEffectSetup<CloudShadowsRenderer>();

            texture = Unpack(o.Find(x => x.texture));
            size = Unpack(o.Find(x => x.size));
            density = Unpack(o.Find(x => x.density));
            speed = Unpack(o.Find(x => x.speed));
            direction = Unpack(o.Find(x => x.direction));
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("cloud-shadows");

            SCPE_GUI.DisplayVRWarning();

            SCPE_GUI.DisplaySetupWarning<CloudShadowsRenderer>(ref isSetup);

            if (CloudShadows.isOrtho) EditorGUILayout.HelpBox("Not available for orthographic cameras", MessageType.Warning);

            PropertyField(texture);
            PropertyField(size);
            PropertyField(density);
            PropertyField(speed);
            PropertyField(direction);
        }
    }
}
#endif