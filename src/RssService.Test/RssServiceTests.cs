namespace RssService.Test
{
    using System;
    using System.Configuration;

    using NUnit.Framework;

    using PusherServer;

    using RssService.Business.Entities;
    using RssService.Business.Repos;
    using RssService.Business.Services;

    [TestFixture]
    public class RssServiceTests
    {
        protected IEntityRepository<Organization> _organizationRepo;
        protected IEntityRepository<DistinctRssAddress> _distinctRssAddressRepo;
        protected IEntityRepository<RssAddress> _rssAddressRepo;
        protected IEntityRepository<RssItem> _rssItemRepo;

        protected Pusher _pusherServer;

        protected IRssService _rssService;

        protected const string organizationId = "51fb4622902c7f0fecca0343";
        protected const string techCrunchRss = "http://feeds.feedburner.com/techcrunch/startups?format=xml";
        protected const string sethGodinRss = "http://feeds.feedblitz.com/sethsblog&x=1";

        [SetUp]
        public void Setup()
        {
            _organizationRepo = new EntityRepository<Organization>();
            _distinctRssAddressRepo = new EntityRepository<DistinctRssAddress>();
            _rssAddressRepo = new EntityRepository<RssAddress>();
            _rssItemRepo = new EntityRepository<RssItem>();

            _pusherServer = new Pusher(ConfigurationManager.AppSettings["pusherAppId"], ConfigurationManager.AppSettings["pusherAppKey"], ConfigurationManager.AppSettings["pusherAppSecret"]);

            this.ClearCollections();


            _rssService = new RssService(_organizationRepo, _distinctRssAddressRepo, _rssAddressRepo, _rssItemRepo, _pusherServer);
        }

        private void ClearCollections()
        {
            this._organizationRepo.Clear();
            this._distinctRssAddressRepo.Clear();
            this._rssAddressRepo.Clear();
        }

        [Test]
        public void Should_save_organization_when_AddOrganization_method_called()
        {
            Assert.AreEqual(true, _rssService.AddOrganization(organizationId));
        }

        [Test]
        public void Should_check_if_organization_exists_when_HasOrganization_method_called()
        {
            _rssService.AddOrganization(organizationId);
            Assert.AreEqual(true, _rssService.HasOrganization(organizationId));
            Assert.AreEqual(false, _rssService.HasOrganization(Guid.NewGuid().ToString()));
        }

        [Test]
        public void Should_check_if_organization_has_the_rss_when_HasRssForOrganization_method_called()
        {
            _rssService.AddOrganization(organizationId);
            _rssService.AddRss(organizationId, techCrunchRss);

            Assert.AreEqual(true, _rssService.HasRssForOrganization(organizationId, techCrunchRss));
        }

        [Test]
        public void Should_check_if_has_the_rss_in_distincts_collection_when_HasDistinctRss_method_called()
        {
            _rssService.AddOrganization(organizationId);
            _rssService.AddRss(organizationId, techCrunchRss);

            Assert.AreEqual(true, _rssService.HasDistinctRss(techCrunchRss));
        }

        [Test]
        public void Should_save_rss_when_AddRss_method_called()
        {
            this.ClearCollections();
            _rssService.AddOrganization(organizationId);

            Assert.AreEqual(true, _rssService.AddRss(organizationId, techCrunchRss));
            Assert.AreEqual(true, _rssService.AddRss(organizationId, sethGodinRss));
        }

        [Test]
        public void Should_remove_rss_when_RemoveRss_method_called()
        {
            this.ClearCollections();
            _rssService.AddOrganization(organizationId);
            _rssService.AddRss(organizationId, techCrunchRss);

            Assert.AreEqual(true, _rssService.HasRssForOrganization(organizationId, techCrunchRss));
            Assert.AreEqual(true, _rssService.RemoveRss(organizationId, techCrunchRss));
            Assert.AreEqual(false, _rssService.HasRssForOrganization(organizationId, techCrunchRss));
        }

        [Test]
        public void Should_retun_rsses_of_organization_when_GetRsses_method_called()
        {
            this.ClearCollections();

            var items = _rssService.GetRsses(organizationId);
            Assert.AreEqual(0, items.Count);

            _rssService.AddOrganization(organizationId);
            _rssService.AddRss(organizationId, techCrunchRss);

            items = _rssService.GetRsses(organizationId);
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void Should_read_rss_when_ReadRss_method_called()
        {
            Assert.AreEqual(true, _rssService.ReadRss(techCrunchRss).Result);
            Assert.AreEqual(true, _rssService.ReadRss(sethGodinRss).Result);
        }

        [Test]
        public void Should_retun_rssitems_of_organization_when_GetRssItems_method_called()
        {
            this.ClearCollections();
            _rssService.AddOrganization(organizationId);
            _rssService.AddRss(organizationId, techCrunchRss);

            var items = _rssService.GetRssItems(organizationId).Result;
            Assert.Greater(0, items.Count);
        }
    }

}
