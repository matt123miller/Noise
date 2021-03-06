using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Dungeon
{
    internal enum DungeonObjects
    {
        EMPTY = 0,
        WALL = 1,
        FLOOR = 2,
        DOOR = 3
    }

    public class DungeonGenerator : ProceduralGenerator
    {
        // I feel like these values are kind of meaningless right now.
        // But I do want to use these to constrain room placement
        // in a given direction in the future.
        [Range( 40, 60 )] public int minWidth;
        [Range( 60, 120 )] public int maxWidth;
        [Range( 40, 60 )] public int minHeight;
        [Range( 60, 120 )] public int maxHeight;

        public Vector2 dungeonSize;
        public bool stepThrough = true;

        private RoomGenerator roomGen;
        private DoorGenerator doorGen;
        private GraphGenerator graphGen;
        private CorridorGenerator corridorGen;
        private SpawnPlayer spawnPlayer;

        private List<Room> rooms;
        private List<Door> doors;

        private int[,] grid;

        private void Awake()
        {
            CacheReferences();

            if (autoBuildAtStart)
            {
                Generate();
            }

        }

        public override void CacheReferences()
        {
            roomGen = GetComponent<RoomGenerator>();
            doorGen = GetComponent<DoorGenerator>();
            graphGen = GetComponent<GraphGenerator>();
            corridorGen = GetComponent<CorridorGenerator>();
            spawnPlayer = GetComponent<SpawnPlayer>();
        }


        public override async void Generate()
        {
            Random.InitState( seed );

            InitialiseDungeon();

            roomGen.EmptyContents( true );
            doorGen.EmptyContents( true );

            // At some point move to a slightly different model.
            // Generate all data first then render everything afterwards.

            rooms = await roomGen.Generate( dungeonSize );
            doors = await doorGen.Generate( rooms );

            // Change dungeon size to be smallest size that fits all rooms.
            ResizeDungeon( rooms );
            //            UpdateGridWithRooms(roomGen.rooms);
            //spawnPlayer.Spawn(rooms.First());

            //print(this);
        }


        private void InitialiseDungeon()
        {
            // Remove any existing dungeon.
            transform.DestroyChildren();

            int w = Random.Range( minWidth, maxWidth );
            int h = Random.Range( minHeight, maxHeight );
            dungeonSize = new Vector2( w, h );
            grid = new int[w, h];
        }

        private void ResizeDungeon( List<Room> rooms )
        {
            maxWidth = (int)rooms.Max( r => r.transform.position.x );
            maxHeight = (int)rooms.Max( r => r.transform.position.z );
        }

        private void UpdateGridWithRooms( List<Room> rooms )
        {
            foreach (Room room in rooms)
            {
                if (room == null)
                { continue; }
                int rootX = (int)room.transform.position.x;
                int rootY = (int)room.transform.position.z;

                for (int x = 0; x < room.width; x++)
                {
                    for (int y = 0; y < room.height; y++)
                    {
                        bool edge = (x == 0 || y == 0 || x == room.width - 1 || y == room.height - 1);

                        grid[rootX + x, rootY + y] = edge ? (int)DungeonObjects.WALL : (int)DungeonObjects.FLOOR;
                    }
                }
            }
        }

        public void SetSeed( int _seed )
        {
            Debug.Log( "seed set to " + seed );
            seed = _seed;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append( "This dungeon:\n" );
            sb.Append( "Has " + roomGen.rooms.Count() + " Rooms\n" );
            sb.Append( "Has the bounds " + dungeonSize + "\n" );

            for (int y = (int)dungeonSize.y - 1; y >= 0; y--)
            {
                for (int x = 0; x < dungeonSize.x; x++)
                {
                    sb.Append( grid[x, y] );
                }
                sb.Append( "\n" );
            }
            return sb.ToString();
        }
    }
}
