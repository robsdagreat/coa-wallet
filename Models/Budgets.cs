namespace coa_Wallet.Models
{
public class Budget

    {
        public int Id { get; set; }
        public decimal Limit { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
    }
}   