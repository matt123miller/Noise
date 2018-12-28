﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dungeon
{

    public class Room : ProceduralObject
    {
        #region Rect bounds and manipulation
        public float Top
        {
            get
            {
                return transform.position.z + height;
            }
        }
        public float Bottom
        {
            get
            {
                return transform.position.z;
            }
        }
        public float Left
        {
            get
            {
                return transform.position.x;
            }
        }
        public float Right
        {
            get
            {
                return transform.position.x + width;
            }
        }
        public Vector3 Centre
        {
            get
            {
                return _centre;
            }
            set
            {
                _centre = value;
                transform.position = new Vector3((int)(value.x - width / 2), 0, (int)(value.z - width / 2));
            }
        }
        public void SetPosition(Vector3 newPos)
        {
            transform.position = newPos;
            _centre = new Vector3(newPos.x + width / 2, 0, newPos.z + width / 2);
        }

        #endregion

        // Always treat width as X and height as Y;
        public int width, height;
        public int floorWidth, floorHeight;
        [SerializeField]
        private Vector3 _centre;
        private Material material;
        public Transform[,] tiles;
        public Transform[] walls;

        public Dictionary<Vector3, Room> neighbours;

        public virtual void SetupRoom(int _width, int _height, Material _material)
        {
            width = _width;
            height = _height;
            floorWidth = _width - 2;
            floorHeight = _height - 2;
            _centre = new Vector3(width / 2, 0, height / 2);
            material = _material;
            neighbours = new Dictionary<Vector3, Room>();
            
            tiles = GenerateTiles();
            //walls = GenerateWalls();
        }

        public Transform[,] GenerateTiles()
        {
            Transform[,] tiles = new Transform[width, height];

            for (int w = 0; w < floorWidth; w++)
            {
                for (int h = 0; h < floorHeight; h++)
                {
                    // Offset by 1 all the time so they're inside the walls.
                    var tile = CreateFloor(w + 1, h + 1);
                    tile.GetComponent<MeshRenderer>().sharedMaterial = material;
                    tiles[w, h] = tile.transform;
                }
            }

            return tiles;
        }

        public Transform[] GenerateWalls()
        {
            List<Transform> walls = new List<Transform>();

            int[] edge = { 0, 1 };
            foreach (var i in edge)
            {
                int z = (height - 1) * i;
                // Make the walls along the width
                for (int w = 0; w < floorWidth; w++)
                {
                    var wall = CreateWall(w + 1, z);
                    walls.Add(wall.transform);
                    wall.name = "width";
                }
                int x = (width - 1) * i;
                // Make the walls along height
                for (int h = 0; h < floorHeight; h++)
                {
                    var wall = CreateWall(x, h + 1);
                    walls.Add(wall.transform);
                    wall.name = "height";
                }
            }

            // Easier to use simpler loops and create 4 corners manually
            walls.Add(CreateWall(0, 0).transform);
            walls.Add(CreateWall(0, height - 1).transform);
            walls.Add(CreateWall(width - 1, 0).transform);
            walls.Add(CreateWall(width - 1, height - 1).transform);

            return walls.ToArray();
        }


        public void PlaceRandomly(Vector2 inBounds)
        {
            int randomX = (int)Random.Range(0, inBounds.x - width);
            int randomY = (int)Random.Range(0, inBounds.y - height);

            transform.position = new Vector3(randomX, 0, randomY);
        }


        public bool IsOverlapping(Room _other, bool _floorDimensions)
        {
            if (!_other) { return false; }
            bool topLeft = false;
            bool bottomRight = false;

            if (_floorDimensions)
            {
                topLeft = this.Left - 2 < _other.Right - 2 && this.Right - 2 > _other.Left - 2;
                bottomRight = this.Top - 2 > _other.Bottom - 2 && this.Bottom - 2 < _other.Top - 2;
            }
            else
            {
                topLeft = this.Left < _other.Right && this.Right > _other.Left;
                bottomRight = this.Top > _other.Bottom && this.Bottom < _other.Top;
            }
            return topLeft && bottomRight;
        }

        //public override string ToString()
        //{
        //    var sb = new StringBuilder();

        //    for (int y = 0; y <= height; y++)
        //    {
        //        for (int x = 0; x <= width; x++)
        //        {
        //            if (y == 0 || y == height || x == 0 || x == width)
        //            {
        //                sb.Append("W");
        //            }
        //            else
        //            {
        //                sb.Append("F");
        //            }

        //            if (x == width)
        //                sb.Append("\n");
        //        }
        //    }
        //    return sb.ToString();
        //}
    }
}
