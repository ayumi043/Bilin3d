using log4net;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.Session;
using Nancy.TinyIoc;
using ServiceStack.OrmLite;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace Bilin3d {
    public class Bootstrapper : DefaultNancyBootstrapper {
              
        protected override void ConfigureConventions(NancyConventions conventions) {
            base.ConfigureConventions(conventions);

            //启用错误提示
            //StaticConfiguration.DisableErrorTraces = false;

            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", @"assets")
            ); 
            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("public", @"public")
            );           
        }

        // 只在启动时触发1次
        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);


            // log4net
            log4net.Config.XmlConfigurator.Configure();
            container.Register(typeof(ILog), (c, o) => LogManager.GetLogger(typeof(Bootstrapper)));


            var dbFactory = new OrmLiteConnectionFactory(
             ConfigurationManager.ConnectionStrings["Mysql1"].ToString(),  //Connection String
             MySqlDialect.Provider);
            container.Register(dbFactory, "dbFactory");
        }


        // 每次请求都会触发,一个页面会触发多次
        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context) {
            base.ConfigureRequestContainer(container, context);

            container.Register<IUserMapper, UserMapper>();
            
            // db
            var dbFactory = container.Resolve<OrmLiteConnectionFactory>("dbFactory");
            var db = dbFactory.OpenDbConnection();
            container.Register<IDbConnection>(db);   
        }


        // 只在启动时触发1次
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            base.ApplicationStartup(container, pipelines);

            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //pipelines.OnError.AddItemToEndOfPipeline((ctx, exception) => {
            //    Task tasks = new Task(() => {
            //        log.Error(exception.Message);
            //    });

            //    DefaultJsonSerializer serializer = new DefaultJsonSerializer();
            //    Response error = new JsonResponse(exception.Message, serializer);
            //    error.StatusCode = HttpStatusCode.InternalServerError;
            //    return error;
            //});
                      
      
            // Enabling sessions in Nancy
            //CookieBasedSessions.Enable(pipelines);

            //pipelines.BeforeRequest += (ctx) => {
            //    var uid = ctx.Request.Session["TempUserId"];
            //    var user = ctx.CurrentUser;
            //    if (user == null & uid == null) {
            //        //ctx.Request.Session["TempUserId"] = "temp-" + DateTime.Now.ToString("-yyyy-MM-dd-hh-mm-ss-fffff");
            //        ctx.Request.Session["TempUserId"] = "temp-" + Guid.NewGuid().ToString();
            //    }               
            //    return null;
            //    //return <null or a Response object>;
            //};
        }


        // 每次请求都会触发,一个页面会触发多次
        protected override void RequestStartup(TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context) {
            base.RequestStartup(container, pipelines, context);

            // At request startup we modify the request pipelines to
            // include forms authentication - passing in our now request
            // scoped user name mapper.
            //
            // The pipelines passed in here are specific to this request,
            // so we can add/remove/update items in them as we please.
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration() {
                    RedirectUrl = "~/account/logon",
                    UserMapper = container.Resolve<IUserMapper>(),
                };
            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);



            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //pipelines.OnError.AddItemToEndOfPipeline((ctx, exception) => {
            //    Task tasks = new Task(() => {
            //        log.Error(exception.Message);
            //    });

            //    DefaultJsonSerializer serializer = new DefaultJsonSerializer();
            //    Response error = new JsonResponse(exception.Message, serializer);
            //    error.StatusCode = HttpStatusCode.InternalServerError;
            //    return error;
            //});

                                
           
            // Enabling sessions in Nancy
            CookieBasedSessions.Enable(pipelines);

            //放RequestStartup这里是每次请求时判断session，为了避免session过期，所以不放在ApplicationStartup
            pipelines.BeforeRequest += (ctx) => {      
                var uid = ctx.Request.Session["TempUserId"];
                var user = ctx.CurrentUser;
                if (user == null && uid == null) {
                    //ctx.Request.Session["TempUserId"] = "temp-" + DateTime.Now.ToString("-yyyy-MM-dd-hh-mm-ss-fffff");
                    ctx.Request.Session["TempUserId"] = "temp-" + Guid.NewGuid().ToString();
                }
                return null;
                //return <null or a Response object>;
            };

        }

    }
}