namespace Kian.Util {
    using System;
    using static Kian.Mod.ShortCuts;
    using ICities;
    using UnityEngine;
    using System.Reflection;
    using ColossalFramework.Globalization;
    using ColossalFramework;

    public class GuideWrapper {
        private GenericGuide m_guide;
        public GuideInfo m_info;

        public GuideWrapper(string key) {
            m_guide = new GenericGuide();
            m_info = new GuideInfo {
                m_delayType = GuideInfo.Delay.OccurrenceCount, // game default: OccurrenceCount
                m_displayDelay = 1, // game default: 1
                m_repeatDelay = 3, // game default: 3
                m_overrideOptions = true,
                m_icon = "ToolbarIconZoomOutGlobe",
                m_tag = "Generic", 
                m_name = key,
            };
        }

        public void Activate() {
            Log("KIAN DEBUG: GuideWrapper.Activate() called ... V4");
            m_guide?.Activate(m_info ?? throw new Exception("m_info is null"));
        }
        public void Deactivate() => m_guide?.Deactivate();

        public static GuideWrapper example = new GuideWrapper("WaterNeeded");
        public static GuideWrapper example2 = new GuideWrapper("guide_example");


        public static void AddStringsTitle(string key, string value) => AddString("TUTORIAL_TITLE", key, value);
        public static void AddStringsBody(string key, string value) => AddString("TUTORIAL_TEXT", key, value);
        private static void AddString(string id, string key, string value) {
            Locale.Key localeKey = new Locale.Key() {
                m_Identifier = id,
                m_Key = key,
            };

            resetFun?.Invoke(locale, new object[] { localeKey });
            locale.AddLocalizedString(localeKey, value);
            // see Translation.ReloadTutorialTranslations
        }

        static MethodInfo resetFun => typeof(Locale).GetMethod(
            "ResetOverriddenLocalizedStrings",
            BindingFlags.Instance | BindingFlags.NonPublic);
        static Locale locale => (Locale)typeof(LocaleManager).GetField(
                "m_Locale",
                BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(SingletonLite<LocaleManager>.instance);

        public class LoadingExtension : LoadingExtensionBase {
            public override void OnLevelLoaded(LoadMode mode) {
                AddStringsTitle("guide_example", "title guide example");
                AddStringsBody("guide_example", "body of my guide example");
            }
        }
    }
}
