namespace B2C_DRP.WebApi.Models
{
    using Microsoft.Azure.Cosmos.Table;

    public class IdentityEntity : TableEntity
    {
        public IdentityEntity()
        {
        }

        public IdentityEntity(string signInName)
        {
            PartitionKey = "Account";
            RowKey = signInName;
        }

        public string signInName { get; set; }
        public string password { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string surName { get; set; }
    }
}