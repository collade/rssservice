namespace RssService.Test
{
    using System;

    using NUnit.Framework;

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

        protected IRssService _rssService;

        protected const string organizationId = "1";

        [SetUp]
        public void Setup()
        {
            _organizationRepo = new EntityRepository<Organization>();
            _distinctRssAddressRepo = new EntityRepository<DistinctRssAddress>();
            _rssAddressRepo = new EntityRepository<RssAddress>();
            _rssItemRepo = new EntityRepository<RssItem>();

            _organizationRepo.Clear();
            _distinctRssAddressRepo.Clear();
            _rssAddressRepo.Clear();
            _rssItemRepo.Clear();

            _rssService = new RssService(_organizationRepo, _distinctRssAddressRepo, _rssAddressRepo, _rssItemRepo);
        }

        [Test]
        public void Should_save_organization_when_addorganization_method_called()
        {
            Assert.AreEqual(true, _rssService.AddOrganization(organizationId));
        }

        [Test]
        public void Should_check_if_organization_exists_when_hasorganization_method_called()
        {
            _rssService.AddOrganization(organizationId);
            Assert.AreEqual(true, _rssService.HasOrganization(organizationId));
            Assert.AreEqual(false, _rssService.HasOrganization(Guid.NewGuid().ToString()));
        }

        [Test]
        public void Should_save_rss_when_addrss_method_called()
        {

        }
    }

}
