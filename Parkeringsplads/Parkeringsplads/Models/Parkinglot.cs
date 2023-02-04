using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkeringsPlads
{
    internal class Parkinglot
    {
        public int ID { get; }
        // ? betyder at den er nullable
        public ParkingSpotType? CarType { get; }
        public ParkingSpotType? CarType2 { get; }
        public Parkinglot(int ID , CarTypeEnum type)
        {
            this.ID = ID;
            this.CarType = new ParkingSpotType(type);
            this.CarType2 = null;
        }
        public Parkinglot(int ID, CarTypeEnum type, CarTypeEnum type2)
        {
            this.ID = ID;
            this.CarType = new ParkingSpotType(type);;
            this.CarType2 = new ParkingSpotType(type2);;

        }

    }
}
