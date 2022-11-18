using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor {
    public class PlatformConverterWindow : EditorWindow {

        public static LayerMask platformLayers;

        public Color tint = Color.white;
        public Sprite horizontalPlatformSprite;
        public Sprite verticalPlatformSprite;
        public bool autoSave;

        [MenuItem("Rototo Tools/Platform Converter")]
        public static void InitWindow() => EditorWindow.GetWindow<PlatformConverterWindow>().Show(true);

        public void OnGUI() {
            tint = EditorGUILayout.ColorField("Platform Tint", tint);
            horizontalPlatformSprite = EditorGUILayout.ObjectField("Horizontal Platform Sprite", horizontalPlatformSprite, typeof(Sprite), false) as Sprite;
            verticalPlatformSprite = EditorGUILayout.ObjectField("Vertical Platform Sprite", verticalPlatformSprite, typeof(Sprite), false) as Sprite;
            autoSave = EditorGUILayout.Toggle("Auto Save", autoSave);
            
            if (!horizontalPlatformSprite || !verticalPlatformSprite) return;
            if (GUILayout.Button("Convert")) ConvertPlatforms();
        }

        public void ConvertPlatforms() {
            platformLayers = LayerMask.GetMask("Standable");
            var scene = SceneManager.GetActiveScene();
            Debug.Log("Collecting all platforms in scene " + scene.name + " ...");

            var roots = scene.GetRootGameObjects();

            foreach (var go in roots) CheckGameObject(go);
            
            Debug.Log("Done converting...");

            EditorSceneManager.MarkSceneDirty(scene);
            if (autoSave) {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                Debug.Log("Modification saved");
            }
        }

        public void CheckGameObject(GameObject go) {
            CheckAndConvert(go);
            var transform = go.transform;
            for (var i = 0; i < transform.childCount; i++) CheckGameObject(transform.GetChild(i).gameObject);
        }

        public void CheckAndConvert(GameObject go) {
            if (!go.activeInHierarchy) return;
            if (platformLayers.value != (1 << go.layer)) return;
            var name = go.name.ToLower();
            if (!name.Contains("platform")) return;
            var r = go.GetComponent<SpriteRenderer>();
            if (r == null || r.drawMode != SpriteDrawMode.Simple) return;
            var c = go.GetComponent<BoxCollider2D>();
            if (c == null) return;

            var scale = go.transform.localScale;
            scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            r.sprite = scale.x >= scale.y ? horizontalPlatformSprite : verticalPlatformSprite;
            r.color = tint;
            r.drawMode = SpriteDrawMode.Tiled;
            r.tileMode = SpriteTileMode.Continuous;
            r.size = scale;
            c.size = scale;
            go.transform.localScale = Vector3.one;
        }
    }
}