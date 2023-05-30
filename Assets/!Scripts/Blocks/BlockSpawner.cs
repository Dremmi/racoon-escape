/* 
*      �������� ������ ������ �� ��������� ��� �������� ������ (�� ����� 2 ������������)
*      �������� ������� ����� ��������� _disposableTrigger.Clear();
 */

using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    private BlockSpawnConfig _blockSpawnConfig;
    private eBlockType _previousBlockType, _nextBlockType;
    private CompositeDisposable _disposable = new();

    private List<GameObject> _blocks = new();

    private Vector3 _pos;
    private Quaternion _rot;

    private BoxCollider _transitionTileCollider;

    private int _blockCount;
    private bool _isFirstBlock;

    public void Launch(BlockSpawnConfig blockSpawnConfig)
    {
        _blockSpawnConfig = blockSpawnConfig;

        _pos = blockSpawnConfig.SpawnPointFirstBlock;
        _rot = Quaternion.identity;
        _blockCount = 0;
        _isFirstBlock = true;

        CreateBlocks();
    }

    private void CreateBlocks()
    {
        var block = Instantiate(_blockSpawnConfig.Block, _pos, _rot, transform);
        block.Launch(_blockSpawnConfig);

        if (_isFirstBlock)
        {
            block.GetFirstBlockParameters();
            _isFirstBlock = false;
        }
        else
            block.GetBlockParameters(_nextBlockType);

        var tileSpawner = Instantiate(_blockSpawnConfig.TileSpawner, _pos, _rot, block.transform);
        tileSpawner.Launch(_blockSpawnConfig);

        tileSpawner.CreateTiles(block, ref _previousBlockType, ref _nextBlockType, ref _pos, _rot);
        _transitionTileCollider = tileSpawner.GetTransitionTileCollider();

        _blocks.Add(block.gameObject);
        _blockCount = _blocks.Count;

        CheckTrigger(_transitionTileCollider);
    }

    private void CheckTrigger(Collider trigger)
    {
        trigger.OnTriggerEnterAsObservable()
            .Where(t => t.gameObject.CompareTag("Player"))
            .Subscribe(other =>
            {
                CheckBlockRemoval();
                CreateBlocks();
            }).AddTo(_disposable);
    }
    private void CheckBlockRemoval()
    {
        if (_blockCount > 2)
        {
            var removableBlock = _blocks[0];
            _blocks.Remove(_blocks[0]);
            Destroy(removableBlock);
        }
    }
}