﻿namespace RssService.Business.Services
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using global::RssService.Business.Entities;

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
        List<string> GetRsses(string organizationId);

        [OperationContract]
        bool HasRssForOrganization(string organizationId, string rss);
        [OperationContract]
        bool HasDistinctRss(string rss);

        [OperationContract]
        void Run();

        [OperationContract]
        Task<bool> ReadRss(string rss);

        [OperationContract]
        Task<List<RssItemDto>> GetRssItems(string organizationId);
    }
}