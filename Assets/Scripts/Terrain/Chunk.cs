using NBT.Tags;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace FrontierIsland
{
    public class Chunk : MonoBehaviour, ISelectable, INBTSerializable
    {
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;

        private Mesh mesh;

        public const int CHUNK_SIZE = 16;
        public const int CHUNK_HEIGHT = 2;

        public readonly byte[] tiles = new byte[CHUNK_SIZE * CHUNK_SIZE];
        public readonly Block[] blocks = new Block[CHUNK_SIZE * CHUNK_SIZE * CHUNK_HEIGHT];

        private void Awake()
        {
            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public TileType GetTile(int x, int z)
        {
            return (TileType)tiles[z * CHUNK_SIZE + x];
        }

        public void SetTile(TileType tile, int x, int z)
        {
            tiles[z * CHUNK_SIZE + x] = (byte)tile;
        }

        public Block GetBlock(int x, int y, int z)
        {
            return blocks[CHUNK_SIZE * CHUNK_SIZE * y + z * CHUNK_SIZE + x];
        }

        public void SetBlock(Block block, int x, int y, int z)
        {
            blocks[CHUNK_SIZE * CHUNK_SIZE * y + z * CHUNK_SIZE + x] = block;
        }

        #region rendering
        // ! IMPORTANT NOTE !
        // The following BuildChunkMesh methods assume the gameobject's
        // material's albedo texture contains the nessesary tile textures.
        //
        // The tile texures should have the following format rules.
        // 
        // 1. Each row is a tile's texture in the order of
        //    top surface, north side, east side, south side, west side
        // 2. The row order should follow TileType enum order from top to bottom
        // 3. TileType must always have Air
        // 
        //  Example:
        //  TileType.Air
        //  TileType.Grass
        //  TileType.Stone
        //
        //
        //  *-----------------------------------------------------*
        //  | Air Top, Air N, Air E, Air S, Air W (or Empty)      |
        //  |-----------------------------------------------------|
        //  | Grass Top, Grass N, Grass E, Grass S, Grass W       |
        //  |-----------------------------------------------------|
        //  | Stone Top, Stone N, Stone E, Stone S, Stone W       |
        //  *-----------------------------------------------------*
        //

        void BuildChunkMeshSurface(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            float tileCount = Enum.GetValues(typeof(TileType)).Length;
            Vector3[] v = new Vector3[6];
            Vector2[] uv = new Vector2[6];
            for (int x = 0; x < Chunk.CHUNK_SIZE; ++x)
            {
                for (int z = 0; z < Chunk.CHUNK_SIZE; ++z)
                {
                    // Quad diagram
                    // a -- b
                    // |    |
                    // c -- d
                    //

                    byte tile = (byte)GetTile(x, z);

                    if (tile == (byte)TileType.Air)
                        continue;

                    // Verticies
                    Vector3 a = new Vector3(x - 0.5f, 0, z + 0.5f);
                    Vector3 b = new Vector3(x + 0.5f, 0, z + 0.5f);
                    Vector3 c = new Vector3(x - 0.5f, 0, z - 0.5f);
                    Vector3 d = new Vector3(x + 0.5f, 0, z - 0.5f);
                    v[0] = a; // a
                    v[1] = b; // b
                    v[2] = c; // c
                    v[3] = b; // b
                    v[4] = d; // d
                    v[5] = c; // c


                    // UV Mapping
                    Vector2 uvA = new Vector2(0, (tileCount - tile) / tileCount);
                    Vector2 uvB = new Vector2(0.2f, (tileCount - tile) / tileCount);
                    Vector2 uvC = new Vector2(0, (tileCount - tile - 1) / tileCount);
                    Vector2 uvD = new Vector2(0.2f, (tileCount - tile - 1) / tileCount);

                    uv[0] = uvA;
                    uv[1] = uvB;
                    uv[2] = uvC;
                    uv[3] = uvB;
                    uv[4] = uvD;
                    uv[5] = uvC;


                    for (int i = 0; i < v.Length; ++i)
                    {
                        vertices.Add(v[i]);
                        triangles.Add(triangles.Count);
                        uvs.Add(uv[i]);
                    }
                }
            }
        }

        void BuildChunkMeshSide(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            int depth = 1;
            float tileCount = Enum.GetValues(typeof(TileType)).Length;
            Vector3[] v = new Vector3[6];
            Vector2[] uv = new Vector2[6];
            for (int x = 0; x < Chunk.CHUNK_SIZE; ++x)
            {
                for (int z = 0; z < Chunk.CHUNK_SIZE; ++z)
                {
                    // Quad diagram
                    // a -- b
                    // |    |
                    // c -- d
                    //

                    byte tile = (byte)GetTile(x, z);

                    if (tile != (byte)TileType.Air)
                    {
                        // West
                        if (x == 0 || GetTile(x - 1, z) == (byte)TileType.Air)
                        {
                            Vector3 a = new Vector3(x - 0.5f, 0, z + 0.5f);
                            Vector3 b = new Vector3(x - 0.5f, 0, z - 0.5f);
                            Vector3 c = new Vector3(x - 0.5f, 0 - depth, z + 0.5f);
                            Vector3 d = new Vector3(x - 0.5f, 0 - depth, z - 0.5f);
                            v[0] = a;
                            v[1] = b;
                            v[2] = c;
                            v[3] = b;
                            v[4] = d;
                            v[5] = c;

                            // UV Mapping
                            Vector2 uvA = new Vector2(0.8f, (tileCount - tile) / tileCount);
                            Vector2 uvB = new Vector2(1f, (tileCount - tile) / tileCount);
                            Vector2 uvC = new Vector2(0.8f, (tileCount - tile - 1) / tileCount);
                            Vector2 uvD = new Vector2(1f, (tileCount - tile - 1) / tileCount);

                            uv[0] = uvA;
                            uv[1] = uvB;
                            uv[2] = uvC;
                            uv[3] = uvB;
                            uv[4] = uvD;
                            uv[5] = uvC;
                            for (int j = 0; j < v.Length; j++)
                            {
                                vertices.Add(v[j]);
                                triangles.Add(triangles.Count);
                                uvs.Add(uv[j]);
                            }
                        }

                        // South
                        if (z == 0 || GetTile(x, z - 1) == (byte)TileType.Air)
                        {
                            Vector3 a = new Vector3(x - 0.5f, 0, z - 0.5f);
                            Vector3 b = new Vector3(x + 0.5f, 0, z - 0.5f);
                            Vector3 c = new Vector3(x - 0.5f, 0 - depth, z - 0.5f);
                            Vector3 d = new Vector3(x + 0.5f, 0 - depth, z - 0.5f);
                            v[0] = a;
                            v[1] = b;
                            v[2] = c;
                            v[3] = b;
                            v[4] = d;
                            v[5] = c;

                            // UV Mapping
                            Vector2 uvA = new Vector2(0.6f, (tileCount - tile) / tileCount);
                            Vector2 uvB = new Vector2(0.8f, (tileCount - tile) / tileCount);
                            Vector2 uvC = new Vector2(0.6f, (tileCount - tile - 1) / tileCount);
                            Vector2 uvD = new Vector2(0.8f, (tileCount - tile - 1) / tileCount);

                            uv[0] = uvA;
                            uv[1] = uvB;
                            uv[2] = uvC;
                            uv[3] = uvB;
                            uv[4] = uvD;
                            uv[5] = uvC;
                            for (int j = 0; j < v.Length; j++)
                            {
                                vertices.Add(v[j]);
                                triangles.Add(triangles.Count);
                                uvs.Add(uv[j]);
                            }
                        }

                        //// North
                        //if (z + 1 == Chunk.CHUNK_SIZE || chunk.GetTile(x, z + 1) == (byte)TileType.Air)
                        //{
                        //    Vector3 a = new Vector3(x + 0.5f, 0, z + 0.5f);
                        //    Vector3 b = new Vector3(x - 0.5f, 0, z + 0.5f);
                        //    Vector3 c = new Vector3(x + 0.5f, 0 - depth, z + 0.5f);
                        //    Vector3 d = new Vector3(x - 0.5f, 0 - depth, z + 0.5f);
                        //    v[0] = a;
                        //    v[1] = b;
                        //    v[2] = c;
                        //    v[3] = b;
                        //    v[4] = d;
                        //    v[5] = c;

                        //    // UV Mapping
                        //    Vector2 uvA = new Vector2(0.2f, (tileCount - tile) / tileCount);
                        //    Vector2 uvB = new Vector2(0.4f, (tileCount - tile) / tileCount);
                        //    Vector2 uvC = new Vector2(0.2f, (tileCount - tile - 1) / tileCount);
                        //    Vector2 uvD = new Vector2(0.4f, (tileCount - tile - 1) / tileCount);

                        //    uv[0] = uvA;
                        //    uv[1] = uvB;
                        //    uv[2] = uvC;
                        //    uv[3] = uvB;
                        //    uv[4] = uvD;
                        //    uv[5] = uvC;
                        //    for (int j = 0; j < v.Length; j++)
                        //    {
                        //        vertices.Add(v[j]);
                        //        triangles.Add(triangles.Count);
                        //        uvs.Add(uv[j]);
                        //    }
                        //}

                        //// East
                        //if (x + 1 == Chunk.CHUNK_SIZE || chunk.GetTile(x + 1, z) == (byte)TileType.Air)
                        //{
                        //    Vector3 a = new Vector3(x + 0.5f, 0, z - 0.5f);
                        //    Vector3 b = new Vector3(x + 0.5f, 0, z + 0.5f);
                        //    Vector3 c = new Vector3(x + 0.5f, 0 - depth, z - 0.5f);
                        //    Vector3 d = new Vector3(x + 0.5f, 0 - depth, z + 0.5f);
                        //    v[0] = a;
                        //    v[1] = b;
                        //    v[2] = c;
                        //    v[3] = b;
                        //    v[4] = d;
                        //    v[5] = c;

                        //    // UV Mapping
                        //    Vector2 uvA = new Vector2(0.4f, (tileCount - tile) / tileCount);
                        //    Vector2 uvB = new Vector2(0.6f, (tileCount - tile) / tileCount);
                        //    Vector2 uvC = new Vector2(0.4f, (tileCount - tile - 1) / tileCount);
                        //    Vector2 uvD = new Vector2(0.6f, (tileCount - tile - 1) / tileCount);

                        //    uv[0] = uvA;
                        //    uv[1] = uvB;
                        //    uv[2] = uvC;
                        //    uv[3] = uvB;
                        //    uv[4] = uvD;
                        //    uv[5] = uvC;
                        //    for (int j = 0; j < v.Length; j++)
                        //    {
                        //        vertices.Add(v[j]);
                        //        triangles.Add(triangles.Count);
                        //        uvs.Add(uv[j]);
                        //    }
                        //}
                    }
                }
            }
        }

        public void UpdateChunkMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            BuildChunkMeshSurface(vertices, triangles, uvs);
            BuildChunkMeshSide(vertices, triangles, uvs);

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray(); // uv assignment must be after vertices
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            // update mesh collider
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                gameObject.SetActive(true);
            }
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z - 0.5f);

            Gizmos.DrawLine(origin, new Vector3(origin.x + CHUNK_SIZE, origin.y, origin.z));
            Gizmos.DrawLine(origin, new Vector3(origin.x, origin.y, origin.z + CHUNK_SIZE));

            Gizmos.DrawLine(new Vector3(origin.x, origin.y, origin.z + CHUNK_SIZE),
                new Vector3(origin.x + CHUNK_SIZE, origin.y, origin.z + CHUNK_SIZE));
            Gizmos.DrawLine(new Vector3(origin.x + CHUNK_SIZE, origin.y, origin.z),
                new Vector3(origin.x + CHUNK_SIZE, origin.y, origin.z + CHUNK_SIZE));
        }

        public void OnSelect()
        {
            //throw new NotImplementedException();
        }

        public void ReadFromNBT(CompoundTag nbt)
        {
            throw new NotImplementedException();
        }

        public void WriteToNBT(CompoundTag nbt)
        {
            throw new NotImplementedException();
        }
    }
}
