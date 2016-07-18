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

        [Required(ErrorMessage = "Logo不能为空")]
        public string Logo { get; set; }

        //[RequiredIf("Ftype", "0",ErrorMessage = "身份证不能为空")]
        public string IdCard { get; set; }
        public string Fname { get; set; }
        
        public string IdCardPic1 { get; set; }
        public string CompanyName { get; set; }
        public string Capital { get; set; }
        public string Fcode { get; set; }
        public string BlicensePic { get; set; }
        public string Ftype { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string State { get; set; }
    }
}