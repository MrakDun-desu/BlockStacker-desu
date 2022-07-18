using System;
using System.Linq;
using Blockstacker.GlobalSettings.BlockSkins;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace Blockstacker.Gameplay.Pieces
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onCleared;
        [SerializeField] private Vector2 _initialPosition;
        [SerializeField] private uint _blockNumber;
        [SerializeField] private GameObject _blockSkinPrefab;
        [SerializeField] private GameObject _skinsParent;

        private string _pieceType;

        public event Action<Block> Cleared;

        public void Reset()
        {
            transform.localPosition = new Vector3(
                _initialPosition.x,
                _initialPosition.y,
                transform.localPosition.z
            );
        }

        private void OnValidate()
        {
            Reset();
        }

        private void Start()
        {
            _pieceType = GetComponentInParent<Piece>().PieceType;
            UpdateBlockSkin();
            SkinLoader.SkinChanged += UpdateBlockSkin;
        }

        private void UpdateBlockSkin()
        {
            var skinRecords = SkinLoader.SkinRecords;
            var blockSkins = 
                skinRecords.Where(record => record.PieceType == _pieceType && record.BlockNumbers.Contains(_blockNumber)).ToArray();

            if (blockSkins.Length == 0)
                return;

            foreach (Transform blockSkin in _skinsParent.transform)
            {
                Destroy(blockSkin.gameObject);
            }
            foreach (var skinRecord in blockSkins)
            {
                var newSkin = Instantiate(_blockSkinPrefab.gameObject, _skinsParent.transform).GetComponent<BlockSkin>();
                newSkin.SkinRecord = skinRecord;
            }
        }

        public void Clear()
        {
            _onCleared.Invoke();
            Cleared?.Invoke(this);
        }
    }

}