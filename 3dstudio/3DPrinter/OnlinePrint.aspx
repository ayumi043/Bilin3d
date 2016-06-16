<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnlinePrint.aspx.cs" Inherits="Yiauo.Three.Printer.OnlinePrint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html lang="cn">
<head>
<meta charset="UTF-8" />
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
<title>在线3D打印</title>
<link href="css/bootstrap.min.css" rel="stylesheet" media="screen" />
<link href="css/stylesheet.css" rel="stylesheet">
<script src="js/jquery-2.1.1.min.js" type="text/javascript"></script>
<script src="js/CFInstall.min.js" type="text/javascript"></script>
<script src="js/three.js" type="text/javascript"></script>
<script src="js/plane.js" type="text/javascript"></script>
<script src="js/thingiview.js" type="text/javascript"></script>
</head>
<body class="product-product-47#redirect">
<div class="body-content">

<div class="container product-body">
    <div class="row">
	    <div class="col-sm-12 product-top-banner"></div>
    </div>
    <div class="product-top-info">您对3D打印感兴趣吗，您对3D打印了解吗？<a href="">在这里您可以找到您所需要的答案</a></div>
  <a name="redirect" style="position: relative;top: -30px;"></a>

    <div class="product-top-tab">
	    <span class="product-top-tab-1">选择材料</span><span class="product-top-tab-2">材料特点</span>
    </div>
 
<div id="product">
<div id="content">
<div class="product-info-content">
	<div class="product-info-content-left">
		<table class="table table-product-info">
			<tr>
				<td colspan="2" class="product-info-title">规格</td>
			</tr>
			<tr>
				<td class="product-info-left">文件名称</td>
				<td class="product-info-right upload-file-name">--</td>
			</tr>
			<tr>
				<td class="product-info-left">材料体积</td>
				<td class="product-info-right" id="product-info-tj">--</td>
			</tr>
			<tr style="display:none;">
				<td class="product-info-left">表面积</td>
				<td class="product-info-right" id="product-info-bmj">--</td>
			</tr>
			<tr>
				<td class="product-info-left">尺寸</td>
				<td class="product-info-right" id="product-info-xyz">--</td>
			</tr>
			<tr>
				<td colspan="2" class="product-info-bottom">
										<button type="button" id="button-upload" data-loading-text="载入中..." class="btn btn-block upload-3d-file">
							上传STL格式的3D设计文件							
						</button>
						
						<input type="hidden" name="option[228]" value="" id="input-option-file"/>
										<form enctype="multipart/form-data" id="form-upload"><input type="file" name="file" style="position:absolute;left:0;top:0;width:100%;height:100%;z-index:99999;opacity:0;cursor:pointer;" /></form>
				</td>
			</tr>
		</table>
	</div>
    <div class="product-info-content-right">
		<ul class="thumbnails">
			<li>
			<div class="image-preview">
				<div id="viewer"><img src="image/no_image2-545x290.png" title="3D打印产品" alt="3D打印产品" /></div>
				<div class="loading_before"><span></span></div>
				<div class="text_loading_before">亚太地区最大的互联网制造云平台</div>
				<div id="text_loading_after" class="text_loading_after">正在渲染设计文件，请稍后...</div>
				<div id="text_loading_runing" class="text_loading_runing">超低价格 | 24小时内发货 | 全进口材料 | 64MB上传限制</div>
				<div id="progressNumber">
					<span class="progressBg"><span class="progressRun"></span></span>
					<span class="progressTitle">adf</span>
				</div>
			</div>
			</li>
		</ul>
    </div>
</div> 
</div>
</div>

</div>

<div class="warning-msg-div">
	<div class="warning-msg-box">
		<div class="msg-close"></div>
		<div class="msg-title">自动检测结果(警告)</div>
		<div class="msg-content">
		</div>
	</div>
</div>
<div class="error-msg-div">
	<div class="error-msg-box">
		<div class="msg-close"></div>
		<div class="msg-title">自动检测结果(错误)</div>
		<div class="msg-content">
		</div>
	</div>
</div>

</div>
<script type="text/javascript"><!--
    function onprogress(evt) {
        if (evt.lengthComputable) {
            var percentComplete = Math.round(evt.loaded * 100 / evt.total);
            $('#progressNumber .progressRun').css('width', percentComplete.toString() + '%');
            $('#progressNumber .progressTitle').html('已经上传 ' + percentComplete.toString() + '%');
        } else {
            $('#progressNumber .progressTitle').html('上传失败');
        }
    }
    var xhr_provider = function () {
        var xhr = jQuery.ajaxSettings.xhr();
        if (onprogress && xhr.upload) {
            xhr.upload.addEventListener('progress', onprogress, false);
        }
        return xhr;
    };  
//--></script> 
<script type="text/javascript"><!--
    $('#form-upload input[name=\'file\']').on('mouseenter', function () {
        $('#button-upload').css('background', '#0068b1');
    });
    $('#form-upload input[name=\'file\']').on('mouseleave', function () {
        $('#button-upload').css('background', '#1BB9FF');
    });
    $('#form-upload input[name=\'file\']').on('change', function () {
        $.ajax({
            url: 'UploadFile.aspx',
            type: 'post',
            dataType: 'json',
            data: new FormData($(this).parent()[0]),
            cache: false,
            contentType: false,
            processData: false,
            xhr: xhr_provider,
            beforeSend: function () {
                $('#viewer').html('<img src="image/no_image2-545x290.png" title="3D打印产品" alt="3D打印产品" />');
                $('#button-upload').html('正在上传...');
                $('.text_loading_runing').show();
                $('.text_loading_before').hide();
                $('.loading_before').hide();
                $('#progressNumber .progressRun').css('width', '0%');
                $('#progressNumber .progressTitle').html('已经上传 0%');
                $('#progressNumber').show();
            },
            complete: function () {
                $('#button-upload').html('重新上传文件');
            },
            success: function (json) {
                $('.text-danger').remove();
                if (json['error']) {
                    $('#button-upload').parent().find('input').after('<div class="text-danger">' + json['error'] + '</div>');
                }

                if (json['success']) {
                    $('#product-info-tj').html(json['gg_tj'] + 'mm&sup3;');
                    $('#product-info-bmj').html(json['gg_bmj'] + 'mm&sup2;');
                    $('#product-info-xyz').html('<span>' + json['gg_length'] + '</span> * <span>' + json['gg_width'] + '</span> * <span>' + json['gg_height'] + '</span> mm');
                    $('#button-upload').parent().find('input').attr('value', json['code']);
                    $('.upload-file-name').html(json['source_name']);
                    $('.text_loading_runing').hide();
                    $('.loading_before').hide();
                    $('.text_loading_before').hide();
                    $('#progressNumber').hide();
                    CFInstall.check({
                        mode: "inline",
                        node: "prompt"
                    });
                    //thingiurlbase = "http://localhost:8080/3dprinter/js";
                    thingiurlbase = "http://localhost:13483/js";
                    thingiview = new Thingiview("viewer");
                    thingiview.setObjectColor('#dddddd');
                    thingiview.initScene();

                    thingiview.loadSTL(json['file_name']);
                    $('#button-upload').html('重新上传文件');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText);
            }
        });
    });
//--></script> 

</body>
</html>
