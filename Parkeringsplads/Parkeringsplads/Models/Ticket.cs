using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkeringsPlads
{
    internal class Ticket
    {
        public string LicensePlate { get; }
        public int ParkingLotId { get; set; }
        public DateTime Started { get; }
        public CarTypeEnum? CarType { get; }

        public bool HasSelectedCarWash { get; set; } = false;
        public CarWashType? SelectedWash { get; set; }
        
        
        public Ticket(string licensePlate)
        {
            this.LicensePlate = licensePlate;
            this.Started = DateTime.Now;
            this.HasSelectedCarWash = false;
            this.SelectedWash = null;
        }

        public Ticket(string licensePlate, int parkingLotId, CarTypeEnum? carTypeEnum)
        {
            this.LicensePlate = licensePlate;
            this.ParkingLotId = parkingLotId;
            this.Started = DateTime.Now;
            this.CarType = carTypeEnum;
            this.HasSelectedCarWash = false;
            this.SelectedWash = null;

        }
        
        public override string ToString()
        {
            return "Your License plate is  " + LicensePlate + " with assigned parking lot " + ParkingLotId + " started at " 
                   + Started.ToShortDateString() + " " + Started.ToShortTimeString() + (HasSelectedCarWash ? " you selected wash " + SelectedWash?.Type : "");

        }

        public string PrintReceipt(double price)
        {
            return @$"Johnnys parkering
************************************
          KUN AUTORISATION
************************************

Betalingskort                PSN: 00
Contactless
XXXX XXXX XXXX 7262
TERM:                71304720-000290
NETS A/S                     1111111
KC1               NETS NR:0008557764
AID:                  A0000001214711
PSAM:             5374978-0000689703
ARC:00                   STATUS:0000
AUT KODE:                     072228
REF:000290               AUTORISERET
PRICE                    {price} DKK";
        }
    }

}
