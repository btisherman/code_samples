//-----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//    Copyright (c) 2010-2013 Microsoft Corporation. 
//
//    You must not remove this notice, or any other, from this software.
// </copyright>
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Research.Yakima.DataModel;
using Microsoft.Research.Yakima.Repository.Interfaces;
using Microsoft.Research.Yakima.Website.Extensions;
using Microsoft.Research.Yakima.Website.Models;
using Microsoft.Research.Yakima.Website.Models.Pager;
using Microsoft.Research.Yakima.Website.Services.Interfaces;
using Topic = Microsoft.Research.Yakima.Website.Models.Topic;

namespace Microsoft.Research.Yakima.Website.Controllers
{
    public class BrowseController : AbstractAsyncController
    {
        private const int DefaultPageSize = 25;

        private readonly IEntityService entityService;

        public BrowseController(IEntityService entityService, IConfigurationRepository configurationRepository) :
            base(configurationRepository)
        {
            this.entityService = entityService;
        }

        public void IndexAsync(string id, BrowseView view = BrowseView.Topics, SortOrder sortOrder = SortOrder.Default, int pageNumber = 0, int pageSize = 25)
        {
            AsyncManager.Begin();
            try
            {
                string currentId = id ?? "0";
                AsyncManager.SynchronizedSetParameter("view", view);
                AsyncManager.ContinueWithAndStart(this.entityService.GetTopic(currentId), "topic");
                AsyncManager.ContinueWithAndStart(this.entityService.GetTopicBrowseTree(currentId), "topicBrowseTree");
                AsyncManager.ContinueWithAndStart(this.entityService.GetTopicAncestry(currentId), "topicAncestry");
                AsyncManager.ContinueWithAndStart(this.entityService.GetPublicationsForTopic(currentId, sortOrder, pageNumber, pageSize), "topicPublicationList");
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult IndexCompleted(
            DataModel.Topic topic,
            TopicBrowseNode topicBrowseTree, 
            List<DataModel.Topic> topicAncestry, 
            PagedListNumeric<PublicationBase> topicPublicationList,
            BrowseView view)
        {
            return this.View(new Browse { CurrentTopic = new Topic { TopicModel = topic }, RootTopic = topicBrowseTree, TopicAncestry = topicAncestry, CurrentView = view, Publications = topicPublicationList });
        }

        public ActionResult Tools()
        {
            return View();
        }

        public void TopicAsync(string id)
        {
            AsyncManager.Begin();
            try
            {
                AsyncManager.ContinueWithAndStart(this.entityService.GetTopic(id ?? "0"), "result");
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult TopicCompleted(DataModel.Topic result)
        {
            var model = new Topic { TopicModel = result, SortOrder = SortOrder.Default };
            return this.PartialView("Topic", model);
        }

        public void TopicTreeAsync(string id)
        {
            AsyncManager.Begin();
            try
            {
                AsyncManager.ContinueWithAndStart(this.entityService.GetTopicBrowseTree(id ?? "0"), "result");
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult TopicTreeCompleted(TopicBrowseNode result)
        {
            return PartialView("TopicTree", result);
        }

        public void TopTrayAsync(string id, BrowseView? viewClicked)
        {
            AsyncManager.Begin();
            try
            {
                AsyncManager.ContinueWithAndStart(this.entityService.GetTopicAncestry(id ?? "0"), "result");
                AsyncManager.SynchronizedSetParameter("viewClicked", viewClicked ?? BrowseView.Topics);
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult TopTrayCompleted(List<DataModel.Topic> result, BrowseView viewClicked)
        {
            return PartialView("TopTray", new TopTray { Topics = result, ViewClicked = viewClicked });
        }

        public void ListViewAsync(string id, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                AsyncManager.ContinueWithAndStart(this.entityService.GetPublicationsForTopic(id, SortOrder.Default, pageNumber ?? 0, pageSize ?? DefaultPageSize), "result");
                AsyncManager.ContinueWithAndStart(this.entityService.GetTopic(id ?? "0"), "topic");
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult ListViewCompleted(PagedListNumeric<PublicationBase> result, DataModel.Topic topic)
        {
            var topicPublications = new TopicPublications { TopicModel = topic, Publications = result };
            return PartialView("ListView", topicPublications);
        }
    }
}