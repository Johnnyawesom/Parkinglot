using Parkinglot;
using System.Collections.Concurrent;

namespace ParkeringsPlads;
class Program
{
    static List<Parkinglot> parkinglots = new List<Parkinglot>() 
    {
        new Parkinglot(1,CarTypeEnum.PassengerCar), 
        new Parkinglot(2,CarTypeEnum.PassengerCar),
        new Parkinglot(3,CarTypeEnum.Bus, CarTypeEnum.Truck),
        new Parkinglot(4,CarTypeEnum.Bus, CarTypeEnum.Truck),
        new Parkinglot(5,CarTypeEnum.CarWithTrailer),
        new Parkinglot(6,CarTypeEnum.CarWithTrailer),
    };
    static List<Ticket> tickets = new List<Ticket>();

    private static Carwash carwash1 = new Carwash(1);
    private static Carwash carwash2 = new Carwash(2);
    
    //https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentqueue-1?view=net-7.0
    private static ConcurrentQueue<Ticket> washingQueue = new ConcurrentQueue<Ticket>();

    /**
     * Main method
     */
    private static void Main(string[] args)
    {
        //"Start" the car wash machines (threads):
        Task.Run(() => carwash1.Run(ref washingQueue));
        Task.Run(() => carwash2.Run(ref washingQueue));

        bool keepRunning = true;
        do
        {
            int selectedMenuOption = ConsoleHelper.MultipleChoice(true, "Buy Ticket", "Buy Carwash", "Washing Queue", "Leave Parkinglot", "END");
            switch (selectedMenuOption)
            {
                case 0:
                {
                    BuyTicket();
                    break;
                }
                case 1:
                    BuyCarWash();
                    break;
                case 2:
                    SeeQueue();
                    break;
                case 3:
                {
                    CheckOut();
                    break;
                }
                //default:
                default:
                    keepRunning = false;
                    break;
            }
        }
        while (keepRunning);
    }

    /**
     * Logic for see queue.
     */
    private static void SeeQueue()
    {
        Console.WriteLine("Washing queue:");
        Console.WriteLine($"IN WASH-1: {carwash1.Current?.LicensePlate} \t {carwash1.RemainingTime/1000} seconds left");
        Console.WriteLine($"IN WASH-2: {carwash2.Current?.LicensePlate} \t {carwash2.RemainingTime/1000} seconds left");

        int queuespot = 1;
        foreach (var ticket in washingQueue)
        {
            Console.WriteLine($"Queue position {queuespot}: {ticket.LicensePlate} \t ({ticket.SelectedWash?.Type})");
            queuespot = queuespot + 1;
        }
        Console.ReadLine();
    }

    /**
     * Logic for buying a car wash.
     */
    private static void BuyCarWash()
    {
        string? licensePlate = null;
        do
        {
            Console.Write("Input License Plate:");
            licensePlate = Console.ReadLine();
        } while (licensePlate == null);
        
        Ticket? foundTicket = tickets.Find(x => x.LicensePlate == licensePlate);
        if (foundTicket == null)
        {
            Console.WriteLine("Could not be found try again press enter");
            Console.ReadLine();
            return;
        }
        
        CarWashTypeEnum? selectedWash;
        int selectedMenuOption = ConsoleHelper.MultipleChoice(true, "Økonomivask", "Basisvask", "Premiumvask");
        switch (selectedMenuOption)
        {
            case 0:
                selectedWash = CarWashTypeEnum.Basis;
                break;
            case 1:
                selectedWash = CarWashTypeEnum.Economy;
                break;
            case 2:
                selectedWash = CarWashTypeEnum.Premium;
                break;
            default:
                selectedWash = null;
                break;
        }

        if (selectedWash == null)
        {
            return;
        }

        foundTicket.HasSelectedCarWash = true;
        foundTicket.SelectedWash = new CarWashType((CarWashTypeEnum)selectedWash);
        
        int queueTime = washingQueue.Sum(x =>
        {
            switch (x.SelectedWash?.Type)
            {
                case CarWashTypeEnum.Economy:
                    return 10;
                case CarWashTypeEnum.Basis:
                    return 15;
                case CarWashTypeEnum.Premium:
                    return 25;
                default:
                    return 0;
            }
        });
        //Add in time for the shortest remaining active wash
        queueTime = queueTime + Math.Min(carwash1.RemainingTime, carwash2.RemainingTime) / 1000;
        
        washingQueue.Enqueue(foundTicket);
        
        Console.WriteLine("Current queue time is " + queueTime + " seconds.");
        Console.ReadLine();
    }

    /**
     * Logic for buying a ticket or wash.
     */
    private static void BuyTicket()
    {
        CarTypeEnum? selectedCar;
        int selectedCarMenuOptions = ConsoleHelper.MultipleChoice(true, "PassengerCar", "Truck", "Bus", "Car with trailer");
        switch (selectedCarMenuOptions)
        {
            case 0:
                selectedCar = CarTypeEnum.PassengerCar;
                break;
            case 1:
                selectedCar = CarTypeEnum.Truck;
                break;
            case 2:
                selectedCar = CarTypeEnum.Bus;
                break;
            case 3:
                selectedCar = CarTypeEnum.CarWithTrailer;
                break;
            default:
                selectedCar = null;
                break;
        }

        if (selectedCar == null)
        {
            Console.WriteLine("Something went wrong please try again press enter");
            Console.ReadLine();
            return;
        }

        string? licensePlate = null;
        do
        {
            Console.Write("Input License Plate:");
            licensePlate = Console.ReadLine();
        } while (licensePlate == null);

        bool anyFreeParking = parkinglots.Any(x => x.CarType?.Type == selectedCar || x.CarType2?.Type == selectedCar);
        if (anyFreeParking == false)
        {
            Console.WriteLine("No free parking spaces available press enter");
            Console.ReadLine();
            return;
        }

        List<int> takenParkingLot = tickets.Select(x => x.ParkingLotId).ToList();
        Parkinglot? freeLot = parkinglots
            .Where(x => x.CarType.Type == selectedCar || x.CarType2?.Type == selectedCar)
            .Where(x => !takenParkingLot.Contains(x.ID))
            .FirstOrDefault();
        if (freeLot == null)
        {
            Console.WriteLine("No free parking spaces available press enter");
            Console.ReadLine();
            return;
        }

        tickets.Add(new Ticket(licensePlate, freeLot.ID, selectedCar));
    }
    
    /**
     * Logic for leaving parking lot.
     */
    private static void CheckOut()
    {
        Console.Write("Input License Plate:");
        string licensePlate = Console.ReadLine();
        Ticket? ticketFound = tickets.Find(x => x.LicensePlate == licensePlate);
        if (ticketFound == null)
        {
            Console.WriteLine("Could not be found try again press enter");
            Console.ReadLine();
            return;
        }

        Parkinglot parkingLot = parkinglots.Single(x => x.ID == ticketFound.ParkingLotId);
        double priceWashing = (ticketFound.SelectedWash?.PriceIOere ?? 0) / 100d;
        double priceParking = 10000 / 100d; //hardcoded parking price (lazy)

        double price = priceWashing + priceParking;

        Console.WriteLine(ticketFound);
        Console.WriteLine("Press enter to pay.");
        Console.ReadLine();
        Console.WriteLine(ticketFound.PrintReceipt(price));
        Console.ReadLine();
    }
}