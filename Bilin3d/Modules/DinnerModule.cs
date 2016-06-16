﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Responses.Negotiation;
using Nancy.Security;
using Bilin3d.Models;
using PagedList;
using Bilin3d.Helpers;
using Nancy.RouteHelpers;
using Nancy.ModelBinding;
using Nancy.Validation;
using System.ComponentModel;

namespace Bilin3d.Modules
{
    public class DinnerModule : BaseModule
    {
        private readonly IDocumentSession documentSession;
        private const int PageSize = 25;

        public DinnerModule(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
            const string basePath = "/dinners";

            Get[basePath] = Dinners;
            Get[basePath + "/page/{pagenumber}"] = Dinners;

            Get[basePath + "/{id}"] = parameters =>
            {
                if (!parameters.id.HasValue && String.IsNullOrWhiteSpace(parameters.id))
                {
                    return 404;
                }

                Dinner dinner = documentSession.Load<Dinner>((int)parameters.id);

                if (dinner == null)
                {
                    return 404;
                }

                base.Page.Title = dinner.Title;
                base.Model.Dinner = dinner;

                return View["Dinners/Details", base.Model];
            };
        }

        private Negotiator Dinners(dynamic parameters)
        {
            base.Page.Title = "Upcoming Nerd Dinners";
            IQueryable<Dinner> dinners = null;

            //Searching?
            if (this.Request.Query.q.HasValue)
            {
                string query = this.Request.Query.q;

                dinners = documentSession.Query<Dinner>().Where(d => d.Title.Contains(query)
                        || d.Description.Contains(query)
                        || d.HostedBy.Contains(query)).OrderBy(d => d.EventDate);
            }
            else
            {
                dinners = documentSession.Query<Dinner, Dinners_Index>().Where(d => d.EventDate > DateTime.Now.Date).OrderBy(x => x.EventDate);
            }

            int pageIndex = parameters.pagenumber.HasValue && !String.IsNullOrWhiteSpace(parameters.pagenumber) ? parameters.pagenumber : 1;

            base.Model.Dinners = dinners.ToPagedList(pageIndex, PageSize);

            return View["Dinners/Index", base.Model];
        }
    }

    public class DinnerModuleAuth : BaseModule
    {
        public DinnerModuleAuth(IDocumentSession documentSession)
            : base("/dinners")
        {
            this.RequiresAuthentication();

            Get["/create"] = parameters =>
            {
                Dinner dinner = new Dinner()
                {
                    EventDate = DateTime.Now.AddDays(7)
                };

                base.Page.Title = "Host a Nerd Dinner";

                base.Model.Dinner = dinner;

                return View["Create", base.Model];
            };

            Post["/create"] = parameters =>
                {
                    var dinner = this.Bind<Dinner>();
                    var result = this.Validate(dinner);

                    if (result.IsValid)
                    {
                        UserIdentity nerd = (UserIdentity)this.Context.CurrentUser;
                        dinner.HostedById = nerd.UserName;
                        dinner.HostedBy = nerd.FriendlyName;

                        RSVP rsvp = new RSVP();
                        rsvp.AttendeeNameId = nerd.UserName;
                        rsvp.AttendeeName = nerd.FriendlyName;

                        dinner.RSVPs = new List<RSVP>();
                        dinner.RSVPs.Add(rsvp);

                        documentSession.Store(dinner);
                        documentSession.SaveChanges();

                        return this.Response.AsRedirect("/dinners/" + dinner.DinnerID);
                    }
                    else
                    {
                        base.Page.Title = "Host a Nerd Dinner";
                        base.Model.Dinner = dinner;
                        foreach (var item in result.Errors)
                        {
                            foreach (var member in item.MemberNames)
                            {
                                base.Page.Errors.Add(new ErrorModel() { Name = member, ErrorMessage = item.GetMessage(member) });
                            }
                        }
                    }

                    return View["Create", base.Model];
                };

            Get["/delete/" + Route.AnyIntAtLeastOnce("id")] = parameters =>
                {
                    Dinner dinner = documentSession.Load<Dinner>((int)parameters.id);

                    if (dinner == null)
                    {
                        base.Page.Title = "Nerd Dinner Not Found";
                        return View["NotFound", base.Model];
                    }

                    if (!dinner.IsHostedBy(this.Context.CurrentUser.UserName))
                    {
                        base.Page.Title = "You Don't Own This Dinner";
                        return View["InvalidOwner", base.Model];
                    }

                    base.Page.Title = "Delete Confirmation: " + dinner.Title;

                    base.Model.Dinner = dinner;

                    return View["Delete", base.Model];
                };

            Post["/delete/" + Route.AnyIntAtLeastOnce("id")] = parameters =>
                {
                    Dinner dinner = documentSession.Load<Dinner>((int)parameters.id);

                    if (dinner == null)
                    {
                        base.Page.Title = "Nerd Dinner Not Found";
                        return View["NotFound", base.Model];
                    }

                    if (!dinner.IsHostedBy(this.Context.CurrentUser.UserName))
                    {
                        base.Page.Title = "You Don't Own This Dinner";
                        return View["InvalidOwner", base.Model];
                    }

                    documentSession.Delete(dinner);
                    documentSession.SaveChanges();

                    base.Page.Title = "Deleted";
                    return View["Deleted", base.Model];
                };

            Get["/edit" + Route.And() + Route.AnyIntAtLeastOnce("id")] = parameters =>
                {
                    Dinner dinner = documentSession.Load<Dinner>((int)parameters.id);

                    if (dinner == null)
                    {
                        base.Page.Title = "Nerd Dinner Not Found";
                        return View["NotFound", base.Model];
                    }

                    if (!dinner.IsHostedBy(this.Context.CurrentUser.UserName))
                    {
                        base.Page.Title = "You Don't Own This Dinner";
                        return View["InvalidOwner", base.Model];
                    }

                    base.Page.Title = "Edit: " + dinner.Title;
                    base.Model.Dinner = dinner;

                    return View["Edit", base.Model];
                };

            Post["/edit" + Route.And() + Route.AnyIntAtLeastOnce("id")] = parameters =>
                {
                    Dinner dinner = documentSession.Load<Dinner>((int)parameters.id);

                    if (!dinner.IsHostedBy(this.Context.CurrentUser.UserName))
                    {
                        base.Page.Title = "You Don't Own This Dinner";
                        return View["InvalidOwner", base.Model];
                    }

                    this.BindTo(dinner);

                    var result = this.Validate(dinner);

                    if (!result.IsValid)
                    {
                        base.Page.Title = "Edit: " + dinner.Title;
                        base.Model.Dinner = dinner;
                        foreach (var item in result.Errors)
                        {
                            foreach (var member in item.MemberNames)
                            {
                                base.Page.Errors.Add(new ErrorModel() { Name = member, ErrorMessage = item.GetMessage(member) });
                            }
                        }

                        return View["Edit", base.Model];
                    }

                    documentSession.SaveChanges();

                    return this.Response.AsRedirect("/" + dinner.DinnerID);

                };

            Get["/my"] = parameters =>
                {
                    string nerdName = this.Context.CurrentUser.UserName;

                    var userDinners = documentSession.Query<Dinner, Dinners_Index>()
                                    .Where(x => x.HostedById == nerdName || x.HostedBy == nerdName || x.RSVPs.Any(r => r.AttendeeNameId == nerdName || (r.AttendeeNameId == null && r.AttendeeName == nerdName)))
                                    .OrderBy(x => x.EventDate)
                                    .AsEnumerable();

                    base.Page.Title = "My Dinners";
                    base.Model.Dinners = userDinners;

                    return View["My", base.Model];
                };
        }
    }
}

