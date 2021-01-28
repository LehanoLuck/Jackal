using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class PirateMovementData
    {
        public MovementSettings Settings { get; set; }
        public byte Id { get; set; }
        public byte ShipId { get; set; }
        public byte XPos { get; set; }
        public byte YPos { get; set; }

        public static object Deserialize(byte[] data)
        {
            var result = new PirateMovementData();
            result.Id = data[0];
            result.XPos = data[1];
            result.YPos = data[2];
            result.ShipId = data[3];
            result.Settings = new MovementSettings(Convert.ToBoolean(data[4]), Convert.ToBoolean(data[5]));
            return result;
        }

        public static byte[] Serialize(object data)
        {
            var ship = (PirateMovementData)data;
            return
                new byte[]
                { ship.Id,
                ship.XPos,
                ship.YPos,
                ship.ShipId,
                Convert.ToByte(ship.Settings.IsAttack),
                Convert.ToByte(ship.Settings.IsMoveWithCoin),
                };
        }
    }
}
