using TenmoServer.DAO;
using System.Collections.Generic;
    using TenmoServer.Models;

namespace TenmoServer.DAO

{
    public interface ITransferDao
    {
        Transfer CreateTransfer(Transfer transfer);

        Transfer RejectTransfer(Transfer transfer);

        Transfer AcceptTransfer(Transfer transfer);

        List<Transfer> ListAllTransfers(int userId);

        Transfer GetTransferById(int userId, int transferId);

        Transfer FulfillRequest(Transfer transfer);

    }
}
