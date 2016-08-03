using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Bilin3d.Models.CustomAnnotations;

namespace Bilin3d.Models {
    public class SupplierModel {
        public string SupplierId { get; set; }

        [Required(ErrorMessage = "电话不能为空")]
        public string Tel { get; set; }

        [Required(ErrorMessage = "QQ不能为空")]
        public string QQ { get; set; }

        [Required(ErrorMessage = "地址不能为空")]
        public string Address { get; set; }

        [Required(ErrorMessage = "物流政策不能为空")]
        public string ExpressId { get; set; }

        public string Logo { get; set; }

        //[RequiredIf("Ftype", "0",ErrorMessage = "身份证不能为空")]
        //[RegularExpression(@"(^\d{18}$)|(^\d{15}$)", ErrorMessage = "请输入正确的身份证号码")]
        public string IdCard { get; set; }

        [Required(ErrorMessage = "名称不能为空")]
        public string Fname { get; set; }
        
        public string IdCardPic1 { get; set; }
        public string IdCardPic2 { get; set; }
        public string CompanyName { get; set; }
        public string Capital { get; set; }
        public string Fcode { get; set; }
        public string BlicensePic { get; set; }

        [Range(0,1,ErrorMessage = "用户类型错误")]
        public string Ftype { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string State { get; set; }             
    }
}