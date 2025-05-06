using ByBitApi.Interface;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs;

[Authorize]
public class CostHub : Hub
{
    public CostHub(IByBitService byBitService)
    {
        var _ = byBitService.CheckIsStarted();
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.User!.Claims.First(x => x.Type == Constant.JwtUserId).Value;
        this.Groups.AddToGroupAsync(Context.ConnectionId, userId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User!.Claims.First(x => x.Type == Constant.JwtUserId).Value;
        this.Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        return base.OnDisconnectedAsync(exception);
    }
}