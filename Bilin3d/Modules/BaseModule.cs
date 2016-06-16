using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using System.Dynamic;
using Bilin3d.Models;
using System.Data;
using ServiceStack.OrmLite;
using System.Configuration;

namespace Bilin3d.Modules {
    public class BaseModule : NancyModule {

        public dynamic Model = new ExpandoObject();

        protected PageModel Page { get; set; }

        public BaseModule()
        {
            SetupModelDefaults();
        }

        public BaseModule(string modulepath)
            : base(modulepath) {
            SetupModelDefaults();
        }

        private void SetupModelDefaults() {
            Before += ctx => {

                var UserId = "";
                if (ctx.CurrentUser != null) {
                    var dbFactory = new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["Mysql1"].ToString(), MySqlDialect.Provider);
                    var db = dbFactory.OpenDbConnection();
                    var userRecord = db.Select<UserModel>(q => q.Email == ctx.CurrentUser.UserName).FirstOrDefault();
                    UserId = userRecord.Id.ToString();
                }

                Page = new PageModel() {
                    IsAuthenticated = ctx.CurrentUser != null,
                    PreFixTitle = "Bilin 3D - ",
                    CurrentUser = ctx.CurrentUser != null ? ctx.CurrentUser.UserName : "",
                    UserId = UserId,
                    Errors = new List<ErrorModel>()
                };

                Model.Page = Page;

                return null;
            };
        }

    }
}