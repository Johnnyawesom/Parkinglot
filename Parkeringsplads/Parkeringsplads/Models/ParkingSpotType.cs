namespace ParkeringsPlads;

public class ParkingSpotType
{
    public CarTypeEnum Type { get; private set; }
    public int PriceIOere { get; private set; }

    public ParkingSpotType(CarTypeEnum type)
    {
        Type = type;
        switch (type)
        {
            case CarTypeEnum.PassengerCar:
                PriceIOere = 1000;
                break;
            case CarTypeEnum.Truck:
            case CarTypeEnum.Bus:
                PriceIOere = 1000;
                break;
            case CarTypeEnum.CarWithTrailer:
                PriceIOere = 1000;
                break;
        }
    }
}