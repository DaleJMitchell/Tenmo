using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using TenmoServer.DAO;
using TenmoServer.Models;
using System.Collections.Generic;

namespace TenmoServer.Controllers
{
    [Route("users")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserDao userDao;
        private readonly IAccountDao accountDao;

        public UserController(IUserDao _userDao, IAccountDao _accountDao)
        {
            this.userDao = _userDao;
            this.accountDao = _accountDao;
        }

        [HttpGet("{id}")]
        public int GetBalance(int id)
        {
            int balance = accountDao.GetBalance(id);
            return balance;
        
        }

        [HttpGet]
        public List<int> ViewAccountIds()
        {
            return userDao.ViewAccountIds();
        }

        [HttpGet("{userId}/account")]
        public int GetAccountId(int userId)
        {
            return userDao.GetAccountId(userId);
        }
    }
}
