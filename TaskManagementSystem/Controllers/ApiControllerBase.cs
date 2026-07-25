using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskManagementSystem.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}