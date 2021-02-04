﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class ShipMovementData
    {
        public byte Id { get; set; }
        public byte XPos { get; set; }
        public byte YPos { get; set; }

        public static object Deserialize(byte[] data)
        {
            var result = new ShipMovementData();
            result.Id = data[0];
            result.XPos = data[1];
            result.YPos = data[2];
            return result;
        }

        public static byte[] Serialize(object data)
        {
            var ship = (ShipMovementData)data;
            return
                new byte[]
                { ship.Id,
                ship.XPos,
                ship.YPos};
        }
    }
}