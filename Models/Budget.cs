namespace coa_Wallet.Models
{
public class Budget

    {
        public int Id { get; set; }
        public required decimal Limit { get; set; }
        public required int AccountId { get; set; }
        public Account? Account { get; set; }
        public DateTime StartDate { get; set; }  
        public DateTime EndDate { get; set; }    
       
    }
}   