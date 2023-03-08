using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private static ITransferDao transferDao;
        public TransferController() 
        { 
        
        }

        [HttpPut("transfer")]
        public ActionResult<Transfer> SendMoney(Transfer transfer)
        {
            Transfer newTransfer = transferDao.SendMoney(transfer);
            return Ok(newTransfer);
        }

        [HttpGet("transfer")]
        public ActionResult<IList<Transfer>> GetTransferHistory()
        {
            return transferDao.List();
        }

        [HttpGet("{id}")]
        public ActionResult<Transfer> GetTransferById(int id)
        {
            Transfer transfer = transferDao.Get(id);
            if (transfer != null)
            {
                return Ok(transfer);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
