using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.IO;

using Yiauo.Three;

namespace Yiauo.Three.Printer
{
    public partial class UploadFile : System.Web.UI.Page
    {
     
        string uploadPath = "upload/models/stl";

        struct UploadFileInfo
        {
            public string gg_tj;
            public string gg_bmj;
            public string gg_length;
            public string gg_width;
            public string gg_height;
            public string source_name;
            public string file_name;
            public string code;
            public string error;
            public bool success;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (this.Page.Request.Files.Count > 0)
                {
                    UploadFileInfo fi = new UploadFileInfo();
                    string fileName = GetNewFilename("STL", "stl");
                    string fileFullPath = Server.MapPath(Path.Combine(uploadPath, fileName));

                    fi.gg_tj = fi.gg_bmj = fi.gg_length = fi.gg_width =
                    fi.gg_height = fi.code = fi.error = "";

                    fi.source_name = this.Page.Request.Files[0].FileName;
                    fi.file_name = this.Page.ResolveUrl("~") + uploadPath + "/" + fileName;
                    fi.success = false;

                    try
                    {
                        this.Page.Request.Files[0].SaveAs(fileFullPath);

                        STLReader stl = new STLReader(fileFullPath);

                        if (stl.IsValid)
                        {
                            fi.gg_tj = stl.Volume.ToString();
                            //fi.gg_bmj = stl.SurfaceArea.ToString();
                            fi.gg_length = stl.Size.Length.ToString();
                            fi.gg_width = stl.Size.Width.ToString();
                            fi.gg_height = stl.Size.Height.ToString();
                            
                            fi.success = true;
                        }
                        else
                        {
                            fi.error = stl.ErrorMessage;
                        }
                    }
                    catch (Exception ex)
                    {
                        fi.code = "-1";
                        fi.error = ex.Message;
                    }

                    string jsonString;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    List<UploadFileInfo> jsonList = new List<UploadFileInfo>();
                    jsonList.Add(fi);
                    jsonString = serializer.Serialize(jsonList);
                    jsonString = jsonString.Replace("[", "").Replace("]", "");
                   
                    this.Context.Response.Clear();
                    this.Context.Response.Write(jsonString);
                    this.Context.Response.Flush();
                    this.Context.Response.End();
                }
            }
        }

        private string GetNewFilename(string prefixString, string extString)
        {
            return String.Format("{0}{1}.{2}", prefixString, DateTime.Now.ToString("yyyyMMddHHmmss"), extString);
        }

    }
}