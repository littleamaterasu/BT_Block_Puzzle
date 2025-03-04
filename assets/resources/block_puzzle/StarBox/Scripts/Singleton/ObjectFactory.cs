using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
/// <summary>
/// This class will gen all object needed in game, like: Tile, Efx, vv
/// It also will need to pool here.
/// </summary>
public class ObjectFactory : OneSoftGame.Tools.PersistentSingleton<ObjectFactory> {

    public Block blockPrefab;
    public Tile tilePrefab;

    [Space(5)]
    public TileSpineDataGraphic cute;
    private int _assetCuteLen = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Block GenBlock(Transform parent)
    {
        return Instantiate(blockPrefab, parent, false);
    }

    /// <summary>
    /// Instantiate.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Tile GenTile(Transform parent)
    {
        return Instantiate(tilePrefab, parent, false);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public SkeletonDataAsset GetAssetInCute(int index)
    {
        if (_assetCuteLen == 0)
            _assetCuteLen = cute.assetData.Length;

        if (index < 0) index = 0;
        if (index >= _assetCuteLen) index = _assetCuteLen - 1;

        return cute.assetData[index];
    }
}

[System.Serializable]
public class TileSpineDataGraphic
{
    public SkeletonDataAsset[] assetData;
}
