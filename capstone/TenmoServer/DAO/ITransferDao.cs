using TenmoServer.DAO;
using System.Collections.Generic;
    using TenmoServer.Models;

namespace TenmoServer.DAO

{
    public interface ITransferDao
    {
        Transfer SendMoney(Transfer newTransfer);
        //Needs to have a method called sendmoney that takes in transfer object called newTransfer. Returns transfer object

        List<Transfer> ListAllTransfers(int userId);

        Transfer GetTransferById(int id);

    }
}
