using TenmoServer.DAO;
using System.Collections.Generic;
    using TenmoServer.Models;

namespace TenmoServer.DAO

{
    public interface ITransferDao
    {
        Transfer SendMoney(Transfer newTransfer);

    }
}
