namespace RssService.Business.Services
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IRssService
    {
        [OperationContract]
        bool HasOrganization(string organizationId);
        [OperationContract]
        bool AddOrganization(string organizationId);

        [OperationContract]
        bool AddRss(string organizationId, string rss);
        [OperationContract]
        bool RemoveRss(string organizationId, string rss);
        [OperationContract]
        bool GetRsses(string organizationId, string rss);

        [OperationContract]
        bool HasRssForOrganization(string organizationId, string rss);
        [OperationContract]
        bool HasDistinctRss(string keyword);

        [OperationContract]
        bool Run();
    }
}