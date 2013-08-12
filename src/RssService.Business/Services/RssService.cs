﻿namespace RssService.Business.Services
{
    using global::RssService.Business.Entities;
    using global::RssService.Business.Repos;

    public class RssService : IRssService
    {
        private readonly IEntityRepository<Organization> organizationRepo;
        private readonly IEntityRepository<DistinctRssAddress> distinctRssAddressRepo;
        private readonly IEntityRepository<RssAddress> rssAddressRepo;
        private readonly IEntityRepository<RssItem> rssItemRepo;

        public RssService(
            IEntityRepository<Organization> organizationRepo,
            IEntityRepository<DistinctRssAddress> distinctRssAddressRepo,
            IEntityRepository<RssAddress> rssAddressRepo,
            IEntityRepository<RssItem> rssItemRepo)
        {
            this.organizationRepo = organizationRepo;
            this.distinctRssAddressRepo = distinctRssAddressRepo;
            this.rssAddressRepo = rssAddressRepo;
            this.rssItemRepo = rssItemRepo;
        }

        public bool HasOrganization(string organizationId)
        {
            throw new System.NotImplementedException();
        }

        public bool AddOrganization(string organizationId)
        {
            throw new System.NotImplementedException();
        }

        public bool AddRss(string organizationId, string rss)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveRss(string organizationId, string rss)
        {
            throw new System.NotImplementedException();
        }

        public bool GetRsses(string organizationId, string rss)
        {
            throw new System.NotImplementedException();
        }

        public bool HasRssForOrganization(string organizationId, string rss)
        {
            throw new System.NotImplementedException();
        }

        public bool HasDistinctRss(string keyword)
        {
            throw new System.NotImplementedException();
        }

        public bool Run()
        {
            throw new System.NotImplementedException();
        }
    }
}