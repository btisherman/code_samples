//-----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//    Copyright (c) 2010-2013 Microsoft Corporation. 
//
//    You must not remove this notice, or any other, from this software.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Research.Yakima.DataModel;
using Microsoft.Research.Yakima.Repository.Interfaces;
using Microsoft.Research.Yakima.Website.Attributes;
using Microsoft.Research.Yakima.Website.Extensions;
using Microsoft.Research.Yakima.Website.Models;
using Microsoft.Research.Yakima.Website.Models.Pager;
using Microsoft.Research.Yakima.Website.Resources;
using Microsoft.Research.Yakima.Website.Services.Interfaces;
using Author = Microsoft.Research.Yakima.Website.Models.Author;

namespace Microsoft.Research.Yakima.Website.Controllers
{
    public class AuthorController : AbstractAsyncController
    {
        private const int DefaultPageSize = 25;

        private const int MaxCoAuthorsToDisplay = 3;

        private const string ControllerName = "Author";
        private const string CoauthorsActionName = "Coauthors";
        private const string CoauthorsPartialListActionName = "CoauthorsList";
        private const string IndexActionName = "Index";
        private const string AuthorPublicationPartialListActionName = "paperslist";
        private const string CoauthorPublicationListActionName = "coauthorpapers";
        private const string CoauthorPublicationPartialListActionName = "coauthorpaperslist";
        private const string SelectablePublicationAction = "selectablepublications";

        private readonly IEntityService entityService;
        private readonly IUserEditService userEditService;

        public AuthorController(IEntityService entityService, IUserEditService userEditService, IConfigurationRepository configurationRepository) :
            base(configurationRepository)
        {
            this.entityService = entityService;
            this.userEditService = userEditService;
        }

        #region Index Action

        public void IndexAsync(string id)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetPublicationsForAuthor(id, SortOrder.Default, 0, 5), "publications");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetCoauthorsForAuthor(id, 0, 3), "coauthors");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetJournalsForAuthor(id, 0, 3), "journals");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetConferencesForAuthor(id, 0, 3), "conferences");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetPublicationCountsPerYearForAuthor(id), "publicationCount");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult IndexCompleted(
            DataModel.Author author,
            string id,
            PagedListNumeric<PublicationBase> publications,
            PagedListNumeric<Coauthor> coauthors,
            PagedListNumeric<AuthorVenue> journals,
            PagedListNumeric<AuthorVenue> conferences,
            AuthorPublicationCountPerYear publicationCount)
        {
            if (author == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, id);
            }

            var model = new Author(author, publications, publicationCount);

            // Add top 3 coauthors
            int coauthCount = coauthors != null ? coauthors.TotalItemCount : 0;
            int maxCoAuthorsToDisplay = coauthors == null || coauthors.Items == null
                                            ? 0
                                            : coauthors.Items.Count > MaxCoAuthorsToDisplay
                                                  ? MaxCoAuthorsToDisplay
                                                  : coauthors.Items.Count;

            var authCoauthors = new RelatedItems
                                    {
                                        Id = model.AuthorModel.Id,
                                        Title = AuthorStrings.SectionHeading_Coauthors,
                                        Count = coauthCount,
                                        ShowCount = true,
                                        Items = new List<RelatedItem>(),
                                        ActionName = CoauthorsActionName,
                                        ControllerName = ControllerName,
                                        RouteValues = new RouteValueDictionary()
                                    };

            authCoauthors.RouteValues.Add("id", model.AuthorModel.Id);

            if (coauthors != null && coauthors.Items != null)
            {
                for (int i = 0; i < maxCoAuthorsToDisplay; i++)
                {
                    var coauthor = coauthors.Items[i];
                    authCoauthors.Items.Add(
                        new RelatedItem
                            {
                                Id = coauthor.Author.Id,
                                Name = coauthor.Author.Name,
                                ImagePath = string.IsNullOrEmpty(coauthor.Author.PhotoUrl) ? "/Content/Images/IconAuthorMed.png" : coauthor.Author.PhotoUrl,
                                RouteValues = new RouteValueDictionary { { "id", id }, { "coauthorid", coauthor.Author.Id } },
                                ActionName = IndexActionName,
                                ControllerName = ControllerName,
                                Count = coauthor.PublicationCount,
                                ClassName = "color6 authorlink",
                                ListActionName = CoauthorPublicationListActionName
                            });
                }
            }

            model.RelatedCoauthors = authCoauthors;

            return View("Index", model);
        }
        #endregion

        #region Coauthors Action
        public void CoauthorsAsync(string id, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    AsyncManager.ContinueWithAndStart(this.entityService.GetCoauthorsForAuthor(id, pageNumber ?? 0, pageSize ?? DefaultPageSize), "result");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult CoauthorsCompleted(PagedListNumeric<Coauthor> result, DataModel.Author author, string id)
        {
            AuthorList authorList = null;

            PagedListNumeric<AuthorListItem> authors = result == null
                                                           ? null
                                                           : new PagedListNumeric<AuthorListItem>(
                                                                 result.Items.Select(c => new AuthorListItem
                                                                         {
                                                                             Author = c.Author,
                                                                             PublicationCount = c.PublicationCount,
                                                                             RouteValues = new RouteValueDictionary { { "id", id }, { "coauthorid", c.Author.Id } },
                                                                             PublicationListActionName = CoauthorPublicationListActionName,
                                                                             PublicationListControllerName = ControllerName
                                                                         }),
                                                                 result.PageNumber,
                                                                 result.PageSize,
                                                                 result.TotalItemCount,
                                                                 5);

            if (author != null && authors != null)
            {
                authorList = new AuthorList
                                 {
                                     Id = id,
                                     Name = author.Name,
                                     Title = AuthorStrings.Heading_AllCoauthors,
                                     Authors = authors,
                                     Count = result.TotalItemCount,
                                     ActionName = IndexActionName,
                                     RouteValues = new RouteValueDictionary { { "id", id }, { "pageNumber", authors.NextPageId } },
                                     PartialListActionName = CoauthorsPartialListActionName,
                                     ControllerName = ControllerName
                                 };
            }

            return View("CoAuthorsView", authorList);
        }

        public void CoauthorsListAsync(string id, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    AsyncManager.ContinueWithAndStart(
                        this.entityService.GetCoauthorsForAuthor(id, pageNumber ?? 0, pageSize ?? DefaultPageSize),
                        "result");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult CoauthorsListCompleted(
            PagedListNumeric<Coauthor> result, DataModel.Author author, string id)
        {
            AuthorList authorList = null;

            PagedListNumeric<AuthorListItem> authors = result == null
                                                           ? null
                                                           : new PagedListNumeric<AuthorListItem>(
                                                                 result.Items.Select(c => new AuthorListItem
                                                                 {
                                                                     Author = c.Author,
                                                                     PublicationCount = c.PublicationCount,
                                                                     RouteValues = new RouteValueDictionary { { "id", id }, { "coauthorid", c.Author.Id } },
                                                                     PublicationListActionName = CoauthorPublicationListActionName,
                                                                     PublicationListControllerName = ControllerName
                                                                 }),
                                                                 result.PageNumber,
                                                                 result.PageSize,
                                                                 result.TotalItemCount,
                                                                 5);

            if (author != null && authors != null)
            {
                authorList = new AuthorList
                {
                    Id = id,
                    Name = author.Name,
                    Title = AuthorStrings.Title_Authors_Coauthors_Listpage,
                    Authors = authors,
                    Count = result.TotalItemCount,
                    RouteValues = new RouteValueDictionary { { "id", id }, { "pageNumber", authors.NextPageId } },
                    ActionName = IndexActionName,
                    PartialListActionName = CoauthorsPartialListActionName,
                    ControllerName = ControllerName
                };
            }

            return this.PartialView("AuthorsListPartial", authorList);
        }
        #endregion

        #region Papers Action
        public void PapersAsync(string id, SortOrder? order, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    order = order ?? SortOrder.Default;
                    AsyncManager.SynchronizedSetParameter("order", order);
                    AsyncManager.ContinueWithAndStart(
                        this.entityService.GetPublicationsForAuthor(
                            id, order.Value, pageNumber ?? 0, pageSize ?? DefaultPageSize),
                        "result");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult PapersCompleted(
            PagedListNumeric<PublicationBase> result, DataModel.Author author, string id, SortOrder order)
        {
            PublicationList authorPapers = null;

            if (author != null)
            {
                authorPapers = new PublicationList
                {
                    Id = id,
                    Name = author.Name,
                    Title = AuthorStrings.Heading_AllPublications,
                    Order = order,
                    ActionName = IndexActionName,
                    ControllerName = ControllerName,
                    PartialListActionName = AuthorPublicationPartialListActionName,
                    Count = result != null ? result.TotalItemCount : 0,
                    Publications = result,
                };
            }

            return View("PublicationsView", authorPapers);
        }

        public void PapersListAsync(string id, SortOrder? order, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    order = order ?? SortOrder.Default;
                    AsyncManager.SynchronizedSetParameter("order", order);
                    AsyncManager.ContinueWithAndStart(
                        this.entityService.GetPublicationsForAuthor(
                            id, order.Value, pageNumber ?? 0, pageSize ?? DefaultPageSize),
                        "result");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult PapersListCompleted(
            PagedListNumeric<PublicationBase> result, DataModel.Author author, string id, SortOrder order)
        {
            PublicationList authorPapers = null;

            if (author != null)
            {
                authorPapers = new PublicationList
                {
                    Id = id,
                    Name = author.Name,
                    Title = AuthorStrings.Title_Authors_Publications_Listpage,
                    Order = order,
                    ActionName = IndexActionName,
                    ControllerName = ControllerName,
                    PartialListActionName = AuthorPublicationPartialListActionName,
                    Count = result != null ? result.TotalItemCount : 0,
                    Publications = result,
                };
            }

            return this.PartialView("PublicationsListPartial", authorPapers);
        }
        #endregion

        #region CoauthorPapers Action
        public void CoauthorPapersAsync(string id, string coauthorid, SortOrder? order, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(coauthorid))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    order = order ?? SortOrder.Default;
                    AsyncManager.SynchronizedSetParameter("order", order);
                    AsyncManager.ContinueWithAndStart(
                        this.entityService.GetPublicationsForAuthorCoauthor(
                            id, coauthorid, order.Value, pageNumber ?? 0, pageSize ?? DefaultPageSize),
                        "result");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(coauthorid), "coauthor");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult CoauthorPapersCompleted(
            PagedListNumeric<PublicationBase> result, DataModel.Author author, DataModel.Author coauthor, string id, SortOrder order)
        {
            PublicationList authorPapers = null;

            if (author != null && coauthor != null)
            {
                authorPapers = new PublicationList
                {
                    Id = id,
                    Name = author.Name,
                    Title = string.Format(CultureInfo.CurrentUICulture, AuthorStrings.Title_Author_Coauthor, coauthor.Name),
                    Order = order,
                    ActionName = IndexActionName,
                    ControllerName = ControllerName,
                    PartialListActionName = CoauthorPublicationPartialListActionName,
                    Count = result != null ? result.TotalItemCount : 0,
                    Publications = result,
                };
            }

            return View("PublicationsView", authorPapers);
        }

        public void CoauthorPapersListAsync(string id, string coauthorid, SortOrder? order, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(coauthorid))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    order = order ?? SortOrder.Default;
                    AsyncManager.SynchronizedSetParameter("order", order);
                    AsyncManager.ContinueWithAndStart(
                        this.entityService.GetPublicationsForAuthorCoauthor(
                            id, coauthorid, order.Value, pageNumber ?? 0, pageSize ?? DefaultPageSize),
                        "result");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(coauthorid), "coauthor");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult CoauthorPapersListCompleted(
            PagedListNumeric<PublicationBase> result, DataModel.Author author, DataModel.Author coauthor, string id, SortOrder order)
        {
            PublicationList authorPapers = null;

            if (author != null && coauthor != null)
            {
                authorPapers = new PublicationList
                {
                    Id = id,
                    Name = author.Name,
                    Title = string.Format(CultureInfo.CurrentUICulture, AuthorStrings.Title_Author_Coauthor, coauthor.Name),
                    Order = order,
                    ActionName = IndexActionName,
                    ControllerName = ControllerName,
                    PartialListActionName = CoauthorPublicationPartialListActionName,
                    Count = result != null ? result.TotalItemCount : 0,
                    Publications = result,
                };
            }

            return this.PartialView("PublicationsListPartial", authorPapers);
        }
        #endregion

        #region SelectablePublications Action
        public void SelectablePublicationsAsync(string id, SortOrder? order, int? pageNumber, int? pageSize)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    order = order ?? SortOrder.Default;
                    AsyncManager.SynchronizedSetParameter("order", order);
                    AsyncManager.ContinueWithAndStart(
                        this.entityService.GetPublicationsForAuthor(
                            id, order.Value, pageNumber ?? 0, pageSize ?? DefaultPageSize),
                        "result");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult SelectablePublicationsCompleted(
            PagedListNumeric<PublicationBase> result, DataModel.Author author, string id, SortOrder order)
        {
            PublicationList authorPapers = null;

            if (author != null)
            {
                authorPapers = new PublicationList
                {
                    Id = id,
                    Name = author.Name,
                    Order = order,
                    Count = result.TotalItemCount,
                    Publications = result,
                    ControllerName = ControllerName,
                    PartialListActionName = SelectablePublicationAction,
                    PartialListActionRouteValues = new RouteValueDictionary { { "id", id }, { "pageNumber", result.NextPageId } }
                };
            }

            return this.PartialView("SelectablePublicationsPartial", authorPapers);
        }
        #endregion

        #region Update Actions
        public void AddPapersAsync(string id)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult AddPapersCompleted(DataModel.Author author, string id)
        {
            if (author == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, id);
            }

            return this.PartialView("AddPublications", new Author(author, null, null));
        }

        public void RemovePapersAsync(string id)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                    AsyncManager.ContinueWithAndStart(this.entityService.GetPublicationsForAuthor(id, SortOrder.Default, 0, DefaultPageSize), "result");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult RemovePapersCompleted(string id, DataModel.Author author, PagedListNumeric<PublicationBase> result)
        {
            if (author == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, id);
            }

            return this.PartialView("RemovePublications", new Author(author, result, null));
        }

        [HttpPost]
        [YakimaAuthorize]
        public void UpdatePapersAsync(string id, string paperIds, UpdateType? action)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(paperIds))
                {
                    List<string> paperIdsList = paperIds.Split(',').ToList();
                    AsyncManager.SynchronizedSetParameter("id", id);
                    AsyncManager.ContinueWithAndStart(
                        this.userEditService.UpdateAuthorPapers(id, paperIdsList, action ?? UpdateType.Add), "result");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult UpdatePapersCompleted(bool result, string id)
        {
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void PopulateProfileAsync(string id)
        {
            AsyncManager.Begin();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    AsyncManager.SynchronizedSetParameter("id", id);
                    AsyncManager.ContinueWithAndStart(this.entityService.GetAuthor(id), "author");
                }
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult PopulateProfileCompleted(DataModel.Author author, string id)
        {
            if (author == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, id);
            }

            return this.PartialView("EditProfile", new Author(author, null, null));
        }

        [HttpPost]
        [ValidateInput(false)]
        [YakimaAuthorize]
        public void EditProfileAsync(
            string id,
            string authorName,
            string imageUrl,
            string displayName,
            string organizationId,
            string organization,
            string bio,
            string linkedInUrl,
            string wikiUrl,
            string twitterUrl,
            string genericUrl)
        {
            AsyncManager.Begin();
            try
            {
                var author = new DataModel.Author
                                 {
                                     Id = id,
                                     Biography = bio,
                                     Name = authorName,
                                     NativeName = displayName,
                                     PhotoUrl = imageUrl,
                                     Websites = new List<DataModel.Website>()
                                 };

                if (!string.IsNullOrEmpty(organizationId) && organizationId != "0")
                {
                    author.Organization = new EntityBase { Id = organizationId, Name = organization };
                }

                if (!string.IsNullOrEmpty(linkedInUrl))
                {
                    author.Websites.Add(new DataModel.Website { Url = linkedInUrl, WebsiteType = WebsiteType.LinkedIn });
                }

                if (!string.IsNullOrEmpty(wikiUrl))
                {
                    author.Websites.Add(new DataModel.Website { Url = wikiUrl, WebsiteType = WebsiteType.Wikipedia });
                }

                if (!string.IsNullOrEmpty(twitterUrl))
                {
                    author.Websites.Add(new DataModel.Website { Url = twitterUrl, WebsiteType = WebsiteType.Twitter });
                }

                if (!string.IsNullOrEmpty(genericUrl))
                {
                    author.Websites.Add(new DataModel.Website { Url = genericUrl, WebsiteType = WebsiteType.Generic });
                }

                AsyncManager.ContinueWithAndStart(this.userEditService.UpdateAuthor(author), "result");
            }
            finally
            {
                AsyncManager.End();
            }
        }

        public ActionResult EditProfileCompleted(bool result)
        {
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
