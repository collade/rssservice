using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssService.Test
{
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

        [SetUp]
        public void Setup()
        {
            _organizationRepo = new EntityRepository<Organization>();
            _distinctRssAddressRepo = new EntityRepository<DistinctRssAddress>();
            _rssAddressRepo = new EntityRepository<RssAddress>();
            _rssItemRepo = new EntityRepository<RssItem>();

            _rssService = new RssService(_organizationRepo, _distinctRssAddressRepo, _rssAddressRepo, _rssItemRepo);
        }


        [Test]
        public void Should_save_rss_when_addrss_method_called()
        {

        }
    }

}
