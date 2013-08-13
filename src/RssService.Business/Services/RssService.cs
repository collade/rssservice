namespace RssService.Business.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using MongoDB.Driver.Builders;

    using PusherServer;

    using global::RssService.Business.Entities;
    using global::RssService.Business.Repos;

    public class RssService : IRssService
    {
        private readonly IEntityRepository<Organization> organizationRepo;
        private readonly IEntityRepository<DistinctRssAddress> distinctRssAddressRepo;
        private readonly IEntityRepository<RssAddress> rssAddressRepo;
        private readonly IEntityRepository<RssItem> rssItemRepo;

        private readonly Pusher pusherServer;

        public RssService(
            IEntityRepository<Organization> organizationRepo,
            IEntityRepository<DistinctRssAddress> distinctRssAddressRepo,
            IEntityRepository<RssAddress> rssAddressRepo,
            IEntityRepository<RssItem> rssItemRepo,
            Pusher pusherServer)
        {
            this.organizationRepo = organizationRepo;
            this.distinctRssAddressRepo = distinctRssAddressRepo;
            this.rssAddressRepo = rssAddressRepo;
            this.rssItemRepo = rssItemRepo;
            this.pusherServer = pusherServer;
        }

        public bool HasOrganization(string organizationId)
        {
            return organizationRepo.AsQueryable().Any(x => x.OrganizationId == organizationId);
        }

        public bool AddOrganization(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                return false;
            }

            if (HasOrganization(organizationId))
            {
                return false;
            }

            var result = organizationRepo.Add(new Organization { CreatedBy = "System", UpdatedBy = "System", OrganizationId = organizationId });

            return result.Ok;
        }

        public bool AddRss(string organizationId, string rss)
        {
            if (string.IsNullOrEmpty(organizationId) ||
                string.IsNullOrEmpty(rss))
            {
                return false;
            }

            if (!HasOrganization(organizationId))
            {
                return false;
            }

            rss = rss.ToLowerInvariant();

            if (HasRssForOrganization(organizationId, rss))
            {
                return false;
            }

            var result =
                rssAddressRepo.Add(
                    new RssAddress
                    {
                        CreatedBy = "System",
                        UpdatedBy = "System",
                        OrganizationId = organizationId,
                        RssUrl = rss
                    });

            if (result.Ok)
            {
                if (!this.HasDistinctRss(rss))
                {
                    var result2 =
                        distinctRssAddressRepo.Add(
                            new DistinctRssAddress { CreatedBy = "System", UpdatedBy = "System", RssUrl = rss });

                    if (!result2.Ok)
                    {
                        // just trying again but will need to somehing better...
                        distinctRssAddressRepo.Add(
                           new DistinctRssAddress { CreatedBy = "System", UpdatedBy = "System", RssUrl = rss });
                    }

                    return true;
                }

                return true;
            }

            return false;
        }

        public bool RemoveRss(string organizationId, string rss)
        {
            if (string.IsNullOrEmpty(organizationId) ||
                string.IsNullOrEmpty(rss))
            {
                return false;
            }

            if (!HasOrganization(organizationId))
            {
                return false;
            }

            rss = rss.ToLowerInvariant();

            if (!HasRssForOrganization(organizationId, rss))
            {
                return false;
            }

            var result =
                rssAddressRepo.Update(
                    Query.And(
                        Query<RssAddress>.EQ(x => x.RssUrl, rss),
                        Query<RssAddress>.EQ(x => x.OrganizationId, organizationId)),
                    Update<RssAddress>.Set(x => x.IsDeleted, true)
                                   .Set(x => x.DeletedAt, DateTime.Now)
                                   .Set(x => x.DeletedBy, "System"));

            if (result.Ok)
            {
                if (!rssAddressRepo.AsQueryable().Any(x => x.RssUrl == rss))
                {
                    var result2 =
                        distinctRssAddressRepo.Update(
                            Query.And(Query<DistinctRssAddress>.EQ(x => x.RssUrl, rss)),
                            Update<DistinctRssAddress>.Set(x => x.IsDeleted, true)
                                                   .Set(x => x.DeletedAt, DateTime.Now)
                                                   .Set(x => x.DeletedBy, "System"));

                    if (!result2.Ok)
                    {
                        //another something better needed place...
                        distinctRssAddressRepo.Update(
                            Query.And(Query<DistinctRssAddress>.EQ(x => x.RssUrl, rss)),
                            Update<DistinctRssAddress>.Set(x => x.IsDeleted, true)
                                                   .Set(x => x.DeletedAt, DateTime.Now)
                                                   .Set(x => x.DeletedBy, "System"));
                    }
                }

                return true;
            }

            return false;
        }

        public List<string> GetRsses(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                return new List<string>();
            }

            if (!HasOrganization(organizationId))
            {
                return new List<string>();
            }

            if (rssAddressRepo.AsQueryable().Any(x => x.OrganizationId == organizationId))
            {
                return
                    rssAddressRepo.AsQueryable()
                                     .Where(x => x.OrganizationId == organizationId)
                                     .Select(x => x.RssUrl)
                                     .ToList();
            }

            return new List<string>();
        }

        public bool HasRssForOrganization(string organizationId, string rss)
        {
            return rssAddressRepo.AsQueryable().Any(x => x.OrganizationId == organizationId && x.RssUrl == rss);
        }

        public bool HasDistinctRss(string rss)
        {
            return distinctRssAddressRepo.AsQueryable().Any(x => x.RssUrl == rss);
        }

        public bool Run()
        {
            if (!this.HasOrganization("51fb4622902c7f0fecca0343"))
            {
                this.AddOrganization("51fb4622902c7f0fecca0343");

                const string TechCrunchRss = "http://feeds.feedburner.com/techcrunch/startups?format=xml";
                const string SethGodinRss = "http://feeds.feedblitz.com/sethsblog&x=1";

                this.AddRss("51fb4622902c7f0fecca0343", TechCrunchRss);
                this.AddRss("51fb4622902c7f0fecca0343", SethGodinRss);
            }

            var oneMinTimer = new Timer(this.TimerTick, null, 0, 60000);

            return true;
        }

        private void TimerTick(object state)
        {
            var rsses = distinctRssAddressRepo.AsQueryable().ToList();
            foreach (var distinctRssAddress in rsses)
            {
                this.ReadRss(distinctRssAddress.RssUrl);
            }
        }

        public Task<bool> ReadRss(string rss)
        {
            try
            {
                var reader = XmlReader.Create(rss);
                var feed = SyndicationFeed.Load(reader);

                if (feed != null)
                {
                    foreach (var item in feed.Items)
                    {
                        if (item.Links.Any())
                        {
                            var link = item.Links.First().Uri.AbsoluteUri;
                            var title = item.Title.Text;
                            var content = item.Summary.Text;
                            var summary = string.Format("{0} ...", Regex.Replace(item.Summary.Text, "<.*?>", string.Empty).Substring(0, 222));
                            var time2 = item.PublishDate.DateTime.ToString("dd MMMM dddd - HH:mm", CultureInfo.InvariantCulture);

                            if (!rssItemRepo.AsQueryable().Any(x => x.RssId == item.Id))
                            {
                                rssItemRepo.Add(new RssItem
                                {
                                    CreatedBy = "System",
                                    UpdatedBy = "System",

                                    CreatedAt = item.PublishDate.DateTime,
                                    RssUrl = rss,
                                    RssId = item.Id,
                                    Link = link,
                                    Title = title,
                                    Content = content,
                                    Summary = summary
                                });

                                //find who to notify
                                var organizatons =
                                    rssAddressRepo.AsQueryable()
                                                  .Where(x => x.RssUrl == rss)
                                                  .Select(x => x.OrganizationId)
                                                  .ToList();

                                foreach (var organizaton in organizatons)
                                {
                                    //notiy with pusher
                                    var orgId = organizaton;
                                    ThreadPool.QueueUserWorkItem(m =>
                                        pusherServer.Trigger(string.Format("presence-{0}", orgId), "rssitem_added", new { link, title, summary, time2 }));
                                }
                            }
                        }
                    }

                    return Task.FromResult(true);
                }
            }
            catch (Exception)
            { }

            return Task.FromResult(false);
        }

        public Task<List<RssItemDto>> GetRssItems(string organizationId)
        {

            var result = new List<RssItemDto>();

            if (string.IsNullOrEmpty(organizationId))
            {
                return Task.FromResult(result);
            }

            if (!HasOrganization(organizationId))
            {
                return Task.FromResult(result);
            }

            var rsses = this.GetRsses(organizationId);
            foreach (var rss in rsses)
            {
                if (rssItemRepo.AsQueryable().Any(x => x.RssUrl == rss))
                {
                    string rss1 = rss;
                    var items = rssItemRepo.AsQueryable().Where(x => x.RssUrl == rss1).ToList();
                    foreach (var rssItem in items)
                    {
                        result.Add(new RssItemDto
                        {
                            Link = rssItem.Link,
                            Title = rssItem.Title,
                            Content = rssItem.Summary,
                            Time = rssItem.CreatedAt.ToString("dd MMMM dddd - HH:mm", CultureInfo.InvariantCulture),
                            CreatedAt = rssItem.CreatedAt
                        });
                    }
                }
            }

            return Task.FromResult(result);
        }
    }
}