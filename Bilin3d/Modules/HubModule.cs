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
using Newtonsoft.Json.Linq;

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
                    new { id = Page.UserId });
                //已经是hub了
                if (!string.IsNullOrEmpty(supplierId)) {
                    return Response.AsRedirect("/hub");
                }

                var expresses = db.Select<ExpressModel>("select ExpressId,Fname from t_express");

                var supplierModel = new SupplierModel();
                supplierModel.Ftype = "0";
                base.Model.SupplierModel = supplierModel;
                base.Model.Expresses = expresses;
                return View["Add", Model];
            };

            Post["/add"] = parameters => {
                var supplierId = db.Single<string>("select SupplierId from t_user where id=@id limit 1",
                    new { id = Page.UserId });
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
                            base.Page.Errors.Add(new ErrorModel() { Name = item.Key, ErrorMessage = member.ErrorMessage });
                        }
                    }
                }

                var files = Request.Files;
                HttpFile file_Logo = null, file_IdCarPic1 = null, file_IdCarPic2 = null, file_BlicensePic = null;
                foreach (var file in files) {
                    if (file.Key == "Logo") file_Logo = file;
                    if (file.Key == "IdCarPic1") file_IdCarPic1 = file;
                    if (file.Key == "IdCarPic2") file_IdCarPic2 = file;
                    if (file.Key == "BlicensePic") file_BlicensePic = file;
                }

                string uploadDirectory;

                if (file_Logo == null) {
                    Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "Logo不能为空" });
                } else {
                    uploadDirectory = Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier", "logo");
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
                    Regex reg = new Regex(@"(^\d{18}$)|(^\d{15}$)");
                    if (model.IdCard == null || !reg.IsMatch(model.IdCard)) {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "身份证号码格式不正确" });
                    }

                    if (file_IdCarPic1 == null || file_IdCarPic2 == null) {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "身份证扫描不能为空" });
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
                }

                //企业
                if (model.Ftype == "1") {
                    if (model.CompanyName == null || model.CompanyName.Trim().Length < 2) {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "请正确填写企业名称" });
                    }
                    if (model.Capital == null || model.Capital.Trim() == "") {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "注册资本不能为空" });
                    }
                    if (model.Fcode == null || model.Fcode.Trim() == "") {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "统一代码不能为空" });
                    }

                    if (file_BlicensePic == null) {
                        Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "营业执照扫描件不能为空" });
                    } else {
                        uploadDirectory = Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier",
                            "idpic");
                        if (!Directory.Exists(uploadDirectory)) {
                            Directory.CreateDirectory(uploadDirectory);
                        }
                        string _filename = "", filename = "";
                        string[] imgs = new string[] { ".jpg", ".png", ".gif", ".bmp", ".jpeg" };
                        if (!imgs.Contains(System.IO.Path.GetExtension(file_BlicensePic.Name).ToLower())) {
                            Page.Errors.Add(new ErrorModel() { Name = "", ErrorMessage = "身份证扫描文件格式不正确" });
                        }
                        _filename = model.SupplierId + "$" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fffff") + "$" +
                                    file_BlicensePic.Name;
                        filename = Path.Combine(uploadDirectory, _filename);
                        using (FileStream fileStream = new FileStream(filename, FileMode.Create)) {
                            file_BlicensePic.Value.CopyTo(fileStream);
                        }

                        model.BlicensePic = _filename;
                    }
                }

                if (Page.Errors.Count > 0) {
                    //验证不通过时，删除本次上传的文件以防止冗余
                    if (model.Logo != null) {
                        System.IO.File.Delete(Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier",
                            "logo",
                            model.Logo));
                    }
                    if (model.IdCardPic1 != null) {
                        System.IO.File.Delete(Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier",
                       "idpic",
                       model.IdCardPic1));
                    }
                    if (model.IdCardPic2 != null) {
                        System.IO.File.Delete(Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier",
                        "idpic",
                        model.IdCardPic2));
                    }
                    if (model.BlicensePic != null) {
                        System.IO.File.Delete(Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "supplier",
                       "idpic",
                       model.BlicensePic));
                    }

                    Model.SupplierModel = model;
                    return View["Add", Model];
                    //return Response.AsJson(base.Page.Errors, Nancy.HttpStatusCode.BadRequest);
                }

                //通过地址，从百度返回经纬度坐标                
                string url = $"http://api.map.baidu.com/geocoder/v2/?address={model.Address}&output=json&ak=26904d2efeb684d7d59d493098e7295d";
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                string json = wc.DownloadString(url);
                JObject m = JObject.Parse(json);
                if (m["status"].ToString() == "0") {
                    model.Lng = m["result"]["location"]["lng"].ToString(); //经度
                    model.Lat = m["result"]["location"]["lat"].ToString(); //纬度
                }


                string sql = $@"
                    INSERT INTO t_supplier (
                        SupplierId,
                        Tel,
                        QQ,
                        Address,
                        Logo,
                        IdCard,
                        Fname,
                        IdCardPic1,
                        IdCardPic2,
                        CompanyName,
                        Capital,
                        Fcode,
                        BlicensePic,
                        Ftype,
                        Lng,
                        Lat,
                        ExpressId
                    )VALUES(
                        '{model.SupplierId}',
                        '{model.Tel}',
                        '{model.QQ}',
                        '{model.Address}',
                        '{model.Logo}',
                        '{model.IdCard}',
                        '{model.Fname}',
                        '{model.IdCardPic1}',
                        '{model.IdCardPic2}',
                        '{model.CompanyName}',
                        '{model.Capital}',
                        '{model.Fcode}',
                        '{model.BlicensePic}',
                        '{model.Ftype}',
                        '{model.Lng}',
                        '{model.Lat}',
                        '{model.ExpressId}'
                        );
                    UPDATE t_user SET SupplierId='{model.SupplierId}' where id='{Page.UserId}';                       
                ";
                db.ExecuteNonQuery(sql);
                Model.Message = "添加Hub成功!";
                Model.Url = "/hub";
                return View["Success", Model];
            };

            Get["/edit"] = parameters => {
                return "123";
            };

            Post["/edit"] = parameters => {
                return null;
            };

            Get["/printer.js"] = parameters => {
                string str = "var printers = [";
                var printers = db.Select<PrinterModel>("SELECT PrinterId,Fname FROM t_printer WHERE State='0'");
                printers.ForEach(i => {
                    str = str + $@"{{value:'{i.PrinterId}',label:'{i.Fname}'}},";
                });
                str = str.TrimEnd(',') + "];";
                return Response.AsText(str);
            };

            Get["/printer"] = parameters => {
                string sql = "select AccuracyId,Fname from t_printaccuracy;";
                var printerAccuracys = db.Select<PrinterAccuracyModel>(sql);
                sql = "select CompleteId,Fname from t_printcomplete;";
                var printerCompletes = db.Select<PrinterCompleteModel>(sql);               

                var accuracyOpt = "<select class='accuracyOpt'>";
                foreach (var item in printerAccuracys) {
                    accuracyOpt += "<option value='" + item.AccuracyId + "'>" + item.Fname + "</option>";
                }
                accuracyOpt += "</select>";

                var completeOpt = "<select class='completeOpt'>";
                foreach (var item in printerCompletes) {
                    completeOpt += "<option value='" + item.CompleteId + "'>" + item.Fname + "</option>";
                }
                completeOpt += "</select>";

                Model.accuracyOpt = accuracyOpt;
                Model.completeOpt = completeOpt;
                return View["Print", Model];
            };
                        
            Get["/printer/material/list"] = parameters => {
                string sql = $@"
                    select t1.SupplierId ,t1.PrinterId,t2.fname as PrintName, t4.MaterialId,t4.Name as MaterialName
                    from t_supplier_printer t1
                    left join t_printer t2 on t2.printerid=t1.printerid
                    left join t_supplier_printer_material t3 on t3.SupplierId=t1.SupplierId  and t3.PrinterId=t2.PrinterId
                    left join t_material t4 on t4.MaterialId = t3.MaterialId
                    where t1.state='0' and t2.state='0'
                        and t1.supplierid=(select SupplierId from t_user where id='{Page.UserId}');";
                var supplierprintermaterials = db.Select<SupplierPrinterMaterialModel>(sql);
                return Response.AsJson(
                    supplierprintermaterials
                        .GroupBy(i => new { i.SupplierId, i.PrinterId, i.PrintName })
                        .Select(x => new { printer = x.Key, result = x })
                    , Nancy.HttpStatusCode.OK);
            };

            Get["/printer/material/list"] = parameters => {
                string sql = $@"
                    select t1.SupplierId ,t1.PrinterId,t2.fname as PrintName, t4.MaterialId,t4.Name as MaterialName
                    from t_supplier_printer t1
                    left join t_printer t2 on t2.printerid=t1.printerid
                    left join t_supplier_printer_material t3 on t3.SupplierId=t1.SupplierId  and t3.PrinterId=t2.PrinterId
                    left join t_material t4 on t4.MaterialId = t3.MaterialId
                    where t1.state='0' and t2.state='0'
                        and t1.supplierid=(select SupplierId from t_user where id='{Page.UserId}');";
                var supplierprintermaterials = db.Select<SupplierPrinterMaterialModel>(sql);
                return Response.AsJson(
                    supplierprintermaterials
                        .GroupBy(i => new { i.SupplierId, i.PrinterId, i.PrintName })
                        .Select(x => new { printer = x.Key, result = x })
                    , Nancy.HttpStatusCode.OK);
            };
            
            Get["/printer/material/{printerid}"] = parameters => {
                string printerid = parameters.printerid;
                return Response.AsText(printerid, "text/html; charset=utf-8");
            };

            Post["/printer/add"] = parameters => {
                string printerid = Request.Form.printerid;
                string sql = $@"select count(1) 
                                from t_supplier_printer 
                                where SupplierId=(select SupplierId from t_user where id='{Page.UserId}') and  PrinterId='{printerid}';";
                var count = db.Scalar<int>(sql);
                if (count > 0) {
                    return Response.AsJson(new { message = "打印机已存在!" }, Nancy.HttpStatusCode.BadRequest);
                }

                sql = $@"INSERT INTO t_supplier_printer (ID,SupplierId,PrinterId)
                                    VALUES('{Guid.NewGuid().ToString("N")}',(select SupplierId from t_user where id='{Page.UserId}'),'{printerid}');";
                db.ExecuteNonQuery(sql);
                return null;
            };

            Get["/material.js"] = parameters => {
                string str = "var materials = [";
                var materials = db.Select<Material>("SELECT * FROM t_material WHERE State='0'");
                materials.ForEach(i => {
                    str = str + $@"{{value:'{i.MaterialId}',label:'{i.Name}'}},";
                });
                str = str.TrimEnd(',') + "];";
                return Response.AsText(str);
            };

            Get["/material/{id}"] = parameters => {
                var materialId = parameters.id;
                var material = db.Single<Material>("select * from t_material where MaterialId=@MaterialId", new { MaterialId = materialId });
                return Response.AsJson(material);                
            };
            
            Post["/printer/material/add"] = parameters => {
                string printerid = Request.Form.printerid;
                string materialid = Request.Form.materialid;
                string completeid = Request.Form.completeid;
                string accuracyid = Request.Form.accuracyid;
                string price = Request.Form.price;
                string sql = $@"select count(1) 
                    from t_supplier_printer_material
                    where SupplierId=(select SupplierId from t_user where id='{Page.UserId}') 
                        and  PrinterId='{printerid}'
                        and  MaterialId='{materialid}';";
                var count = db.Scalar<int>(sql);
                if (count > 0) {
                    return Response.AsJson(new { message = "材料已存在!" }, Nancy.HttpStatusCode.BadRequest);
                }

                sql = $@"
                        INSERT INTO t_supplier_printer_material (
                            ID,
                            SupplierId,
                            PrinterId,
                            MaterialId,
                            CompleteId,
                            AccuracyId,
                            Price)
                        VALUES(
                            '{Guid.NewGuid().ToString("N")}',
                            (select SupplierId from t_user where id='{Page.UserId}'),
                            '{printerid}',
                            '{materialid}',
                            '{completeid}',
                            '{accuracyid}',
                            '{price}'
                        );";
                db.ExecuteNonQuery(sql);
                return null;
            };

            Get["/blance"] = parameters => {
                return Response.AsText("账户余额", "text/html; charset=utf-8");
            };

            Get["/withdraw"] = parameters => {
                return Response.AsText("取现", "text/html; charset=utf-8");                
            };

        }
    }
}