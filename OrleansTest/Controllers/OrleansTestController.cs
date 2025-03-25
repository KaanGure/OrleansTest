using Microsoft.AspNetCore.Mvc;
using OrleansTest.Grains.GrainInterfaces;

namespace OrleansTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrleansTestController (IClusterClient client) : ControllerBase
{
    [HttpPost("account")]
    public async Task<ActionResult> CreateAccount([FromQuery] string accountName)
    {
        var accountGrain = client.GetGrain<IAccountGrain>(Guid.NewGuid());
        await accountGrain.Initialize(accountName);

        return Ok(accountGrain.GetGrainId().GetGuidKey());
    }

    [HttpGet("account")]
    public async Task<ActionResult> GetAccount([FromQuery] Guid accountId)
    {
        var accountGrain = client.GetGrain<IAccountGrain>(accountId);
        string name = await accountGrain.GetName();
        DateTime timeCreated = await accountGrain.GetCreationTime();

        return new JsonResult(new { name, timeCreated });
    }

    [HttpPost("account/camera")]
    public async Task<ActionResult> CreateCamera([FromQuery] string cameraName, [FromQuery] Guid accountId)
    {
        var cameraGrain = client.GetGrain<ICameraGrain>(Guid.NewGuid());
        await cameraGrain.Initialize(cameraName);

        var accountGrain = client.GetGrain<IAccountGrain>(accountId);
        await accountGrain.AddCamera(cameraGrain.GetGrainId().GetGuidKey());

        return Ok(cameraGrain.GetGrainId().GetGuidKey());
    }

    [HttpGet("account/camera")]
    public async Task<ActionResult> GetCamera([FromQuery] Guid cameraId)
    {
        var cameraGrain = client.GetGrain<ICameraGrain>(cameraId);
        string cameraName = await cameraGrain.GetName();

        return Content(cameraName);
    }

    [HttpGet("account/cameras")]
    public async Task<ActionResult> GetAccountCameras([FromQuery] Guid accountId)
    {
        var accountGrain = client.GetGrain<IAccountGrain>(accountId);
        var cameras = await accountGrain.GetCameraIds();

        return new JsonResult(cameras);
    }
}
