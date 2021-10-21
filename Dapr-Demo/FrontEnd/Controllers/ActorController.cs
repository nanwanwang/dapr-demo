using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActorController:ControllerBase
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        public ActorController(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }

        [HttpGet("paid/{orderId}")]
        public async Task<ActionResult> PaidAsync(string orderId)
        {
            var actorId = new ActorId("myid-" + orderId);
            var proxy = ActorProxy.Create<IOrderStatusActor>(actorId, "OrderStatusActor");
            var result = await proxy.Paid(orderId);
            return Ok(result);
        }


        [HttpGet("get/{orderId}")]
        public async Task<ActionResult> GetAsync(string orderId)
        {
            var proxy = _actorProxyFactory.CreateActorProxy<IOrderStatusActor>(
                new ActorId("myid-" + orderId),
                "OrderStatusActor");
            return Ok(await proxy.GetStatus(orderId));
        }


        
        /// <summary>
        /// 停止timer
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("stop/{orderId}/{name}")]
        public async Task<ActionResult> StopTimerAsync(string orderId,string name)
        {
            var proxy = _actorProxyFactory.CreateActorProxy<IOrderStatusActor>(
             new ActorId("myid-" + orderId),
             "OrderStatusActor");
            await proxy.StopTimerAsync(name);

            return Ok("done");
        }
    }
}
