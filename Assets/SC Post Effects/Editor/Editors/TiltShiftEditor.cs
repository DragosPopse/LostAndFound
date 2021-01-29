using UnityEditor;
using UnityEngine;
#if PPS
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;
#endif

namespace SCPE
{
#if !PPS
    public sealed class TiltShiftEditor : Editor {} }
#else
    [PostProcessEditor(typeof(TiltShift))]
    public sealed class TiltShiftEditor : PostProcessEffectEditor<TiltShift>
    {

        SerializedParameterOverride mode;
        SerializedParameterOverride quality;
        SerializedParameterOverride areaSize;
        SerializedParameterOverride areaFalloff;
        SerializedParameterOverride amount;

        public override void OnEnable()
        {
            mode = FindParameterOverride(x => x.mode);
            quality = FindParameterOverride(x => x.quality);
            areaSize = FindParameterOverride(x => x.areaSize);
            areaFalloff = FindParameterOverride(x => x.areaFalloff);
            amount = FindParameterOverride(x => x.amount);
        }

        public override string GetDisplayTitle()
        {
            return base.GetDisplayTitle() + SCPE_GUI.ModeTitle(mode);
        }

        public override void OnInspectorGUI()
        {
            SCPE_GUI.DisplayDocumentationButton("tilt-shift");

            SCPE_GUI.DisplaySetupWarning<TiltShiftRenderer>();

            using (new EditorGUILayout.HorizontalScope())
            {
                var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
                overrideRect.yMin += 4f;
                EditorUtilities.DrawOverrideCheckbox(overrideRect, mode.overrideState);
                using (new EditorGUI.DisabledGroupScope(mode.overrideState.boolValue == false))
                {
                    EditorGUILayout.PrefixLabel(mode.displayName);
                    mode.value.intValue = GUILayout.Toolbar(mode.value.intValue, mode.value.enumDisplayNames, GUILayout.Height(17f));
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                var overrideRect = GUILayoutUtility.GetRect(17f, 17f, GUILayout.ExpandWidth(false));
                overrideRect.yMin += 4f;
                EditorUtilities.DrawOverrideCheckbox(overrideRect, quality.overrideState);
                using (new EditorGUI.DisabledGroupScope(quality.overrideState.boolValue == false))
                {
                    EditorGUILayout.PrefixLabel(quality.displayName);
                    quality.value.intValue = GUILayout.Toolbar(quality.value.intValue, quality.value.enumDisplayNames, GUILayout.Height(17f));
                }
            }
            EditorGUILayout.LabelField("Screen area", EditorStyles.boldLabel);
            PropertyField(areaSize, new GUIContent("Size"));
            PropertyField(areaFalloff, new GUIContent("Falloff"));

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(" ");
                TiltShift.debug = EditorGUILayout.ToggleLeft(" Visualize area", TiltShift.debug);
            }

            PropertyField(amount, new GUIContent("Blur amount"));
        }
    }
}
#endif