
using UnityEngine;
using UnityEditor;

namespace Planet.Weapons
{
    [CustomEditor(typeof(WeaponBuilder))]
    [InitializeOnLoadAttribute]
    public class BuildWeapon : Editor
    {
        BuildWeapon()
        {
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Build Weapon"))
            {
                BuildWeaponObject();
            }

            if (GUILayout.Button("Destroy Weapon"))
            {
                DestroyWeapon();
            }

            if (GUILayout.Button("Clear Pieces"))
            {
                ClearWeaponPieces();
            }
        }

        private void DestroyWeapon()
        {
            WeaponBuilder myScript = (WeaponBuilder)target;
            myScript.DeleteWeapon(true);
        }

        private void BuildWeaponObject()
        {
            WeaponBuilder myScript = (WeaponBuilder)target;
            myScript.DrawWeapon();
        }

        private void ClearWeaponPieces()
        {
            WeaponBuilder myScript = (WeaponBuilder)target;
            myScript.ClearPiecesList();
        }
    }
}