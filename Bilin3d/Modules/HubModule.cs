using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Extensions;
using Nancy.Validation;
using Nancy.ModelBinding;
using Bilin3d.Models;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using System.Data;
using System.Net.Mail;
using log4net;
using ServiceStack.OrmLite;
using System.Text;
using System.Text.RegularExpressions;
using Nancy.Security;
using System.IO;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace Bilin3d.Modules {
    public class HubModule : BaseModule {
        public HubModule(IDbConnection db, ILog log, IRootPathProvider pathProvider)
            : base("/hub") {

            this.RequiresAuthentication();

            Get["/"] = parameters => {
                Page.Title = "HUB首页";
                return View["Index", Model];
            };

            Get["/add"] = parameters => {
                Page.Title = "成为HUB";
                var supplierId = db.Single<string>("select SupplierId from t_user where id=@id limit 1",
                    new {id = Page.UserId});
                //已经是hub了
                if (!string.IsNullOrEmpty(supplierId)) {
                    Response.AsRedirect("/hub");
                    //return null;
                }

                return View["Add", Model];
            };

            Post["/add"] = parameters => {
                var supplierId = db.Single<string>("select SupplierId from t_user where id=@id limit 1",
                    new {id = Page.UserId});
                //已经是hub了
                if (!string.IsNullOrEmpty(supplierId)) {
                    throw new Exception("已经是hub(供应商)了");
                }

                var model = this.Bind<SupplierModel>();
                model.SupplierId = Guid.NewGuid().ToString("N");
                var result = this.Validate(model);
                if (!result.IsValid) {
                    foreach (var item in result.Errors) {
                        foreach (var member in item.Value) {
                            base.Page.Errors.Add(new ErrorModel() {Name = item.Key, ErrorMessage = member.ErrorMessage});
                        }
                    }
                }

                var files = Request.Files;
                HttpFile file_Logo = null, file_IdCarPic1 = null, file_IdCarPic2 = null,file_BlicensePic = null;
                foreach (var file in files) {
                    if (file.Key == "Logo") file_Logo = file;
                    if (file.Key == "IdCarPic1") file_IdCarPic1 = file;
                    if (file.Key == "IdCarPic2") file_IdCarPic2 = file;
                    if (file.Key == "BlicensePic") file_BlicensePic = file;
                }

                string uploadDirectory;
                
                if (file_Logo == null) {
                    Page.Errors.Add(new ErrorModel() {Name = "", ErrorMessage = "Logo不能为空"});
                } else {
                    uploadDirectory = Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier","logo");
                    if (!Directory.Exists(uploadDirectory)) {
                        Directory.CreateDirectory(uploadDirectory);
                    }
                    string _filename = "", filename = "";
                    string[] imgs = new string[] { ".jpg", ".png", ".gif", ".bmp", ".jpeg" };
                    if (!imgs.Contains(System.IO.Path.GetExtension(file_Logo.Name).ToLower())) {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "Logo文件格式不正确" });
                    }
                    _filename = model.SupplierId + "$" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fffff") + "$" + file_Logo.Name;
                    filename = Path.Combine(uploadDirectory, _filename);
                    using (FileStream fileStream = new FileStream(filename, FileMode.Create)) {
                        file_Logo.Value.CopyTo(fileStream);
                    }

                    model.Logo = _filename;
                }

                //个人
                if (model.Ftype == "0") {
                    if (file_IdCarPic1 == null || file_IdCarPic2 == null) {
                        Page.Errors.Add(new ErrorModel() {Name = "", ErrorMessage = "身份证扫描不能为空"});
                    } else {
                        uploadDirectory = Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier", "idpic");
                        if (!Directory.Exists(uploadDirectory)) {
                            Directory.CreateDirectory(uploadDirectory);
                        }
                        string _filename = "", filename = "";
                        string[] imgs = new string[] { ".jpg", ".png", ".gif", ".bmp", ".jpeg" };
                        if (!imgs.Contains(System.IO.Path.GetExtension(file_IdCarPic1.Name).ToLower())
                            || !imgs.Contains(System.IO.Path.GetExtension(file_IdCarPic2.Name).ToLower())
                        ) {
                            Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "身份证扫描文件格式不正确" });
                        }
                        _filename = model.SupplierId + "$" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fffff") + "$" + file_IdCarPic1.Name;
                        filename = Path.Combine(uploadDirectory, _filename);
                        using (FileStream fileStream = new FileStream(filename, FileMode.Create)) {
                            file_IdCarPic1.Value.CopyTo(fileStream);
                        }

                        model.IdCardPic1 = _filename;

                        _filename = model.SupplierId + "$" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fffff") + "$" + file_IdCarPic2.Name;
                        filename = Path.Combine(uploadDirectory, _filename);
                        using (FileStream fileStream = new FileStream(filename, FileMode.Create)) {
                            file_IdCarPic2.Value.CopyTo(fileStream);
                        }

                        model.IdCardPic2 = _filename;
                    }

                    Regex reg = new Regex(@"(^\d{18}$)|(^\d{15}$)");
                    if (model.IdCard == null || !reg.IsMatch(model.IdCard)) {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "身份证号码格式不正确" });
                    }
                }

                //企业
                if (model.Ftype == "1")
                {
                    if (file_BlicensePic == null ){
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "营业执照扫描件不能为空" });
                    } else {
                        uploadDirectory = Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier", "idpic");
                        if (!Directory.Exists(uploadDirectory)) {
                            Directory.CreateDirectory(uploadDirectory);
                        }
                        string _filename = "", filename = "";
                        string[] imgs = new string[] { ".jpg", ".png", ".gif", ".bmp", ".jpeg" };
                        if (!imgs.Contains(System.IO.Path.GetExtension(file_BlicensePic.Name).ToLower())) {
                            Page.Errors.Add(new ErrorModel() {Name = "", ErrorMessage = "身份证扫描文件格式不正确"});
                        }
                        _filename = model.SupplierId + "$" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fffff") + "$" + file_BlicensePic.Name;
                        filename = Path.Combine(uploadDirectory, _filename);
                        using (FileStream fileStream = new FileStream(filename, FileMode.Create)) {
                            file_BlicensePic.Value.CopyTo(fileStream);
                        }

                        model.BlicensePic = _filename;
                    }

                    if (model.CompanyName == null || model.CompanyName.Trim().Length < 2) {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "请正确填写企业名称" });
                    }
                    if (model.Capital == null || model.Capital.Trim() == "") {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "注册资本不能为空" });
                    }
                    if (model.Fcode == null || model.Fcode.Trim() == "") {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "统一代码不能为空" });
                    }
                }

                if (Page.Errors.Count > 0) {
                    return View["Add", Model];
                    //return Response.AsJson(base.Page.Errors, Nancy.HttpStatusCode.BadRequest);
                }

                
                string sql = $@"
                    INSERT INTO t_supplier (
                        SupplierId,
                        Tel,
                        IdCard,
                        Fname,
                        IdCardPic1,
                        CompanyName,
                        Capital,
                        Fcode,
                        BlicensePic,
                        Ftype
                    )VALUES(
                        '{supplierId}',
                        '2',
                        '3',
                        '4',
                        '5',
                        '6',
                        '7',
                        '8',
                        '9',
                        '10'
                        );";
                db.ExecuteNonQuery(sql);


                return View["Add", Model];
            };

            Post["/"] = parameters => {
                string addressid = Request.Form.addressid;
                string payid = Request.Form.payid;
                string remark = Request.Form.remark;

                string province = db.Select<string>(@"
                    select Province from t_address where Id=@addressid", new {addressid = addressid}).FirstOrDefault();
                if (province == null) {
                    base.Page.Errors.Add(new ErrorModel() {Name = "", ErrorMessage = "收货地址不能为空"});
                    return Response.AsJson(base.Page.Errors, Nancy.HttpStatusCode.BadRequest);
                }

                string pay = db.Select<string>(@"
                    select name from t_pay where Id=@payid", new {payid = payid}).FirstOrDefault();
                if (pay == null) {
                    base.Page.Errors.Add(new ErrorModel() {Name = "", ErrorMessage = "支付方式不能为空"});
                    return Response.AsJson(base.Page.Errors, Nancy.HttpStatusCode.BadRequest);
                }

                decimal kd = 22;
                if (province.Contains("福建")) {
                    kd = 13;
                }

                string stateId = "1";
                string orderid = Page.UserId + DateTime.Now.ToString("yyyyMMddhhmmssffff");
                string sql = string.Format(@"
                        INSERT INTO t_order (OrderId,UserId, Amount, AddressId, Remark, StateId,EditTime, CreateTime)
                        SELECT '{0}',UserId, Amount+{2}, '{3}','{4}','{5}',NOW(), NOW()
                        FROM T_Car
                        WHERE UserId='{1}';

                        INSERT INTO t_orderdetail (OrderId,FileName,Weight,Area,Size,Volume,Num,MaterialId,Price,Amount,EditTime,CreateTime)
                        SELECT '{0}',FileName,Weight,Area,Size,Volume,Num,MaterialId,Price,Amount,NOW(),NOW()
                        FROM T_CarDetail
                        WHERE CarId=(SELECT CarId FROM T_Car WHERE UserId='{1}');

                        delete FROM T_CarDetail WHERE CarId=(SELECT CarId FROM T_Car WHERE UserId='{1}');
                        delete from T_Car WHERE UserId='{1}';
                    ", orderid, Page.UserId, kd, addressid, remark, stateId);
                db.ExecuteNonQuery(sql);
                return Response.AsJson(new {
                    message = "success"
                });
            };

            Get["/{id}"] = parameters => {
                string id = parameters.id;
                var order = db.Select<OrderModel>(string.Format(@"
                    select t1.OrderId,
                        t1.Express,
                        t1.CreateTime,
                        t1.Amount,
                        t2.Area,
                        t2.Size,
                        t2.Volume,
                        t2.Weight,
                        t2.FileName,
                        t2.Num,
                        t5.name as MatName,
                        t3.StateName,
                        t3.Id as StateId,
                        t4.Consignee,
                        t4.Address 
                    from t_order  t1
                    left join t_orderdetail  t2 on t2.OrderId=t1.OrderId
                    left join t_orderstate   t3 on t3.Id=t1.StateId
                    left join t_address  t4 on t4.Id=t1.AddressId
                    left join t_material  t5 on t5.MaterialId=t2.MaterialId
                    where t1.UserId='{0}' and t2.OrderId='{1}'
                    order by t1.CreateTime desc", Page.UserId, id)).FirstOrDefault();
                base.Page.Title = "订详明细";
                base.Model.Order = order;
                return View["Detail", base.Model];
            };

        }

    }
}