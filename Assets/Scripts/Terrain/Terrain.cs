
using System;
using UnityEngine;


namespace FrontierIsland
{
    public class Terrain : MonoBehaviour
    {
        public Chunk chunkPrefab;

        private Chunk[] chunks;

        public int WidthChunks { get { return 3; } }
        public int LengthChunks { get { return 3; } }

        public int WidthTiles { get { return WidthChunks * Chunk.CHUNK_SIZE; } }
        public int LengthTiles { get { return LengthChunks * Chunk.CHUNK_SIZE; } }

        public static Terrain Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("More than one instance of terrain exists");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            chunks = new Chunk[WidthChunks * LengthChunks];
        }


        void Start()
        {
            // FOR TESTING
            for (int x = 0; x < WidthTiles; ++x)
            {
                for (int z = 0; z < LengthTiles; ++z)
                {
                    SetTile(TileType.Grass, x, z);
                }
            }


            for (int i = 0; i < (int)(LengthTiles * 0.8f) * 5; ++i)
            {
                PlaceBlock((BlockType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BlockType)).Length), 
                    new Vector3Int(UnityEngine.Random.Range(0, LengthTiles-1), 0, UnityEngine.Random.Range(0, WidthTiles-1)));
            }

            for (int i = 0; i < Chunk.CHUNK_SIZE; ++i)
            {
                PlaceBlock(BlockType.Crate,
                    new Vector3Int(16, 2, i));
            }
            foreach (Chunk c in chunks)
            c.UpdateChunkMesh();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(Vector3.zero, new Vector3(WidthTiles, 0, 0));
            Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, LengthTiles));
            Gizmos.DrawLine(new Vector3(0, 0, LengthTiles), new Vector3(WidthTiles, 0, LengthTiles));
            Gizmos.DrawLine(new Vector3(WidthTiles, 0, 0), new Vector3(WidthTiles, 0, LengthTiles));
        }

        #region utility
        private Chunk GetChunk(int x, int z)
        {
            x /= Chunk.CHUNK_SIZE;
            z /= Chunk.CHUNK_SIZE;
            return GetChunkByIndex(x, z);
        }

        private Chunk GetChunkByIndex(int chunkX, int chunkZ)
        {
            return chunks[chunkZ * WidthChunks + chunkX];
        }

        private Chunk GetOrNewChunkByIndex(int chunkX, int chunkZ)
        {
            Chunk chunk = GetChunkByIndex(chunkX, chunkZ);
            if (chunk == null)
            {
                Vector3 chunkPos = new Vector3(chunkX * Chunk.CHUNK_SIZE, 0, chunkZ * Chunk.CHUNK_SIZE);
                chunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity);
                SetChunk(chunkX, chunkZ, chunk);
            }
            return chunk;
        }

        private Chunk GetOrNewChunk(int x, int z)
        {
            x /= Chunk.CHUNK_SIZE;
            z /= Chunk.CHUNK_SIZE;
            return GetOrNewChunkByIndex(x, z);
        }

        private void SetChunk(int x, int z, Chunk chunk)
        {
            chunks[z * WidthChunks + x] = chunk;
        }

        public bool WithinBounds(int x, int z)
        {
            return (x >= 0 && x < WidthTiles && z >= 0 && z < LengthTiles);
        }

        public bool WithinBounds(int x, int y, int z)
        {
            return WithinBounds(x, z) && y >= 0 && y < Chunk.CHUNK_HEIGHT;
        }

        public bool Walkable(int x, int z)
        {
            Block block0 = GetBlock(new Vector3Int(x, 0, z));
            Block block1 = GetBlock(new Vector3Int(x, 1, z));

            return GetTile(x, z) != TileType.Air &&
                 (block0 == null || !block0.Solid) &&
                 (block1 == null || !block0.Solid);
        }
        #endregion

        #region tile
        public void SetTile(TileType tile, int x, int z, bool refresh = false)
        {
            if (!WithinBounds(x, z))
            {
                Debug.LogError("Cannot set tile outside of bounds");
            }
            Chunk chunk = GetOrNewChunk(x, z);

            chunk.SetTile(tile, x % Chunk.CHUNK_SIZE, z % Chunk.CHUNK_SIZE);
            if (refresh)
                chunk.UpdateChunkMesh();
        }

        public TileType GetTile(int x, int z)
        {
            if (!WithinBounds(x, z))
            {
                Debug.LogError("Cannot get tile outside of bounds");
            }

            Chunk chunk = GetChunk(x, z);
            if (chunk == null)
            {
                return TileType.Air;
            }
            return chunk.GetTile(x % Chunk.CHUNK_SIZE, z % Chunk.CHUNK_SIZE);
        }
        #endregion

        #region block

        /// <summary>
        /// Sets <paramref name="block"/> reference at <paramref name="pos"/>
        /// </summary>
        /// <param name="block"></param>
        /// <param name="pos"></param>
        public void SetBlock(Block block, Vector3Int pos)
        {
            Chunk chunk = GetOrNewChunk(pos.x, pos.z);
            chunk.SetBlock(block, pos.x % Chunk.CHUNK_SIZE, pos.y, pos.z % Chunk.CHUNK_SIZE);
        }

        /// <summary>
        /// Tries to instantiate a <see cref="Block"/> of type <paramref name="type"/> at <paramref name="pos"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <returns>
        /// <br>
        /// <see langword="null"/> if <paramref name="pos"/> does not have space for this <see cref="Block"/>
        /// </br>
        /// <br>
        /// Otherwise, return the <see cref="Block"/> placed
        /// </br>
        /// </returns>
        public Block PlaceBlock(BlockType type, Vector3Int pos)
        {
            Block prefab = GameController.Instance.BlockPrefabs[type];

            if (prefab is MultiBlock)
            {
                return PlaceMultiBlock((MultiBlock)prefab, pos);
            }

            if (!CanPlaceBlock(pos))
                return null;

            Chunk chunk = GetOrNewChunk(pos.x, pos.z);
            Block block = Instantiate(prefab, pos, Quaternion.identity, chunk.transform);
            chunk.SetBlock(block, pos.x % Chunk.CHUNK_SIZE, pos.y, pos.z % Chunk.CHUNK_SIZE);
            return block;
        }

        /// <summary>
        /// Tries to instantiate a <see cref="Block"/> represented by <paramref name="item"/> at <paramref name="pos"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pos"></param>
        /// <returns>
        /// <br>
        /// <see langword="null"/> if <paramref name="pos"/> does not have space for the <see cref="Block"/> 
        /// represented by <paramref name="item"/>
        /// </br>
        /// <br>
        /// Otherwise, return the <see cref="Block"/> placed
        /// </br>
        /// </returns>
        public Block PlaceBlockItem(BlockItem item, Vector3Int pos)
        {
            Block block = PlaceBlock(item.BlockType, pos);

            if (item.HasNBT && block != null)
            {
                block.ReadFromNBT(item.NBT);
            }
            return block;
        }

        /// <summary>
        /// Destroy <paramref name="block"/> and clear from chunk data
        /// </summary>
        /// <param name="block"></param>
        public void DestroyBlock(Block block)
        {
            SetBlock(null, block.Position);
            Destroy(block.gameObject);
        }

        /// <summary>
        /// Get <see cref="Block"/> reference at <paramref name="pos"/>
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Block GetBlock(Vector3Int pos)
        {
            if (!WithinBounds(pos.x, pos.z))
            {
                Debug.LogError("Cannot get block outside of bounds");
            }

            Chunk chunk = GetChunk(pos.x, pos.z);
            if (chunk == null)
            {
                return null;
            }
            return chunk.GetBlock(pos.x % Chunk.CHUNK_SIZE, pos.y, pos.z % Chunk.CHUNK_SIZE);
        }

        /// <summary>
        /// Check if there is a single <see cref="Block"/> worth of space at <paramref name="pos"/>
        /// </summary>
        /// <param name="pos"></param>
        /// <returns><see langword="true"/> if there is space</returns>
        public bool CanPlaceBlock(Vector3Int pos)
        {
            return GetBlock(pos) == null;
        }

        /// <summary>
        /// <br>
        /// Check if there is <see cref="MultiBlock.Size"/> worth of space at 
        /// </br>
        /// <br>
        /// <paramref name="pos"/> starting at bottom south-west corner
        /// </br>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="pos"></param>
        /// <returns><see langword="true"/> if there is space</returns>
        public bool CanPlaceMultiBlock(MultiBlock multiBlock, Vector3Int pos)
        {
            Vector3Int size = multiBlock.Size;

            // breadth iteration order detect invalid spots faster
            for (int testY = pos.y; testY < size.y + pos.y; ++testY)
                for (int testX = pos.x; testX < size.x + pos.x; ++testX)
                    for (int testZ = pos.z; testZ < size.z + pos.z; ++testZ)
                    {
                        if (!WithinBounds(testX, testY, testZ))
                        {
                            return false;
                        }
                        Block testBlock = GetBlock(new Vector3Int(testX, testY, testZ));
                        if (testBlock != null) return false;
                    }
            return true;
        }

        /// <summary>
        /// Tries to place a <see cref="MultiBlock"/> clone of <paramref name="prefab"/> at <paramref name="pos"/>
        /// 
        /// 
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <returns><see langword="null"/> if <paramref name="pos"/> does not have space for this <see cref="MultiBlock"/></returns>
        private MultiBlock PlaceMultiBlock(MultiBlock prefab, Vector3Int pos)
        {
            if (!CanPlaceMultiBlock(prefab, pos)) return null;

            Chunk chunk = GetOrNewChunk(pos.x, pos.z);
            MultiBlock block = Instantiate(prefab, pos, Quaternion.identity, chunk.transform);
            Vector3Int size = block.Size;
            for (int pY = pos.y; pY < size.y + pos.y; ++pY)
                for (int pX = pos.x; pX < size.x + pos.x; ++pX)
                    for (int pZ = pos.z; pZ < size.z + pos.z; ++pZ)
                    {
                        chunk = GetOrNewChunk(pX, pZ);
                        chunk.SetBlock(block, pX % Chunk.CHUNK_SIZE, pY, pZ % Chunk.CHUNK_SIZE);
                    }
            return block;
        }

        /// <summary>
        /// Destroy <paramref name="multiBlock"/> and clear from chunk data
        /// </summary>
        /// <param name="multiBlock"></param>
        public void DestroyMultiBlock(MultiBlock multiBlock)
        {
            Vector3Int pos = multiBlock.Position;
            Vector3Int size = multiBlock.Size;
            for (int pY = pos.y; pY < size.y + pos.y; ++pY)
                for (int pX = pos.x; pX < size.x + pos.x; ++pX)
                    for (int pZ = pos.z; pZ < size.z + pos.z; ++pZ)
                    {
                        Chunk chunk = GetOrNewChunk(pX, pZ);
                        chunk.SetBlock(null, pX % Chunk.CHUNK_SIZE, pY, pZ % Chunk.CHUNK_SIZE);
                    }
            Destroy(multiBlock.gameObject);
        }
        #endregion
    }
}
