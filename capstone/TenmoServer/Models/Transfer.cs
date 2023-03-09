namespace TenmoServer.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public int type_Id { get; set; }
        public int status_Id { get; set; }
        public int account_From { get; set; }
        public int account_To { get; set;}
        public decimal amounttoTransfer { get; set; }
        

        public Transfer()
        {

        }

        public Transfer(int id, int type_Id, int status_Id, int account_From, int account_To, decimal amounttoTransfer)
        {
            this.Id = id;
            this.type_Id = type_Id;
            this.status_Id = status_Id;
            this.account_From = account_From;
            this.account_To = account_To;
            this.amounttoTransfer = amounttoTransfer;
        }
    }
}
