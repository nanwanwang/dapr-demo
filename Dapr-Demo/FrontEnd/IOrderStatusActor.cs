using Dapr.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd
{
    public interface IOrderStatusActor:IActor
    {
        Task<string> Paid(string id);

        Task<string> GetStatus(string orderId);


        Task StopTimerAsync(string name);
    }
}
