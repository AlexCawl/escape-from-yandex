using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(AbstractMapGenerator), true)]
    public class RandomMapGeneratorEditor : UnityEditor.Editor
    {
        private AbstractMapGenerator _generator;

        private void OnEnable()
        {
            _generator = (AbstractMapGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Dungeon"))
            {
                _generator.GenerateMap();
            }
        }
    }
}