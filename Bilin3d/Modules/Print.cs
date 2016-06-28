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
using Yiauo.Three;

namespace Bilin3d.Modules {
    public class PrintModule : BaseModule {
        public PrintModule(IDbConnection db, ILog log, IRootPathProvider pathProvider) {

            Get["/print"] = parameters => {
                base.Page.Title = "3D打印";
                var materials = db.Select<Material>(q => q.State == 0);
                base.Model.Materials = materials;

                return View["Index", base.Model];
            };

            Post["/print/upload"] = parameters => {
                string uploadDirectory;
                string filepath = "";
                if (Context.CurrentUser == null) {
                    uploadDirectory = Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "temp");
                    filepath = "/Content/uploads/temp/";
                } else {
                    uploadDirectory = Path.Combine(pathProvider.GetRootPath(), "Content", "uploads", "3d");
                    filepath = "/Content/uploads/3d/";
                }

                if (!Directory.Exists(uploadDirectory)) {
                    Directory.CreateDirectory(uploadDirectory);
                }

                string _filename = "";
                string filename = "";
                List<Models.File> files = new List<Models.File>();

                foreach (var file in Request.Files) {
                    if (System.IO.Path.GetExtension(file.Name).ToLower() != ".stl") {
                        //return Response.AsJson("{\"files\":[{error:\"文件格式不是stl\"}]}",Nancy.HttpStatusCode.BadRequest);
                        files.Add(new Models.File() { error = "文件格式不是stl!" });
                        return Response.AsJson<JsonFileUpload>(new JsonFileUpload() {
                            files = files
                        });
                    }
                    if (file.Name.IndexOf("$") >= 0) {
                        files.Add(new Models.File() { error = "文件名不能包含'$'符号!" });
                        return Response.AsJson<JsonFileUpload>(new JsonFileUpload() {
                            files = files
                        });
                    }

                    //_filename = file.Name;
                    if (Context.CurrentUser == null) {
                        _filename = Session["TempUserId"].ToString() + "$" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fffff") + "$" + file.Name;
                        filename = Path.Combine(uploadDirectory, _filename);
                    } else {
                        string userid = "";
                        userid = db.Select<string>(string.Format(@"select Id from T_User where Email=@email", new { email = base.Page.CurrentUser })).FirstOrDefault();

                        _filename = userid + "$" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fffff") + "$" + file.Name;
                        filename = Path.Combine(uploadDirectory, _filename);
                    }

                    using (FileStream fileStream = new FileStream(filename, FileMode.Create)) {
                        file.Value.CopyTo(fileStream);
                    }
                }


                STLReader stl = new STLReader(filename);
                if (stl.IsValid) {
                    //jsonfile.files= stl.Size.Length.ToString();
                    //fi.gg_width = stl.Size.Width.ToString();
                    //fi.gg_height = stl.Size.Height.ToString();
                    //stl.Surface.
                    files.Add(new Models.File() {
                        name = _filename,
                        fullname = filepath + _filename,
                        length = stl.Size.Length.ToString(),
                        width = stl.Size.Width.ToString(),
                        height = stl.Size.Height.ToString(),
                        surface = stl.Surface.ToString(),
                        volume = stl.Volume.ToString()
                    });
                } else {
                    files.Add(new Models.File() {
                        error = stl.ErrorMessage
                    });
                }
                //return Response.AsJson("{\"files\":[{name:\"" + _filename + "\"}]}", Nancy.HttpStatusCode.OK);
                //files.Add(new Models.File() { name = _filename });
                return Response.AsJson<JsonFileUpload>(new JsonFileUpload() {
                    files = files
                });

                //base.Page.Title = "上传成功";
                //return View["Index", base.Model];
            };

            Post["/print/addcar"] = parameters => {
                //string matid = Request.Form.matid;
                //string filename = Request.Form.filename;                
                //string area = Request.Form.Area;
                var model = this.Bind<CarModel>();
                var result = this.Validate(model);
                if (!result.IsValid) {
                    foreach (var item in result.Errors) {
                        foreach (var member in item.Value) {
                            base.Page.Errors.Add(new ErrorModel() { Name = item.Key, ErrorMessage = member.ErrorMessage });
                        }
                    }
                    return Response.AsJson(base.Page.Errors, Nancy.HttpStatusCode.BadRequest);
                }

                if (base.Page.IsAuthenticated) {
                    //Mysql返回主表自增ID: SELECT LAST_INSERT_ID();
                    //                    string sql = string.Format(@"        
                    //                            SELECT Id FROM T_Car WHERE UserId IN(SELECT Id FROM T_User WHERE Email='{0}');
                    //                            ", Page.CurrentUser);
                    string sql = "SELECT CarId FROM T_Car WHERE UserId =@UserId;";
                    var carid = db.Select<string>(sql, new { UserId = Page.UserId }).FirstOrDefault();
                    if (carid == null) {  //购物车主表没记录
                        carid = Guid.NewGuid().ToString("N");
                        sql = string.Format(@"
                            INSERT INTO T_Car(UserId,Amount,EditTime,CarId) VALUES('{0}','',NOW(),'{1}');
                            INSERT INTO T_CarDetail (
	                            CarId,
	                            FileName,
	                            Volume,
	                            Area,
	                            Size,
	                            Num,
	                            MaterialId,
	                            Price,
                                Weight,
	                            EditTime
                            )
                            VALUES
	                            (
		                            '{1}',
		                            '{2}',                
		                            '{3}',                       
		                            '{4}',                   
		                            '{5}',
		                            '{6}',
		                            '{7}',
		                            (SELECT Price FROM T_Material WHERE Id={7}),
                                    (SELECT Density*{3} FROM T_Material WHERE Id={7}),
		                            NOW()
	                            );
                            
                            ", base.Page.UserId, carid, model.FileName, model.Volume, model.Area, model.Size, model.Num, model.MaterialId);
                    } else {
                        sql = string.Format(@"select 1 from T_CarDetail where MaterialId={0} and FileName='{1}'", model.MaterialId, model.FileName);
                        var materialid = db.Select<string>(sql).FirstOrDefault();
                        if (materialid == null) {
                            sql = string.Format(@"
                            INSERT INTO T_CarDetail (
	                            CarId,
	                            FileName,
                                Volume,
	                            Area,
	                            Size,
	                            Num,
	                            MaterialId,
	                            Price,
                                Weight,
	                            EditTime
                            )
                            VALUES
	                            (
		                            '{0}',
		                            '{1}',                
		                            '{2}',                       
		                            '{3}',                   
		                            '{4}',
		                            '{5}',
		                            '{6}',
		                            (SELECT Price FROM T_Material WHERE Id={6}),
                                    (SELECT Density*{2} FROM T_Material WHERE Id={6}),
		                            NOW()
	                            );                            
                            ", carid, model.FileName, model.Volume, model.Area, model.Size, model.Num, model.MaterialId);
                        } else {
                            sql = string.Format(@"update T_CarDetail set EditTime=NOW(), Num={0} where MaterialId={1} and FileName='{2}';", model.Num, model.MaterialId, model.FileName);
                        }

                    }

                    //更新更新子表金额Amount字段，不足最低接单价格的按最低接单价格算
                    //更新主表总金额Amount字段
                    sql = sql + string.Format(@"
                       UPDATE T_CarDetail t1
                       SET EditTime = NOW(),
                           Amount = (
	                            SELECT
		                            CASE
		                            WHEN SUM(t1.Price * t1.Num) > mat.Price1 THEN
			                            SUM(t1.Price * t1.Num)
		                            ELSE
			                            mat.Price1
		                            END
	                            FROM
		                            T_Material mat where mat.Id = t1.MaterialId                                
                           )
                        WHERE t1.CarId='{0}';

                        update T_Car set EditTime=NOW(), Amount=(select SUM(Amount) from T_CarDetail where CarId='{0}') where CarId='{0}'
                        ", carid);
                    var num = db.ExecuteNonQuery(sql);
                } else {  // 未登陆
                    string sql = string.Format(@"SELECT CarId FROM T_CarTemp WHERE UserId='{0}';", Session["TempUserId"].ToString());
                    var carid = db.Select<string>(sql).FirstOrDefault();
                    if (carid == null) {  //临时购物车主表没记录
                        carid = Guid.NewGuid().ToString("N");
                        sql = string.Format(@"
                            INSERT INTO T_CarTemp(UserId,Amount,EditTime,CarId) VALUES('{0}','',NOW(),'{1}');
                            INSERT INTO T_CarDetailTemp (
	                            CarId,
	                            FileName,
                                Volume,
	                            Area,
	                            Size,
	                            Num,
	                            MaterialId,
	                            Price,
                                Weight,
	                            EditTime
                            )
                            VALUES
	                            (
		                            '{1}',
		                            '{2}',                
		                            '{3}',                       
		                            '{4}',                   
		                            '{5}',
		                            '{6}',
		                            '{7}',
		                            (SELECT Price FROM T_Material WHERE Id={7}),
                                    (SELECT Density*{3} FROM T_Material WHERE Id={7}),
		                            NOW()
	                            );                            
                            ", Session["TempUserId"].ToString(), carid, model.FileName, model.Volume, model.Area, model.Size, model.Num, model.MaterialId);
                    } else {
                        sql = string.Format(@"select 1 from T_CarDetailTemp where MaterialId={0} and FileName='{1}'", model.MaterialId, model.FileName);
                        var materialid = db.Select<string>(sql).FirstOrDefault();
                        if (materialid == null) {
                            sql = string.Format(@"
                            INSERT INTO T_CarDetailTemp (
	                            CarId,
	                            FileName,
                                Volume,
	                            Area,
	                            Size,
	                            Num,
	                            MaterialId,
	                            Price,
                                Weight,
	                            EditTime
                            )
                            VALUES
	                            (
		                            '{0}',
		                            '{1}',                
		                            '{2}',                       
		                            '{3}',                   
		                            '{4}',
		                            '{5}',
		                            '{6}',
		                            (SELECT Price FROM T_Material WHERE Id={6}),
                                    (SELECT Density*{2} FROM T_Material WHERE Id={6}),
		                            NOW()
	                            );                            
                            ", carid, model.FileName, model.Volume, model.Area, model.Size, model.Num, model.MaterialId);
                        } else {
                            sql = string.Format(@"update T_CarDetailTemp set Num={0},EditTime=NOW() where MaterialId={1} and FileName='{2}';", model.Num, model.MaterialId, model.FileName);
                        }
                    }

                    //更新更新子表金额Amount字段，不足最低接单价格的按最低接单价格算
                    //更新主表总金额Amount字段
                    sql = sql + string.Format(@"
                        UPDATE T_CarDetailTemp t1
                        SET EditTime = NOW(),
                            Amount = (
	                            SELECT
		                            CASE
		                            WHEN SUM(t1.Price * t1.Num) > mat.Price1 THEN
			                            SUM(t1.Price * t1.Num)
		                            ELSE
			                            mat.Price1
		                            END
	                            FROM
		                            T_Material mat where mat.Id = t1.MaterialId                                
                            )
                        WHERE t1.CarId='{0}';

                        update T_CarTemp set EditTime=NOW(), Amount=(select SUM(Amount) from T_CarDetailTemp where CarId='{0}') where CarId='{0}'
                        ", carid);
                    var num = db.ExecuteNonQuery(sql);

                    Session["CarAdded"] = "1"; // 表示已加入临时购物车了， 用于在登录/注册成功后，转换成正式用户购物车时判断（ConvertTempCar），避免一次数据库操作；
                }
                return null;
                //return Response.AsJson(new { result = "success" });
            };

            //获取顶部的购物车的商品
            Get["/print/car"] = parameters => {
                if (base.Page.IsAuthenticated) {
                    var cars = db.Select<CarModel>(@"
                        select t1.amount,
                            t1.CarId as CarId,
                            t2.Id as CarDetailId,
                            t2.filename,
                            t2.area,
                            t2.size,
                            t2.num,
                            t2.price,
                            t3.name as MatName, 
                            t3.Color as MatColor,
                            t3.Accuracy as Accuracy,
                            t3.Delivery as Delivery
                        from T_Car t1
                        left join T_CarDetail t2 on t2.CarId=t1.CarId
                        left join T_Material t3 on t3.Id=t2.MaterialId
                        where t1.UserId=(SELECT Id FROM T_User WHERE Email=@Email);
                        ", new { Email = Page.CurrentUser });
                    return Response.AsJson(cars.Select(i => new {
                        i.Accuracy,
                        i.Amount,
                        i.Num,
                        i.Price,
                        i.Size,
                        i.MatColor,
                        i.MatName,
                        i.Delivery,
                        i.CarDetailId,
                        i.CarId,
                        FileName = i.FileName.Split('$').Last()
                    }));

                } else {
                    var cars = db.Select<CarModel>(@"
                        select t1.amount,
                            t1.CarId as CarId,
                            t2.Id as CarDetailId,
                            t2.filename,
                            t2.area,
                            t2.size,
                            t2.num,
                            t2.price,
                            t3.name as MatName, 
                            t3.Color as MatColor,
                            t3.Accuracy as Accuracy,
                            t3.Delivery as Delivery
                        from T_CarTemp t1
                        left join T_CarDetailTemp t2 on t2.CarId=t1.CarId
                        left join T_Material t3 on t3.Id=t2.MaterialId
                        where t1.UserId=@UserId;
                        ", new { UserId = Session["TempUserId"].ToString() });
                    return Response.AsJson(cars.Select(i => new {
                        i.Accuracy,
                        i.Amount,
                        i.Num,
                        i.Price,
                        i.Size,
                        i.MatColor,
                        i.MatName,
                        i.Delivery,
                        i.CarDetailId,
                        i.CarId,
                        FileName = i.FileName.Split('$').Last()
                    }));
                }
            };

            //删除购物车里的商品
            Post["/print/material/del/{id}-{carid}"] = parameters => {
                string id = parameters.id;
                string carid = parameters.carid;
                CarCount carcount;
                if (Page.IsAuthenticated) {
                    // 第二条是删除子表没记录时的sql
                    db.ExecuteNonQuery(@"
                        delete from T_CarDetail where id=@Id;
                        delete from T_Car where carid not in (select carid from T_CarDetail);
                        update T_Car set EditTime=NOW(), Amount=(select SUM(Amount) from T_CarDetail where CarId=@CarId) where CarId=@CarId;
                        ", new { Id = id, CarId = carid });
                    carcount = db.Select<CarCount>(@"                                             
                        select sum(t2.amount) as SumAmount,
                            sum(t2.num) as SumNum
                        from T_Car t1
                        join T_CarDetail t2 on t2.CarId=t1.CarId
                        where t1.UserId=@UserId;
                        ", new { UserId = Page.UserId }).FirstOrDefault();
                } else {
                    // 第二条是删除子表没记录时的sql
                    db.ExecuteNonQuery(@"
                        delete from T_CarDetailTemp where id=@Id;
                        delete from T_Car where carid not in (select carid from T_CarDetail);
                        update T_CarTemp set EditTime=NOW(), Amount=(select SUM(Amount) from T_CarDetailTemp where CarId=@CarId) where CarId=@CarId;
                        ", new { Id = id, CarId = carid });
                    carcount = db.Select<CarCount>(@"                                             
                        select sum(t2.amount) as SumAmount,
                            sum(t2.num) as SumNum
                        from T_CarTemp t1
                        join T_CarDetailTemp t2 on t2.CarId=t1.CarId
                        where t1.UserId=@UserId;
                        ", new { UserId = Session["TempUserId"].ToString() }).FirstOrDefault();
                }
                //return Response.AsJson(new { result = "success: " + parameters.id + "!" });
                return Response.AsJson(carcount);
            };

        }

    }
}