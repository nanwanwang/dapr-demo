using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StateController:ControllerBase
    {
        private readonly ILogger<StateController> _logger;
        private readonly DaprClient _daprClient;

        public StateController(ILogger<StateController> logger,DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        /// <summary>
        /// 获取状态值
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var result = await _daprClient.GetStateAsync<string>("statestore", "guid");
            return Ok(result);
        }

        /// <summary>
        /// 新增一个值
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PostAsync()
        {
            await _daprClient.SaveStateAsync<string>("statestore", "guid", Guid.NewGuid().ToString(), new StateOptions { Consistency = ConsistencyMode.Strong });
            return Ok("done");
        }

        /// <summary>
        /// 删除一个值
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync()
        {
            await _daprClient.DeleteStateAsync("statestore", "guid");
            return Ok("done");
        }


        /// <summary>
        /// 获取一个值和etag
        /// </summary>
        /// <returns></returns>
        [HttpGet("withetag")]
        public async Task<ActionResult> GetWithEtagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>("statestore", "guid");
            return Ok($"value is {value},etag is{etag}");
        }

        /// <summary>
        /// 通过tag防止并发冲突,删除一个值
        /// </summary>
        /// <returns></returns>
        [HttpDelete("withtag")]
        public async Task<ActionResult> PostWithTagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>("statestore", "guid");
            return Ok(await _daprClient.TryDeleteStateAsync("statestore", "guid", etag));
        }


        /// <summary>
        /// 根据绑定获取并修改值,键值name从路由模板获取
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost("withbinding/{name}")]
        public async Task<ActionResult> GetFromBindingAsync([FromState("statestore","name")] StateEntry<string> state)
        {
            state.Value = Guid.NewGuid().ToString();
            return Ok(await state.TrySaveAsync());

        }


        /// <summary>
        /// 获取多个值
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult> GetListAsync()
        {
            var result = await _daprClient.GetBulkStateAsync("statestore", new List<string> { "guid" }, 10);
            return Ok(result);
        }

        public async Task<ActionResult> DeleteListAsync()
        {
            var data = await _daprClient.GetBulkStateAsync("statestore", new List<string> { "guid" }, 10);
            var removeList = new List<BulkDeleteStateItem>();
            foreach (var item in data)
            {
                removeList.Add(new BulkDeleteStateItem(item.Key, item.Value));
            }
            await _daprClient.DeleteBulkStateAsync("statestore", removeList);
            return Ok("done");
        }

    }
}
