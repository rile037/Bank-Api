using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;



namespace transactionApi.Models
{
    public class User 
    {
        [Key]
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int userID { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        [JsonIgnore]
        public int balance { get; set; }
        [JsonIgnore]

        public string account { get; set; }

        [JsonIgnore]
        public string status { get; set; }

        [JsonIgnore]
        public string userRole { get; set; }


        public User()
        {
            this.userID = generateID();
            this.status = "aktivan";
            //this.password = BCrypt.Net.BCrypt.HashPassword(password);
            this.balance = 0;
            this.account = generateAccount();
            this.userRole = "";
        }


        public string generateAccount()
        {
            string prefix = "256";
            string sufix = "123";
            string numbers = GenerateRandomDigits(10);

            return prefix + numbers + sufix;
        }

        static string GenerateRandomDigits(int length)
        {
            Random random = new Random();
            string randomDigits = "";

            for (int i = 0; i < length; i++)
            {
                randomDigits += random.Next(10).ToString();
            }

            return randomDigits;
        }

        public int generateID()
        {
            Random rnd = new Random();
            var id = rnd.Next(1, 99991);
            return id;
        }

     

    }

}
