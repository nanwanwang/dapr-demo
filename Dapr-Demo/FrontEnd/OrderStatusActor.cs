using Dapr.Actors.Runtime;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd
{
    public class OrderStatusActor : Actor, IOrderStatusActor, IRemindable
    {
        private readonly ILogger<OrderStatusActor> _logger;
        public OrderStatusActor(ILogger<OrderStatusActor> logger,ActorHost host) : base(host)
        {
            _logger = logger;
            StartTimerAsync("test-timer", "this is a test timer").ConfigureAwait(false).GetAwaiter().GetResult();
            SetReminderAsync("this is a test reminder").ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<string> GetStatus(string orderId)
        {

            return await StateManager.GetStateAsync<string>(orderId);
        }

        public async Task<string> Paid(string orderId)
        {
            await StateManager.AddOrUpdateStateAsync(orderId, "init", (key, currentStatus) => "paid");
            return orderId;
        }


        public Task StartTimerAsync(string name,string text)
        {
            return RegisterTimerAsync(name, nameof(TimerCallbackAsync), Encoding.UTF8.GetBytes(text), TimeSpan.Zero, TimeSpan.FromSeconds(3));
        }

        public Task TimerCallbackAsync(byte[] state)
        {
            var text = Encoding.UTF8.GetString(state);
            _logger.LogInformation($"Timer fired:{text}");
            return Task.CompletedTask;
        }

        public Task StopTimerAsync(string name)
        {
            return UnregisterTimerAsync(name);
        }


        public Task  SetReminderAsync(string  text)
        {
            return RegisterReminderAsync("test-reminder", Encoding.UTF8.GetBytes(text), TimeSpan.Zero, TimeSpan.FromSeconds(1));

        }

        public Task ReceiveReminderAsync(string reminderName,byte[] state,TimeSpan dueTime,TimeSpan period)
        {
            if(reminderName == "test-reminder")
            {
                var text = Encoding.UTF8.GetString(state);
                Logger.LogWarning($"reminder fired:{text}");

            }
            return Task.CompletedTask;
        }
    }
}
