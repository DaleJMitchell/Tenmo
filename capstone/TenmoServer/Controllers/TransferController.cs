using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("transfer")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private static ITransferDao transferDao;
      

        [HttpPost]
        public ActionResult<Transfer> SendMoney(Transfer transfer)
        {
            Transfer newTransfer = transferDao.CreateTransfer(transfer);
            if (newTransfer.status_Id == 2)
            {
                return Accepted(newTransfer);
            }
            return BadRequest(newTransfer);
        }

        [HttpPost("request")]
        public ActionResult<Transfer> MakeRequest(Transfer transfer)
        {
            Transfer newRequest = transferDao.CreateTransfer(transfer);
            if (newRequest.status_Id == 1)
            {
                return Ok(newRequest);
            }
            return BadRequest(newRequest);
        }

        [HttpPut("request/fulfill")]
        public ActionResult<Transfer> FulfillRequst(Transfer transfer)
        {
            Transfer fulfilledRequest = transferDao.FulfillRequest(transfer);
            if (fulfilledRequest.status_Id == 3)
            {
                return BadRequest(fulfilledRequest);
            }
            if (fulfilledRequest.status_Id == 2)
            {
                return Accepted(fulfilledRequest);
            }
            return BadRequest(fulfilledRequest);
        }

        [HttpPut("request/reject")]
        public ActionResult<Transfer> RejectRequst(Transfer transfer)
        {
            Transfer fulfilledRequest = transferDao.RejectTransfer(transfer);
            
            if (fulfilledRequest.status_Id == 3)
            {
                return Accepted(fulfilledRequest);
            }
            return BadRequest(fulfilledRequest);
        }

        [HttpGet("{userId}")]
        public ActionResult<IList<Transfer>> GetTransferHistory(int userId)
        {
            return transferDao.ListAllTransfers(userId);
        }

        [HttpGet("{userId}/{transferId}")]
        public ActionResult<Transfer> GetTransferById(int userId, int transferId)
        {
            Transfer transfer = transferDao.GetTransferById(userId,transferId);
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
