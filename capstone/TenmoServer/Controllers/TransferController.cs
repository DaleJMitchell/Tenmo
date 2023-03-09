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
        public TransferController() 
        { 
        
        }

        [HttpPost]
        public ActionResult<Transfer> SendMoney(Transfer transfer)
        {
            Transfer newTransfer = transferDao.SendMoney(transfer);
            if (newTransfer.status == "accepted")
            {
                return Accepted(newTransfer);
            }
            return BadRequest(newTransfer);
        }

        [HttpPost("request")]
        public ActionResult<Transfer> MakeRequest(Transfer transfer)
        {
            Transfer newRequest = transferDao.MakeRequest(transfer);
            if (newRequest.status == "pending")
            {
                return Created(newRequest);
            }
            return BadRequest(newRequest);
        }

        [HttpPut("request/fulfill")]
        public ActionResult<Transfer> FulfillRequst(Transfer transfer)
        {
            Transfer fulfilledRequest = transferDao.FulfillRequest(transfer);
            if (fulfilledRequest == null)
            {
                return NotFound();
            }
            if (fulfilledRequest.status == "approved")
            {
                return Accepted(fulfilledRequest);
            }
            return BadRequest(fulfilledRequest);
        }

        [HttpPut("request/reject")]
        public ActionResult<Transfer> RejectRequst(Transfer transfer)
        {
            Transfer fulfilledRequest = transferDao.RejectRequest(transfer);
            if (fulfilledRequest == null)
            {
                return NotFound();
            }
            if (fulfilledRequest.status == "rejected")
            {
                return Accepted(fulfilledRequest);
            }
            return BadRequest(fulfilledRequest);
        }

        [HttpGet("{userId}")]
        public ActionResult<IList<Transfer>> GetTransferHistory(int userId)
        {
            return transferDao.ListAllTransfer(userId);
        }

        [HttpGet("{userId}/{transferId}")]
        public ActionResult<Transfer> GetTransferById(int userId, int transferId)
        {
            Transfer transfer = transferDao.GetTransferById(transferId);
            if (transfer != null)
            {
                return Ok(transfer);
            }
            else
            {
                return NotFound();
            }
        }

        //To do: add status codes
        [HttpGet("request/{userId}")]
        public ActionResult<List<Transfer>> GetRequests()
        {
            return transferDao.GetRequests();
        }
    }
}
