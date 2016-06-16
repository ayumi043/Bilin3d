﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bilin3d.Models;
using Nancy;

namespace Bilin3d.Modules
{
    public class ServicesModule : BaseModule
    {
        public ServicesModule(IDocumentSession documentSession)
            : base("/services")
        {
            Get["/RSS"] = parameters =>
                {
                    var dinners = documentSession.Query<Dinner, Dinners_Index>().Where(d => d.EventDate > DateTime.Now.Date).OrderBy(x => x.EventDate).AsEnumerable();

                    if (dinners == null)
                    {
                        base.Page.Title = "Nerd Dinner Not Found";
                        return View["NotFound", base.Model];
                    }

                    return this.Response.AsRSS(dinners, "Upcoming Nerd Dinners");
                };
        }
    }
}