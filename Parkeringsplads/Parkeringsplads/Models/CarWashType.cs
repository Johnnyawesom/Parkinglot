namespace ParkeringsPlads;

public class CarWashType
{
    public CarWashTypeEnum Type { get; private set; }
    public int PriceIOere { get; private set; }

    public CarWashType(CarWashTypeEnum type)
    {
        Type = type;
        switch (type)
        {
            case CarWashTypeEnum.Economy:
                PriceIOere = 1000;
                break;
            case CarWashTypeEnum.Basis:
                PriceIOere = 5000;
                break;
            case CarWashTypeEnum.Premium:
                PriceIOere = 10000;
                break;
        }
    }
}