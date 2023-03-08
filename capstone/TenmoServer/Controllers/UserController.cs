using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("users")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserDao userDao;

        public UserController(IUserDao _userDao)
        {
            this.userDao = _userDao;
        }

        [HttpGet("{id}")]
        public IActionResult GetBalance(int id)
        {
            int balance = userDao.GetBalance(id);
            return Ok(balance);
        }
    }
}
