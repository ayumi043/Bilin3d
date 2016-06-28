using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Security;
using Bilin3d.Models;
using System.Data;
using ServiceStack.OrmLite;
using log4net;

using Nancy.Validation;

namespace Bilin3d.Modules {
    public class HomeModule : BaseModule {
        public HomeModule(IDbConnection db, ILog log) {

            Get["/"] = parameters => {             
                base.Page.Title = "首页";

                //log.Debug("hello");
                //log.Info("hello world!");
                //log.Error("xxxxx");

                //throw (new System.Exception("test"));
                return View["Index", base.Model];
            };

            Get["/about"] = parameters => {
                base.Page.Title = "关于我们";

                return View["About", base.Model];
            };

            Get["/faq"] = parameters => {
                base.Page.Title = "使用说明";

                return View["Faq", base.Model];
            };

            Get["/wiki"] = parameters => {
                base.Page.Title = "知识教育";

                return View["Wiki", base.Model];
            };

            Get["/help"] = parameters => {
                base.Page.Title = "帮助中心";

                return View["About", base.Model];
            };

            Get["/bilinadminlogin"] = parameters => {
                base.Page.Title = "管理员后台";
                return View["Admin/Login", base.Model];
            };

            Post["/bilinadminlogin"] = parameters => {
                Session["adminid"] = "abc";
                return Response.AsRedirect("/bilinadmin/");
            };

        }
    }
}