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

        [HttpGet]
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

        [HttpGet("request")]
        public ActionResult<Transfer> GetRequests()
        {
            return transferDao.GetRequests();
        }
    }
}
