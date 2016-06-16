using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using log4net;
using Nancy.ErrorHandling;
using System.Threading.Tasks;

namespace Bilin3d {
    public class LoggingErrorHandler : IStatusCodeHandler {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggingErrorHandler));
        public void Handle(HttpStatusCode statusCode, NancyContext context) {
            object errorObject;
            context.Items.TryGetValue(NancyEngine.ERROR_EXCEPTION, out errorObject);
            var error = errorObject as Exception;

            //Task tasks = new Task(() => {
            //    _logger.Error("Unhandled error", error);
            //});
            //tasks.Start();
             
            //new Task(() => {
            //    _logger.Error("发生错误啦!", error);
            //}).Start();

            System.Threading.ThreadPool.QueueUserWorkItem((i) => {
                _logger.Error("发生错误啦!", error);
            }); 
        }
        
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context) {
            return statusCode == HttpStatusCode.InternalServerError;
        }
    }
}