using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace transactionApi.Models
{
    public class Transaction
    {
        //private readonly Random rnd = new Random();

        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // gasimo auto inkrement
        public string TransactionId { get; set; }

        public string TransactionName { get; set; }

        [JsonIgnore]
        public int TransactionStatus { get; set; }

        [JsonIgnore]
        public string? transactionDescription { get; set; }

        [JsonIgnore]
        public string? transactionType { get; set; }

//        public int SenderId { get; set; } // Use PascalCase for property names
        public string senderAccount { get; set; }
  //      public int ReceiverId { get; set; } // Use PascalCase for property names
        public string receiverAccount { get; set; }
        
        public int amount { get; set;  }

        [JsonIgnore]
        public DateTime DateTime { get; set; }


        public Transaction()
        {
            TransactionId = GenerateTransactionId();
            transactionDescription = "default";
            DateTime = DateTime.Now;
        }

        public string GenerateTransactionId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[16];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            string finalString = new String(stringChars);
            return finalString;
        }

    }
}
