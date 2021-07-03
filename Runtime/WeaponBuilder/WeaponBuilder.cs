using System.Collections.Generic;
using UnityEngine;

namespace Planet.Weapons
{
    //All attachment types
    public enum WeaponAttachment
    {
        NONE,
        BARREL,
        RECEIVER,
        GRIP,
        HANDGUARD,
        STOCK
    }

    public class WeaponBuilder : MonoBehaviour
    {
        [SerializeField] public WeaponMaster weapon = new WeaponMaster();
        public bool allowClearDuringPlay = false;

        GameObject weaponObject;
        List<GameObject> weaponPiecesIstantiated = new List<GameObject>();

        public void DrawWeapon()
        {
            DeleteWeapon(Application.isEditor);

            //weaponPiecesIstantiated = new List<GameObject>();

            if (weapon.masterPiece is null)
            {
                Debug.LogError("Master piece was not set!");
                return;
            }

            weaponObject = Instantiate(weapon.masterPiece, this.transform.position, this.transform.rotation, this.transform);
            WeaponPieceData component = weaponObject.AddComponent<WeaponPieceData>();
            component.pieceType = weapon.pieceType;

            weaponPiecesIstantiated.Add(weaponObject);

            foreach (WeaponPiece piece in weapon.weaponParts)
            {
                RecurseBuild(piece, weaponObject);
            }
        }

        public void DeleteWeapon(bool isEditor = false)
        {
            Debug.Log("Destroying weapon, isEditor? " + isEditor);
            if (isEditor)
            {
                foreach (GameObject obj in weaponPiecesIstantiated)
                {
                    DestroyImmediate(obj);
                }
                weaponPiecesIstantiated.Clear();
                return;
            }


            foreach (GameObject obj in weaponPiecesIstantiated)
            {
                Destroy(obj);
            }
            weaponPiecesIstantiated.Clear();
        }

        public void ClearPiecesList()
        {
            if (!allowClearDuringPlay && Application.isEditor)
            {
                DeleteWeapon(Application.isEditor);
                weapon.weaponParts.Clear();
                weapon.masterPiece = null;
            }
        }

        //Recursevly build the weapon, each children can have another children
        private void RecurseBuild(WeaponPiece piece, GameObject weaponObject)
        {
            if (piece.pieceType == WeaponAttachment.NONE)
            {
                Debug.LogError("Unknown attachment type");
                return;
            }

            foreach (GameObject obj in weaponPiecesIstantiated)
            {
                if (piece.pieceType == obj.GetComponent<WeaponPieceData>().pieceType)
                {
                    Debug.LogError("Type already in use for this weapon: " + piece.pieceType);
                    return;
                }
            }


            Transform bone = weaponObject.transform.Find(piece.boneName);
            if (bone is null)
            {
                bone = weaponObject.transform;
            }

            GameObject pieceObject = Instantiate(piece.pieceMesh, bone.position, bone.rotation, bone);
            WeaponPieceData component = pieceObject.AddComponent<WeaponPieceData>();
            component.pieceType = piece.pieceType;

            weaponPiecesIstantiated.Add(pieceObject);
            if (!piece.hasChild)
            {
                return;
            }

            foreach (WeaponPiece child in piece.children)
            {
                RecurseBuild(child, pieceObject);
            }
        }
    }

    [System.Serializable]
    public class WeaponPiece
    {
        [TooltipAttribute("Name of this piece or attachment, just for designers")]
        public string pieceName;

        [TooltipAttribute("Name of this piece or attachment, used to find parts from code")]
        public WeaponAttachment pieceType;

        [TooltipAttribute("Bone this piece attaches to")]
        public string boneName;

        [TooltipAttribute("The piece model to spawn")]
        public GameObject pieceMesh;

        [TooltipAttribute("If this piece has another child, for example an hand grip followed by a barrel")]
        public bool hasChild = false;

        [TooltipAttribute("The children pieces")]
        [SerializeField] public List<WeaponPiece> children;
    }

    [System.Serializable]
    public class WeaponMaster
    {
        [TooltipAttribute("Name of this piece or attachment, just for designers")]
        public string pieceName;

        [TooltipAttribute("Name of this piece or attachment, used to find parts from code")]
        public WeaponAttachment pieceType;

        //[Header("Weapon Parts")]
        [SerializeField] public List<WeaponPiece> weaponParts;
        public GameObject masterPiece;
    }


}


