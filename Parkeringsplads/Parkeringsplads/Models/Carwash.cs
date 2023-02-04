using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkeringsPlads
{
    internal class Carwash
    {
        private int ID;
        public bool Active { get; set; }
        public Ticket? Current { get; private set; }
        public int RemainingTime { get; private set; }
        
        public Carwash(int ID)
        {
            this.ID = ID;
            this.Active = true;
            this.Current = null;
            this.RemainingTime = 0;
        }

        public void Run(ref ConcurrentQueue<Ticket> concurrentQueue)
        {
            while (Active)
            {
                if (Current == null)
                {
                    if (concurrentQueue.TryDequeue(out Ticket? dequeueItem))
                    {
                        this.Current = dequeueItem;
                        switch (dequeueItem.SelectedWash?.Type)
                        {
                            case CarWashTypeEnum.Economy:
                                this.RemainingTime = 10 * 1000;
                                break;
                            case CarWashTypeEnum.Basis:
                                this.RemainingTime = 15 * 1000;
                                break;
                            case CarWashTypeEnum.Premium:
                                this.RemainingTime = 25 * 1000;
                                break;
                            case null:
                                break;
                        }

                        while (RemainingTime > 0)
                        {
                            this.RemainingTime = this.RemainingTime - 1 * 1000;
                            Thread.Sleep(1000);
                        }
                        
                        //Automatically returns this info to parent thread to print it.
                        Console.WriteLine($"WASH-{ID}: Finished washing " + Current.LicensePlate);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
