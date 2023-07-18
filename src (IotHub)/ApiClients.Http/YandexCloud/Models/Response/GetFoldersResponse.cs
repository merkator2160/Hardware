namespace ApiClients.Http.YandexCloud.Models.Response
{
    public class GetFoldersResponse
    {
        public CloudFolders[] Folders { get; set; }
    }
    public class CloudFolders
    {
        public String Id { get; set; }
        public String CloudId { get; set; }
        public DateTime CreatedAt { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Status { get; set; }
    }
}